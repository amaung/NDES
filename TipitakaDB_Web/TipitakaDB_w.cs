using Azure.Data.Tables;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Tipitaka_DBTables;

namespace Tipitaka_DB
{
#nullable enable
    public partial class TipitakaDB_w
    {
        string? connectionString = null;
        string PartitionKey = string.Empty;
        string SubPartitionKey = string.Empty;
        TableClient? tableClient;
        // public access
        public int StatusCode;
        public string DBErrMsg = string.Empty;
        public object objResult = new object();
        public string ErrMsg = string.Empty;
        public static bool devModeDebug = false;
        public TipitakaDB_w(string partitionKey)
        {
            try
            {
                if (devModeDebug) connectionString = "DefaultEndpointsProtocol=https;AccountName=dystorage2021;AccountKey=JopS1zXQXsvQNAAbAVRNIbY5qDzFeVyXzJgKwgpnYLsODtRfjIx5UDwr7J5EYit8aKMeCyPxdM12pz/6SYfBGQ==;TableEndpoint=https://dystorage2021.table.core.windows.net/;";
                else connectionString = "DefaultEndpointsProtocol=https;AccountName=tipitakadata2023;AccountKey=XudohdFrs+d+cgZLY69sgzf7MUHQIlAfJey8yo8BFhP7Cgnnyq3kdcnE8gusj9fAV0JvMIsb/QUB+AStqcY2Kw==;TableEndpoint=https://tipitakadata2023.table.core.windows.net/;";
            }
            catch (Exception) 
            {
                ErrMsg = "Connection string not found."; return;
            }
            PartitionKey = partitionKey;
            if (connectionString is not null && connectionString.Length > 0)
            {
                // New instance of the TableClient class
                TableServiceClient tableServiceClient = new TableServiceClient(connectionString);
                tableClient = tableServiceClient.GetTableClient(tableName: partitionKey);
            }
        }
        public string GetCloudStorageName()
        {
            string s = string.Empty;
            if (connectionString != null)
            {
                int pos = connectionString.IndexOf("https:");
                if (pos != -1)
                {
                    string[] f = connectionString.Substring(pos + 8).Split('.');
                    s = f[0];
                }
            }
            return s;
        }
        public void SetSubPartitionKey(string partitionKey) { SubPartitionKey = partitionKey; }
        //public void SetPartitionKey(string partitionKey) {  PartitionKey = partitionKey; }
        //*******************************************************************
        //*** Retrieve a table rec
        //*******************************************************************
        public async Task<object> RetrieveTableRec(string rowKey)
        {
            object obj = new object();
            try
            {
                switch (PartitionKey)
                {
                    case "UserProfile":
                        if (tableClient != null)
                        {
                            var result = await tableClient.GetEntityAsync<UserProfile>(
                                rowKey: rowKey,
                                partitionKey: PartitionKey
                            ).ConfigureAwait(false);

                            StatusCode = result.GetRawResponse().Status;
                            if (StatusCode == 200)
                            {
                                obj = objResult = (object)result.Value;
                            }
                        }
                        break;
                    case "SuttaPageData":
                        if (tableClient != null)
                        {
                            var result = await tableClient.GetEntityAsync<SuttaPageData>(
                                rowKey: rowKey,
                                partitionKey: SubPartitionKey
                            ).ConfigureAwait(false);

                            StatusCode = result.GetRawResponse().Status;
                            if (StatusCode == 200)
                            {
                                obj = objResult = (object)result.Value;
                            }
                        }
                        break;
                    case "UserDocumentLog":
                        if (tableClient != null)
                        {
                            var result = await tableClient.GetEntityAsync<UserDocumentLog>(
                                partitionKey: PartitionKey,
                                rowKey: rowKey
                            ).ConfigureAwait(false);

                            StatusCode = result.GetRawResponse().Status;
                            if (StatusCode == 200)
                            {
                                obj = objResult = (object)result.Value;
                            }
                        }
                        break;
                    case "SuttaInfo":
                        if (tableClient != null)
                        {
                            var result = await tableClient.GetEntityAsync<SuttaInfo>(
                                rowKey: rowKey,
                                partitionKey: PartitionKey
                            ).ConfigureAwait(false);

                            StatusCode = result.GetRawResponse().Status;
                            if (StatusCode == 200)
                            {
                                obj = objResult = (object)result.Value;
                            }
                        }
                        break;
                    case "UserPageActivity":
                        if (tableClient != null)
                        {
                            var result = await tableClient.GetEntityAsync<UserPageActivity>(
                                rowKey: rowKey,
                                partitionKey: PartitionKey
                            ).ConfigureAwait(false);

                            StatusCode = result.GetRawResponse().Status;
                            if (StatusCode == 200)
                            {
                                obj = objResult = (object)result.Value;
                            }
                        }
                        break;
                    //    case "UpdateHistory":
                    //        if (tableClient != null)
                    //        {
                    //            var result = await tableClient.GetEntityAsync<UpdateHistory>(
                    //                rowKey: rowKey,
                    //                partitionKey: PartitionKey
                    //            ).ConfigureAwait(false);

                    //            StatusCode = result.GetRawResponse().Status;
                    //            if (StatusCode == 200)
                    //            {
                    //                obj = objResult = (object)result.Value;
                    //            }
                    //        }
                    //        break;
                    case "KeyValueData":
                        if (tableClient != null)
                        {
                            var result = await tableClient.GetEntityAsync<KeyValueData>(
                                rowKey: rowKey,
                                partitionKey: PartitionKey
                            ).ConfigureAwait(false);

                            StatusCode = result.GetRawResponse().Status;
                            if (StatusCode == 200)
                            {
                                obj = objResult = (object)result.Value;
                            }
                        }
                        break;
                }
            }
            catch(Exception e)
            {
                await Task.Yield();
                if (e.Message.Contains("Status")) ProcessCatchErrorMessage(e.Message.Split('\n'));
                else StatusCode = -1;
            }
            return obj;
        }

