using Tipitaka_DB;
using static NissayaEditor_Web.Data.NissayaEditor;

namespace NissayaEditor_Web.Data
{
    public class State
    {
        //public List<NIS> NISRecordsCollection { get; set; } = new List<NIS>();
        public ClientTipitakaDB_w? clientTipitakaDB = null;
        public DataFile? dataFile = null;
        public int? screenID = null;
        public List<NIS>? NISRecords = null;
        public List<PageData>? Pages = null;
        public string doc_ID = "";
        public string doc_Title = "";
        public string page_No = "";
        public int selectedPageIdx = -1;
    }
}
