using System.Drawing.Printing;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.XtraReports.UI;

namespace LTFYYJXT
{
    public partial class XtraReport1 : XtraReport
    {
        private ObjectDataSource objectDataSource1;

        public XtraReport1()
        {
            InitializeComponent();
            //objectDataSource1.DataSource = typeof(DataValue);
            //this.Report.DataSource = objectDataSource1;
        }

        private void xrRichText1_BeforePrint(object sender, PrintEventArgs e)
        {
        }
    }
}