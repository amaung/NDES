using Tipitaka_DBTables;
using Tipitaka_DB;

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
            RetrieveTableRec(rowKey).Wait();
            if (StatusCode == 404) return keyValueData;
            keyValueData = (KeyValueData)objResult;
            keyValueData.PartitionKey = "KeyValueData";
            return keyValueData;
        }
        public List<KeyValueData> QuerySuttaDataInfo(string suttaNo)
        {
            string rowKey1 = String.Format("{0}-000", suttaNo);
            string rowKey2 = String.Format("{0}-999", suttaNo); ;
            string qualifier = String.Format("(RowKey gt '{0}' and RowKey lt '{1}')", rowKey1, rowKey2);
            QueryTableRec(qualifier).Wait();
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
                UpdateTableRec(keyValueData1).Wait();
                return;
            }
            if (StatusCode == 404) // record not found
            {
                // add new record
                InsertTableRec(keyValueData).Wait();
                //if (StatusCode != 204)
                //    MessageBox.Show(String.Format("KeyValueData insert error.RowKey = {0}, Value = {1}", keyValueData.RowKey, keyValueData.Value));
            }
        }
    }
}
