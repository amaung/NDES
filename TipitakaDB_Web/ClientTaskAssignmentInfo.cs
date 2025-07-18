using Newtonsoft.Json;
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
        public async Task AddTaskAssignmentInfoAsync(TaskAssignmentInfo taskAssignmentInfo)
        {
            await InsertTableRec(taskAssignmentInfo);
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
        public async Task<TaskAssignmentInfo?> GetTaskAssignmentInfoAsync(string rowKey)
        {
            TaskAssignmentInfo? taskAssignmentInfo = null;
            await RetrieveTableRec(rowKey);
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
        public async Task<TaskAssignmentInfo?> RemoveTaskAssignmentInfoAsync(string rowKey)
        {
            TaskAssignmentInfo? taskAssignmentInfo = null;
            await RetrieveTableRec(rowKey);
            if (StatusCode == 200)
            {
                taskAssignmentInfo = (TaskAssignmentInfo)objResult;
                await DeleteTableRec(taskAssignmentInfo);
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
        public async Task<List<TaskAssignmentInfo>> QueryTaskAssignmentInfoAsync(string query)
        {
            List<TaskAssignmentInfo> list = new List<TaskAssignmentInfo>();
            await QueryTableRec(query);
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
                taskAssignmentInfo1.Status = taskAssignmentInfo.Status;
                UpdateTableRec(taskAssignmentInfo1).Wait();
                return; 
            }
        }
        public async Task UpdateTaskAssignmentInfoAsync(TaskAssignmentInfo taskAssignmentInfo)
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
                taskAssignmentInfo1.Status = taskAssignmentInfo.Status;
                await UpdateTableRec(taskAssignmentInfo1);
                return;
            }
        }
        public async Task ResetTaskAssignmentInfoAsync(TaskAssignmentInfo? taskAssignmentInfo, string email, string userName)
        {
            string assigneeProgress = "";
            UserTaskProgressInfo? userTaskProgressInfo = null;
            if (taskAssignmentInfo != null)
            {
                assigneeProgress = taskAssignmentInfo.AssigneeProgress;
                var listUserTaskProgressInfo = JsonConvert.DeserializeObject<List<UserTaskProgressInfo>>(taskAssignmentInfo.AssigneeProgress);
                if (listUserTaskProgressInfo != null)
                {
                    var x = listUserTaskProgressInfo.FirstOrDefault(x => x.task == "NewDoc");
                    if (x != null) 
                    { 
                        userTaskProgressInfo = (UserTaskProgressInfo?)x;
                        userTaskProgressInfo!.lastDate = userTaskProgressInfo!.startDate;
                        userTaskProgressInfo!.author = userTaskProgressInfo!.userID;
                        taskAssignmentInfo.LastDate = "";
                        taskAssignmentInfo.AssigneeProgress = JsonConvert.SerializeObject(new List<UserTaskProgressInfo>() { userTaskProgressInfo! });
                        taskAssignmentInfo.Status = "Created";
                    }
                    else
                    {
                        userTaskProgressInfo = new UserTaskProgressInfo()
                        {
                            userID = email,
                            author = email,
                            task = "NewDoc",
                            startDate = DateTime.Now.ToString("dd/MM/yyyy"),
                            status = "Created"
                        };
                        string progress = JsonConvert.SerializeObject(new List<UserTaskProgressInfo>() { userTaskProgressInfo });
                        if (taskAssignmentInfo != null)
                        {
                            taskAssignmentInfo.PagesSubmitted = 0;
                            taskAssignmentInfo.AssigneeProgress = progress;
                            taskAssignmentInfo.StartDate = DateTime.Now.ToString("dd/MM/yyyy");
                            taskAssignmentInfo.LastDate = "";
                            taskAssignmentInfo.CorrectionCount = 0;
                            taskAssignmentInfo.Status = "Created";
                        }
                    }
                    await UpdateTableRec(taskAssignmentInfo!);
                }
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