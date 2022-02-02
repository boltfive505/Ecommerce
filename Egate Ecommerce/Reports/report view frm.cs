using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace Egate_Ecommerce.Reports
{
    public partial class report_view_frm : Form
    {
        private ReportViewer rv;

        public report_view_frm(string reportFileName)
        {
            InitializeComponent();

            rv = new ReportViewer();
            this.Controls.Add(rv);

            rv.Dock = DockStyle.Fill;
            rv.PageCountMode = PageCountMode.Actual;
            rv.ProcessingMode = ProcessingMode.Local;
            rv.SetDisplayMode(DisplayMode.PrintLayout);
            rv.ShowBackButton = false;
            rv.ShowCredentialPrompts = false;
            rv.ShowDocumentMapButton = false;
            rv.ShowFindControls = false;
            rv.ShowRefreshButton = false;
            rv.ShowStopButton = false;
            rv.ShowZoomControl = false;
            rv.ReportError += Rv_ReportError;

            LocalReport report = rv.LocalReport;
            report.EnableExternalImages = true;
            report.EnableHyperlinks = true;
            report.ReportEmbeddedResource = @"Egate_Ecommerce.Reports.ReportItems." + reportFileName + ".rdlc";
            report.DataSources.Clear();
        }

        private void Rv_ReportError(object sender, ReportErrorEventArgs e)
        {
            Logs.WriteExceptionLogs(e.Exception);
        }

        private void report_view_frm_Load(object sender, EventArgs e)
        {
            rv.RefreshReport();
        }

        public void LoadDataSet(string datasourceName, object datasourceValue)
        {
            rv.LocalReport.DataSources.Add(new ReportDataSource(datasourceName, datasourceValue));
        }

        public static void ShowReport(string reportFileName, string datasourceName, object datasourceValue)
        {
            report_view_frm.ShowReport(reportFileName, new Dictionary<string, object>() { { datasourceName, datasourceValue } });
        }

        public static void ShowReport(string reportFileName, Dictionary<string, object> datasourceCollection)
        {
            var frm = new report_view_frm(reportFileName);
            foreach (var i in datasourceCollection)
                frm.LoadDataSet(i.Key, i.Value);
            frm.ShowDialog();
        }
    }
}
