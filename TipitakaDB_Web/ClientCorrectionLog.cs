using Tipitaka_DBTables;

namespace Tipitaka_DB
{
    public class ClientCorrectionLog : TipitakaDB_w
    {
        const string _CorrectionLog_ = "CorrectionLog";
        //*******************************************************************
        //*** ClientActivityLog
        //*******************************************************************
        public ClientCorrectionLog() : base(_CorrectionLog_) { }
        const string _timeFormat_ = "yyyy-MM-ddTHH:mm:ss.ffff";
        //*******************************************************************
        //*** Add Correction Log
        //*******************************************************************
        public void AddCorrectionLog(string userID, string activity, string desc)
        {
            string rowKey = DateTime.UtcNow.ToString(_timeFormat_);

            CorrectionLog correctionLog = new CorrectionLog()
            {
                PartitionKey = "CorrectionLog",
                RowKey = rowKey,
                UserID = userID,
            };
            InsertTableRec(correctionLog).Wait();
        }
        //*******************************************************************
        //*** GetCorrectionLog
        //*******************************************************************
        public List<CorrectionLog> GetActivities(DateTime date1, DateTime date2)
        {
            List<CorrectionLog> activities = new List<CorrectionLog>();
            string query = string.Empty;
 
            string d1 = date1.ToString(_timeFormat_);
            string d2 = date2.ToString(_timeFormat_);
            query += String.Format("(RowKey ge '{0}') and (RowKey lt '{1}')", d1, d2);

            QueryTableRec(query).Wait();
            activities = (List<CorrectionLog>)objResult;
            activities.Reverse();
            return activities;
        }
        //*******************************************************************
        //*** Query COrrections
        //*******************************************************************
        public List<CorrectionLog> QueryCorrections(string docNo)
        {
            string query = String.Format("RowKey gt '{0}-000' and RowKey lt '{0}-999'", docNo);
            QueryTableRec(query).Wait();
            List<CorrectionLog> corrections = (List<CorrectionLog>)objResult;
            return corrections;
        }
        //*******************************************************************
        //*** Delete Activities
        //*******************************************************************
    }
}
