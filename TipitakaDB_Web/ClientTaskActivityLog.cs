using Tipitaka_DBTables;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tipitaka_DB
{
    public class ClientTaskActivityLog : TipitakaDB_w
    {
        const string _TaskActivityLog_ = "TaskActivityLog";
        //*******************************************************************
        //*** ClientActivityLog
        //*******************************************************************
        public ClientTaskActivityLog() : base(_TaskActivityLog_) { }
        const string _timeFormat_ = "yyyy-MM-ddTHH:mm:ss.ffff";
        const string _timeFormat2_ = "yyyy-MM-ddTHH:mm:ss.fffffffZ";
        //*******************************************************************
        //*** Add Activity Log
        //*******************************************************************
        public void AddTaskActivityLog(string docNo, string userID, string activity, int pages, int totalSubmitted, 
                int submittedPages, string desc)
        {
            string rowKey = DateTime.UtcNow.ToString(_timeFormat_);
            TaskActivityLog taskActivityLog = new TaskActivityLog()
            {
                PartitionKey = "TaskActivityLog",
                RowKey = rowKey,
                DocNo = docNo,
                UserID = userID,
                Activity = activity,
                Pages = pages,
                TotalSubmitted = totalSubmitted,
                SubmittedPages = submittedPages,
                Description = desc,
            };
            InsertTableRec(taskActivityLog).Wait();
        }
        //*******************************************************************
        //*** UpdateUserTaskActivity
        //*******************************************************************
        public void UpdateUserTaskActivity(string docNo, string userID, string activity, int pages, int totalSubmitted,
        int submittedPages, string desc)
        {
            string rowKey = string.Format("{0}${1}${2}", docNo, userID, activity);
            SetSubPartitionKey("UserTaskActivity");
            RetrieveTableRec(rowKey).Wait();
            if (StatusCode == 200)
            {
                // existing record found
                TaskActivityLog taskActivityLog = (TaskActivityLog) objResult;
                taskActivityLog.Pages = pages;
                taskActivityLog.TotalSubmitted = totalSubmitted;
                taskActivityLog.SubmittedPages = submittedPages;
                taskActivityLog.Description = desc;
                UpdateTableRec(taskActivityLog).Wait();
            }
            else
            {
                TaskActivityLog taskActivityLog = new TaskActivityLog()
                {
                    PartitionKey = "UserTaskActivity",
                    RowKey = rowKey,
                    DocNo = docNo,
                    UserID = userID,
                    Activity = activity,
                    Pages = pages,
                    TotalSubmitted = totalSubmitted,
                    SubmittedPages = submittedPages,
                    Description = desc,
                };
                InsertTableRec(taskActivityLog).Wait();
            }
        }
        public void UpdateUserTaskActivity(TaskActivityLog userTaskActivity) 
        {
            UpdateTableRec(userTaskActivity).Wait();
        }
        //*******************************************************************
        //*** GetActivities
        //*******************************************************************
        public List<TaskActivityLog> GetActivities(DateTime date1, DateTime date2, string partitionKey = "TaskActivityLog")
        {
            List<TaskActivityLog> activities = new List<TaskActivityLog>();
            string query = string.Empty;
 
            string d1 = date1.ToString(_timeFormat_);
            string d2 = date2.ToString(_timeFormat_);
            query += string.Format("(RowKey ge '{0}') and (RowKey lt '{1}')", d1, d2);
            SetSubPartitionKey(partitionKey);
            QueryTableRec(query).Wait();
            activities = (List<TaskActivityLog>)objResult;
            activities.Reverse();
            return activities;
        }
        public List<TaskActivityLog> GetUserTaskActivities(DateTime date1, DateTime date2)
        {
            List<TaskActivityLog> activities = new List<TaskActivityLog>();
            string query = string.Empty;

            string d1 = date1.ToString(_timeFormat2_);
            string d2 = date2.ToString(_timeFormat2_);
            query += string.Format("(Timestamp ge datetime'{0}') and (Timestamp lt datetime'{1}')", d1, d2);
            SetSubPartitionKey("UserTaskActivity");
            QueryTableRec(query).Wait();
            activities = (List<TaskActivityLog>)objResult;
            //activities.Reverse();
            return activities;
        }
        public void UpdateUserStartDate(string docNo, string userID, string startDate)
        {
            List<TaskActivityLog> activities = new List<TaskActivityLog>();
            string query = string.Format("RowKey ge '{0}$' and RowKey lt '{0}$~'", docNo);
            SetSubPartitionKey("UserTaskActivity");
            QueryTableRec(query).Wait();
            activities = (List<TaskActivityLog>)objResult;

            foreach(TaskActivityLog log in activities)
            {
                string[] f = log.Description.Split('|');
                var listUserTaskProgressInfo = JsonConvert.DeserializeObject<List<UserTaskProgressInfo>>(f[0]);
                if (listUserTaskProgressInfo == null) continue;
                bool found = false;
                foreach(UserTaskProgressInfo item in listUserTaskProgressInfo)
                {
                    if (item.userID == userID)
                    {
                        found = true;
                        item.startDate = startDate;
                        break;
                    }
                }
                if (found)
                {
                    log.Description = JsonConvert.SerializeObject(listUserTaskProgressInfo) + "|" + f[1];
                    UpdateUserTaskActivity(log);
                }
            }
        }
        public void DeleteAll(string userID)
        {
            if (userID == null || userID != "dhammayaungchi2011@gmail.com") return;
            string query = "";
            QueryTableRec(query).Wait();
            List<TaskActivityLog> allTaskActivities = (List<TaskActivityLog>)objResult;

            List<TaskActivityLog> delTaskActivities;
            List<object> delList;
            
            // delete TaskActivityLog
            delTaskActivities = allTaskActivities.Where(a => a.PartitionKey == "TaskActivityLog").ToList();
            delList = delTaskActivities.ToList<object>();
            if (delList.Count > 0) DeleteTableRecBatch(delList).Wait();

            // delete UserTaskActivity
            delTaskActivities = allTaskActivities.Where(a => a.PartitionKey == "UserTaskActivity").ToList();
            delList = delTaskActivities.ToList<object>();
            if (delList.Count > 0) DeleteTableRecBatch(delList).Wait();
        }
    }
}
