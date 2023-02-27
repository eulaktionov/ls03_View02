using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

using Microsoft.Data.SqlClient;

namespace ls02_PicTable04
{
  public partial class GridForm : Form
  {
    SqlDataAdapter adapter;
    DataTable table;
    protected DataGridView grid;

    public GridForm(SqlConnection connection, string query)
    {
      InitializeComponent();
      grid = new();
      Controls.Add(grid);
      FillData(connection, query);
      StartPosition = FormStartPosition.CenterParent;
      Load += (s, e) => SetGrid();
      FormClosed += (s, e) => SaveData();
    }

    void FillData(SqlConnection connection, string query)
    {
      try
      {
        adapter = new SqlDataAdapter(query, connection);
        var commandBuilder = new SqlCommandBuilder(adapter);
        table = new();
        adapter.Fill(table);
      }
      catch(Exception ex)
      {
        MessageBox.Show(ex.Message);
      }
    }

    virtual protected void SetGrid()
    {
      grid.Dock = DockStyle.Fill;
      grid.DataSource = table;
      grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

      grid.Columns["Id"].ReadOnly = true;
      grid.Columns["Id"].HeaderText = "Код";
      grid.Columns["Id"].DefaultCellStyle.Alignment =
          DataGridViewContentAlignment.MiddleCenter;

      grid.Columns["Id"].DisplayIndex = 0;
    }

    void SaveData()
    {
      adapter.Update(table);
    }
  }
}
