using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
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
        public Timesheet AddTimesheet(string rowKey, DateTime selectedDate, DateTime startTime, DateTime endTime, string docNo, 
            string task, string desc, int startPage, int endPage, string status)
        {
            Timesheet timesheet = new Timesheet()
            {
                
                PartitionKey = _Timesheet_,
                RowKey = rowKey,
                StartTime = startTime.ToUniversalTime(),
                EndTime = endTime.ToUniversalTime(),
                DocNo = docNo,
                Task = task,
                Description = desc,
                StartPage = startPage,
                EndPage = endPage,
                Status = status 
            };
            InsertTableRec(timesheet).Wait();
            return timesheet;
        }
        public Timesheet AddTimesheet(DateTime selectedDate, string userID, DateTime startTime, DateTime endTime, string docNo,
            string task, string desc, int startPage, int endPage, string status)
        {
            Timesheet timesheet = new Timesheet()
            {

                PartitionKey = _Timesheet_,
                RowKey = DateTime.Now.Ticks.ToString(),
                Date = selectedDate.ToString("yyyy-MM-dd"),
                UserID = userID,
                StartTime = startTime.ToUniversalTime(),
                EndTime = endTime.ToUniversalTime(),
                DocNo = docNo,
                Task = task,
                Description = desc,
                StartPage = startPage,
                EndPage = endPage,
                Status = status
            };
            InsertTableRec(timesheet).Wait();
            return timesheet;
        }
        public void AddTimesheet(Timesheet timesheet)
        {
            InsertTableRec(timesheet).Wait();
        }
        //*******************************************************************
        //*** GetTimesheet
        //*******************************************************************
        public List<Timesheet> GetMonthTimesheet(string userID, int year, int mon)
        {
            List<Timesheet> timesheets = new List<Timesheet>();
            string query = string.Empty;
            string d1 = String.Format("{0}-{1}-01", year.ToString("d4"), mon.ToString("d2"));
            string d2 = String.Format("{0}-{1}-32", year.ToString("d4"), mon.ToString("d2"));
            if (userID != null && userID.Length == 0) query += String.Format("(Date gt '{0}' and Date lt '{1}')", d1, d2);
            else query += String.Format("(UserID eq '{0}' and Date ge '{1}' and Date lt '{2}')", userID, d1, d2);

            QueryTableRec(query).Wait();
            timesheets = (List<Timesheet>)objResult;
            foreach(Timesheet timesheet in timesheets)
            {
                timesheet.StartTime = timesheet.StartTime.ToLocalTime();
                timesheet.EndTime = timesheet.EndTime.ToLocalTime();
            }
            return timesheets;
        }
        public List<Timesheet> GetTimesheetRange(DateTime date1, DateTime date2)
        {
            List<Timesheet> timesheets = new List<Timesheet>();
            string query = String.Format("(Date ge '{0}') and (Date le '{1}')", date1.ToString("yyyy-MM-dd"), date2.ToString("yyyy-MM-dd"));

            QueryTableRec(query).Wait();
            timesheets = (List<Timesheet>)objResult;
            return timesheets;
        }
        public void UpdateTimesheet(Timesheet timesheet)
        {
            timesheet.StartTime = timesheet.StartTime.ToUniversalTime();
            timesheet.EndTime = timesheet.EndTime.ToUniversalTime();
            UpdateTableRec(timesheet).Wait();
        }
        public void UpdateTimesheetStatus(Timesheet timesheet)
        {
            UpdateTableRec(timesheet).Wait();
            //if (timesheet != null)
            //{
            //    timesheet.Status = status;
            //    UpdateTableRec (timesheet).Wait();
            //}
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
