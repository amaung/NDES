﻿using Tipitaka_DBTables;

namespace Tipitaka_DB
{
#nullable enable
    public class ClientSuttaPageData : TipitakaDB_w
    {
        public const string _SuttaPageData_ = "SuttaPageData";
        ClientTipitakaDB_w? clientTipitakaDB;
        //ClientUserPageActivity? clientUserPageActivity;
        //ClientSuttaPageAssignment? clientSuttaAssignment;
        //ClientTipitakaDBLogin? clientTipitakaDBLogin;
        //ClientUpdateHistory? clientUpdateHistory;

        //ClientSuttaInfo? clientSuttaInfo;

        //*******************************************************************
        //*** ClientSuttaPageData
        //*******************************************************************
        public ClientSuttaPageData(ClientTipitakaDB_w clientTipitakaDB) : base(_SuttaPageData_) 
        { 
            this.clientTipitakaDB = clientTipitakaDB;
            //clientTipitakaDBLogin = clientTipitakaDB.GetClientTipitakaDBLogin();
            //clientUserPageActivity = clientTipitakaDB.GetClientUserPageActivity();
        }
        //Dictionary<string, string>
        public async Task UploadSutta(string userID, string docID, Dictionary<string, string> dataPages)
        {
            if (clientTipitakaDB == null) { return; }
            var dataList = new List<SuttaPageData>();
            SetSubPartitionKey(docID);
            foreach (KeyValuePair<string, string> item in dataPages)
            {
                string pgnoKey = "00" + item.Key;
                pgnoKey = pgnoKey.Substring(pgnoKey.Length-3);
                var pageData = new SuttaPageData()
                {
                    //PartitionKey = docID,
                    PageNo = Convert.ToInt16(item.Key),
                    RowKey = string.Format("{0}-{1}", docID, pgnoKey),
                    PageData = item.Value,
                    UserID = userID,
                };
                dataList.Add(pageData);
            };
            await InsertTableRecBatch(dataList);
        }
        public async Task InsertSuttaPages(string userID, string docID, Dictionary<string, string> dataPages)
        {
            //InsertTableRec
            if (clientTipitakaDB == null) { return; }
            foreach (KeyValuePair<string, string> item in dataPages)
            {
                string pgnoKey = "00" + item.Key;
                pgnoKey = pgnoKey.Substring(pgnoKey.Length - 3);
                var pageData = new SuttaPageData()
                {
                    //PartitionKey = docID,
                    PageNo = Convert.ToInt16(item.Key),
                    RowKey = string.Format("{0}-{1}", docID, pgnoKey),
                    PageData = item.Value,
                    UserID = userID,
                };
                await InsertTableRec(pageData);
            };
        }
        public int UpdateSuttaPageData(SuttaPageData suttaPageData)
        {
            UpdateTableRec(suttaPageData).Wait();
            return StatusCode;
        }
        public void UploadSutta(string userID, string dataType, int suttaNo, Dictionary<string, string> dataPages,
                Dictionary<int, int> NISRecCount)
        {
            //int pgNo = 0, pagesSubmitted = 0;
            //if (clientTipitakaDB == null) { return; }
            //clientUserPageActivity = clientTipitakaDB.GetClientUserPageActivity();

            //clientSuttaAssignment = clientTipitakaDB.GetClientSuttaAssignment();
            //clientSuttaInfo = clientTipitakaDB.GetClientSuttaInfo();

            //SuttaPageData suttaPageData = new SuttaPageData();
            //string partitionKey, rowKey;
            //partitionKey = rowKey = String.Format("{0}-{1}", dataType, suttaNo.ToString("D3"));
            //SetSubPartitionKey(partitionKey);
            ////// check if user has been assigned to this 
            ////clientSuttaAssignment.GetSuttaPageAssignment(partitionKey, userID);
            //clientSuttaAssignment.QuerySuttaPageAssignment(rowKey);
            //List<SuttaPageAssignment> listSuttaPageAssignment = (List<SuttaPageAssignment>)clientSuttaAssignment.objResult;
            //if (listSuttaPageAssignment.Count == 0 || listSuttaPageAssignment.Count(x => x.AssignedTo == userID) == 0)
            //{
            //    MessageBox.Show(userID + " is not the assigned user for " + rowKey); return;
            //}
            //// get sutta page data from the server
            //string MNno = String.Format("{0}-{1}", dataType, suttaNo.ToString("D3"));
            //SetSubPartitionKey(MNno);
            //SortedDictionary<int, SuttaPageData> dictSuttaDataPages = GetSuttaPageData();

            //foreach (KeyValuePair<string, string> kv in dataPages)
            //{
            //    pgNo = Convert.ToInt16(kv.Key); //listNissayaRecords = kv.Value;
            //    rowKey = String.Format("{0}-{1}-{2}", dataType, suttaNo.ToString("D3"), pgNo.ToString("D3"));
            //    // check for existing data record
            //    if (dictSuttaDataPages.Count > 0 && dictSuttaDataPages.ContainsKey(pgNo))
            //    {
            //        // sutta already has some data on the server
            //        // skip if the uploaded page and existing page are identical
            //        if (dataPages[kv.Key] == dictSuttaDataPages[pgNo].PageData) { continue; }

            //        // do UpdateHistory
            //        SuttaPageData pageData = dictSuttaDataPages[pgNo];
            //        List<NissayaRecord> existingNissayaRecords = ParseNissayaPage(pageData.PageData);
            //        List<NissayaRecord> newNissayaRecords = ParseNissayaPage(dataPages[pgNo.ToString()]);
            //        //Dictionary<int, NissayaRecord> newNissayaRecords = ParseNissayaPage(kv.Value.ToString());
            //        switch (DoUpdateHistory(userID, MNno, pgNo, newNissayaRecords, DateTime.Now.ToUniversalTime(), existingNissayaRecords, pageData.Timestamp!.Value.UtcDateTime))
            //        {
            //            case 1:
            //                // update required
            //                pageData.PageData = kv.Value; // MakeNissayaPage(pgNo, kv.Value);
            //                pageData.Status = "Updated";
            //                UpdateTableRec(pageData).Wait();
            //                if (StatusCode != 204)
            //                {
            //                    MessageBox.Show("Page data update error." + pageData.RowKey); return;
            //                }
            //                // update UserPageActivity
            //                UpdateUserPageActivity(rowKey, "Updated", NISRecCount[pgNo]);
            //                break;
            //            default: break;
            //        }
            //    }
            //    else
            //    {
            //        // this is a new page
            //        suttaPageData.PartitionKey = partitionKey;
            //        suttaPageData.RowKey = rowKey;
            //        suttaPageData.PageNo = pgNo;
            //        suttaPageData.PageData = kv.Value; // MakeNissayaPage(pgNo, kv.Value);
            //        suttaPageData.Status = "Submitted";
            //        suttaPageData.UserID = userID;
            //        InsertTableRec(suttaPageData).Wait();
            //        if (StatusCode != 204)
            //        {
            //            MessageBox.Show("Data insert error."); return;
            //        }
            //        ++pagesSubmitted;

            //        // UserPageActivity update
            //        if (!UpdateUserPageActivity(rowKey, "Submitted", NISRecCount[pgNo])) return;
            //    }
            //}
            //if (pagesSubmitted > 0)
            //{
            //    SuttaPageAssignment? suttaPageAssignment = listSuttaPageAssignment.Find(x => (x.StartPage <= pgNo && x.EndPage >= pgNo));
            //    // update SuttaPageAssignment
            //    if (suttaPageAssignment == null) return;
            //    suttaPageAssignment.Status = "Submitted";
            //    clientSuttaAssignment.UpdateTableRec(suttaPageAssignment).Wait();
            //    if (clientSuttaAssignment.StatusCode != 204)
            //    {
            //        MessageBox.Show("SuttaPageAssignment update error at rowkey = " + suttaPageAssignment.PartitionKey
            //            + "\\" + suttaPageAssignment.RowKey);
            //        return;
            //    }

            //    // update SuttaInfo
            //    clientSuttaInfo.RetrieveTableRec(MNno).Wait();
            //    if (clientSuttaInfo.StatusCode == 200)
            //    {
            //        SuttaInfo suttaInfo = (SuttaInfo)clientSuttaInfo.objResult;
            //        suttaInfo.PagesSubmitted += pagesSubmitted;
            //        if (suttaInfo.PagesSubmitted > suttaInfo.Pages) suttaInfo.PagesSubmitted = suttaInfo.Pages;
            //        clientSuttaInfo.UpdateTableRec(suttaInfo).Wait();
            //        if (clientSuttaInfo.StatusCode != 204)
            //        {
            //            MessageBox.Show("SuttaInfo update failed at " + MNno);
            //        }
            //    }
            //}
        }
        public SuttaPageData? GetPageData(string docID, string recNo)
        {
            SetSubPartitionKey(docID);
            string s = "000" + recNo;
            string rowKey = docID + "-" + s.Substring(s.Length - 3); 
            RetrieveTableRec(rowKey).Wait();
            if (StatusCode == 404) return null;
            SuttaPageData suttaPageData = (SuttaPageData)objResult;
            return suttaPageData;
        }
        public async Task<SortedDictionary<int, string>> GetSutta(string docID)
        {
            SortedDictionary<int, string> dictSuttaPages = new SortedDictionary<int, string>();
            SetSubPartitionKey(docID);
            await QueryTableRec(docID);
            List<SuttaPageData> listSuttaPageData = (List<SuttaPageData>)objResult;
            foreach(SuttaPageData suttaPageData in listSuttaPageData)
            {
                dictSuttaPages.Add(suttaPageData.PageNo, suttaPageData.PageData);
            }
            return dictSuttaPages;
        }
        public SortedDictionary<int, SuttaPageData> GetSuttaPageData(string rowKey = "")
        {
            SortedDictionary<int, SuttaPageData> dictSuttaPages = new SortedDictionary<int, SuttaPageData>();
            QueryTableRec(rowKey).Wait();
            List<SuttaPageData> listSuttaPageData = (List<SuttaPageData>)objResult;
            foreach (SuttaPageData suttaPageData in listSuttaPageData)
            {
                dictSuttaPages.Add(suttaPageData.PageNo, suttaPageData);
            }
            return dictSuttaPages;
        }
        public String[] GetAllPartitionKeys()
        {
            //    ConcurrentDictionary<string, byte> partitionKeys = new ConcurrentDictionary<string, byte>();
            //    Parallel.ForEach(this.ExecuteQuery(new TableQuery()), entity =>
            //    {
            //        partitionKeys.TryAdd(entity.PartitionKey, 0);
            //    });
            this.QueryTableRec("LoadedSuttas").Wait();
            List<SuttaPageData> listSuttaPageData = (List<SuttaPageData>)objResult;
            List<string> result = new List<string>();
            foreach(SuttaPageData suttaPageData in listSuttaPageData)
            {
                if (!result.Contains(suttaPageData.PartitionKey))
                    result.Add(suttaPageData.PartitionKey);
            }
            return result.ToArray();
        }
    }
#nullable disable
}
