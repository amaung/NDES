using Tipitaka_DBTables;
using Tipitaka_DB;
using System.Collections.Generic;

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
        public List<KeyValueData> QuerySuttaDataInfo(string suttaNo)
        {
            string rowKey1 = String.Format("{0}-000", suttaNo);
            string rowKey2 = String.Format("{0}-999", suttaNo); ;
            string qualifier = String.Format("(RowKey gt '{0}' and RowKey lt '{1}')", rowKey1, rowKey2);
            QueryTableRec(qualifier).ConfigureAwait(false);
            if (StatusCode == 404) return new List<KeyValueData>();
            List<KeyValueData> keyValueData = (List<KeyValueData>)objResult;
            return keyValueData;
        }
        public void UpdateKeyValueData(KeyValueData keyValueData)
        {
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
        public void AddUserTask(string[] userIDs, string docNo)
        {
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
        public Dictionary<string, string> GetUserTasks()
        {
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
        public void RemoveUserTask(string userID, string docNo)
        {
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
        public void RemoveUserTask(string[] userID, string docNo)
        {
            foreach(string user in userID)
                RemoveUserTask(user, docNo);
        }
        private Dictionary<string, string> GetUserTaskList(string userTaskList)
        {
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
        //*************************************************************
        // Get DocNos worked on by the user
        //*************************************************************
        public List<string> GetUserDocs(string userID, string taskCategory)
        {
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
            string rowKey = String.Format("{0}-Task-{1}", userID, taskCategory);
            RetrieveTableRec(rowKey).Wait();
            if (StatusCode == 200 || StatusCode == 204)
            {
                KeyValueData keyValueData = (KeyValueData)objResult;
                // if docNo is in the list do nothing
                List<string> list;
                list = keyValueData.Value.Trim().Split('|').ToList();
                if (list.Contains(docNo)) { return; }
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
            foreach(string userID in userIDs)
            {
                AddUserDocByCategory(userID, taskCategory, docNo);
            }
        }
        public void RemoveUserDocByCategory(string userID,  string taskCategory, string docNo)
        {
            string rowKey = String.Format("{0}-Task-{1}", userID, taskCategory);
            RetrieveTableRec(rowKey).Wait();
            if (StatusCode == 200 || StatusCode == 204)
            {
                KeyValueData keyValueData = (KeyValueData)objResult;
                if (!keyValueData.Value.Contains(docNo)) { return; }
                List<string> listDocs = keyValueData.Value.Split('|').ToList();
                if (listDocs == null || listDocs.Count == 0) {  return; }
                listDocs.Remove(docNo);
                if (listDocs.Count == 0) { keyValueData.Value = ""; }
                else keyValueData.Value = String.Join("|", listDocs);
                UpdateTableRec(keyValueData).Wait();
            }
            return;
        }
        public void RemoveUserDocByCategory(string[] userIDs, string taskCategory, string docNo)
        {
            foreach (string userID in userIDs)
                RemoveUserDocByCategory(userID, taskCategory, docNo);
        }

        public async Task DeleteUserTasks(string userID)
        {
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

    }
}
