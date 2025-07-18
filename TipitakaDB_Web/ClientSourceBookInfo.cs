//using Tipitaka_DBTables;

using System.Collections.Generic;
using Tipitaka_DBTables;

namespace Tipitaka_DB
{
    public class ClientSourceBookInfo : TipitakaDB_w
    {
        const string _SourceBookInfo_ = "SourceBookInfo";
        ClientTipitakaDB_w? clientTipitakaDB;
        //*******************************************************************
        //*** ClientSourceBookInfo
        //*******************************************************************
        public ClientSourceBookInfo(ClientTipitakaDB_w clientTipitakaDB) : base(_SourceBookInfo_) 
        {
            this.clientTipitakaDB = clientTipitakaDB;
        }
        //*******************************************************************
        //*** Add SourceBookInfo
        //*******************************************************************
        //public void AddSourceBookInfo(string bookID, string PDFFilename, string DocNosIncluded, int DocCount, int pages, int pagesCompleted)
        //{
        //    SourceBookInfo sourceBookInfo = new SourceBookInfo() 
        //    {
        //        RowKey = bookID,
        //        BookFilename = PDFFilename,
        //        AllDocs = DocNosIncluded,
        //        DocCount = DocCount,
        //        Pages = pages,
        //        Completed = pagesCompleted,
        //    };
        //    InsertTableRec(sourceBookInfo).Wait();
        //}
        public void AddSourceBookInfo(SourceBookInfo sourceBookInfo)
        {
            InsertTableRec(sourceBookInfo).ConfigureAwait(false); //.Wait();
        }
        //*******************************************************************
        //*** Update SourceBookInfo
        //*******************************************************************
        public async Task UpdateSourceBookInfo(string bookID, int docCount, int pageCount)
        {
            try
            {
                var obj = await RetrieveTableRec(bookID).ConfigureAwait(false);
                if (StatusCode == 200)
                {
                    SourceBookInfo sourceBookInfo = (SourceBookInfo)obj;
                    sourceBookInfo.DocCount = docCount;
                    sourceBookInfo.Pages = pageCount;
                    await UpdateTableRec(sourceBookInfo).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
            }
        }
        public async Task UpdateSourceBookInfo(SourceBookInfo sourceBookInfo)
        {
            try
            {
                await UpdateTableRec(sourceBookInfo).ConfigureAwait(false);
                if (StatusCode != 204)
                {
                    ErrMsg = "Error in updating SourceBookInfo";
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
            }
        }

        //*******************************************************************
        //*** GetSourceBookInfo
        //*******************************************************************
        public async Task<List<SourceBookInfo>> GetSourceBookInfo()
        {
            List<SourceBookInfo> sourceBookInfo = new List<SourceBookInfo>();
            string query = string.Empty;
            query = string.Empty;
            var obj = await QueryTableRec(query).ConfigureAwait(false);
            sourceBookInfo = (List<SourceBookInfo>)obj;
            return sourceBookInfo;
        }
        //*******************************************************************
        //*** GetSourceBookInfo - returns SourceBookInfo of
        //*******************************************************************
        public SourceBookInfo? GetSourceBookInfo(string docID)
        {
            SourceBookInfo? sourcebookInfo = null;
            RetrieveTableRec(docID).Wait();
            if (StatusCode == 200)
            {
                sourcebookInfo = (SourceBookInfo?)objResult;
            }
            return sourcebookInfo;
        }
        public async Task<SourceBookInfo?> GetSourceBookInfoAsync(string docID)
        {
            SourceBookInfo? sourcebookInfo = null;
            await RetrieveTableRec(docID);
            if (StatusCode == 200)
            {
                sourcebookInfo = (SourceBookInfo?)objResult;
            }
            return sourcebookInfo;
        }
        public void ResetSourceBookDocs(string userID)
        {
            if (userID == null || userID != "dhammayaungchi2011@gmail.com") { return; }
            string qualifier = "DocCount gt 0 or DocNos gt ''";
            QueryTableRec(qualifier).Wait();
            List<SourceBookInfo> sourcebookInfo = (List<SourceBookInfo>)objResult;
            foreach(SourceBookInfo sourceBookInfo in sourcebookInfo)
            {
                sourceBookInfo.DocCount = 0;
                sourceBookInfo.DocNos = string.Empty;
                UpdateSourceBookInfo(sourceBookInfo).Wait();
            }
        }
    }
}
