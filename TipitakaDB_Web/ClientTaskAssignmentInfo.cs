using Syncfusion.Blazor.Data;
using Tipitaka_DBTables;

namespace Tipitaka_DB
{
    public class ClientTaskAssignmentInfo : TipitakaDB_w
    {
        const string _TaskAssignmentInfo_ = "TaskAssignmentInfo";

        public ClientTaskAssignmentInfo() : base(_TaskAssignmentInfo_)
        { 
        }
        public void AddTaskAssignmentInfo(TaskAssignmentInfo taskAssignmentInfo)
        {
            InsertTableRec(taskAssignmentInfo).Wait();
        }
        public TaskAssignmentInfo? GetTaskAssignmentInfo(string rowKey)
        {
            TaskAssignmentInfo? taskAssignmentInfo = null;
            RetrieveTableRec(rowKey).Wait();
            if (StatusCode == 200)
            {
                taskAssignmentInfo = (TaskAssignmentInfo)objResult;
            }
            return taskAssignmentInfo;
        }
        public TaskAssignmentInfo? RemoveTaskAssignmentInfo(string rowKey)
        {
            TaskAssignmentInfo? taskAssignmentInfo = null;
            RetrieveTableRec(rowKey).Wait();
            if (StatusCode == 200)
            {
                taskAssignmentInfo = (TaskAssignmentInfo)objResult;
                DeleteTableRec(taskAssignmentInfo).Wait();
            }
            return taskAssignmentInfo;
        }
        public void RemoveTaskAssignmentInfo(TaskAssignmentInfo taskAssignmentInfo)
        {
            DeleteTableRec(taskAssignmentInfo).Wait();
            return;
        }
        public List<TaskAssignmentInfo> QueryTaskAssignmentInfo(string query)
        {
            List<TaskAssignmentInfo> list = new List<TaskAssignmentInfo>();
            QueryTableRec(query).Wait();
            list = (List<TaskAssignmentInfo>)objResult;
            return list;
        }
        public void UpdateTaskAssignmentInfo(TaskAssignmentInfo taskAssignmentInfo)
        {
            if (taskAssignmentInfo == null || taskAssignmentInfo.RowKey.Length == 0) { return; }
            TaskAssignmentInfo? taskAssignmentInfo1 = GetTaskAssignmentInfo(taskAssignmentInfo.RowKey);
            if (taskAssignmentInfo1 != null) 
            {
                taskAssignmentInfo1.DocTitle = taskAssignmentInfo.DocTitle;
                taskAssignmentInfo1.PageNos = taskAssignmentInfo.PageNos;
                taskAssignmentInfo1.PagesSubmitted = taskAssignmentInfo.PagesSubmitted;
                taskAssignmentInfo1.AssigneeProgress = taskAssignmentInfo.AssigneeProgress;
                taskAssignmentInfo1.StartDate = taskAssignmentInfo.StartDate;
                taskAssignmentInfo1.LastDate = taskAssignmentInfo.LastDate;
                taskAssignmentInfo1.CorrectionCount = taskAssignmentInfo.CorrectionCount;
                taskAssignmentInfo1.Status = taskAssignmentInfo1.Status;
                UpdateTableRec(taskAssignmentInfo1).Wait();
                return; 
            }
        }
        public void DeleteAll(string userID)
        {
            if (userID == null || userID != "dhammayaungchi2011@gmail.com") return;
            string query = "";
            List<TaskAssignmentInfo> list = QueryTaskAssignmentInfo(query);
            if (list.Count > 0)
            {
                List<object> list1 = list.ToList<object>();
                DeleteTableRecBatch(list1).Wait();
            }
        }
    }
}