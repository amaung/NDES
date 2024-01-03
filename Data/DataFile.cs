//using System.IO;
//using System.Linq.Expressions;
//using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System;
//using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
//using System.Xml.XPath;
//using static System.Runtime.InteropServices.JavaScript.JSType;
//using System.Net.NetworkInformation;
//using System.IO;
using Tipitaka_DBTables;
using Tipitaka_DB;
using System.Diagnostics;

//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Cryptography;
//using Azure;
//using static System.Runtime.CompilerServices.RuntimeHelpers;

// https://stackoverflow.com/questions/25391244/how-to-create-and-save-a-temporary-file-on-microsoft-azure-virtual-server

namespace NissayaEditor_Web.Data
{
    //public struct NISRecord { public string pali; public string trans; public string footnote; }
    public class NIS
    {
        public int? RecNo { get; set; } = null;
        //public bool? Para { get; set; } = false;
        public string? Pali { get; set; } = "";
        public string? Trans { get; set; } = "";
        public string? Trans2 { get; set; } = "";
        public string? Footnote { get; set; } = "";
        public string? Remarks { get; set; } = "";
        public void Clear()
        {
            RecNo = null; Pali = Trans = Trans2 = Footnote = Remarks = "";
        }
    }
    public class DataFile
    {
        public string fname = string.Empty;
        private string fext = string.Empty;
        public string fileContent = string.Empty;
        public string headerData = string.Empty;
        public string version = string.Empty;
        public string newParaMarker = "။ ။";    // default
        public string email = string.Empty;
        public string userName = string.Empty;
        public string DocID { get; set; } = string.Empty;
        public string DocTitle { get; set; } = string.Empty;
        public Dictionary<string, List<NIS>> Pages = new Dictionary<string, List<NIS>>();
        public List<string> pageNos = new List<string>();
        public char[] fieldSeparators = { ',', '\t' };
        public string ErrMsg = string.Empty;
        private bool currentPageUploaded = false;
        ClientTipitakaDB_w? clientUserTipitakaDB = null;
        ClientSuttaInfo? clientSuttaInfo = null;
        ClientSuttaPageData? clientSuttaPageData = null;
        ClientKeyValueData? clientKeyValueData = null;
        ClientActivityLog? clientActivityLog = null;
        ClientUserPageActivity? clientUserPageActivity = null;
        TipitakaFileStorage? tipitakaFileStorage = new TipitakaFileStorage();

