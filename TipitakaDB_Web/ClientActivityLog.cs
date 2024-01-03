using Tipitaka_DBTables;

namespace Tipitaka_DB
{
    public class ClientActivityLog : TipitakaDB_w
    {
        const string _ActivityLog_ = "ActivityLog";
        //*******************************************************************
        //*** ClientActivityLog
        //*******************************************************************
        public ClientActivityLog() : base(_ActivityLog_) { }
        //*******************************************************************
        //*** Add Activity Log
        //*******************************************************************
        public void AddActivityLog(string userID, string DocID, string activity, int startPage, int endPage)
        {
            string rowKey = userID + "-" + DocID;

            ActivityLog activityLog = new ActivityLog()
            {
                PartitionKey = "ActivityLog",
                RowKey = rowKey,
                DocID = DocID,
                Activity = activity,
                PageRanges = string.Format("{0}-{1}", startPage, endPage),
                Pages = endPage - startPage + 1
            };
            InsertTableRec(activityLog).Wait();
        }
        //*******************************************************************
        //*** GetActivities
        //*******************************************************************
        public List<string> GetActivities(string userID = "", DateTime? date1 = null, DateTime? date2 = null)
        {
            List<string> activities = new List<string>();
            string query = string.Empty;
            if (userID.Length > 0)
            {
                query += String.Format(" and (UserID eq '{0}')", userID);
            }
            if (date1 != null && date2 != null) 
            { 
                string s = "";
                string[] f = s.Split('|');
                if (f.Length >= 2)
                {
                    query += String.Format(" and (RowKey ge '{0}') and (RowKey lt '{1}')", f[0], f[1]);
                }
            }
            QueryTableRec(query).Wait();
            activities = (List<string>)objResult;
            return activities;
        }
    }
}
