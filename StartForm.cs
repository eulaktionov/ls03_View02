using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Windows.Forms;

using Microsoft.Data.SqlClient;

namespace ls02_PicTable04
{
  public partial class StartForm : Form
  {
    #region Constants
    public static int ImageSize { get; } = 300;
    public static int SmallSize { get; } = 60;
    
    string authorTable = "Authors";
    string authorQuery = "select * from Authors";
    
    string pictureTable = "Pictures";
    string pictureQuery = "select * from Pictures";

    string booksTable = "Books"; 
    string query =
      "select Books.Id, Books.Title, Books.AuthorId, " +
      "(Authors.FirstName + ' ' + Authors.LastName) as Author, " +
      "Books.Price, Books.Pages, Pictures.Picture " +
      "from Books " +
      "join Authors on Books.AuthorId = Authors.Id " +
      "left join Pictures on Pictures.BookId = Books.Id";
    string deleteQuery = "delete from Books " +
      "where Id = @Id";
    string updateQuery = "update Books set " +
      "AuthorId = @AuthorId, " +
      "Title = @Title, " +
      "Price = @Price, " +
      "Pages = @Pages " +
      "Where Id = @Id";
    string insertQuery = "insert Books " +
      "(AuthorId, Title, Price, Pages) " +
      "values (@AuthorId, @Title, @Price, @Pages)";
    #endregion

    #region Fields
    SqlConnection connection;
    SqlDataAdapter adapter;
    DataSet dataSet;
    DataTable table;
    DataView view;
    DataViewManager viewManager;
    BindingSource binding;
    DataGridView grid;
        
    PictureBox picture;
    RadioButton sortBook;
    RadioButton sortAuthor;
    RadioButton sortId;
    CheckBox viewPictures;
    #endregion

    public StartForm()
    {
      InitializeComponent();
      CreateControls();

      try
      {
        LoadData();
      }
      catch(Exception ex)
      {
        MessageBox.Show(ex.Message);
      }

      Load += (s, e) => SetForm();
      FormClosed += (s, e) => SaveData();
    }

    void CreateControls()
    {
      grid = new DataGridView();
      grid.Dock = DockStyle.Fill;
      Controls.Add(grid);

      Panel buttonPanel = CreateButtonPanel();
      buttonPanel.Dock = DockStyle.Top;
      Controls.Add(buttonPanel);

      Panel picturePanel = CreatePicturePanel();
      picturePanel.Dock = DockStyle.Right;
      Controls.Add(picturePanel);
    }

    Panel CreateButtonPanel()
    {
      Panel panel = new Panel();
      panel.Height = 40;

      Button authorButton = new _Button(authorTable, 
        (s, e) => ShowAuthors());
      authorButton.Dock = DockStyle.Left;
      panel.Controls.Add(authorButton);

      Button pictureButton = new _Button(pictureTable, 
        (s, e) => ShowPictures());
      pictureButton.Dock = DockStyle.Right;
      panel.Controls.Add(pictureButton);

      Button booksButton = new _Button(booksTable, 
        (s, e) => AddBooks());
      booksButton.Dock = DockStyle.Fill;
      panel.Controls.Add(booksButton);

      return panel;
    }

    Panel CreatePicturePanel()
    {
      Panel panel = new Panel();

      picture = new PictureBox();
      picture.Size = new Size(ImageSize, ImageSize);
      panel.Controls.Add(picture);

      sortId = new RadioButton();
      sortId.Text = "Sort by Id";
      sortId.Dock = DockStyle.Bottom;
      panel.Controls.Add(sortId);

      sortAuthor = new RadioButton();
      sortAuthor.Text = "Sort by Author";
      sortAuthor.Dock = DockStyle.Bottom;
      sortAuthor.CheckedChanged += (s, e) => //SortGrid();
        view.Sort = sortBook.Checked ? "Title" :
        sortAuthor.Checked ? "Author" : "Id";
      panel.Controls.Add(sortAuthor);

      sortBook = new RadioButton();
      sortBook.Text = "Sort by Book";
      sortBook.Dock = DockStyle.Bottom;
      panel.Controls.Add(sortBook);

      viewPictures = new CheckBox();
      viewPictures.Text = "View only with pictures";
      viewPictures.Dock = DockStyle.Bottom;
      viewPictures.Height = 40;
      viewPictures.CheckedChanged += (s, e) => //SelectGrid();
        view.RowFilter = viewPictures.Checked ? "Picture is not null" : "";
      panel.Controls.Add(viewPictures);

      panel.Width = picture.Width;

      return panel;
    }
    /*
    void SortGrid()
    {
      view.Sort = sortBook.Checked ? "Title" :
        sortAuthor.Checked ? "Author" : "Id";
      //binding.ResetBindings(false);
    }

    void SelectGrid()
    {
      view.RowFilter = viewPictures.Checked ? "Picture is not null" : "";
    }
    */
    void LoadData()
    {
      var connectionString = ConfigurationManager.AppSettings
        .Get("connectionString");
      connection = new SqlConnection(connectionString);
      FillData();
    }

