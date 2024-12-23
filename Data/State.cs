﻿using Microsoft.AspNetCore.Components.Forms;
using Tipitaka_DB;
using Tipitaka_DBTables;
using static NissayaEditor_Web.Data.NissayaEditor;

namespace NissayaEditor_Web.Data
{
    public class State
    {
        //public List<NIS> NISRecordsCollection { get; set; } = new List<NIS>();
        public ClientTipitakaDB_w? clientTipitakaDB = null;
        public DataFile? dataFile = null;
        public int? screenID = -1;
        public List<NIS>? NISRecords = null;
        public List<PageData>? Pages = null;
        public string doc_ID = "";
        public string doc_Title = "";
        public string page_No = "";
        public int selectedPageIdx = -1;
        public string email = string.Empty;
        public string userName = string.Empty;
        public string userName_M = string.Empty;
        public Dictionary<string, string> UserTaskInfo = new Dictionary<string, string>();
        public List<DocReportInfo> listDocReportInfo = new List<DocReportInfo>();
        public List<DocSelectInfo>? ReadDocGridList = new List<DocSelectInfo>();
        public string ReadDocSearchPattern = string.Empty;
        public IBrowserFile? importFile = null;
        public InputFileChangeEventArgs? inputFileChangeEvent = null;
    }
}
