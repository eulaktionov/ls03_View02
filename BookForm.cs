using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Data.SqlClient;

namespace ls02_PicTable04
{
  public partial class BookForm : Form
  {
    string authorTable = "Authors";
    string authorQuery = "select Id, " +
      "(Authors.FirstName + ' ' + Authors.LastName) as Author " +
      "from Authors";
    SqlDataAdapter adapter;
    DataTable table;

    public string? Title { get; set; }
    public int? Price { get; set; }
    public int? Pages { get; set; }
    public int? AuthorId { get; set; }
    public string? Author { get; set; }

    TextBox title;
    TextBox price;
    TextBox pages;
    ListBox authors;
    ErrorProvider errorHandler;
    bool isOk;

    public BookForm(SqlConnection connection)
    {
      InitializeComponent();
      CreateControls();

      try
      {
        LoadData(connection);
      }
      catch(Exception ex)
      {
        MessageBox.Show(ex.Message);
      }

      StartPosition = FormStartPosition.CenterParent;
      FormClosing += BookForm_FormClosing;
    }

    private void BookForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
      if(!isOk) return;

      if(IsTitleValid() && IsPriceValid() && 
        IsPagesValid() && IsAuthorValid())
      {
        Title = title.Text;
        Price = int.Parse(price.Text);
        Pages = int.Parse(pages.Text);
        AuthorId = (int)authors.SelectedValue;
        Author = table.Rows[authors.SelectedIndex]["Author"].ToString();
      }
      else
      {
        isOk = false;
        e.Cancel = true;
      }
    }

    bool IsTitleValid()
    {
      if(string.IsNullOrEmpty(title.Text))
      {
        errorHandler.SetError(title, "Title error!");
        title.Focus();
        return false;
      }
      
      errorHandler.SetError(title, "");
      errorHandler.Clear();
      return true;
    }

    bool IsPriceValid()
    {
      if(!int.TryParse(price.Text, out _))
      {
        errorHandler.SetError(price, "Price error!");
        price.Focus();
        return false;
      }
      return true;
    }

    bool IsPagesValid()
    {
      if(!int.TryParse(pages.Text, out _))
      {
        errorHandler.SetError(pages, "Pages error!");
        pages.Focus();
        return false;
      }
      return true;
    }

    bool IsAuthorValid()
    {
      if(authors.SelectedIndex == -1)
      {
        errorHandler.SetError(authors, "Authors error!");
        authors.Focus();
        return false;
      }
      return true;
    }

    void CreateControls()
    {
      int offset = 10;
      int width = 200;

      authors = new();
      authors.Width = width;
      authors.Dock = DockStyle.Right;
      Controls.Add(authors);

      Panel buttonPanel = CreateButtonPanel();
      buttonPanel.Dock = DockStyle.Top;
      Controls.Add(buttonPanel);

      var labelTitle = new _Label("Title",
        new Point(offset, buttonPanel.Height + offset));
      Controls.Add(labelTitle);
      title = new _TextBox(width,
        new Point(offset, labelTitle.Bottom));
      Controls.Add(title);

      var labelPrice = new _Label("Price",
        new Point(offset, title.Bottom + offset));
      Controls.Add(labelPrice);
      price = new _TextBox(width,
        new Point(offset, labelPrice.Bottom));
      price.Validating += (s, e) => CheckPrice();
      Controls.Add(price);

      var labelPages = new _Label("Pages",
        new Point(offset, price.Bottom + offset));
      Controls.Add(labelPages);
      pages = new _TextBox(width,
        new Point(offset, labelPages.Bottom));
      Controls.Add(pages);

      errorHandler = new ErrorProvider(this);
    }

    Panel CreateButtonPanel()
    {
      Panel panel = new Panel();
      panel.Height = 40;

      Button OkButton = new _Button("Ok",
        (s, e) => isOk = true);
      OkButton.Dock = DockStyle.Left;
      OkButton.DialogResult = DialogResult.OK;
      panel.Controls.Add(OkButton);

      Button cancelButton = new _Button("Cansel", null);
      cancelButton.Dock = DockStyle.Right;
      cancelButton.DialogResult = DialogResult.Cancel;
      panel.Controls.Add(cancelButton);

      return panel;
    }

    void LoadData(SqlConnection connection)
    {
      adapter = new SqlDataAdapter(authorQuery, connection);
      table = new();
      adapter.Fill(table);
      authors.DataSource = table;
      authors.DisplayMember = "Author";
      authors.ValueMember = "Id";
    }

    void CheckPrice()
    {
      if (string.IsNullOrEmpty(price.Text))
      {
        errorHandler.SetError(price, "Price error!");
        price.Focus();
      }
    }
  }
}