    void FillData()
    {
      adapter = CreateAdapter();
      dataSet = new();
      adapter.Fill(dataSet, booksTable);
      table = dataSet.Tables[booksTable];
      viewManager = new(dataSet);
      viewManager.DataViewSettings[booksTable].RowFilter = "";
      viewManager.DataViewSettings[booksTable].Sort = "";
      view = viewManager.CreateDataView(table);
      binding = new BindingSource();
      binding.DataSource = view;
    }

    SqlDataAdapter CreateAdapter()
    {
      SqlDataAdapter dataAdapter = 
        new SqlDataAdapter(query, connection);
            
      dataAdapter.UpdateCommand = new SqlCommand(updateQuery, connection);
      dataAdapter.UpdateCommand.Parameters
          .Add(new SqlParameter("@Id", SqlDbType.Int, 4, "Id"));
      dataAdapter.UpdateCommand.Parameters["@Id"]
          .SourceVersion = DataRowVersion.Original;
      dataAdapter.UpdateCommand.Parameters
          .Add(new SqlParameter("@AuthorId", SqlDbType.Int, 4, "AuthorId"));
      dataAdapter.UpdateCommand.Parameters
          .Add(new SqlParameter("@Title", SqlDbType.VarChar, 100, "Title"));
      dataAdapter.UpdateCommand.Parameters
          .Add(new SqlParameter("@Price", SqlDbType.Int, 4, "Price"));
      dataAdapter.UpdateCommand.Parameters
          .Add(new SqlParameter("@Pages", SqlDbType.Int, 4, "Pages"));

      dataAdapter.InsertCommand = new SqlCommand(insertQuery, connection);
      dataAdapter.InsertCommand.Parameters
          .Add(new SqlParameter("@AuthorId", SqlDbType.Int, 4, "AuthorId"));
      dataAdapter.InsertCommand.Parameters
          .Add(new SqlParameter("@Title", SqlDbType.VarChar, 100, "Title"));
      dataAdapter.InsertCommand.Parameters
          .Add(new SqlParameter("@Price", SqlDbType.Int, 4, "Price"));
      dataAdapter.InsertCommand.Parameters
          .Add(new SqlParameter("@Pages", SqlDbType.Int, 4, "Pages"));

      dataAdapter.DeleteCommand = new SqlCommand(deleteQuery, connection);
      dataAdapter.DeleteCommand.Parameters
    .Add(new SqlParameter("@Id", SqlDbType.Int, 4, "Id"));
      dataAdapter.DeleteCommand.Parameters["@Id"]
          .SourceVersion = DataRowVersion.Original;

      return dataAdapter;
    }

    void ShowAuthors()
    {
      GridForm authorForm = new GridForm(connection, authorQuery);
      authorForm.ShowDialog();
    }

    void ShowPictures()
    {
      GridForm pictureForm = new PictureForm(connection, pictureQuery);
      pictureForm.ShowDialog();
    }

    void AddBooks()
    {
      BookForm bookForm = new BookForm(connection); 
      if (bookForm.ShowDialog() == DialogResult.OK)
      {
        DataRow row = table.NewRow();
        row["Title"] = bookForm.Title;
        row["AuthorId"] = bookForm.AuthorId;
        row["Author"] = bookForm.Author;
        row["Price"] = bookForm.Price;
        row["Pages"] = bookForm.Pages;
        table.Rows.Add(row);
        binding.ResetBindings(false);
      }
    }

    void SaveData()
    {
      adapter.Update(table);
    }
    
    void SetForm()
    {
      Text = "Data Table and Data View";
      this.StartPosition = FormStartPosition.CenterScreen;

      grid.DataSource = binding;
      grid.Columns["Id"].ReadOnly = true;
      grid.Columns["AuthorId"].Visible = false;
      grid.Columns["Picture"].Visible = false;
      grid.AllowUserToAddRows= false;

      grid.RowEnter += Grid_RowEnter;
    }

    private void Grid_RowEnter(object? sender, DataGridViewCellEventArgs e)
    {
      if(DBNull.Value.Equals(grid.Rows[e.RowIndex].Cells["Picture"].Value))
      {
        picture.Image = null;
      }
      else
      {
        byte[]? image = (byte[])grid.Rows[e.RowIndex].Cells["Picture"].Value;
        using(var ms = new MemoryStream(image))
        {
          picture.Image = Image.FromStream(ms);
        }
      }
    }
  }
}

internal class _Button : Button
{
  public _Button(string text, EventHandler handler)
  {
    Text = text;
    Size = new Size(80, 10); 
    Click += handler;
  }
}

internal class _Label : Label
{
  public _Label(string text, Point location)
  {
    Text = text;
    Location = location;
  }
}

internal class _TextBox : TextBox
{
  public _TextBox(int width, Point location)
  {
    Width = width;
    Location = location;
  }
}