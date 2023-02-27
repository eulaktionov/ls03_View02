using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;

namespace ls02_PicTable04
{
  internal class PictureForm : GridForm
  {

    public PictureForm(SqlConnection connection, string query)
      : base(connection, query)
    {
    }

    protected override void SetGrid()
    {
      base.SetGrid();

      grid.Columns["Picture"].Visible = false;

      grid.CellMouseDoubleClick += Grid_CellMouseDoubleClick;
      grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
      foreach(DataGridViewRow row in grid.Rows)
      {
        row.Height = StartForm.SmallSize;
      }
    }

    private void Grid_CellMouseDoubleClick(object? sender, DataGridViewCellMouseEventArgs e)
    {
      OpenFileDialog dialog = new ();
      dialog.Filter = "*.png|*.png|*.jpeg|*.jpeg|*.*|*.*";
      if (dialog.ShowDialog() == DialogResult.OK)
      {
        Bitmap inImage = new(dialog.FileName);
        var smallImage = resizeImage(inImage, new Size(StartForm.SmallSize, StartForm.SmallSize));
        grid.Rows[e.RowIndex].Cells["SmallPicture"].Value = smallImage;

        grid.Rows[e.RowIndex].Height = smallImage.Height;
        grid.Columns[e.ColumnIndex].Width = smallImage.Width;
        
        var image = resizeImage(inImage, new Size(StartForm.ImageSize, StartForm.ImageSize));
        grid.Rows[e.RowIndex].Cells["Picture"].Value = image;
      }
    }

    private static Image resizeImage(Image imgToResize, Size size)
    {
      int sourceWidth = imgToResize.Width;
      int sourceHeight = imgToResize.Height;

      float nPercent = 0;
      float nPercentW = 0;
      float nPercentH = 0;

      nPercentW = ((float)size.Width / (float)sourceWidth);
      nPercentH = ((float)size.Height / (float)sourceHeight);
      if(nPercentH < nPercentW)
        nPercent = nPercentH;
      else
        nPercent = nPercentW;

      int destWidth = (int)(sourceWidth * nPercent);
      int destHeight = (int)(sourceHeight * nPercent);

      Bitmap b = new Bitmap(destWidth, destHeight);
      Graphics g = Graphics.FromImage((System.Drawing.Image)b);
      g.InterpolationMode = InterpolationMode.HighQualityBicubic;

      g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
      g.Dispose();
      return (Image)b;
    }
  }
}
