using Syncfusion.Blazor.Data;
using Tipitaka_DBTables;

namespace Tipitaka_DB
{
    public class ClientTimesheet : TipitakaDB_w
    {
        const string _Timesheet_ = "Timesheet";
        //*******************************************************************
        //*** ClientActivityLog
        //*******************************************************************
        public ClientTimesheet() : base(_Timesheet_) { }
        const string _timeFormat_ = "yyyy-MM-ddTHH:mm:ss.ffff";
        //*******************************************************************
        //*** Add Timesheet
        //*******************************************************************
        public void AddTimesheet(int idx, string userID, DateTime startTime, DateTime endTime, string docNo, 
            string task, string desc, int startPage, int endPage)
        {
            string rowKey = String.Format("{0}${1}${2}", userID, startTime.ToString("yyyy-MM-dd"), idx);

            Timesheet timesheet = new Timesheet()
            {
                
                PartitionKey = _Timesheet_,
                RowKey = rowKey,
                StartTime = startTime,
                EndTime = endTime,
                DocNo = docNo,
                Task = task,
                Description = desc,
                StartPage = startPage,
                EndPage = endPage
            };
            InsertTableRec(timesheet).Wait();
        }
        //*******************************************************************
        //*** GetTimesheet
        //*******************************************************************
        public List<Timesheet> GetTimesheet(string userID, DateTime date1, DateTime date2)
        {
            List<Timesheet> activities = new List<Timesheet>();
            string query = string.Empty;

            string d1 = String.Format("{0}${1}$", userID, date1.ToString("yyyy-MM-dd"));
            string d2 = String.Format("{0}${1}$~", userID, date2.ToString("yyyy-MM-dd"));
            query += String.Format("(RowKey ge '{0}') and (RowKey lt '{1}')", d1, d2);

            QueryTableRec(query).Wait();
            activities = (List<Timesheet>)objResult;
            activities.Reverse();
            return activities;
        }
        public List<Timesheet> GetTimesheetMonth(string userID)
        {
            List<Timesheet> activities = new List<Timesheet>();
            string query = string.Empty;
            DateTime date1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime date2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            string d1 = String.Format("{0}${1}$", userID, date1.ToString("yyyy-MM-dd"));
            string d2 = String.Format("{0}${1}$~", userID, date2.ToString("yyyy-MM-dd"));
            query += String.Format("(RowKey ge '{0}') and (RowKey lt '{1}')", d1, d2);

            QueryTableRec(query).Wait();
            activities = (List<Timesheet>)objResult;
            activities.Reverse();
            return activities;
        }
        //*******************************************************************
        //*** Delete Activities
        //*******************************************************************
        const string _CleanUp_ = "CleanUp";
        public void DeleteOldTimesheet(string userID, string userName)
        {
            //if (HasOldActivitiesDeleted()) return;
            //const int daysRetained = 7;
            //string todayUTC = DateTime.UtcNow.ToString("yyyy-MM-ddT00:00:00.0000");
            //string pastDay7UTC = DateTime.UtcNow.AddDays(-daysRetained).ToString("yyyy-MM-ddT00:00:00.0000");
            ////string checkUTC = todayUTC;

            //string query = String.Format("RowKey lt '{0}'", pastDay7UTC);
            //QueryTableRec(query).Wait();
            //List<ActivityLog> activities = (List<ActivityLog>)objResult;
            //if (activities.Count > 0)
            //{
            //    DeleteTableRecBatch(objResult).Wait();
            //    AddActivityLog(userID, _CleanUp_, String.Format("Deleted {0} old activities.", activities.Count));
            //}
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
