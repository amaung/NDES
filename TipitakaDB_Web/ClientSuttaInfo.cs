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
        public List<SuttaInfo> QuerySuttaInfo(string suttaType)
        {
            List<SuttaInfo> list = new List<SuttaInfo>();
            string rowKey = string.Format("RowKey ge '{0}' and RowKey le '{0}z'", suttaType);
            QueryTableRec(rowKey).Wait();
            list = (List<SuttaInfo>)objResult;
            return list;
        }
        public SortedDictionary<string, string> GetAllSuttaInfo()
        {
            SortedDictionary<string, string> sortedSuttaList = new SortedDictionary<string, string>();
            List<SuttaInfo> list = new List<SuttaInfo>();
            string rowKey = "";
            QueryTableRec(rowKey).Wait();
            list = (List<SuttaInfo>)objResult;
            foreach(var suttaInfo in list)
                sortedSuttaList.Add(suttaInfo.RowKey, suttaInfo.Title);
            return sortedSuttaList;
        }
    }
}