        public DataFile(ClientTipitakaDB_w? clientUserTipitakaDB)
        {
            if (clientUserTipitakaDB != null)
            {
                this.clientSuttaInfo = clientUserTipitakaDB.GetClientSuttaInfo();
                this.clientSuttaPageData = clientUserTipitakaDB.GetClientSuttaPageData();
                this.clientKeyValueData = clientUserTipitakaDB.GetClientKeyValueData();
                //this.clientActivityLog = clientUserTipitakaDB.GetClientActivityLog();
                this.clientUserPageActivity = clientUserTipitakaDB.GetClientUserPageActivity();
            }
        }
        public DataFile(ClientTipitakaDB_w? clientUserTipitakaDB, string email, string userName)
        {
            this.email = email;
            this.userName = userName;
            this.clientUserTipitakaDB = clientUserTipitakaDB;
            if (clientUserTipitakaDB != null)
            {
                this.clientSuttaInfo = clientUserTipitakaDB.GetClientSuttaInfo();
                this.clientSuttaPageData = clientUserTipitakaDB.GetClientSuttaPageData();
                this.clientKeyValueData = clientUserTipitakaDB.GetClientKeyValueData();
                //this.clientActivityLog = clientUserTipitakaDB.GetClientActivityLog();
                this.clientUserPageActivity = clientUserTipitakaDB.GetClientUserPageActivity();
            }
        }
        public void ClearPageData() { Pages = new Dictionary<string, List<NIS>>(); }
        public void SetFileContent(string fnm, string content)
        {
            fname = fnm;
            fileContent = content;
            if (fileContent.Length > 0)
            {
                fext = fname.Substring(fname.Length - 3).ToLower();
                if (fext == "txt") CleanupMarkers();
                SpellChecker sp = new SpellChecker();
                fileContent = sp.CorrectCommonErrors(fileContent);
                ExtractHeader();
                parseIntoPages();
            }
        }
        private void CleanupMarkers()
        {
            fileContent = fileContent.Replace("* ", "*");
            fileContent = fileContent.Replace("^ ", "^");
            fileContent = fileContent.Replace("^*", "^ *");
            fileContent = fileContent.Replace("*^", "* ^");
            if (fileContent[fileContent.Length - 1] == '^') fileContent += " ";
        }
        private void ExtractHeader()
        {
            headerData = string.Empty;
            int pos = fileContent.IndexOf("}\r");
            if (pos == -1)
            {
                pos = fileContent.IndexOf("}\n");
                if (pos == -1) return;
            }

            string s = fileContent.Substring(0, pos + 1).Trim();
            if (s[0] == '{' && s[s.Length - 1] == '}')
            {
                headerData = s = s.Substring(1, s.Length - 2);
                if (headerData.Length > 0)
                {
                    string[] f = headerData.Split(';');
                    for (int i = 0; i < f.Length; i++)
                    {
                        string[] ff = f[i].Split("=");
                        if (ff.Length > 1)
                        {
                            switch (ff[0].Trim().ToLower())
                            {
                                case "version":
                                    version = ff[1].Trim();
                                    break;
                                case "id":
                                    DocID = ff[1].Replace("\"", "").Trim();
                                    break;
                                case "title":
                                    DocTitle = ff[1].Replace("\"", "").Trim();
                                    break;
                                case "newparamarker":
                                    newParaMarker = ff[1].Replace("\"", "").Trim();
                                    if (newParaMarker.Length == 0) { newParaMarker = "none"; }
                                    break;
                            }
                        }
                    }
                }
            }
            fileContent = fileContent.Substring(pos + 1).Trim();
            return;
        }
        private void parseIntoPages()
        {
            ErrMsg = string.Empty;
            if (fname.EndsWith(".txt")) parseTXTIntoPages();
            else
            {
                if (fname.EndsWith(".csv") || fname.EndsWith(".tsv")) parseCSVIntoPages();
                else ErrMsg = string.Format("'{0}' file extension not recognized.", fname);
            }
        }
        private void parseTXTIntoPages()
        {
            List<string> paraPages = fileContent.Split('\n').ToList();
            pageNos = new List<string>();
            int recno = 0;
            string pgno = string.Empty;
            try
            {
                List<NIS> listNISRecords = new List<NIS>();
                // cleanup
                foreach (string s in paraPages)
                {
                    string s1 = s.Trim();
                    if (s1.Length > 0)
                    {
                        if (s1[0] == '{' && s1[s1.Length - 1] == '}') continue;
                        // find page #
                        if (s1[0] == '#')
                        {
                            listNISRecords = new List<NIS>();
                            int p = s1.IndexOf('*');
                            if (p > -1)
                            {
                                pgno = s1.Substring(1, p - 1).Trim();
                                s1 = s1.Substring(p);
                            }
                        }
                        string[] nisRecords = s1.Split(new char[] { '*' });
                        recno = 0;
                        foreach (string nisRecord in nisRecords)
                        {
                            if (nisRecord.Trim().Length > 0)
                            {
                                string[] f = nisRecord.Trim().Split(new char[] { '^', '@' });
                                if (f.Length > 0)
                                {
                                    NIS nis = new NIS();
                                    nis.RecNo = ++recno;
                                    nis.Pali = f[0].Trim();
                                    if (f.Length > 1) nis.Trans = f[1].Trim();
                                    if (f.Length == 3) nis.Footnote = f[2].Trim();
                                    listNISRecords.Add(nis);
                                }
                            }
                        }
                        // store pgno and NIS recrods
                        Pages[pgno] = listNISRecords;
                        pageNos.Add(pgno);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrMsg = string.Format("Error: {0} After Page {1} RecNo {2}.", ex.Message, pgno, recno.ToString());
            }
        }
        private void parseCSVIntoPages()
        {
            int len = fileContent.Length;
            List<string> paraPages = fileContent.Split('\n').ToList();
            pageNos = new List<string>();
            int recno = 0;
            string pgno = string.Empty;
            ErrMsg = string.Empty;
            try
            {
                List<NIS> listNISRecords = new List<NIS>();
                // cleanup
                pgno = string.Empty;
                foreach (string s in paraPages)
                {
                    string s1 = s.Trim();
                    if (s1.Length > 0)
                    {
                        if (s1[0] == '{' && s1[s1.Length - 1] == '}') continue;
                        string[] nisfields = s1.Split(fieldSeparators);
                        if (nisfields.Length > 2 && nisfields[0].Length == 0 && nisfields[1].Length == 0) continue;
                        // find page #
                        if (nisfields[0].Length > 0 && nisfields[0][0] == '#')
                        {
                            if (pgno.Length > 0 && listNISRecords.Count > 0)
                            {
                                Pages[pgno] = listNISRecords;
                                pageNos.Add(pgno);
                                recno = 0;
                            }
                            listNISRecords = new List<NIS>();
                            pgno = nisfields[0].Substring(1).Trim();
                            continue;
                        }
                        if (nisfields[0].Length > 0 && nisfields[1].Length > 0)
                        {
                            NIS nis = new NIS();
                            nis.RecNo = ++recno;
                            if (nisfields.Length >= 1)
                            {
                                nis.Pali = nisfields[0].Trim();
                                if (nis.Pali.Length > 0 && nis.Pali[0] == '*')
                                { nis.Pali = nis.Pali.Substring(1).Trim(); }
                            }
                            if (nisfields.Length >= 2)
                            {
                                nis.Trans = nisfields[1].Trim();
                                if (nis.Trans.Length == 0)
                                {
                                    // Error: no translation
                                    ErrMsg = string.Format("Error: Translation missing at {0} RecNo {1}.", pgno, recno);
                                    return;
                                }
                                if (nis.Trans[0] == '@')
                                { nis.Trans = nis.Trans.Substring(1).Trim(); }
                            }
                            if (nisfields.Length >= 3)
                            {
                                nis.Footnote = string.Empty;
                                nis.Remarks = nisfields[2].Trim();
                            }
                            listNISRecords.Add(nis);
                        }
                    }
                }
                // store pgno and NIS recrods
                if (pgno.Length > 0 && listNISRecords.Count > 0)
                {
                    Pages[pgno] = listNISRecords;
                    pageNos.Add(pgno);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = string.Format("Error: {0} after Page {1} RecNo {2}.", ex.Message, pgno, recno);
            }
        }
        public List<NIS> GetPageData(string pgno)
        {
            if (Pages.ContainsKey(pgno))
            {
                return Pages[pgno];
            }
            return new List<NIS>();
        }
        public void SetPageData(string pgno, List<NIS> listNISRecords)
        {
            Pages[pgno].Clear(); Pages[pgno] = new List<NIS>(listNISRecords);
        }
        public List<NIS> GetAllPageData()
        {
            ErrMsg = "";
            List<NIS> allPages = new List<NIS>();
            try
            {
                foreach (string pgno in pageNos)
                {
                    List<NIS> pageData = GetPageData(pgno).ToList();
                    NIS nis = new NIS();
                    allPages.Add(nis);  // empty line
                    nis = new NIS()
                    {
                        RecNo = Convert.ToInt16(pgno),
                        Pali = "### Page Number ###",
                        Trans = "### Page Number ###"
                    };
                    allPages.Add(nis);
                    allPages.AddRange(pageData);
                }
                return allPages;
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
            }
            return allPages;
        }
        public void AddNISRecord(string pgno, NIS record)
        {
            if (Pages == null) { Pages = new Dictionary<string, List<NIS>>(); }
            if (Pages.ContainsKey(pgno))
            {
                Pages[pgno].Add(record);
            }
            else
            {
                List<NIS> nisRecords = new List<NIS>();
                nisRecords.Add(record);
                Pages[pgno] = nisRecords;
            }
        }
        public void DelNISRecord(string pgno, NIS record)
        {
            if (Pages.ContainsKey(pgno))
            {
                List<NIS> nisRecords = Pages[pgno];
                int recno = Convert.ToInt16(record.RecNo);
                if (nisRecords[recno - 1].RecNo == record.RecNo)
                    nisRecords.RemoveAt(recno - 1);
            }
        }
        public List<string> GetPageNo() { return pageNos; }
        public string GetDocID() { return DocID; }
        public string GetDocTitle() { return DocTitle; }
        public string GetNewParaMarker() { return newParaMarker; }
        public string GetVersion() { return version; }
        public int GetNoPages() { return Pages.Count; }
        public void AddNewPage(string pgno)
        {
            if(!pageNos.Contains(pgno)) { pageNos.Add(pgno);}
            if (!Pages.ContainsKey(pgno))
                Pages[pgno] = new List<NIS>();
        }
        public Dictionary<string, string> GetUploadPageData()
        {
            Dictionary<string, string> pageData = new Dictionary<string, string>();
            int n = 0; ErrMsg = "";
            foreach (KeyValuePair<string, List<NIS>> kv in Pages)
            {
                string page_data = string.Empty;
                n = 0;
                foreach (NIS nis in kv.Value)
                {
                    ++n;
                    if (nis.Pali!.Length == 0 || nis.Trans!.Length == 0)
                    {
                        ErrMsg = string.Format("Empty Pali or Trans text found in Page {0} Sr No #{1}. ", kv.Key, n);
                        return pageData;
                    }
                    if (nis.Pali!.Length > 0)
                    {
                        if (page_data.Length > 0) page_data += " *" + nis.Pali;
                        else page_data += "*" + nis.Pali;
                        if (nis.Trans != null && nis.Trans.Length > 0) { page_data += " ^" + nis.Trans; }
                        if (nis.Footnote != null && nis.Footnote.Length > 0) { page_data += " @" + nis.Footnote; }
                        if (nis.Remarks != null && nis.Remarks.Length > 0) { page_data += " !" + nis.Remarks; }
                    }
                }
                pageData[kv.Key] = page_data;
            }
            return pageData;
        }
        public async Task<string> UploadDocument()
        {
            int ErrCode = 0;
            ErrMsg = string.Empty;
            if (clientSuttaInfo == null || clientSuttaPageData == null)
            {
                ErrMsg = "ClientTipitakaDB is null.";
                return ErrMsg;
            }
            bool newSuttaInfo = false;
            SuttaInfo? suttaInfo = clientSuttaInfo.GetSuttaInfo(DocID);
            if (suttaInfo != null)
            {
                // existing doc
                if (!suttaInfo.SubmittedBy.Contains(email))
                {
                    suttaInfo.SubmittedBy += ", " + email;
                }
            }
            else
            {
                // new doc
                newSuttaInfo = true;
                suttaInfo = new SuttaInfo();
                suttaInfo.SubmittedBy = email;
                suttaInfo.NISRecCount = 0;
                suttaInfo.StartPage = Convert.ToInt16(Pages.First().Key);
                suttaInfo.EndPage = Convert.ToInt16(Pages.Last().Key);
            }
            suttaInfo.Title = DocTitle;
            suttaInfo.RowKey = DocID;
            //suttaInfo.SubmittedBy = email;
            suttaInfo.NoPages += Pages.Count();
            suttaInfo.StartPage = Math.Min(suttaInfo.StartPage, Convert.ToInt16(Pages.First().Key));
            suttaInfo.EndPage = Math.Max(suttaInfo.EndPage, Convert.ToInt16(Pages.Last().Key));
            foreach (string pgno in Pages.Keys)
            {
                suttaInfo.NISRecCount += Pages[pgno].Count();
            }
            if (newSuttaInfo)
                clientSuttaInfo.AddSuttaInfo(suttaInfo);
            else
                clientSuttaInfo.UpdateSuttaInfo(suttaInfo);

            if (clientSuttaInfo.StatusCode != 204) ErrMsg = clientSuttaInfo.DBErrMsg;

            if (ErrMsg.Length == 0)
            {
                // https://stackoverflow.com/questions/19003594/fire-callback-after-async-task-method
                Dictionary<string, string> data = GetUploadPageData();
                if (ErrMsg.Length == 0) 
                {
                    //UploadDocument2(data, callback);
                    await clientSuttaPageData!.InsertTableRecBatch(DocID, email, data);
                    if (clientSuttaPageData.StatusCode != 202)
                    {
                        ErrCode = clientSuttaPageData.StatusCode;
                        ErrMsg = clientSuttaPageData.DBErrMsg;
                    }
                    if (clientUserPageActivity != null)
                    {
                        clientUserPageActivity.AddUserPageActivity(email, DocID, "Upload", suttaInfo.StartPage, suttaInfo.EndPage);
                    }
                }
            }
            return ErrMsg;
        }
        public void UpdateCurrentPage(string docID, string pgno, List<NIS> nisRecords)
        {
            if (docID.Length == 0 || pgno.Length == 0)
            {
                ErrMsg = "Doc or page number is empty."; return;
            }
            if (clientSuttaPageData == null)
            {
                ErrMsg = "clientSuttaPageData is null"; return;
            }
            bool newInsert = false;
            // retrienve current page data
            SuttaPageData? suttaPageData = clientSuttaPageData.GetPageData(docID, pgno);
            if (suttaPageData == null)
            {
                // the current page is new
                suttaPageData = new SuttaPageData();
                if (suttaPageData != null)
                {
                    suttaPageData.PartitionKey = docID;
                    string pno = "000" + pgno;
                    suttaPageData.RowKey = docID + "-" + pno.Substring(pno.Length - 3);
                    suttaPageData.PageNo = Convert.ToInt16(pgno);
                    suttaPageData.UserID = email;
                    newInsert = true;
                }
            }
            if (!suttaPageData!.UserID.Contains(email)) suttaPageData.UserID += ", " + email;
            if (newInsert)
            {
                if (Pages.ContainsKey(pgno) && Pages[pgno].Count == 0) Pages[pgno] = nisRecords;
                if (!Pages.ContainsKey(pgno)) Pages.Add(pgno, nisRecords);
                // update SuttaInfo for new page
                SuttaInfo? suttaInfo = clientSuttaInfo!.GetSuttaInfo(docID);
                if (suttaInfo != null)
                {
                    int n = Convert.ToInt16(pgno);
                    if (suttaInfo.EndPage < n) suttaInfo.EndPage = n;
                    if (suttaInfo.StartPage > n) suttaInfo.StartPage = n;
                    suttaInfo.NoPages = suttaInfo.EndPage - suttaInfo.StartPage + 1;
                    clientSuttaInfo.UpdateSuttaInfo(suttaInfo);
                }
                // update UserPageActivity
                if (clientUserPageActivity != null)
                {
                    string rowKey = string.Format("{0}-{1}", email!, docID!);
                    UserPageActivity? userPageActivity = clientUserPageActivity.GetUserPageActivity(rowKey);
                    if (userPageActivity != null)
                    {
                        string[] f = userPageActivity.PageRange.Split("-");
                        int pno1 = Convert.ToInt16(f[0]);
                        int pno2 = Convert.ToInt16(f[1]);
                        int pno = Convert.ToInt16(pgno);
                        if (pno == pno1 - 1) pno1--;
                        if (pno == pno2 + 1) pno2++;
                        userPageActivity.Pages = pno2 - pno1 + 1;
                        userPageActivity.PageRange = string.Format("{0}-{1}", pno1, pno2);
                        clientUserPageActivity.UpdateUserPageActivity(userPageActivity);
                    }
                    else
                    {
                        int n = Convert.ToInt16(pgno);
                        clientUserPageActivity.AddUserPageActivity(email, docID, "Entry", n, n);
                        if (clientUserPageActivity.StatusCode != 204) ErrMsg = "Insert error to UserPageActiviy";
                    }
                }
            }
            suttaPageData!.PageData = NISPageDataToServerData(Pages[pgno]);
            int statusCode;
            if (newInsert)
            {
                clientSuttaPageData!.InsertTableRec(suttaPageData).Wait();
                statusCode = clientSuttaPageData.StatusCode;
            }
            else statusCode = clientSuttaPageData.UpdateSuttaPageData(suttaPageData);
            if (statusCode != 204)
            {
                ErrMsg = "Page data update error."; return;
            }
        }
        public void AddNewPageData(string pgno, List<NIS> nisRecords)
        {
            Pages.Add(pgno, nisRecords);
        }
        private string NISPageDataToServerData(List<NIS> pageData, bool includeRemarks = true)
        {
            string rec = "";
            int n = 0;
            string space = " ";
            foreach (NIS page in pageData)
            {
                if (includeRemarks)
                {
                    ++n;
                    space = (n == 1) ? "" : " ";
                }
                rec += space + "*" + page.Pali;
                rec += " ^" + page.Trans;
                if (page.Footnote != null && page.Footnote.Length > 0) rec += " @" + page.Footnote;
                if (includeRemarks && page.Remarks != null && page.Remarks.Length > 0) rec += " !" + page.Remarks;
            }
            return rec;
        }
        public string PagesToFileContent()
        {
            fileContent = "";
            foreach (KeyValuePair<string, List<NIS>> kv in Pages)
            {
                if (fileContent.Length > 0) fileContent += "\r\n\r\n";
                fileContent += "#" + kv.Key;
                fileContent += NISPageDataToServerData(kv.Value, false);
            }
            return fileContent;
        }
        public List<string> GetSuttaList(string docType)
        {
            List<string> suttaList = new List<string>();
            if (clientSuttaInfo != null)
            {
                List<SuttaInfo> suttaInfo = clientSuttaInfo.QuerySuttaInfo(docType);
                foreach (SuttaInfo item in suttaInfo)
                {
                    suttaList.Add(string.Format("{0}|{1}", item.RowKey, item.Title));
                }
            }
            return suttaList;
        }
        public SortedDictionary<string, string>? GetAllSuttaList()
        {
            if (clientSuttaInfo != null)
                return clientSuttaInfo.GetAllSuttaInfo();
            return null;
        }
        public async Task GetServerSuttaData(string docID, Action<string> callback)
        {
            //parentCallback = callback;
            if (docID.Length == 0 || clientSuttaPageData == null || clientSuttaInfo == null)
            {
                ErrMsg = "clientSuttaPageData or clientSuttaInfo is null.";
                return;
            }
            DocID = docID;
            Pages.Clear(); pageNos.Clear();
            SuttaInfo? suttaInfo = clientSuttaInfo.GetSuttaInfo(docID);
            if (suttaInfo == null)
            {
                ErrMsg = "No data found for the current document.";
                return;
            }
            DocTitle = suttaInfo.Title;
            clientSuttaPageData!.SetSubPartitionKey(docID);
            var objResult = await clientSuttaPageData!.QueryTableRec(docID);
            List<SuttaPageData> listSuttaPageData = (List<SuttaPageData>)objResult;

            string pgno;
            List<NIS> listNIS;
            char[] fn_rm = new char[] { '@', '!' };
            foreach (var item in listSuttaPageData)
            {
                pgno = item.PageNo.ToString();
                pageNos.Add(pgno);
                listNIS = new List<NIS>();
                string[] f = item.PageData.Split("*");
                string[] ff2;
                int recNo = 0;
                foreach (var nisRec in f)
                {
                    if (nisRec.Length == 0) continue;
                    NIS nis = new NIS();
                    nis.RecNo = ++recNo;
                    string[] ff = nisRec.Split("^");
                    if (ff.Length > 1)
                    {
                        nis.Pali = ff[0].Trim();
                        int pos1 = ff[1].IndexOf("@");
                        int pos2 = ff[1].IndexOf("!");
                        int n = 0;
                        if (pos1 == -1 && pos2 == -1) n = 1;
                        if (pos1 != -1 && pos2 != -1) n = 2;
                        if (pos1 != -1 && pos2 == -1) n = 3;
                        if (pos1 == -1 && pos2 != -1) n = 4;

                        switch (n)
                        {
                            case 1:
                                nis.Trans = ff[1].Trim();
                                break;
                            case 2:
                                ff2 = ff[1].Split(fn_rm);
                                nis.Trans = ff2[0].Trim();
                                nis.Footnote = ff2[1].Trim();
                                nis.Remarks = ff2[2].Trim();
                                break;
                            case 3:
                                ff2 = ff[1].Split('@');
                                nis.Trans = ff2[0].Trim();
                                nis.Footnote = ff2[1].Trim();
                                break;
                            case 4:
                                ff2 = ff[1].Split('!');
                                nis.Trans = ff2[0].Trim();
                                nis.Remarks = ff2[1].Trim();
                                break;
                        }
                    }
                    listNIS.Add(nis);
                }
                Pages[pgno] = listNIS;
            }
            callback(DocTitle);
        }
        public List<string> GetRUFList(List<string>? RUFlist = null)
        {
            List<string> list = new List<string>();
            if (clientKeyValueData != null)
            {
                KeyValueData kvdata = clientKeyValueData.GetKeyValueData("RUF-" + email);
                if (clientKeyValueData.StatusCode == 404)
                {
                    if (RUFlist == null) return list;
                }
                else list = kvdata.Value.Split("|").ToList();
                if (RUFlist != null) 
                {
                    //if (clientKeyValueData.StatusCode == 404)
                    //{
                    //    // insert new record
                    //    kvdata = new KeyValueData();
                    //    kvdata.RowKey = "RUF-" + email;
                    //}
                    kvdata.Value = string.Join("|", RUFlist);
                    clientKeyValueData.UpdateKeyValueData(kvdata);
                    list = RUFlist;
                }
            }
            return list;
        }
        public void CreaetNewDoc(string docID, string title, string email, int startPage) 
        {
            ErrMsg = "";
            SuttaInfo suttaInfo = new SuttaInfo()
            {
                RowKey = docID,
                Title = title,
                StartPage = startPage,
                EndPage = startPage,
                NoPages = 1,
                SubmittedBy = email,
            };
            if (clientSuttaInfo != null)
            {
                clientSuttaInfo.AddSuttaInfo(suttaInfo);
                if (clientSuttaInfo.StatusCode != 204) ErrMsg = "New document create error.";
            }
        }
        public void AddDummySuttaInfo()
        {
            Dictionary<int, string> MN_Titles = new Dictionary<int, string>()
            {
                { 1, "မူလပရိယာယသုတ်" },
                { 2, "သဗ္ဗာသဝသုတ်" },
                { 3, "ဓမ္မဒါယာဒသုတ်" },
                { 4, "ဘယဘေရဝသုတ်" },
                { 5, "အနင်္ဂဏသုတ်" },
                { 6, "အာကင်္ခေယျသုတ်" },
                { 7, "ဝတ္ထသုတ်" },
                { 8, "သလ္လေခသုတ်" },
                { 9, "သမ္မာဒိဋ္ဌိသုတ်" },
                { 10, "မဟာသတိပဋ္ဌာနသုတ်" },
                { 11, "စူဠသီဟနာဒသုတ်" },
                { 12, "မဟာသီဟနာဒသုတ်" },
                { 13, "မဟာဒုက္ခက္ခန္ဓသုတ်" },
                { 14, "စူဠဒုက္ခက္ခန္ဓသုတ်" },
                { 15, "အနုမာနသုတ်" },
                { 16, "စေတောခိလသုတ်" },
                { 17, "ဝနပတ္တသုတ်" },
                { 18, "မဓုပဏ္ဍိကသုတ်" },
                { 19, "ဒွေဓာဝိတက္ကသုတ်" },
                { 20, "ဝိတက္ကသဏ္ဍာနသုတ်" },
                { 21, "ကကစူပမသုတ်" },
                { 22, "အလဂဒ္ဒူပမသုတ်" },
                { 23, "ဝမ္မိကသုတ်" },
                { 24, "ရထဝိနီတသုတ်" },
                { 25, "နိဝါပသုတ်" },
                { 26, "ပါသရာသိသုတ်" },
                { 27, "စူဠဟတ္ထိပဒေါပမသုတ်" },
                { 28, "မဟာသာရောပမသုတ်" },
                { 29, "မဟာသာရောပမသုတ်" },
                { 30, "စူဠသာရောပမသုတ်" },
                { 31, "စူဠဂေါသိင်္ဂသုတ်" },
                { 32, "မဟာဂေါသိင်္ဂသုတ်" },
                { 33, "မဟာဂေါပါလကသုတ်" },
                { 34, "စူဠဂေါပါလကသုတ်" },
                { 35, "စူဠသစ္စကသုတ်" },
                { 36, "မဟာသစ္စကသုတ်" },
                { 37, "စူဠတဏှာသင်္ခယသုတ်" },
                { 38, "မဟာတဏှာသင်္ခယသုတ်" },
                { 39, "မဟာအဿပုရသုတ်" },
                { 40, "စူဠအဿပုရသုတ်" },
                { 41, "သာလေယျကသုတ်" },
                { 42, "ဝေရဉ္ဇကသုတ်" },
                { 43, "မဟာဝေဒလဒသုတ်" },
                { 44, "စူဠဝေဒလဒသုတ်" },
                { 45, "စူဠဓမ္မသမာဒါနသုတ်" },
                { 46, "မဟာဓမ္မသမာဒါနသုတ်" },
                { 47, "ဝီမံသကသုတ်" },
                { 48, "ကောသမ္ဗိယသုတ်" },
                { 49, "ဗြဟ္မနိမန္တနိကသုတ်" },
                { 50, "မာရတဇ္ဇနီယသုတ်" },
                { 51, "ကန္ဒရကသုတ်" },
                { 52, "အဋ္ဌကနာဂရသုတ်" },
                { 53, "သေခသုတ်" },
                { 54, "ပေါတလိယသုတ်" },
                { 55, "ဇီဝကသုတ်" },
                { 56, "ဥပါလိသုတ်" },
                { 57, "ကုက္ကုရဝတိကသုတ်" },
                { 58, "အဘယရာဇကုမာရသုတ်" },
                { 59, "ဗဟုဝေဒနီယသုတ်" },
                { 60, "အပဏ္ဏကသုတ်" },
                { 61, "အမ္ဗလဋ္ဌိကရာဟုလောဝါဒသုတ်" },
                { 62, "မဟာရာဟုလောဝါဒသုတ်" },
                { 63, "စူဠမာလုကျသုတ်" },
                { 64, "မဟာမာလုကျသုတ်" },
                { 65, "ဘဒ္ဒါလိသုတ်" },
                { 66, "ကဋုကိကောပမသုတ်" },
                { 67, "စတုမသုတ်" },
                { 68, "နဠကပါနသုတ်" },
                { 69, "ဂေါလိယာနိသုတ်" },
                { 70, "ကီဋာဂိရိသုတ်" },
                { 71, "တေဝိဇ္ဇဝစ္ဆသုတ်" },
                { 72, "အဂ္ဂိဝစ္ဆသုတ်" },
                { 73, "မဟာဝစ္ဆသုတ်" },
                { 74, "ဒီဃနခသုတ်" },
                { 75, "မာဂဏ္ဍိယသုတ်" },
                { 76, "သန္ဒကသုတ်" },
                { 77, "မဟာသကုလုဒါယိသုတ်" },
                { 78, "သမဏမုဏ္ဍိကသုတ်" },
                { 79, "စူဠသကုလုဒါယိသုတ်" },
                { 80, "ဝေခနသသုတ်" },
                { 81, "ဃဋိကာရသုတ်" },
                { 82, "ရဋ္ဌပါလသုတ်" },
                { 83, "မဃဒေဝသုတ်" },
                { 84, "မဓုရသုတ်" },
                { 85, "ဗောဓိရာဇကုမာရသုတ်" },
                { 86, "အင်္ဂုလိမာလသုတ်" },
                { 87, "ပိယဇာတိကသုတ်" },
                { 88, "ဗာဟိဘိကသုတ်" },
                { 89, "ဓမ္မစေတိယသုတ်" },
                { 90, "ကဏ္ဏကတ္ထလသုတ်" },
                { 91, "ဗြဟ္မာယုသုတ်" },
                { 92, "သေလသုတ်" },
                { 93, "အဿလာယနသုတ်" },
                { 94, "ဃောဋမုခသုတ်" },
                { 95, "စင်္ကီသုတ်" },
                { 96, "ဧသုကာရီသုတ်" },
                { 97, "ဓနဉ္ဇာနိသုတ်" },
                { 98, "ဝါသေဋ္ဌသုတ်" },
                { 99, "သုဘသုတ်" },
                { 100, "သင်္ဂါရဝသုတ်" },
                { 101, "ဒေဝဒဟသုတ်" },
                { 102, "ပဉ္စတ္တယသုတ်" },
                { 103, "ကိန္တိသုတ်" },
                { 104, "သာမဂါမသုတ်" },
                { 105, "သုနက္ခတ္တသုတ်" },
                { 106, "အာနေဉ္ဇသပ္ပါယသုတ်" },
                { 107, "ဂဏကမောဂ္ဂလ္လာနသုတ်" },
                { 108, "ဂေါပကမောဂ္ဂလ္လာနသုတ်" },
                { 109, "မဟာပုဏ္ဏမသုတ်" },
                { 110, "စူဠပုဏ္ဏမသုတ်" },
                { 111, "အနုပဒသုတ်" },
                { 112, "ဆဗ္ဗိသောဓနသုတ်" },
                { 113, "သပ္ပုရိသသုတ်" },
                { 114, "သေဝိတဗ္ဗာသေဝိတဗ္ဗသုတ်" },
                { 115, "ဗဟုဓာတုကသုတ်" },
                { 116, "ဣသိဂိလိသုတ်" },
                { 117, "မဟာစတ္တာရီသကသုတ်" },
                { 118, "အာနာပါနဿတိသုတ်" },
                { 119, "ကာယဂတာသတိသုတ်" },
                { 120, "သင်္ခါရုပပတ္တိသုတ်" },
                { 121, "စူဠသုညတသုတ်" },
                { 122, "မဟာသုညတသုတ်" },
                { 123, "အစ္ဆရိယအဗ္ဘုတသုတ်" },
                { 124, "ဗာကုလသုတ်" },
                { 125, "ဒန္တဘူမိသုတ်" },
                { 126, "ဘူမိဇသုတ်" },
                { 127, "အနုရုဒ္ဓသုတ်" },
                { 128, "ဥပက္ကိလေသသုတ်" },
                { 129, "ဗာလပဏ္ဍိတသုတ်" },
                { 130, "ဒေဝဒူတသုတ်" },
                { 131, "ဘဒ္ဒေကရတ္တသုတ်" },
                { 132, "အာနန္ဒဘဒ္ဒေကရတ္တသုတ်" },
                { 133, "မဟာကစ္စာနဘဒ္ဒေကရတ္တသုတ်" },
                { 134, "လောမသကင်္ဂိယဘဒ္ဒေကရတ္တသုတ်" },
                { 135, "စူဠကမ္မဝိဘင်္ဂသုတ်" },
                { 136, "မဟာကမ္မဝိဘင်္ဂသုတ်" },
                { 137, "သဠာယတနဝိဘင်္ဂသုတ်" },
                { 138, "ဥဒ္ဒေသဝိဘင်္ဂသုတ်" },
                { 139, "အရဏဝိဘင်္ဂသုတ်" },
                { 140, "ဓာတုဝိဘင်္ဂသုတ်" },
                { 141, "သစ္စဝိဘင်္ဂသုတ်" },
                { 142, "ဒက္ခိဏာဝိဘင်္ဂသုတ်" },
                { 143, "အနာထပိဏ္ဍိကောဝါဒသုတ္တံ" },
                { 144, "ဆန္နောဝါဒသုတ္တံ" },
                { 145, "ပုဏ္ဏောဝါဒသုတ္တံ" },
                { 146, "နန္ဒကောဝါဒသုတ္တံ" },
                { 147, "စူဠရာဟုလောဝါဒသုတ္တံ" },
                { 148, "ဆဆက္ကသုတ္တံ" },
                { 149, "မဟာသဠာယတနိကသုတ္တံ" },
                { 150, "နဂရဝိန္ဒေယျသုတ္တံ" },
                { 151, "ပိဏ္ဍပါတပါရိသုဒ္ဓိသုတ္တံ" },
                { 152, "ဣန္ဒြိယဘာဝနာသုတ္တံ" }
            };
            string type = "MN-";
            for(int i = 5; i <= 150; i++)
            {
                if (i >= 130) type = "DN-";
                SuttaInfo suttaInfo = new SuttaInfo()
                {
                    RowKey = type + i.ToString("D3"),
                    Title = MN_Titles[i],
                    StartPage = 1,
                    EndPage = 100,
                    NoPages = 100,
                    NISRecCount = 1000,
                    SubmittedBy = "AuzM",
                };
                if (clientSuttaInfo != null) clientSuttaInfo.AddSuttaInfo(suttaInfo);
            }
        }
    }
}

    public class SpellChecker
    {
        public string errMsg = string.Empty;
        const char thaythayTin = '\u1036';
        const char thagyi = '\u103F';
        const char nyakalay = '\u1009';
        const char yapin = '\u103B';
        const char yayit = '\u103C';
        const char virama = '\u1039';
        const char Asat = '\u103A';
        const char oughtMyit = '\u1037';
        const char naughtPyin = '\u1032';
        const char waswae = '\u103D';
        const char huthoe = '\u102F';
        const char vassa2lonepauk = '\u1038';

        char[] Consonants = { 'က', 'ခ', 'ဂ', 'ဃ', 'င', 'စ', 'ဆ', 'ဇ', 'ဈ', 'ည', 'ဋ', 'ဌ', 'ဍ', 'ဎ', 'ဏ',
                                    'တ', 'ထ', 'ဒ', 'ဓ', 'န', 'ပ', 'ဖ', 'ဗ', 'ဘ', 'မ', 'ယ', 'ရ', 'လ', 'ဝ', 'သ',
                                    'ဟ', 'ဠ','အ', thaythayTin, thagyi, nyakalay};
        char[] ReducedConjCons = { '\u103D', '\u103E', '\u103B', '\u103C' };
        char[] Vowels = { '\u102C', '\u102B', '\u102D', '\u102E', '\u102F', '\u1030', '\u1031',
                          '\u1032', '\u1036', '\u1037', '\u1038'};
        char[] consAssociates = { '\u102C', '\u102B', '\u102D', '\u102E', '\u102F', '\u1030', '\u1031',
                                    '\u1032', '\u1036', '\u1037', '\u1038',
                                // vowel killer
                                Asat, virama, 
                                // tone
                                thaythayTin, oughtMyit, naughtPyin, vassa2lonepauk,
                                // semi-vowels
                                yapin, yayit, huthoe, waswae
                                };
        const string pattern = "[^\u1000-\u1027\u1029-\u1032\u1036-\u104F\u0020\u0030-\u0039\\-#*^@(){}\r\n]" +
                    "|စျ|ဥ်|ဦ|သြ|သြော်";
        Dictionary<string, string> baseErrMapping = new Dictionary<string, string>()
        {
            {"စျ", "ဈ"}, {"ဥ်", "ဉ်"}, {"ဥ္", "ဉ္"}
        };

        Dictionary<string, string> baseErrDescription = new Dictionary<string, string>()
        {
            {"စျ", "စ + \u103B|ဈ - ဈမျဉ်းဆွဲ"}, {"ဥ်", "အက္ခရာ 'ဥ' + \u103A|ညကလေး 'ဉ' + \u103A"}, {"ဥ္", "အက္ခရာ 'ဥ'|ညကလေး 'ဉ'"}
        };
        public MatchCollection FindZeroes(string s)
        {
            string mmChars = new string(Consonants) + new string(Vowels) +
                                new string(ReducedConjCons) + new string(consAssociates);
            string pattern = "[" + mmChars + "]၀";
            pattern += "|၀[" + mmChars + "]";
            MatchCollection matches = Regex.Matches(s, pattern);
            return matches;
        }
        public Dictionary<string, int> PatternOccurrences(string s)
        {
            MatchCollection matches;
            Dictionary<string, int> patOcc = new Dictionary<string, int>()
            { { "စျ", 0 }, { "ဥ်", 0 }, { "ဥ္", 0 }, { "ဦ", 0 }, { "သြ", 0 }, { "သြော်", 0 } };
            foreach(KeyValuePair<string, int> kvp in patOcc)
            {
                matches = Regex.Matches(s, kvp.Key);
                patOcc[kvp.Key] = matches.Count;
            }
            s = "အဟံ၀ိ";
            matches = FindZeroes(s);
            patOcc["ဝ"] = matches.Count;
            return patOcc;
        }
        public string CorrectCommonErrors(string s)
        {
            if (s.Length == 0) return s;
            // replace common errors
            foreach(KeyValuePair<string, string> kv in baseErrMapping)
            {
                s = s.Replace(kv.Key, kv.Value);
            }
            // replace သုည with walone
            string mmChars = new string(Consonants) + new string(Vowels) +
                                new string(ReducedConjCons) + new string(consAssociates);
            string pattern = "[" + mmChars + "]၀";
            pattern += "|၀[" + mmChars + "]";
            MatchCollection matches = Regex.Matches(s, pattern);
            StringBuilder sb = new StringBuilder(s);
            int pos;
            foreach (Match m in matches)
            {
                pos = m.Index + m.Value.IndexOf("၀");
                sb[pos] = 'ဝ';
            }
            return sb.ToString();
        }
        public string MyanmarCharsCleanup(string s)
        {
            s = s.Replace("ဦ", "ဦ");
            s = s.Replace("သြ", "ဩ");
            s = s.Replace("သြော်", "ဪ");
            return s;
        }
        public MatchCollection? MyanmarBasicErrors(string s)
        {
            string[] pattern = { "စျ", "ဥ်", "ဥ္" };
            MatchCollection? matches = null, matches1 = null;
            foreach (string p in pattern)
            {
                matches = Regex.Matches(s, p);
                if (matches.Count > 0)
                {
                    matches1 = matches;
                }
            }
            return matches1;
        }

        public MatchCollection FindInvalidMyanmarLetters(string s)
        {
            const string pattern = "[^\u1000-\u1027\u1029-\u1032\u1036-\u104F\u0020\u0030-\u0039\\-\\#\\*\\^(){}\\[\\]'\"\r\n]";
            int prevPos = -1;
            errMsg = string.Empty;
            string errTxt = string.Empty;
            MatchCollection matches = Regex.Matches(s, pattern);
            foreach (Match m in matches)
            {
                if (prevPos != -1)
                {
                    if (prevPos + 1 == m.Index)
                    {
                        errMsg += m.Value;
                        errTxt += m.Value;
                    }
                    else
                    {
                        //Form1._ErrDesc d = new Form1._ErrDesc();
                        //d.incorrectMsg = errMsg;
                        //d.incorrectText = errTxt;
                        //d.correctMsg = string.Empty;
                        //d.errDesc = "Invalid character(s). Click 'Correct' to remove them.";
                        //listErrDesc.Add(d);
                        //errMsg += " " + m.Value;
                        //errTxt = m.Value;
                    }
                }
                else
                    errMsg = errTxt = m.Value;
                prevPos = m.Index;
            }
            if (errMsg.Length > 0)
            {
                //errMsg += "||" + "Invalid character(s).";
                //if (listErrDesc.Count > 0)
                //{
                //    Form1._ErrDesc d0 = listErrDesc[0];
                //    d0.incorrectMsg = errMsg;
                //    listErrDesc[0] = d0;
                //}
                //Form1._ErrDesc d = new Form1._ErrDesc();
                //d.incorrectMsg = errMsg;
                //d.incorrectText = errTxt;
                //d.correctMsg = string.Empty;
                //d.errDesc = "Invalid character(s). Click 'Correct' to remove them.";
                //listErrDesc.Add(d);
            }
            return matches;
        }

    }

