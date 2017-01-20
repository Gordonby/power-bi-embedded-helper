using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.PowerBI.Api.V1;
using Microsoft.PowerBI.Security;
using Microsoft.Rest;
using paas_demo.Models;

namespace paas_demo.Controllers
{
    public class DashboardController : Controller
    {
        private readonly string workspaceCollection;
        //private readonly string workspaceId;
        private readonly string accessKey;
        private readonly string apiUrl;

        public DashboardController()
        {
            this.workspaceCollection = ConfigurationManager.AppSettings["powerbi:WorkspaceCollection"];
            //this.workspaceId = ConfigurationManager.AppSettings["powerbi:WorkspaceId"];
            this.accessKey = ConfigurationManager.AppSettings["powerbi:AccessKey"];
            this.apiUrl = ConfigurationManager.AppSettings["powerbi:ApiUrl"];
        }

        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult Reports()
        {
            using (var client = this.CreatePowerBIClient())
            {
                var workspaces = client.Workspaces.GetWorkspacesByCollectionName(this.workspaceCollection);
                var viewModel = new WorkspaceReportsViewModel() { WorkspaceReports = new System.Collections.Generic.List<ReportsViewModel>() };
                
                foreach (var workspace in workspaces.Value)
                {
                    var reportsResponse = client.Reports.GetReports(this.workspaceCollection, workspace.WorkspaceId);

                    //var t1 = new ReportsViewModel();
                    //t1.Reports = reportsResponse.Value.ToList();
                    //t1.WorkspaceId = workspace.WorkspaceId;
                    //viewModel.WorkspaceReports.Add(t1);

                    //var t2 = new ReportsViewModel()
                    //{
                    //    Reports = reportsResponse.Value.ToList(),
                    //    WorkspaceId = workspace.WorkspaceId
                    //};
                    //viewModel.WorkspaceReports.Add(t2);

                    viewModel.WorkspaceReports.Add( new ReportsViewModel()
                    {
                        Reports = reportsResponse.Value.ToList(),
                        WorkspaceId = workspace.WorkspaceId
                    });
                }

                return PartialView(viewModel);
            }
        }

        public async Task<ActionResult> Report(string reportId, string workspaceId)
        {
            using (var client = this.CreatePowerBIClient())
            {
                var reportsResponse = await client.Reports.GetReportsAsync(this.workspaceCollection, workspaceId);
                var report = reportsResponse.Value.FirstOrDefault(r => r.Id == reportId);
                var embedToken = PowerBIToken.CreateReportEmbedToken(this.workspaceCollection, workspaceId, report.Id);

                var viewModel = new ReportViewModel
                {
                    Report = report,
                    AccessToken = embedToken.Generate(this.accessKey)
                };

                return View(viewModel);
            }
        }

        private IPowerBIClient CreatePowerBIClient()
        {
            var credentials = new TokenCredentials(accessKey, "AppKey");
            var client = new PowerBIClient(credentials)
            {
                BaseUri = new Uri(apiUrl)
            };

            return client;
        }
    }
}