        //*******************************************************************
        //*** Query table records
        //*******************************************************************
        public async Task<object> QueryTableRec(string qualifier = "", Action<List<SuttaPageData>>? callback = null)
        {
            //    object obj = new object();
            List<string> listObj = new List<string>();
            string? token = null;
            try
            {
                switch (PartitionKey)
                {
                    case "UserProfile":
                        List<UserProfile> listUserProfile = new List<UserProfile>();
                        //https://learn.microsoft.com/en-us/visualstudio/azure/vs-azure-tools-table-designer-construct-filter-strings?view=vs-2022
                        if (tableClient != null)
                        {
                            qualifier = string.Format("(PartitionKey eq '{0}')", PartitionKey) + qualifier;
                            var pages = tableClient.QueryAsync<UserProfile>(qualifier, maxPerPage: 1000).AsPages().ConfigureAwait(false);
                            await foreach (var page in pages)
                            {
                                listUserProfile.AddRange(page.Values.ToList());
                                token = page.ContinuationToken; // usage ?
                            }
                            objResult = (object)listUserProfile;
                        }
                        break;
                    case "ActivityLog":
                        //https://learn.microsoft.com/en-us/visualstudio/azure/vs-azure-tools-table-designer-construct-filter-strings?view=vs-2022
                        if (tableClient != null)
                        {
                            string filter = string.Format("(PartitionKey eq '{0}')", PartitionKey);
                            if (qualifier != null && qualifier.Length > 0) filter += qualifier;
                            token = null;
                            var pages = tableClient.QueryAsync<ActivityLog>(filter, maxPerPage: 1000).AsPages().ConfigureAwait(false);
                            await foreach (var page in pages)
                            {
                                List<ActivityLog> listActivities = page.Values.ToList();
                                foreach (var activityLog in listActivities)
                                {
                                    listObj.Add(string.Format("{0}|{1}|{2}", activityLog.RowKey, activityLog.DocID, activityLog.Activity));
                                }
                                token = page.ContinuationToken; // usage ?
                            }
                            listObj.Reverse();
                            objResult = (object)listObj;
                        }
                        break;
                    case "SuttaInfo":
                        List<SuttaInfo> listSuttaInfo = new List<SuttaInfo>();
                        if (tableClient != null)
                        {
                            string filter = string.Format("(PartitionKey eq '{0}')", PartitionKey);
                            if (qualifier.Length > 0)
                            {
                                filter += string.Format(" and ({0})", qualifier);
                            }
                            var pages = tableClient.QueryAsync<SuttaInfo>(filter, maxPerPage: 1000).AsPages().ConfigureAwait(false);
                            await foreach (var page in pages)
                            {
                                listSuttaInfo.AddRange(page.Values.ToList());
                                token = page.ContinuationToken; // usage ?
                            }
                            objResult = (object)listSuttaInfo;
                        }
                        break;
                    case "UserPageActivity":
                        List<UserPageActivity> listUserPageActivity = new List<UserPageActivity>();
                        if (tableClient != null)
                        {
                            string filter = String.Format("(PartitionKey eq '{0}')", PartitionKey);
                            if (qualifier.Length > 0)
                                filter += String.Format(" and {0}", qualifier);

                            var pages = tableClient.QueryAsync<UserPageActivity>(filter, maxPerPage: 1000).AsPages().ConfigureAwait(false);
                            await foreach (var page in pages)
                            {
                                listUserPageActivity.AddRange(page.Values.ToList());
                                token = page.ContinuationToken; // usage ?
                            }
                            objResult = (Object)listUserPageActivity;
                        }
                        break;
                    case "SuttaPageData":
                        List<SuttaPageData> listSuttaPageData = new List<SuttaPageData>();
                        if (tableClient != null)
                        {
                            string filter = string.Empty;
                            if (SubPartitionKey.Length > 0) filter = string.Format("(PartitionKey eq '{0}')", SubPartitionKey);
                            if (qualifier != null && qualifier.Length > 0)
                            {
                                filter += string.Format(" and (RowKey ge '{0}-000') and (RowKey le '{0}-999')", qualifier);
                            }
                            var pages = tableClient.QueryAsync<SuttaPageData>(filter, maxPerPage: 1000).AsPages().ConfigureAwait(false);
                            await foreach (var page in pages)
                            {
                                listSuttaPageData.AddRange(page.Values.ToList());
                                token = page.ContinuationToken;
                            }
                            objResult = (object)listSuttaPageData;
                            if (callback != null) callback(listSuttaPageData);
                        }
                        break;
                        //    case "KeyValueData":
                        //        List<KeyValueData> listKeyValueData = new List<KeyValueData>();
                        //        if (tableClient != null)
                        //        {
                        //            string filter = String.Format("(PartitionKey eq '{0}')", PartitionKey);
                        //            if (qualifier.Length > 0) { filter += " and " + qualifier; }
                        //            var listKVD = tableClient.Query<KeyValueData>(filter);
                        //            listKeyValueData = (List<KeyValueData>)listKVD.ToList();
                        //            objResult = (object)listKeyValueData;
                        //        }
                        //        break;
                }
                return objResult;
            }
            catch (Exception e)
            {
                await Task.Yield();
                ProcessCatchErrorMessage(e.Message.Split('\n'));
            }
            return (object)listObj;
        }
        //*******************************************************************
        //*** Convert dt string into local DateTime
        //*******************************************************************
        public DateTime GetLocalDateTime(string s)
        {
            string[] f = s.Split('-');
            int year = Convert.ToInt16(f[0].Substring(0, 4));
            int mon = Convert.ToInt16(f[0].Substring(4, 2));
            int day = Convert.ToInt16(f[0].Substring(6, 2));
            f = f[1].Split(':');
            int hr = Convert.ToInt16(f[0]);
            int min = Convert.ToInt16(f[1]);
            int pos = f[2].IndexOf('.');
            if (pos >= 0)
                f[2] = f[2].Substring(0, pos);
            int sec = Convert.ToInt16(f[2]);
            DateTime dt = new DateTime(year, mon, day, hr, min, sec).ToLocalTime();
            return dt;
        }
        //*******************************************************************
        //*** Add table records in a batch
        //*******************************************************************
        // https://stackoverflow.com/questions/77082662/inserting-batches-in-azure-table-storage-using-the-azure-data-tables-sdk-does-no
        public async Task<object> InsertTableRecBatch(List<SuttaPageData> obj)
        {
            if (tableClient != null)
            {
                var transactionActions = new List<TableTransactionAction>();
                try
                {
                    int n = 0;
                    foreach(SuttaPageData data in obj)
                    {
                        ++n;
                        if (SubPartitionKey.Length > 0) data.PartitionKey = SubPartitionKey;
                        transactionActions.Add(new TableTransactionAction(TableTransactionActionType.Add, data));
                        if (n == 100)
                        {
                            var result = await tableClient.SubmitTransactionAsync(transactionActions);
                            ProcessCatchErrorMessage(result.ToString().Split('\n'));
                            if (StatusCode != 202) return obj;
                            DBErrMsg = string.Empty;
                            n = 0;
                            transactionActions.Clear();
                        }
                    }
                    if (n > 0)
                    {
                        var result = await tableClient.SubmitTransactionAsync(transactionActions);
                        ProcessCatchErrorMessage(result.ToString().Split('\n'));
                        if (StatusCode == 202) DBErrMsg = string.Empty;
                    }
                }
                catch (Exception e)
                {
                    ProcessCatchErrorMessage(e.Message.Split('\n'));
                }

            }
            return obj;
        }
        public async Task InsertTableRecBatch(string docID, string email, Dictionary<string, string> obj)
        {
            List<SuttaPageData> dataList = new List<SuttaPageData>();

            foreach (KeyValuePair<string, string> item in obj)
            {
                string pgnoKey = "00" + item.Key;
                pgnoKey = pgnoKey.Substring(pgnoKey.Length - 3);
                var pageData = new SuttaPageData()
                {
                    PartitionKey = docID,
                    RowKey = string.Format("{0}-{1}", docID, pgnoKey),
                    PageNo = Convert.ToInt16(item.Key),
                    PageData = item.Value,
                    UserID = email,
                };
                dataList.Add(pageData);
            };
            if (tableClient != null)
            {
                var transactionActions = new List<TableTransactionAction>();
                try
                {
                    int n = 0;
                    foreach (SuttaPageData data in dataList)
                    {
                        ++n;
                        transactionActions.Add(new TableTransactionAction(TableTransactionActionType.Add, data));
                        if (n == 100)
                        {
                            var result = await tableClient.SubmitTransactionAsync(transactionActions);
                            ProcessCatchErrorMessage(result.ToString().Split('\n'));
                            if (StatusCode != 202) return;
                            DBErrMsg = string.Empty;
                            n = 0;
                            transactionActions.Clear();
                        }
                    }
                    if (n > 0)
                    {
                        var result = await tableClient.SubmitTransactionAsync(transactionActions);
                        ProcessCatchErrorMessage(result.ToString().Split('\n'));
                        if (StatusCode == 202) DBErrMsg = string.Empty;
                    }
                }
                catch (Exception e)
                {
                    ProcessCatchErrorMessage(e.Message.Split('\n'));
                }

            }
            return;
        }
        //*******************************************************************
        //*** Add a table rec
        //*******************************************************************
        public async Task<object> InsertTableRec(object obj)
        {
            try
            {
                switch (PartitionKey)
                {
                    case "UserProfile":
                        UserProfile userProfile = (UserProfile)obj;
                        if (tableClient != null)
                        {
                            var result = await tableClient.AddEntityAsync<UserProfile>(userProfile).ConfigureAwait(false);
                            StatusCode = result.Status;
                        }
                        break;
                    case "ActivityLog":
                        ActivityLog activity = (ActivityLog)obj;
                        if (tableClient != null)
                        {
                            var result = await tableClient.AddEntityAsync<ActivityLog>(activity).ConfigureAwait(false);
                            StatusCode = result.Status;
                        }
                        break;
                    case "SuttaPageData":
                        SuttaPageData pages = (SuttaPageData)obj;
                        if (tableClient != null)
                        {
                            var result = await tableClient.AddEntityAsync<SuttaPageData>(pages).ConfigureAwait(false);
                            StatusCode = result.Status;
                        }
                        break;
                    case "UserPageActivity":
                        UserPageActivity userPageActivity = (UserPageActivity)obj;
                        if (tableClient != null)
                        {
                            var result = await tableClient.AddEntityAsync<UserPageActivity>(userPageActivity).ConfigureAwait(false);
                            StatusCode = result.Status;
                        }
                        break;
                    case "SuttaInfo":
                        SuttaInfo suttaInfo = (SuttaInfo)obj;
                        if (tableClient != null)
                        {
                            var result = await tableClient.AddEntityAsync<SuttaInfo>(suttaInfo).ConfigureAwait(false);
                            StatusCode = result.Status;
                        }
                        break;
                    case "KeyValueData":
                        KeyValueData keyValueData = (KeyValueData)obj;
                        if (tableClient != null)
                        {
                            var result = await tableClient.AddEntityAsync<KeyValueData>(keyValueData).ConfigureAwait(false);
                            StatusCode = result.Status;
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                ProcessCatchErrorMessage(e.Message.Split('\n'));
            }
            return obj;
        }
        //*******************************************************************
        //*** Update a table rec
        //*******************************************************************
        public async Task UpdateTableRec(object obj)
        {
            try
            {
                switch (PartitionKey)
                {
                    case "UserProfile":
                        UserProfile rec = (UserProfile)obj;
                        if (tableClient != null)
                        {
                            var result = await tableClient.UpdateEntityAsync<UserProfile>(rec, rec.ETag).ConfigureAwait(false);
                            StatusCode = result.Status;
                            DBErrMsg = string.Empty;
                        }
                        break;
                    case "UserPageActivity":
                        UserPageActivity msg = (UserPageActivity)obj;
                        if (tableClient != null)
                        {
                            var result = await tableClient.UpdateEntityAsync<UserPageActivity>(msg, msg.ETag).ConfigureAwait(false);
                            StatusCode = result.Status;
                            DBErrMsg = string.Empty;
                        }
                        break;
                    case "SuttaInfo":
                        SuttaInfo suttaInfo = (SuttaInfo)obj;
                        if (tableClient != null)
                        {
                            var result = await tableClient.UpdateEntityAsync(suttaInfo, suttaInfo.ETag).ConfigureAwait(false);
                            StatusCode = result.Status;
                            DBErrMsg = string.Empty;
                        }
                        break;
                    case "SuttaPageData":
                        SuttaPageData suttaPageData = (SuttaPageData)obj;
                        if (tableClient != null)
                        {
                            var result = await tableClient.UpdateEntityAsync(suttaPageData, suttaPageData.ETag).ConfigureAwait(false);
                            StatusCode = result.Status;
                            DBErrMsg = string.Empty;
                        }
                        break;
                    case "KeyValueData":
                        KeyValueData keyValueData = (KeyValueData)obj;
                        if (tableClient != null)
                        {
                            var result = await tableClient.UpdateEntityAsync(keyValueData, keyValueData.ETag).ConfigureAwait(false);
                            StatusCode = result.Status;
                            DBErrMsg = string.Empty;
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                //StatusCode = ProcessCatchErrorMessage(e.Message.Split('\n'));
            }
            return;
        }
        //*******************************************************************
        //*** Delete a table rec
        //*******************************************************************
        public async Task DeleteTableRec(object obj)
        {
            try
            {
                switch (PartitionKey)
                {
                    //case "UserProfile":
                    //    UserProfile rec = (UserProfile)obj;
                    //    rec.PartitionKey = PartitionKey;
                    //    if (tableClient != null)
                    //    {
                    //        var result = await tableClient.DeleteEntityAsync(rec.PartitionKey, rec.RowKey).ConfigureAwait(false);
                    //        StatusCode = result.Status;
                    //        DBErrMsg = String.Empty;
                    //    }
                    //    break;
                    //case "SuttaPageAssignment":
                    //    SuttaPageAssignment sutta = (SuttaPageAssignment)obj;
                    //    if (tableClient != null)
                    //    {
                    //        var result = await tableClient.DeleteEntityAsync(sutta.PartitionKey, sutta.RowKey).ConfigureAwait(false);
                    //        StatusCode = result.Status;
                    //        DBErrMsg = String.Empty;
                    //    }
                    //    break;
                    //case "UserPageActivity":
                    //    UserPageActivity userPageActivity = (UserPageActivity)obj;
                    //    if (tableClient != null)
                    //    {
                    //        var result = await tableClient.DeleteEntityAsync(userPageActivity.PartitionKey, userPageActivity.RowKey).ConfigureAwait(false);
                    //        StatusCode = result.Status;
                    //        DBErrMsg = String.Empty;
                    //    }
                    //    break;
                }
            }
            catch (Exception e)
            {
                //StatusCode = ProcessCatchErrorMessage(e.Message.Split('\n'));
            }
            return;
        }
        //*******************************************************************
        //*** ProcessCatchErrorMessage
        //*******************************************************************
        private void ProcessCatchErrorMessage(string[] msg)
        {
            DBErrMsg = string.Empty;
            StatusCode = -1;
            if (msg.Length == 0) return;
            DBErrMsg = msg[0];
            var s1 = msg.Where(x => x.Contains("Status"));
            if (s1 != null)
            {
                var s = s1.ToArray()[0];
                int start = s.IndexOf(" ") + 1;
                int end = s.IndexOfAny(new char[] { ' ', ','}, start);
                if (end != -1)
                    StatusCode = Convert.ToInt16(s.Substring(start, end - start));
            }
        }
    }
    public class TipitakaFileStorage
    {
#nullable enable
        //string connectionString = "DefaultEndpointsProtocol=https;AccountName=dystorage2021;AccountKey=JopS1zXQXsvQNAAbAVRNIbY5qDzFeVyXzJgKwgpnYLsODtRfjIx5UDwr7J5EYit8aKMeCyPxdM12pz/6SYfBGQ==;TableEndpoint=https://dystorage2021.table.core.windows.net/;";
        string? connectionString = string.Empty;
        //https://docs.microsoft.com/en-us/dotnet/api/overview/azure/storage.files.shares-readme
        public int StatusCode = 0;
        public string DBErrMsg = string.Empty;
        string sourceDir;
#nullable disable
        public TipitakaFileStorage()
        {
            try
            {
                //if (ConfigurationManager.AppSettings != null)
                //{
                //    connectionString = ConfigurationManager.AppSettings["ConnectionString"];
                //}
                connectionString = "DefaultEndpointsProtocol=https;AccountName=tipitakadata2023;AccountKey=XudohdFrs+d+cgZLY69sgzf7MUHQIlAfJey8yo8BFhP7Cgnnyq3kdcnE8gusj9fAV0JvMIsb/QUB+AStqcY2Kw==;TableEndpoint=https://tipitakadata2023.table.core.windows.net/;";
            }
            catch(Exception) 
            {
                connectionString = "DefaultEndpointsProtocol=https;AccountName=dystorage2021;AccountKey=JopS1zXQXsvQNAAbAVRNIbY5qDzFeVyXzJgKwgpnYLsODtRfjIx5UDwr7J5EYit8aKMeCyPxdM12pz/6SYfBGQ==;TableEndpoint=https://dystorage2021.table.core.windows.net/;";
            }
        }
        public bool TempFileDownload(string fileName)
        {
            if (connectionString == string.Empty) return false;
            string shareName = "tipitaka-nissaya-files";
            string subDirName = "Temp";
            string localFilePath = ""; 
            DBErrMsg = string.Empty;

            // https://stackoverflow.com/questions/50605487/download-azure-file-share-documents-programmatically#:~:text=The%20easiest%20way%20to%20do,going%20to%20the%20downloaded%20folder.&text=Then%20go%20to%20your%20Storage,and%20Get%20the%20SAS%20Token.
            try
            {
                // Get a reference to a share
                ShareClient share = new ShareClient(connectionString, shareName);
                // Get a reference to a directory
                ShareDirectoryClient directory = share.GetDirectoryClient(subDirName);
                ShareFileClient file = directory.GetFileClient(fileName);
                // Download the file
                ShareFileDownloadInfo download = file.Download();
                using (FileStream stream = System.IO.File.OpenWrite(localFilePath))
                {
                    download.Content.CopyTo(stream);
                }
                file.Delete();  // delete after download
            }
            catch (Exception ex)
            {
            }
            return true;
        }
        public bool FileDownload(string dataType, int fileNo, int pgNo)
        {
            string shareName = "tipitaka-nissaya-files";
            string subDirName = dataType + "-" + fileNo.ToString("D3");
            string dirName = dataType + "\\" + subDirName;
            string fileName = subDirName + "-" + pgNo.ToString("D3") + ".jpg";
            string localFilePath = sourceDir + "\\" + fileName;
            DBErrMsg = string.Empty;

            if (connectionString!.Length == 0)
            {
                DBErrMsg = "No connection string to connect.";
                return false;
            }
            try
            {
                // Get a reference to a share
                ShareClient share = new ShareClient(connectionString, shareName);
                // Get a reference to a directory
                ShareDirectoryClient directory = share.GetDirectoryClient(dirName);
                ShareFileClient file = directory.GetFileClient(fileName);
                // Download the file
                ShareFileDownloadInfo download = file.Download();
                using (FileStream stream = System.IO.File.OpenWrite(localFilePath))
                {
                    download.Content.CopyTo(stream);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                DBErrMsg = ex.Message;
                return false;
                //string s = TipitakaDB.ProcessCatchErrorMessageStr(ex.Message.Split('\n'));
                //string[] f = s.Split("|");
                //if (f.Length == 2)
                //{
                //    StatusCode = Convert.ToInt32(f[0]);
                //    DBErrMsg = f[1];
                //}
            }
            return true;
        }
        public void SourceFileDownload(string dataType, string fileName)
        {
            string shareName = "tipitaka-nissaya-files";
            string subDirName = string.Format("{0}\\{0}-PDF", dataType);
            string dirName = dataType + "\\" + subDirName;
            string localFilePath = Directory.GetCurrentDirectory() + "\\" + sourceDir + "\\" + fileName;
            //MessageBox.Show("LocalFileDirPath = " + localFilePath);
            DBErrMsg = string.Empty;

            if (connectionString!.Length == 0)
            {
                DBErrMsg = "No connection string to connect.";
                return;
            }
            try
            {
                // Get a reference to a share
                ShareClient share = new ShareClient(connectionString, shareName);
                // Get a reference to a directory
                ShareDirectoryClient directory = share.GetDirectoryClient(subDirName);
                ShareFileClient file = directory.GetFileClient(fileName);
                // Download the file
                ShareFileDownloadInfo download = file.Download();
                using (FileStream stream = System.IO.File.OpenWrite(localFilePath))
                {
                    download.Content.CopyTo(stream);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                DBErrMsg = ex.Message;
                return;
            }
        }
    }
#nullable disable
}
