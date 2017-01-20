﻿using System.Collections.Generic;
using Microsoft.PowerBI.Api.V1.Models;

namespace paas_demo.Models
{
    public class ReportsViewModel
    {
        public List<Report> Reports { get; set; }
        public string WorkspaceId { get; set; }
    }

    public class WorkspaceReportsViewModel
    {
        public List<ReportsViewModel> WorkspaceReports { get; set; }
    }
}