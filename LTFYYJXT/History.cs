using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LTFYYJXT
{
    public partial class History : Form
    {
        public DataRow dr;
        public DataTable dt;

        public History()
        {
            InitializeComponent();
        }



        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            if (e.Clicks == 2)
            {
              //MessageBox.Show(  gridView1.GetFocusedDataSourceRowIndex().ToString());
                dr = dt.Rows[gridView1.GetFocusedDataSourceRowIndex()];
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void History_Load(object sender, EventArgs e)
        {
            if (dt != null)
            gridControl1.DataSource = dt;
        }
    }
}
