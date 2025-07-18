using Tipitaka_DBTables;
using Tipitaka_DB;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace NissayaEditor_Web.Data
{
    public class ClientKeyValueData : TipitakaDB_w
    {
        public const string _KeyValueData_ = "KeyValueData";
        ClientTipitakaDB_w? clientTipitakaDB;

        //*******************************************************************
        //*** ClientSuttaPageData
        //*******************************************************************
        public ClientKeyValueData(ClientTipitakaDB_w clientTipitakaDB) : base(_KeyValueData_)
        {
            this.clientTipitakaDB = clientTipitakaDB;
            //clientTipitakaDBLogin = clientTipitakaDB.GetClientTipitakaDBLogin();
        }
        public KeyValueData GetKeyValueData(string rowKey)
        {
            SetSubPartitionKey(_KeyValueData_);
            KeyValueData keyValueData = new KeyValueData();
            keyValueData.PartitionKey = "KeyValueData";
            keyValueData.RowKey = rowKey;
            RetrieveTableRec(rowKey).Wait();//.ConfigureAwait(false);
            if (StatusCode == 404) return keyValueData;
            keyValueData = (KeyValueData)objResult;
            keyValueData.PartitionKey = "KeyValueData";
            return keyValueData;
        }
        public void QueryKeyValueData(string qualifier)
        {
            SetSubPartitionKey(_KeyValueData_);
            List<KeyValueData> result = new List<KeyValueData>();
            //QueryTableRec(qualifier).ConfigureAwait(false);
            QueryTableRec(qualifier).Wait();
            /*await*/
            //QueryTableRec(qualifier);
            if (StatusCode == 0)
            {
                result = (List<KeyValueData>)objResult;
            }
            return;
        }
        //public async Task<List<KeyValueData>> QueryUserActiveTaskAsync(string userID, string qualifier)
        //{
        //    SetSubPartitionKey("User-" + userID);
        //    await QueryTableRec(qualifier);
        //    if (StatusCode == 404) return new List<KeyValueData>();
        //    List<KeyValueData> keyValueData = (List<KeyValueData>)objResult;
        //    return keyValueData;
        //}
        public void UpdateKeyValueData(KeyValueData keyValueData)
        {
            SetSubPartitionKey(_KeyValueData_);
            RetrieveTableRec(keyValueData.RowKey).Wait();
            if (StatusCode == 200 || StatusCode == 204)
            {
                KeyValueData keyValueData1 = (KeyValueData)objResult;
                keyValueData1.Value = keyValueData.Value;
                UpdateTableRec(keyValueData1).ConfigureAwait(false);
                return;
            }
            if (StatusCode == 404) // record not found
            {
                // add new record
                InsertTableRec(keyValueData).ConfigureAwait(false);
                //if (StatusCode != 204)
                //    MessageBox.Show(String.Format("KeyValueData insert error.RowKey = {0}, Value = {1}", keyValueData.RowKey, keyValueData.Value));
            }
        }
        public async Task UpdateKeyValueDataAsync(KeyValueData keyValueData)
        {
            SetSubPartitionKey(keyValueData.PartitionKey);
            await RetrieveTableRec(keyValueData.RowKey);
            if (StatusCode == 200 || StatusCode == 204)
            {
                KeyValueData keyValueData1 = (KeyValueData)objResult;
                keyValueData1.Value = keyValueData.Value;
                await UpdateTableRec(keyValueData1);
                return;
            }
            if (StatusCode == 404) // record not found
            {
                // add new record
                await InsertTableRec(keyValueData);
            }
        }
        public async Task<SortedDictionary<string, List<KeyValueData>>> GetUserTasksAsync(string userID, string value = "")
        {
            SortedDictionary<string, List<KeyValueData>> dictKeyValueData = new SortedDictionary<string, List<KeyValueData>>();
            List<KeyValueData> listKeyValueData = new List<KeyValueData>();
            string query;
            if (userID == "All")
            {
                if (value == "Assigned" || value == "Completed")
                    query = String.Format("PartitionKey ge 'User-' and PartitionKey le 'User-~' and Value eq '{0}'", value);
                else query = "PartitionKey ge 'User-' and PartitionKey le 'User-~'";
            }
            else
            {
                if (value == "Assigned" || value == "Completed")
                    query = String.Format("PartitionKey eq 'User-{0}' and Value eq '{1}'", userID, value);
                else query = String.Format("PartitionKey eq 'User-{0}'", userID);
            }
            var result = await QueryPartitionRecs(query);
            listKeyValueData = (List<KeyValueData>)result;
            // sort list in descending order on timestamp value
            List<KeyValueData> listSortedKeyValueData = listKeyValueData.OrderByDescending(c => c.Timestamp).ToList();
            
            foreach (KeyValueData keyValueData in listKeyValueData)
            {
                if (!dictKeyValueData.ContainsKey(keyValueData.PartitionKey))
                {
                    dictKeyValueData.Add(keyValueData.PartitionKey, new List<KeyValueData>() { keyValueData });
                }
                else
                    dictKeyValueData[keyValueData.PartitionKey].Add(keyValueData);
            }
            return dictKeyValueData;
        }
        public void AddUserTask(string[] userIDs, string docNo)
        {
            SetSubPartitionKey(_KeyValueData_);
            string rowKey = "User-Tasks";
            RetrieveTableRec(rowKey).Wait();
            if (StatusCode == 200 || StatusCode == 204)
            {
                KeyValueData keyValueData = (KeyValueData)objResult;
                Dictionary<string, string> dict = GetUserTaskList(keyValueData.Value);
                string tasks = "";
                foreach (string userID in userIDs)
                {
                    if (dict.ContainsKey(userID))
                    {
                        tasks = dict[userID];
                        if (tasks == null)
                        {
                            tasks = docNo;
                        }
                        else
                        {
                            List<string> list = tasks.Split(';').ToList();
                            if (list.Contains(docNo)) { continue; }
                            list.Add(docNo);
                            tasks = String.Join(';', list);
                        }
                    }
                    else
                    {
                        tasks = docNo;
                    }
                    dict[userID] = tasks;
                }
                string value = "", separator = "";
                foreach(KeyValuePair<string, string> kvp in dict)
                {
                    separator = value.Length > 0 ? "|" : "";
                    value += String.Format("{0}{1}={2}", separator, kvp.Key, kvp.Value);
                }
                keyValueData.Value = value;
                UpdateTableRec(keyValueData).ConfigureAwait(false);
                return;
            }
            if (StatusCode == 404) // record not found
            {
                string value = "", separator = "";
                foreach(string userID in userIDs) 
                {
                    separator = value.Length > 0 ? "|" : "";
                    value += String.Format("{0}{1}={2}", separator, userID, docNo);
                }
                // add first data
                InsertTableRec(new KeyValueData()
                {
                    RowKey = rowKey,
                    Value = value,
                }).ConfigureAwait(false);
            }
        }
        //********************************************************************
        //*** AddUserTasksAsync()
        //*** This adds users with their assigned tasks in KeyValueData table.
        //********************************************************************
        public async Task AddUserTasksAsync(string[] userIDs, string docNo)
        {
            SetSubPartitionKey(_KeyValueData_);
            string rowKey = "User-Tasks";
            await RetrieveTableRec(rowKey);
            if (StatusCode == 200 || StatusCode == 204)
            {
                KeyValueData keyValueData = (KeyValueData)objResult;
                Dictionary<string, string> dict = GetUserTaskList(keyValueData.Value);
                string tasks = "";
                foreach (string userID in userIDs)
                {
                    if (dict.ContainsKey(userID))
                    {
                        tasks = dict[userID];
                        if (tasks == null)
                        {
                            tasks = docNo;
                        }
                        else
                        {
                            List<string> list = tasks.Split(';').ToList();
                            if (list.Contains(docNo)) { continue; }
                            list.Add(docNo);
                            tasks = String.Join(';', list);
                        }
                    }
                    else
                    {
                        tasks = docNo;
                    }
                    dict[userID] = tasks;
                }
                string value = "", separator = "";
                foreach (KeyValuePair<string, string> kvp in dict)
                {
                    separator = value.Length > 0 ? "|" : "";
                    value += String.Format("{0}{1}={2}", separator, kvp.Key, kvp.Value);
                }
                keyValueData.Value = value;
                await UpdateTableRec(keyValueData);
                return;
            }
            if (StatusCode == 404) // record not found
            {
                string value = "", separator = "";
                foreach (string userID in userIDs)
                {
                    separator = value.Length > 0 ? "|" : "";
                    value += String.Format("{0}{1}={2}", separator, userID, docNo);
                }
                // add first data
                await InsertTableRec(new KeyValueData()
                {
                    RowKey = rowKey,
                    Value = value,
                });
            }
        }

        public Dictionary<string, string> GetUserTasks()
        {
            SetSubPartitionKey(_KeyValueData_);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string rowKey = "User-Tasks";
            RetrieveTableRec(rowKey).Wait();
            if (StatusCode == 200 || StatusCode == 204)
            {
                KeyValueData keyValueData = (KeyValueData)objResult;
                dict = GetUserTaskList(keyValueData.Value);
            }
            return dict;
        }
        public async Task<Dictionary<string, string>> GetUserTasksAsync()
        {
            SetSubPartitionKey(_KeyValueData_);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string rowKey = "User-Tasks";
            await RetrieveTableRec(rowKey);
            if (StatusCode == 200 || StatusCode == 204)
            {
                KeyValueData keyValueData = (KeyValueData)objResult;
                dict = GetUserTaskList(keyValueData.Value);
            }
            return dict;
        }
        public void RemoveUserTask(string userID, string docNo)
        {
            SetSubPartitionKey(_KeyValueData_);
            string rowKey = "User-Tasks";
            RetrieveTableRec(rowKey).Wait();
            if (StatusCode == 200 || StatusCode == 204)
            {
                KeyValueData keyValueData = (KeyValueData)objResult;
                string val = "";
                string[] sl = keyValueData.Value.Split("|");
                int idx = 0;
                bool found = false;
                foreach(string s in sl)
                {
                    if (s.Contains(userID))
                    {
                        List<string> listDoc = s.Substring(userID.Length + 1).Split(';').ToList();
                        listDoc.Remove(docNo);
                        val = String.Join(";", listDoc);
                        found = true;
                        break;
                    }
                    ++idx;
                }
                if (found)
                {
                    sl[idx] = String.Format("{0}={1}", userID, val);
                }
                //l.RemoveAll(p => p == docNo);
                keyValueData.Value = String.Join('|', sl);
                UpdateTableRec(keyValueData).Wait();
            }
            return;
        }
        public async Task RemoveUserTaskAsync(string userID, string docNo)
        {
            SetSubPartitionKey(_KeyValueData_);
            string rowKey = "User-Tasks";
            await RetrieveTableRec(rowKey);
            if (StatusCode == 200 || StatusCode == 204)
            {
                KeyValueData keyValueData = (KeyValueData)objResult;
                string val = "";
                string[] sl = keyValueData.Value.Split("|");
                int idx = 0;
                bool found = false;
                foreach (string s in sl)
                {
                    if (s.Contains(userID))
                    {
                        List<string> listDoc = s.Substring(userID.Length + 1).Split(';').ToList();
                        listDoc.Remove(docNo);
                        val = String.Join(";", listDoc);
                        found = true;
                        break;
                    }
                    ++idx;
                }
                if (found)
                {
                    sl[idx] = String.Format("{0}={1}", userID, val);
                }
                //l.RemoveAll(p => p == docNo);
                keyValueData.Value = String.Join('|', sl);
                await UpdateTableRec(keyValueData);
            }
            return;
        }
        public async Task RemoveUserTaskAsync(string[] userID, string docNo)
        {
            SetSubPartitionKey(_KeyValueData_);
            foreach (string user in userID)
                await RemoveUserTaskAsync(user, docNo);
        }
        public void RemoveUserTask(string[] userID, string docNo)
        {
            SetSubPartitionKey(_KeyValueData_);
            foreach (string user in userID)
                RemoveUserTask(user, docNo);
        }
        public async Task RemoveUserTaskAsync(string userID, string docNo, string task)
        {
            string query = String.Format("RowKey eq {0}|{1}", docNo, task);
            KeyValueData keyValueData = new KeyValueData()
            {
                PartitionKey = "User-" + userID,
                RowKey = String.Format("{0}|{1}", docNo, task),
            };
            await DeleteTableRec(keyValueData);
        }
        private Dictionary<string, string> GetUserTaskList(string userTaskList)
        {
            SetSubPartitionKey(_KeyValueData_);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string[] users = userTaskList.Split("|");
            foreach (string user in users)
            {
                string[] f = user.Split("=");
                if (f.Length == 2)
                {
                    if (!dict.ContainsKey(f[0])) { dict.Add(f[0], f[1]); }
                }
            }
            return dict;
        }
        public List<string> GetUsersForDoc(string docNo)
        {
            SetSubPartitionKey(_KeyValueData_);
            List<string> users = new List<string>();
            string rowKey = "User-Tasks";
            RetrieveTableRec(rowKey).Wait();
            if (StatusCode == 200 || StatusCode == 204)
            {
                KeyValueData keyValueData = (KeyValueData)objResult;
                string[] userTasks = keyValueData.Value.Split("|");
                foreach(string userTask in userTasks)
                {
                    string[] f = (string[])userTask.Split("=");
                    if (f.Length == 2 && f[1].Length > 0)
                    {
                        List<string> docList = f[1].Split(';').ToList();
                        if (docList.Contains(docNo)) users.Add(f[0]);
                    }
                }
            }
            return users;
        }
        public async Task<List<string>> GetUsersForDocAsync(string docNo)
        {
            SetSubPartitionKey(_KeyValueData_);
            List<string> users = new List<string>();
            string rowKey = "User-Tasks";
            await RetrieveTableRec(rowKey);
            if (StatusCode == 200 || StatusCode == 204)
            {
                KeyValueData keyValueData = (KeyValueData)objResult;
                string[] userTasks = keyValueData.Value.Split("|");
                foreach (string userTask in userTasks)
                {
                    string[] f = (string[])userTask.Split("=");
                    if (f.Length == 2 && f[1].Length > 0)
                    {
                        List<string> docList = f[1].Split(';').ToList();
                        if (docList.Contains(docNo)) users.Add(f[0]);
                    }
                }
            }
            return users;
        }
        //*************************************************************
        // Get DocNos worked on by the user
        //*************************************************************
        public List<string> GetUserDocs(string userID, string taskCategory)
        {
            SetSubPartitionKey(_KeyValueData_);
            string rowKey = String.Format("{0}-Task-{1}", userID, taskCategory);
            List<string> docs = new List<string>();
            RetrieveTableRec(rowKey).Wait();
            if (StatusCode == 200 || StatusCode == 204)
            {
                KeyValueData keyValueData = (KeyValueData)objResult;
                if (keyValueData.Value.Length > 0) docs = keyValueData.Value.Split('|').ToList();
            }
            return docs;
        }
        //*************************************************************
        // Add DocNo worked on by the user to a task category
        //*************************************************************
        public void AddUserDocByCategory(string userID, string taskCategory, string docNo)
        {
            SetSubPartitionKey(_KeyValueData_);
            string rowKey = String.Format("{0}-Task-{1}", userID, taskCategory);
            RetrieveTableRec(rowKey).Wait();
            if (StatusCode == 200 || StatusCode == 204)
            {
                KeyValueData keyValueData = (KeyValueData)objResult;
                // if docNo is in the list do nothing
                List<string> list;
                list = keyValueData.Value.Trim().Split('|').ToList();
                if (list.Contains(docNo)) { return; }
                list = list.Where(s => !string.IsNullOrEmpty(s)).ToList();
                // add new doc
                //if (keyValueData.Value.Trim().Length > 0) list = keyValueData.Value.Split('|').ToList();
                //else list = new List<string>();
                list.Add(docNo);
                // _Recent_ holds 10 items only
                if (taskCategory == TaskCategories._Recent_)
                {
                    if (list.Count > 10) 
                    {
                        int n = list.Count - 10;
                        list.RemoveRange(0, n);
                    }
                }
                keyValueData.Value = String.Join("|", list);
                UpdateTableRec(keyValueData).Wait();
                return;
            }
            if (StatusCode == 404) // record not found
            {
                // add new record
                InsertTableRec(new KeyValueData()
                {
                    RowKey = rowKey,
                    Value = docNo
                }).ConfigureAwait(false);
            }
        }
        public void AddUserDocByCategory(string[] userIDs, string taskCategory, string docNo)
        {
            SetSubPartitionKey(_KeyValueData_);
            foreach (string userID in userIDs)
            {
                AddUserDocByCategory(userID, taskCategory, docNo);
            }
        }
        public void RemoveUserDocByCategory(string userID,  string taskCategory, string docNo)
        {
            SetSubPartitionKey(_KeyValueData_);
            string rowKey = String.Format("{0}-Task-{1}", userID, taskCategory);
            RetrieveTableRec(rowKey).Wait();
            if (StatusCode == 200 || StatusCode == 204)
            {
                KeyValueData keyValueData = (KeyValueData)objResult;
                if (!keyValueData.Value.Contains(docNo)) { return; }
                List<string> listDocs = keyValueData.Value.Split('|').ToList();
                if (listDocs == null || listDocs.Count == 0) {  return; }
                listDocs.Remove(docNo);
                listDocs = listDocs.Where(s => !string.IsNullOrEmpty(s)).ToList();
                if (listDocs.Count == 0) { keyValueData.Value = ""; }
                else keyValueData.Value = String.Join("|", listDocs);
                UpdateTableRec(keyValueData).Wait();
            }
            return;
        }
        public async Task RemoveUserDocByCategoryAsync(string userID, string taskCategory, string docNo)
        {
            SetSubPartitionKey(_KeyValueData_);
            string rowKey = String.Format("{0}-Task-{1}", userID, taskCategory);
            await RetrieveTableRec(rowKey);
            if (StatusCode == 200 || StatusCode == 204)
            {
                KeyValueData keyValueData = (KeyValueData)objResult;
                if (!keyValueData.Value.Contains(docNo)) { return; }
                List<string> listDocs = keyValueData.Value.Split('|').ToList();
                if (listDocs == null || listDocs.Count == 0) { return; }
                listDocs.Remove(docNo);
                listDocs = listDocs.Where(s => !string.IsNullOrEmpty(s)).ToList();
                if (listDocs.Count == 0) { keyValueData.Value = ""; }
                else keyValueData.Value = String.Join("|", listDocs);
                await UpdateTableRec(keyValueData);
            }
            return;
        }
        public void RemoveUserDocByCategory(string[] userIDs, string taskCategory, string docNo)
        {
            SetSubPartitionKey(_KeyValueData_);
            foreach (string userID in userIDs)
                RemoveUserDocByCategory(userID, taskCategory, docNo);
        }
        public async Task RemoveUserDocByCategoryAsync(string[] userIDs, string taskCategory, string docNo)
        {
            SetSubPartitionKey(_KeyValueData_);
            foreach (string userID in userIDs)
                await RemoveUserDocByCategoryAsync(userID, taskCategory, docNo);
        }
        public async Task DeleteUserTasks(string userID)
        {
            SetSubPartitionKey(_KeyValueData_);
            string qualifier = "RowKey ne 'DocSubTypes' and RowKey ne 'DocTypes'";
            await QueryTableRec(qualifier).ConfigureAwait(false);
            List<KeyValueData> listData = (List<KeyValueData>)objResult;
            //List<KeyValueData> listData = QueryKeyValueData(qualifier).Wait();
            if (listData.Count > 0)
            {
                List<object> list = listData.ToList<object>();
                DeleteTableRecBatch(list).Wait();
            }
        }
        public async Task UpdateUserTaskStatus(string userID, string docNo, string taskStatus)
        {
            string val = taskStatus;
            string key = userID + "-TaskStatus:" + docNo;
            KeyValueData keyValueData;
            SetSubPartitionKey(_KeyValueData_);
            var result = await RetrieveTableRec(key);
           // RetrieveTableRec(rowKey).Wait();//.ConfigureAwait(false);
            if (StatusCode == 404)
            {
                keyValueData = new KeyValueData()
                {
                    RowKey = key,
                    Value = val
                };
                await InsertTableRec(keyValueData);
            }
            else
            {
                if (result != null)
                {
                    keyValueData = (KeyValueData)result;
                    keyValueData.Value = val;
                    await UpdateTableRec(keyValueData);
                }
            }
            return;
        }
        public async Task RemoveUserAssignedTasksAsync(string docNo, TaskAssignmentInfo? taskAssignmentInfo)
        {
            if (taskAssignmentInfo == null) return;
            var listUserTaskProgressInfo = JsonConvert.DeserializeObject<List<UserTaskProgressInfo>>(taskAssignmentInfo.AssigneeProgress);
            if (listUserTaskProgressInfo == null) return;
            //string rowKey, userId, task;//, status;
            KeyValueData keyValueData = new KeyValueData();
            if (listUserTaskProgressInfo != null)
            {
                // remove each user's task from UserData
                foreach (UserTaskProgressInfo userTaskProgressInfo in listUserTaskProgressInfo)
                {
                    if (userTaskProgressInfo.task != "NewDoc")
                    {
                        //userId = userTaskProgressInfo.userID;
                        //task = userTaskProgressInfo.task;
                        ////status = userTaskProgressInfo.status;
                        //rowKey = docNo + "-" + userTaskProgressInfo.task;
                        //string partitionKey = "User-" + userTaskProgressInfo.userID;
                        keyValueData.PartitionKey = "User-" + userTaskProgressInfo.userID;
                        keyValueData.RowKey = docNo + "|" + userTaskProgressInfo.task;
                        await DeleteTableRec(keyValueData);
                    }
                }
                //keyValueData.PartitionKey = "KeyValueData";
                //keyValueData.RowKey = "user6@gmail.com-Task-Recent";
                //await DeleteTableRec(keyValueData);
            }
        }
        public async Task UpdateUserTaskAssignmentAsync(string docNo, TaskAssignmentInfo? taskAssignmentInfo)
        {
            Dictionary<string, string> userTaskStatus = new Dictionary<string, string>();
            if (taskAssignmentInfo == null) return;
            var listUserTaskProgressInfo = JsonConvert.DeserializeObject<List<UserTaskProgressInfo>>(taskAssignmentInfo.AssigneeProgress);
            if (listUserTaskProgressInfo == null) return;
            string rowKey, userId, task, status;
            foreach (UserTaskProgressInfo userTaskProgressInfo in listUserTaskProgressInfo)
            {
                if (userTaskProgressInfo.task != "NewDoc")
                {
                    userId = userTaskProgressInfo.userID;
                    task = userTaskProgressInfo.task;
                    status = userTaskProgressInfo.status;
                    string partitionKey = "User-" + userId;
                    SetSubPartitionKey(partitionKey);
                    rowKey = docNo + "|" + task;
                    var result = await RetrieveTableRec(rowKey);
                    if (result != null)
                    {
                        KeyValueData keyValueData = (KeyValueData)result;
                        keyValueData.Value = status;
                        await UpdateTableRec(keyValueData);
                    }
                    else
                    {
                        KeyValueData keyValueData = new KeyValueData()
                        {
                            PartitionKey = partitionKey,
                            RowKey = rowKey,
                            Value = status
                        };
                        await InsertTableRec(keyValueData);
                    }// save user info
                    if (userTaskStatus.ContainsKey(userId))
                    {
                        if (userTaskStatus[userId] == "Completed" && status != "Completed")
                            userTaskStatus[userId] = status;
                    }
                    else 
                    { 
                        if (status != "Completed") userTaskStatus.Add(userId, status); 
                    }
                }
            }
            // adjust the Users-Task-Active
            SetSubPartitionKey("");
            rowKey = "Users-Task-Active";
            var result1 = await RetrieveTableRec(rowKey);
            if (result1 != null) 
            {
                KeyValueData keyValueData = (KeyValueData)result1;
                List<string> value = keyValueData.Value.ToString().Split("|").ToList();
                foreach(KeyValuePair<string, string> kv in userTaskStatus)
                {
                    if (kv.Value == "Completed") value.Remove(kv.Key);
                    else
                    {
                        if (!value.Contains(kv.Key)) value.Add(kv.Key);
                    }
                }
                keyValueData.Value = String.Join('|', userTaskStatus.Keys.ToArray());
                await UpdateTableRec(keyValueData);
            }
        }
    }
}
