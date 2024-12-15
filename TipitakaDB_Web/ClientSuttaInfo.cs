using System.Collections.Generic;
using Tipitaka_DBTables;

namespace Tipitaka_DB
{
    public class ClientSuttaInfo : TipitakaDB_w
    {
        const string _SuttaInfo_ = "SuttaInfo";
        public ClientSuttaInfo() : base(_SuttaInfo_)
        { }
        public void AddSuttaInfo(SuttaInfo suttaInfo)
        {
            InsertTableRec(suttaInfo).Wait();
        }
        public void UpdateSuttaInfo(SuttaInfo suttaInfo)
        {
            RetrieveTableRec(suttaInfo.RowKey).Wait();
            if (StatusCode == 200)
            {
                SuttaInfo newSuttaInfo = (SuttaInfo)objResult;
                newSuttaInfo.Copy(suttaInfo);
                UpdateTableRec(newSuttaInfo).Wait();
            }
        }
        public void UpdateSuttaInfo(string docNo, string status, string team)
        {
            RetrieveTableRec(docNo).Wait();
            if (StatusCode == 200)
            {
                SuttaInfo newSuttaInfo = (SuttaInfo)objResult;
                newSuttaInfo.Status = status;
                newSuttaInfo.Team = team;
                UpdateTableRec(newSuttaInfo).Wait();
            }
        }
        public SuttaInfo? GetSuttaInfo(string rowKey)
        {
            SuttaInfo? newSuttaInfo = null;
            RetrieveTableRec(rowKey).Wait();
            if (StatusCode == 200)
            {
                newSuttaInfo = (SuttaInfo)objResult;
            }
            return newSuttaInfo;
        }
        public void DeleteSuttaInfo(SuttaInfo suttaInfo) 
        {
            DeleteTableRec(suttaInfo).Wait();
        }
        public List<SuttaInfo> QuerySuttaInfo(string query)
        {
            List<SuttaInfo> list = new List<SuttaInfo>();
            //string rowKey = string.Format("RowKey ge '{0}' and RowKey le '{0}z'", suttaType);
            QueryTableRec(query).Wait();
            list = (List<SuttaInfo>)objResult;
            return list;
        }
        public async Task<List<SuttaInfo>> QuerySuttaInfoAsync(string query)
        {
            List<SuttaInfo> list = new List<SuttaInfo>();
            await QueryTableRec(query);
            list = (List<SuttaInfo>)objResult;
            return list;
        }
        public SortedDictionary<string, string> GetAllSuttaInfo(bool withData = true)
        {
            SortedDictionary<string, string> sortedSuttaList = new SortedDictionary<string, string>();
            List<SuttaInfo> list = new List<SuttaInfo>();
            string query = "";
            if (withData) { query = "PagesSubmitted gt 0"; }
            QueryTableRec(query).Wait();
            list = (List<SuttaInfo>)objResult;
            foreach(var suttaInfo in list)
                sortedSuttaList.Add(suttaInfo.RowKey, suttaInfo.Title + "|" + suttaInfo.NoPages.ToString());
            return sortedSuttaList;
        }
        public SortedDictionary<string, string> GetCompletedSuttaInfo()
        {
            SortedDictionary<string, string> sortedSuttaList = new SortedDictionary<string, string>();
            List<SuttaInfo> list = new List<SuttaInfo>();
            string query = "Status eq 'Completed' or Status eq 'Done'";
            QueryTableRec(query).Wait();
            list = (List<SuttaInfo>)objResult;
            foreach (var suttaInfo in list)
                sortedSuttaList.Add(suttaInfo.RowKey, suttaInfo.Title + "|" + suttaInfo.NoPages.ToString());
            return sortedSuttaList;
        }
        public async Task<int> CompletedDocCount()
        {
            string query = "Status eq 'Completed'";
            await QueryTableRec(query);
            return ((List<SuttaInfo>)objResult).Count;
        }
        public void DeleteAll(string userID)
        {
            if (userID == null || userID != "dhammayaungchi2011@gmail.com") return;
            string query = "";
            QueryTableRec(query).Wait();
            List<SuttaInfo> list = (List<SuttaInfo>)objResult;
            if (list.Count > 0)
            {
                List<object> list1 = list.ToList<object>();
                DeleteTableRecBatch(list1).Wait();
            }
        }
    }
}
