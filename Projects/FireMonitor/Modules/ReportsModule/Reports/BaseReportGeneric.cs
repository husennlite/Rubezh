﻿using System.Collections.Generic;
using CrystalDecisions.CrystalReports.Engine;
using FiresecAPI.Models;
using FiresecClient;
using SAPBusinessObjects.WPF.Viewer;

namespace ReportsModule.Reports
{
    public class BaseReportGeneric<T> : BaseReport
    {
        protected string ReportFileName;
        protected ReportDocument reportDocument;

        public BaseReportGeneric()
        {
            DataList = new List<T>();
            reportDocument = new ReportDocument();
        }

        protected List<T> _dataList;
        public List<T> DataList { get; protected set; }

        public override ReportDocument CreateCrystalReportDocument()
        {
            reportDocument.Load(FileHelper.GetReportFilePath(ReportFileName));
            reportDocument.SetDataSource(DataList);
            return reportDocument;
        }
    }
}