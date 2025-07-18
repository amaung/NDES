using Syncfusion.Blazor.Data;
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
        const string _timeFormat_ = "yyyy-MM-ddTHH:mm:ss.ffff";
        //*******************************************************************
        //*** Add Activity Log
        //*******************************************************************
        public void AddActivityLog(string userID, string activity, string desc)
        {
            string rowKey = DateTime.UtcNow.ToString(_timeFormat_);

            ActivityLog activityLog = new ActivityLog()
            {
                PartitionKey = "ActivityLog",
                RowKey = rowKey,
                UserID = userID,
                Activity = activity,
                Description = desc,
            };
            InsertTableRec(activityLog).Wait();
        }
        public async Task AddActivityLogAsync(string userID, string activity, string desc)
        {
            string rowKey = DateTime.UtcNow.ToString(_timeFormat_);

            ActivityLog activityLog = new ActivityLog()
            {
                PartitionKey = "ActivityLog",
                RowKey = rowKey,
                UserID = userID,
                Activity = activity,
                Description = desc,
            };
            await InsertTableRec(activityLog);
        }
        //*******************************************************************
        //*** GetActivities
        //*******************************************************************
        public List<ActivityLog> GetActivities(DateTime date1, DateTime date2)
        {
            List<ActivityLog> activities = new List<ActivityLog>();
            string query = string.Empty;

            string d1 = date1.ToString(_timeFormat_);
            string d2 = date2.ToString(_timeFormat_);
            query += String.Format("(RowKey ge '{0}') and (RowKey lt '{1}')", d1, d2);

            QueryTableRec(query).Wait();
            activities = (List<ActivityLog>)objResult;
            activities.Reverse();
            return activities;
        }
        public async Task<List<ActivityLog>> GetActivitiesByDocNoAsync(string docNo)
        {
            List<ActivityLog> activities = new List<ActivityLog>();
            string query = string.Empty;

            query += String.Format("DocNo eq '{0}", docNo);
            await QueryTableRec(query);
            activities = (List<ActivityLog>)objResult;
            activities.Reverse();
            return activities;
        }
        //*******************************************************************
        //*** Delete Activities
        //*******************************************************************
        const string _CleanUp_ = "CleanUp";
        public void DeleteOldActivities(string userID, string userName)
        {
            if (HasOldActivitiesDeleted()) return;
            const int daysRetained = 7;
            string todayUTC = DateTime.UtcNow.ToString("yyyy-MM-ddT00:00:00.0000");
            string pastDay7UTC = DateTime.UtcNow.AddDays(-daysRetained).ToString("yyyy-MM-ddT00:00:00.0000");
            //string checkUTC = todayUTC;

            string query = String.Format("RowKey lt '{0}'", pastDay7UTC);
            QueryTableRec(query).Wait();
            List<ActivityLog> activities = (List<ActivityLog>)objResult;
            if (activities.Count > 0)
            {
                DeleteTableRecBatch(objResult).Wait();
                AddActivityLog(userID, _CleanUp_, String.Format("Deleted {0} old activities.", activities.Count));
            }
            return;
        }
        //*******************************************************************
        //*** Delete Activities
        //*******************************************************************
        public bool HasOldActivitiesDeleted()
        {
            string todayUTC = DateTime.UtcNow.ToString("yyyy-MM-dd");
            string nextdayUTC = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd");
            string qualifier = String.Format("(RowKey gt '{0}') and (RowKey lt '{1}') and (Activity eq '{2}')", todayUTC, nextdayUTC, _CleanUp_);
            QueryTableRec(qualifier).Wait();
            List<ActivityLog> listActivities = (List<ActivityLog>)objResult;
            return listActivities.Count == 1 ? true : false;
        }
        public void AddTestData()
        {
            DateTime todayUTC = DateTime.UtcNow;
            ActivityLog activityLog = new ActivityLog();
            for (int i = 0; i < 10; i++)
            {
                todayUTC = todayUTC.AddDays(-1);
                activityLog.RowKey = todayUTC.ToString("yyyy-MM-dd");
                activityLog.UserID = "auzm2002@yahoo.com";
                activityLog.Activity = "TestData";
                activityLog.Description = "To be deleted";
                InsertTableRec(activityLog).Wait();
            }
        }
        public void DeleteAll(string userID)
        {
            if (userID == null || userID != "dhammayaungchi2011@gmail.com") return;
            string query = "";
            QueryTableRec(query).Wait();
            List<ActivityLog> activities = (List<ActivityLog>)objResult;
            if (activities.Count > 0)
            {
                DeleteTableRecBatch(objResult).Wait();
            }
        }
    }
}
