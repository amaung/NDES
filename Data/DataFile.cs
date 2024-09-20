//using System.IO;
//using System.Linq.Expressions;
//using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Tipitaka_DBTables;
using Tipitaka_DB;
using System.Diagnostics;
using static Azure.Core.HttpHeader;
using Syncfusion.Blazor.Popups;
using System.Net.NetworkInformation;
using Syncfusion.PdfExport;
using Microsoft.AspNetCore.Mvc.ModelBinding;
//using static NissayaEditor_Web.Data.Document;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor;


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
        public NIS() { }
        public NIS(NIS rec)
        {
            RecNo = rec.RecNo;
            Pali = rec.Pali;
            Trans = rec.Trans;
            Trans2 = rec.Trans2;
            Footnote = rec.Footnote;
            Remarks = rec.Remarks;
        }
    }
    public class SourceDocInfo()
    {
        public string docPDFfile = "";
        public int pages = 0;
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
        public string userClass = string.Empty;
        public int AtCount = 0;
        public int QCount = 0;
        public string DocID { get; set; } = string.Empty;
        public string DocTitle { get; set; } = string.Empty;
        public Dictionary<string, List<NIS>> Pages = new Dictionary<string, List<NIS>>();
        public List<UserProfile> UserProfiles = new List<UserProfile>();
        public List<string> pageNos = new List<string>();
        public Dictionary<string, List<NIS>> dictInvalidNISRecords = new Dictionary<string, List<NIS>>();
        public char[] fieldSeparators = { ',', '\t' };
        public string ErrMsg = string.Empty;
        private bool currentPageUploaded = false;
        ClientTipitakaDB_w? clientUserTipitakaDB = null;
        ClientUserProfile? clientUserProfile = null;
        ClientSuttaInfo? clientSuttaInfo = null;
        ClientSuttaPageData? clientSuttaPageData = null;
        ClientKeyValueData? clientKeyValueData = null;
        ClientActivityLog? clientActivityLog = null;
        ClientTaskActivityLog? clientTaskActivityLog = null;
        ClientUserPageActivity? clientUserPageActivity = null;
        ClientSourceBookInfo? clientSourceBookInfo = null;
        ClientTaskAssignmentInfo? clientTaskAssignmentInfo = null;
        ClientCorrectionLog? clientCorrectionLog = null;
        ClientTimesheet? clientTimesheet = null;
        //TipitakaFileStorage? tipitakaFileStorage = new TipitakaFileStorage();
        //Dictionary<string, SuttaInfo> dictSuttaInfo = new Dictionary<string, SuttaInfo>();

        public ClientKeyValueData? GetClientKeyValueData() { return clientKeyValueData;  }
        public ClientTaskAssignmentInfo? GetClientTaskAssignmentInfo() { return clientTaskAssignmentInfo; }
        public ClientCorrectionLog? GetClientCorrectionLog() { return clientCorrectionLog; }
        public ClientTaskActivityLog? GetClientTaskActivityLog() { return clientTaskActivityLog; }
        public ClientActivityLog? GetClientActivityLog() { return clientActivityLog; }
        public ClientSuttaInfo? GetClientSuttaInfo() { return clientSuttaInfo; }
        public ClientTimesheet? GetClientTimesheet() { return clientTimesheet; }
        public ClientSourceBookInfo? GetClientSourceBookInfo() { return clientSourceBookInfo; }
        public DataFile(ClientTipitakaDB_w? clientUserTipitakaDB)
        {
            if (clientUserTipitakaDB != null)
            {
                this.clientSuttaInfo = clientUserTipitakaDB.GetClientSuttaInfo();
                this.clientSuttaPageData = clientUserTipitakaDB.GetClientSuttaPageData();
                this.clientKeyValueData = clientUserTipitakaDB.GetClientKeyValueData();
                this.clientUserProfile = clientUserTipitakaDB.GetClientUserProfile();
                this.clientUserPageActivity = clientUserTipitakaDB.GetClientUserPageActivity();
                this.clientActivityLog = clientUserTipitakaDB.GetClientActivityLog();
                this.clientSourceBookInfo = clientUserTipitakaDB.GetClientSourceBookInfo();
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
                this.clientUserProfile = clientUserTipitakaDB.GetClientUserProfile();
                this.clientUserPageActivity = clientUserTipitakaDB.GetClientUserPageActivity();
                this.clientActivityLog = clientUserTipitakaDB.GetClientActivityLog();
                this.clientTaskActivityLog = clientUserTipitakaDB.GetClientTaskActivityLog();
                this.clientSourceBookInfo = clientUserTipitakaDB.GetClientSourceBookInfo();
                this.clientTaskAssignmentInfo = clientUserTipitakaDB.GetClientTaskAssignmentInfo();
                this.clientCorrectionLog = clientUserTipitakaDB.GetClientCorrectionLog();
                this.clientTimesheet = clientUserTipitakaDB.GetClientTimesheet();
            }
        }
        public void ClearPageData() { Pages = new Dictionary<string, List<NIS>>(); }
        public void SetFileContent(string fnm, string content)
        {
            ErrMsg = "";
            fname = fnm;
            fileContent = content.Replace("ၕ", "^");
            if (fileContent.Length > 0)
            {
                fext = fname.Substring(fname.Length - 3).ToLower();
                if (fext == "txt") CleanupMarkers();
                SpellChecker sp = new SpellChecker();
                fileContent = sp.CorrectCommonErrors(fileContent);
                ExtractHeader();
                parseIntoPages();
                AtCount = fileContent.Count(x => x == '@');
                QCount = fileContent.Count(x => x == '?');
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
            int pos = fileContent.IndexOf("\n");
            string s = fileContent.Substring(0, pos).Trim();
            if (s[0] != '{' || s[s.Length - 1] != '}') return;

            fileContent = fileContent.Substring(pos + 1).Trim();
            headerData = s;
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
                            case "docno":
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
            return;
        }
        public Dictionary<string, List<NIS>> GetInvalidNISRecords() { return dictInvalidNISRecords; }
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
            dictInvalidNISRecords = new Dictionary<string, List<NIS>>();
            // split data by page#
            List<string> paraPages = GetPageData();
            //List<string> paraPages = fileContent.Split('\n').ToList();
            pageNos = new List<string>();
            int recno = 0;
            string pgno = string.Empty, nextpgno = "";
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
                                p = pgno.IndexOf(" ");
                                if (p > -1) { pgno = pgno.Substring(0, p).Trim(); }
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
                                    else nis.Trans = "";
                                    nis.Footnote = "";
                                    if (f.Length == 3)
                                    {
                                        string[] ff = f[2].Trim().Split(new char[] { '?', '!' });
                                        nis.Footnote = ff[0].Trim();
                                        if (ff.Length > 1) nis.Remarks = ff[1].Trim();
                                    }
                                    listNISRecords.Add(nis);

                                    if (nis.Pali.Length == 0 || nis.Trans.Length == 0 || 
                                        (nis.Pali == "-" && nis.Trans == "-" && nis.Footnote.Length < 50))
                                    {
                                        string key = String.Format("{0}-{1}", pgno, nis.RecNo);
                                        if (dictInvalidNISRecords.ContainsKey(key))
                                        {
                                            dictInvalidNISRecords[key].Add(nis);
                                        }
                                        else
                                        {
                                            dictInvalidNISRecords.Add(key, new List<NIS>() { nis });
                                        }
                                    }
                                }
                            }
                        }
                        if (nextpgno.Length > 0 && nextpgno != pgno)
                        {
                            ErrMsg = String.Format("Page numbering out of sequence. Page #{0} expected but #{1} found.", nextpgno, pgno);
                            return;
                        }
                        //if (pageNos.Contains(pgno))
                        //{
                        //    ErrMsg = "Duplicate page number found.";
                        //    return;
                        //}
                        // store pgno and NIS recrods
                        Pages[pgno] = listNISRecords;
                        pageNos.Add(pgno);
                        nextpgno = (Int32.Parse(pgno) + 1).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrMsg = string.Format("{0} after page {1} NIS rec {2}.", ex.Message, pgno, recno);
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
        public List<string> GetPageData()
        {
            List<string> result = new List<string>();
            if (fileContent.Length == 0) return result;
            List<string> pageData = fileContent.Split('#').ToList();
            string pg;
            for (int i = 0; i < pageData.Count; i++)
            {
                pg = pageData[i].Trim();
                if (pg.Length == 0) { continue; }
                pg = "#" + pg.Replace("\n", "").Replace("\r", "");
                result.Add(pg);
            }
            return result;
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
            if (!pageNos.Contains(pgno)) { pageNos.Add(pgno); }
            if (!Pages.ContainsKey(pgno))
                Pages[pgno] = new List<NIS>();
        }
        public int GetFirstPageNo() { return Pages.Count > 0 ? Convert.ToInt16(Pages.First().Key) : 0; }
        public int GetLastPageNo() { return Pages.Count > 0 ? Convert.ToInt16(Pages.Last().Key) : 0; }
        public string GetFootnoteRemarkCountWarning()
        {
            string warnMsg = "";
            //AtCount = fileContent.Count(x => x == '@');
            //QCount = fileContent.Count(x => x == '?');
            if (AtCount > 0 || QCount > 0) 
            {
                warnMsg = String.Format("ALERT: {0} footnote(@) and {1} remark(?) symbols found. Please verify for their validities.",
                    AtCount, QCount);
            }
            return warnMsg;
        }
        public Dictionary<string, string> GetUploadPageData(string startPage = "", string endPage = "")
        {
            Dictionary<string, string> pageData = new Dictionary<string, string>();
            int n = 0; ErrMsg = "";
            bool firstPageFound = false, lastPageFound = false;
            bool allPages = startPage.Length == 0 && endPage.Length == 0;
            foreach (KeyValuePair<string, List<NIS>> kv in Pages)
            {
                string page_data = string.Empty;
                n = 0;
                if (!firstPageFound && startPage == kv.Key) { firstPageFound = true; }
                if (allPages || (firstPageFound && !lastPageFound))
                {  
                    foreach (NIS nis in kv.Value)
                    {
                        ++n;
                        if (nis.Pali!.Length == 0 || nis.Trans!.Length == 0)
                        {
                            ErrMsg = string.Format("Empty Pali or Trans text found in Page {0} Sr No #{1}. Upload aborted.", kv.Key, n);
                            return pageData;
                        }
                        if (nis.Pali!.Length > 0)
                        {
                            if (page_data.Length > 0) page_data += " *" + nis.Pali;
                            else page_data += "*" + nis.Pali;
                            if (nis.Trans != null && nis.Trans.Length > 0) { page_data += " ^" + nis.Trans; }
                            if (nis.Footnote != null && nis.Footnote.Length > 0) { page_data += " @" + nis.Footnote; }
                            if (nis.Remarks != null && nis.Remarks.Length > 0) 
                            {
                                if (nis.Footnote == null || nis.Footnote.Length == 0)
                                    // this is the case where no Footnote but Remarks exist
                                    page_data += " @"; // preceded with footnote symbol first
                                page_data += "?" + nis.Remarks; 
                            }
                        }
                    }
                    pageData[kv.Key] = page_data;
                }
                if (!lastPageFound && endPage == kv.Key) { lastPageFound = true; }
                if (firstPageFound && lastPageFound) { break; }
            }
            return pageData;
        }
        public async Task<string> UploadDocument(string startPage, string endPage)
        {
            Dictionary<string, string> data = GetUploadPageData(startPage, endPage);
            if (clientKeyValueData != null && clientSuttaPageData != null)
            {
                // first clear all records for DocID
                //List<SuttaPageData> suttaPageData = new List<SuttaPageData>();
                clientSuttaPageData.SetSubPartitionKey(DocID);
                await clientSuttaPageData.QueryTableRec("");
                List<SuttaPageData> listSuttaPageData = (List<SuttaPageData>)clientSuttaPageData.objResult;
                if (listSuttaPageData.Count() > 0)
                {
                    // delete existing records if they exist
                    await clientSuttaPageData.DeleteTableRecBatch(new List<Object>(listSuttaPageData));
                }
                await clientSuttaPageData!.InsertTableRecBatch(DocID, email, data);
                if (clientSuttaPageData.StatusCode != 202)
                {
                    ErrMsg = clientSuttaPageData.DBErrMsg;
                }
            }
            return ErrMsg;
        }
        public List<CorrectionLog> listOfCorrections = new List<CorrectionLog>();
        public List<CorrectionLog> GetCorrectionLog() { return listOfCorrections; }
        public int Update_Document(string startPage, string endPage, string userTask)
        {
            List<int> pageNosToUpdate = new List<int>();
            List<object> listCorrections = new List<object>();
            listOfCorrections = new List<CorrectionLog>();
            CorrectionLog? correctionLog = null;
            string NISRecNo = "", NISRecNo_Type = "";
            bool diff = false;
            Dictionary<string, string> data = GetUploadPageData(startPage, endPage);
            if (clientSuttaPageData != null)
            {
                int start = Int32.Parse(startPage);
                int end = Int32.Parse(endPage);
                for (int i = start; i <= end; i++)
                {
                    diff = false;
                    SuttaPageData? suttaPageData = clientSuttaPageData.GetPageData(DocID, i.ToString("d3"));
                    if (suttaPageData != null)
                    {
                        List<NIS> userData = GetPageData(i.ToString()); //GetNISDataFromPage 1124
                        List<NIS> serverData = GetNISDataFromPage(suttaPageData.PageData);
                        int idx = 0;
                        foreach (NIS nis in userData)
                        {
                            NISRecNo = (idx+1).ToString("d2");
                            // compare userData and serverData
                            // check for Pali data
                            if (nis.Pali != null && idx < serverData.Count && serverData[idx].Pali != null && 
                                nis.Pali != serverData[idx].Pali)
                            {
                                NISRecNo_Type = NISRecNo + "P";
                                correctionLog = new CorrectionLog()
                                {
                                    RowKey = String.Format("{0}-{1}-{2}-{3}", DocID, i.ToString("d3"), NISRecNo_Type, DateTime.Now.Ticks),
                                    UserID = email,
                                    Task = userTask,
                                    PageNo = i,
                                    NISRec = NISRecNo_Type,
                                    OrigText = serverData[idx].Pali!,
                                    EditedText = nis.Pali,
                                    Timestamp = DateTime.Now,
                                };
                                listCorrections.Add(correctionLog);
                                listOfCorrections.Add(correctionLog);
                                diff = true;
                            }
                            // check for translation
                            if (nis.Trans != null && idx < serverData.Count && serverData[idx].Trans != null &&
                                nis.Trans != serverData[idx].Trans)
                            {
                                NISRecNo_Type = NISRecNo + "T";
                                correctionLog = new CorrectionLog()
                                {
                                    RowKey = String.Format("{0}-{1}-{2}-{3}", DocID, i.ToString("d3"), NISRecNo_Type, DateTime.Now.Ticks),
                                    UserID = email,
                                    Task = userTask,
                                    PageNo = i,
                                    NISRec = NISRecNo_Type,
                                    OrigText = serverData[idx].Trans!,
                                    EditedText = nis.Trans,
                                    Timestamp = DateTime.Now,
                                };
                                listCorrections.Add(correctionLog);
                                listOfCorrections.Add(correctionLog);
                                diff = true;
                            }
                            // check for footnote
                            if (nis.Footnote != null && idx < serverData.Count && serverData[idx].Footnote != null &&
                                nis.Footnote != serverData[idx].Footnote)
                            {
                                NISRecNo_Type = NISRecNo + "F";
                                correctionLog = new CorrectionLog()
                                {
                                    RowKey = String.Format("{0}-{1}-{2}-{3}", DocID, i.ToString("d3"), NISRecNo_Type, DateTime.Now.Ticks),
                                    UserID = email,
                                    Task = userTask,
                                    PageNo = i,
                                    NISRec = NISRecNo_Type,
                                    OrigText = serverData[idx].Footnote!,
                                    EditedText = nis.Footnote,
                                    Timestamp = DateTime.Now,
                                };
                                listCorrections.Add(correctionLog);
                                listOfCorrections.Add(correctionLog);
                                diff = true;
                            }
                            // check for remarks
                            if (nis.Remarks != null && idx < serverData.Count && serverData[idx].Remarks != null &&
                                nis.Remarks != serverData[idx].Remarks)
                            {
                                diff = true;
                            }
                            ++idx;
                        }
                        if (diff) pageNosToUpdate.Add(i);
                    }
                    else
                        ErrMsg = clientSuttaPageData.DBErrMsg;
                }
                if (pageNosToUpdate.Count > 0) 
                {
                    // update those pages
                    foreach(int pageno  in pageNosToUpdate)
                    {
                        string pNo = pageno.ToString();
                        UpdateCurrentPage(DocID, pNo, GetPageData(pNo));
                    }
                }
                // if there correctionlog data 
                if (clientCorrectionLog != null && listCorrections != null && listCorrections.Count > 0)
                {
                    clientCorrectionLog.InsertBatch(listCorrections).Wait();
                }
            }
            return (listCorrections == null) ? 0 : listCorrections.Count;
        }
        public void LogUserTaskActivity(Dictionary<string, string> dictUserTask, string userTask, string userTaskStartDate)
        {
            string desc = "";
            int pages = Int32.Parse(dictUserTask["NoPages"]);
            int totalSubmitted = Int32.Parse(dictUserTask["PagesSubmitted"]);
            int pagesToSubmit = Int32.Parse(dictUserTask["PagesToSubmit"]);
            int newSubmittedPages = Math.Min(pages, totalSubmitted);
            string userStatus = String.Empty;
            string sourceBookID = String.Empty, pdfFileName = String.Empty, assigneeProgress = String.Empty;
            int correctionCount = 0;
            if (dictUserTask.ContainsKey("CorrectionCount")) correctionCount = Int32.Parse(dictUserTask["CorrectionCount"]);
            
            if (clientActivityLog != null)
            {
                desc = String.Format("DocNo={0}; Pages={1}", DocID, pagesToSubmit);
                clientActivityLog.AddActivityLog(email, "Upload", desc);
            }

            if (newSubmittedPages == pages) 
            { 
                userStatus = dictUserTask["Task"].Contains("Upload") ? "Uploaded" : "Completed";
                // remove from user task list when completed
                if (clientKeyValueData != null)
                {
                    clientKeyValueData.RemoveUserTask(email, DocID);
                    clientKeyValueData.RemoveUserDocByCategory(email, TaskCategories._Assigned_, DocID);
                    clientKeyValueData.AddUserDocByCategory(email, TaskCategories._Recent_, DocID);
                    clientKeyValueData.AddUserDocByCategory(email, TaskCategories._Completed_, DocID);
                }
                desc = String.Format("{0} uploaded. @'s = {1}, ?'s = {2} ({3})", DocID, AtCount, QCount, dictUserTask["ServerUploadTime"]);
            }
            else { userStatus = String.Format("{0}%", (int) (newSubmittedPages * 100.0 / pages )); }
            AddTaskActivityLog(DocID, email, dictUserTask["Task"], pages, totalSubmitted, pagesToSubmit, desc);


            // update TaskAssignmentInfo
            int completeCount = 0;
            int userTaskCount = 0;
            if (clientTaskAssignmentInfo != null)
            {
                TaskAssignmentInfo? taskAssignmentInfo = clientTaskAssignmentInfo.GetTaskAssignmentInfo(dictUserTask["DocNo"]);
                if (taskAssignmentInfo != null)
                {
                    taskAssignmentInfo.PagesSubmitted = newSubmittedPages;
                    taskAssignmentInfo.CorrectionCount += correctionCount;
                    taskAssignmentInfo.Status = userStatus;
                    
                    var listUserTaskProgressInfo = JsonConvert.DeserializeObject<List<UserTaskProgressInfo>>(taskAssignmentInfo.AssigneeProgress);
                    if (listUserTaskProgressInfo != null && listUserTaskProgressInfo.Count > 0)
                    {
                        userTaskCount = listUserTaskProgressInfo.Count;
                        foreach (UserTaskProgressInfo userTaskProgressInfo in listUserTaskProgressInfo)
                        {
                            if (userTaskProgressInfo.userID == email)
                            {
                                if (userTaskProgressInfo.startDate.Length == 0)
                                    userTaskProgressInfo.startDate = userTaskStartDate;
                                userTaskProgressInfo.lastDate = DateTime.Now.ToString("d/M/yyyy");
                                userTaskProgressInfo.submitted = newSubmittedPages;
                                userTaskProgressInfo.status = userStatus;
                                userTaskProgressInfo.corrections += correctionCount;
                            }
                            if (userTaskProgressInfo.status == "Completed" || userTaskProgressInfo.status == "Uploaded") ++completeCount;
                            //if (userTaskProgressInfo.status == "Completed") ++completeCount;
                        }
                        // if all tasks are complete the whole Doc is also complete
                        if (completeCount == listUserTaskProgressInfo.Count - 1) userStatus = "Completed";
                        taskAssignmentInfo.Status = userStatus;
                        taskAssignmentInfo.AssigneeProgress = JsonConvert.SerializeObject(listUserTaskProgressInfo);
                        assigneeProgress = taskAssignmentInfo.AssigneeProgress;

                        clientTaskAssignmentInfo.UpdateTableRec(taskAssignmentInfo).Wait();
                    }
                }
            }

            // update SuttaInfo
            if (clientSuttaInfo != null)
            {
                SuttaInfo? suttaInfo = clientSuttaInfo.GetSuttaInfo(dictUserTask["DocNo"]);
                if (suttaInfo != null)
                {
                    sourceBookID = suttaInfo.BookID ?? sourceBookID;
                    SourceBookInfo? sourceBookInfo = GetSourceBookInfo(sourceBookID);
                    if (sourceBookInfo != null)
                    {
                        pdfFileName = sourceBookInfo.BookFilename ?? pdfFileName;
                    }
                    suttaInfo.PagesSubmitted = (newSubmittedPages > suttaInfo.NoPages) ? suttaInfo.NoPages : newSubmittedPages;
                    if (completeCount == userTaskCount - 1 && newSubmittedPages == pages)
                    {
                        // work on this doc is complete now
                        if (suttaInfo.Status != "Uploaded") suttaInfo.Status = "Completed";
                        // update in the project 
                        //UpdateSourceBookCompletedPages(suttaInfo.BookID!, pages);
                    }
                    else
                        suttaInfo.Status = dictUserTask["Task"];
                    clientSuttaInfo.UpdateSuttaInfo(suttaInfo);
                    UpdateSourceBookCompletedPages(suttaInfo.BookID!, pages);
                }
            }

            // update UserTaskActivity
            if (clientTaskActivityLog != null)
            {
                desc = assigneeProgress + "|" + sourceBookID + ":" + pdfFileName;
                clientTaskActivityLog.UpdateUserTaskActivity(
                    dictUserTask["DocNo"], email, userTask, pages, totalSubmitted, newSubmittedPages, desc);
            }
        }
        public string GetTaskStartDate(string docNo)
        {
            string startDate = "";
            if (clientTaskAssignmentInfo != null)
            {
                TaskAssignmentInfo? taskAssignmentInfo = clientTaskAssignmentInfo.GetTaskAssignmentInfo(docNo);
                if (taskAssignmentInfo != null)
                {
                    var listUserTaskProgressInfo = JsonConvert.DeserializeObject<List<UserTaskProgressInfo>>(taskAssignmentInfo.AssigneeProgress);
                    if (listUserTaskProgressInfo != null && listUserTaskProgressInfo.Count > 0)
                    {
                        foreach (UserTaskProgressInfo userTaskProgressInfo in listUserTaskProgressInfo)
                        {
                            if (userTaskProgressInfo.userID == email)
                            {
                                startDate = userTaskProgressInfo.startDate; break;
                            }
                        }
                    }
                }
            }
            return startDate;
        }
        public void RemoveTaskFromUsers(string docNo)
        {
            // remove from User-Tasks, Task-Assigned, Task-Edit-Upload
            if (clientKeyValueData == null) return;
            List<string> userList = clientKeyValueData.GetUsersForDoc(docNo);
            foreach (string userID in userList)
            {
                if (GetUserName(userID).Length > 0)
                {
                    clientKeyValueData.RemoveUserTask(userID, docNo);
                    clientKeyValueData.RemoveUserDocByCategory(userID, TaskCategories._Assigned_, docNo);
                }
            }
        }
        public void RemoveUserDocFromKeyValue(string docNo, string userID)
        {
            if (clientKeyValueData == null) return;
            clientKeyValueData.RemoveUserTask(userID, docNo);
            clientKeyValueData.RemoveUserDocByCategory(userID, "Assigned", docNo);
        }
        public void RemoveUserFromDocTeam(string docNo, string userID)
        {
            string userName = GetUserName(userID);
            if (userName.Length > 0 && clientSuttaInfo != null)
            {
                SuttaInfo? suttaInfo = clientSuttaInfo.GetSuttaInfo(docNo);
                if (suttaInfo != null)
                {
                    List<string> team = suttaInfo.Team.Split(',').Select(x => x.Trim()).ToList();
                    team.Remove(userName);
                    suttaInfo.Team = String.Join(", ", team);
                    clientSuttaInfo.UpdateSuttaInfo(suttaInfo);
                }
            }
        }
        public void LogTaskStartDate(string docNo, Dictionary<string, string> userTaskInfo)
        {
            if (clientTaskAssignmentInfo != null)
            {
                TaskAssignmentInfo? taskAssignmentInfo = clientTaskAssignmentInfo.GetTaskAssignmentInfo(docNo);
                if (taskAssignmentInfo != null)
                {
                    var listUserTaskProgressInfo = JsonConvert.DeserializeObject<List<UserTaskProgressInfo>>(taskAssignmentInfo.AssigneeProgress);
                    if (listUserTaskProgressInfo != null && listUserTaskProgressInfo.Count > 0)
                    {
                        foreach (UserTaskProgressInfo userTaskProgressInfo in listUserTaskProgressInfo)
                        {
                            if (userTaskProgressInfo.userID == email)
                            {
                                if (userTaskProgressInfo.startDate.Length == 0)
                                {
                                    userTaskProgressInfo.startDate = DateTime.Now.ToString("d/M/yyyy");
                                    taskAssignmentInfo.AssigneeProgress = JsonConvert.SerializeObject(listUserTaskProgressInfo);
                                    clientTaskAssignmentInfo.UpdateTableRec(taskAssignmentInfo).Wait();

                                    // update tasks of docNo on this startDate in UserTaskActivity
                                    if (clientTaskActivityLog != null)
                                    {
                                        clientTaskActivityLog.UpdateUserStartDate(docNo, email, userTaskProgressInfo.startDate);
                                        string activity = userTaskInfo[email + "-Task"];
                                        int pages = Int32.Parse(userTaskInfo[email + "-NoPages"]);
                                        string desc = string.Format("{0} data pages retrieved.", pages);
                                        clientTaskActivityLog.AddTaskActivityLog(docNo, email, activity, pages, 0, 0, desc);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        public void UpdateSourceBookCompletedPages(string bookID, int pageCount)
        {
            if (clientSourceBookInfo != null && clientSuttaInfo != null)
            {
                string query = String.Format("BookID eq '{0}'", bookID);
                List<SuttaInfo> listSuttaInfo = clientSuttaInfo.QuerySuttaInfo(query);
                pageCount = 0;
                foreach (SuttaInfo suttaInfo in listSuttaInfo)
                {
                    pageCount += suttaInfo.PagesSubmitted;
                }
                SourceBookInfo? sourceBookInfo = clientSourceBookInfo.GetSourceBookInfo(bookID);
                if (sourceBookInfo != null)
                {
                    sourceBookInfo.Completed = pageCount;
                    clientSourceBookInfo.UpdateSourceBookInfo(sourceBookInfo).Wait();
                }
            }
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
            SuttaInfo? suttaInfo = clientSuttaInfo.GetSuttaInfo(DocID);
            if (userName.Length == 0) 
                userName = GetUserName(email);

            // new doc
            suttaInfo = new SuttaInfo()
            {
                RowKey = DocID,
                Title = DocTitle,
                StartPage = Convert.ToInt16(Pages.First().Key),
                EndPage = Convert.ToInt16(Pages.Last().Key),
                NoPages = Pages.Count(),
                PagesSubmitted = Pages.Count(),
                Status = "Uploaded",
                Team = userName,

            };
            
            clientSuttaInfo.AddSuttaInfo(suttaInfo);
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
                    if (clientActivityLog != null)
                    {
                        string desc = String.Format("{0}; DocNo={1}; Pages={2}", userName, DocID, Pages.Count);
                        clientActivityLog.AddActivityLog(email, "Upload", desc);
                    }
                }
            }
            return ErrMsg;
        }
        Dictionary<string, string> dictServerDocInfo = new Dictionary<string, string>();
        public Dictionary<string, string> GetServerDocInfo(string docNo, string task)
        {
            if (dictServerDocInfo.ContainsKey("DocNo") && docNo != null && dictServerDocInfo["DocNo"] == docNo) 
            {
                return dictServerDocInfo;
            }
            // this is for new docNo
            dictServerDocInfo = new Dictionary<string, string>();
            if (clientSuttaInfo != null && clientTaskAssignmentInfo != null)
            {
                TaskAssignmentInfo? taskAssignmentInfo = clientTaskAssignmentInfo.GetTaskAssignmentInfo(DocID);
                if (taskAssignmentInfo != null)
                {
                    string[] f = ParsePageInfo(taskAssignmentInfo.PageNos);
                    dictServerDocInfo["DocNo"] = taskAssignmentInfo.RowKey;
                    if (f != null && f.Length == 3)
                    {
                        dictServerDocInfo["BookDocStartPage"] = f[0];
                        dictServerDocInfo["BookDocEndPage"] = f[1];
                        dictServerDocInfo["NoPages"] = f[2];
                    }
                    Dictionary<string, string> dict = GetUserTaskProgressInfo(task, taskAssignmentInfo.AssigneeProgress);
                    if (dict.ContainsKey("PagesSubmitted")) dictServerDocInfo["PagesSubmitted"] = dict["PagesSubmitted"];
                    if (dict.ContainsKey("Status")) dictServerDocInfo["Status"] = dict["Status"];
                    dictServerDocInfo["ImportedStartPage"] = Pages.First().Key;
                    dictServerDocInfo["ImportedEndPage"] = Pages.Last().Key;
                    dictServerDocInfo["ImportedNoPages"] = Pages.Count().ToString();
                }
            }
            return dictServerDocInfo;
        }
        public string[] ParsePageInfo(string  pageInfo)
        {
            List<string> listPageInfo = new List<string>();
            if (pageInfo != null && pageInfo.Length > 0) 
            {
                string[] f = pageInfo.Trim().Split(new char[] { '-', ' ' });
                if (f.Length == 3) 
                {
                    listPageInfo.Add(f[0]);
                    listPageInfo.Add(f[1]);
                    listPageInfo.Add(f[2].Substring(1, f[2].Length - 2));
                }
            }
            return listPageInfo.ToArray();
        }
        private Dictionary<string, string> GetUserTaskProgressInfo(string task, string progressInfo)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            var listNDESItems = JsonConvert.DeserializeObject<List<UserTaskProgressInfo>>(progressInfo);
            var taskList = (from item in listNDESItems
                            where item.task == task
                            select item).ToList();
            if (taskList.Count == 1 ) 
            {
                dict["PagesSubmitted"] = taskList[0].submitted.ToString();
                dict["Status"] = taskList[0].status;
            }
            return dict;
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
                        userPageActivity.Activity = "Update";
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
                if (includeRemarks && page.Remarks != null && page.Remarks.Length > 0)
                {
                    if (page.Footnote == null || page.Footnote.Length == 0) rec += " @";
                    rec += "?" + page.Remarks;
                }
            }
            return rec;
        }
        public string PagesToFileContent()
        {
            try
            {
                ErrMsg = "";
                // header
                //  {"Version":"2.0","DocNo":"MN-001","Title":"မူလပရိယာယသုတ္တံ"}
                //fileContent = String.Format("{2}Version=2.0; DocNo=\"{0}\"; Title=\"{1}\"; NewParaMarker=\"။ ။\"{3}\r\n", DocID, DocTitle, "{", "}");
                fileContent = "";
                int line = 0;
                foreach (KeyValuePair<string, List<NIS>> kv in Pages)
                {
                    if (line++ > 0) fileContent += "\r\n\r\n";
                    fileContent += String.Format("#{0} ", kv.Key);
                    fileContent += NISPageDataToServerData(kv.Value);
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
            }
            return fileContent;
        }
        //***************************************************************************************
        //*** SuttaInfo functions
        //***************************************************************************************
        // TaskCategories defined in TipitakaDBTables_w.cs
        public List<string> listDocSearchTypes = new List<string>()
        {
            TaskCategories._Recent_, TaskCategories._New_, TaskCategories._Assigned_, TaskCategories._Completed_, TaskCategories._Uploaded_,
            TaskCategories._HTMLCompleted_, TaskCategories._HTMLToDo_, TaskCategories._BookID_, TaskCategories._ExactMatch_,
            TaskCategories._StartsWith_
        };
        public List<string> listTaskSearchTypes = new List<string> 
        {
            TaskCategories._Recent_, TaskCategories._Assigned_, TaskCategories._Completed_, TaskCategories._Uploaded_,
            TaskCategories._HTMLCompleted_, TaskCategories._ExactMatch_, TaskCategories._StartsWith_ 
        };
        public SuttaInfo? GetSuttaInfo(string rowKey)
        {
            SuttaInfo? suttaInfo = null;
            if (clientSuttaInfo != null)
            {
                suttaInfo = clientSuttaInfo.GetSuttaInfo(rowKey);
            }
            return suttaInfo;
        }
        public void RemoveSuttaInfo(string rowKey)
        {
            if (clientSuttaInfo != null)
            {
                SuttaInfo suttaInfo = new SuttaInfo();
                suttaInfo.RowKey = rowKey;
                clientSuttaInfo.DeleteSuttaInfo(suttaInfo);
                if (clientSuttaInfo.StatusCode != 204) 
                {
                    ErrMsg = "Error deleting " + rowKey;
                }
            }
        }
        public Dictionary<string, List<TaskAssignmentInfo>> dict_UserTaskAssignmentInfo = new Dictionary<string, List<TaskAssignmentInfo>>();
        public List<TaskAssignmentInfo> GetUserTaskInfoList(string userID, string curRecentTask)
        {
            string key = "";
            List<TaskAssignmentInfo> taskAssignmentInfoList = new List<TaskAssignmentInfo>();
            switch (curRecentTask)
            {
                case TaskCategories._Current_:
                    key = String.Format("{0}-Task-Current", userID);
                    //if (dict_UserTaskAssignmentInfo.ContainsKey(key))
                    //{
                    //    return dict_UserTaskAssignmentInfo[key];
                    //}
                    break;
                case TaskCategories._Recent_:
                    key = String.Format("{0}-Task-Recent", userID);
                    //if (dict_UserTaskAssignmentInfo.ContainsKey(key))
                    //{
                    //    return dict_UserTaskAssignmentInfo[key];
                    //}
                    break;
            }
            //string query = String.Format("(RowKey gt '{0}-Task-A') and (RowKey lt '{0}-Task-Z')", email);

            if (clientKeyValueData != null && clientTaskAssignmentInfo != null)
            {
                KeyValueData? keyValueData = clientKeyValueData.GetKeyValueData(key);
                if (keyValueData != null && keyValueData.Value != null)
                {
                    string[] docNos = keyValueData.Value.Split('|');
                    foreach(string docNo in docNos)
                    {
                        TaskAssignmentInfo? taskAssignmentInfo = clientTaskAssignmentInfo.GetTaskAssignmentInfo(docNo);
                        if (taskAssignmentInfo != null)
                        {
                            taskAssignmentInfoList.Add(taskAssignmentInfo);
                        }
                    }
                }
            }
            return taskAssignmentInfoList;
        }
        public List<SuttaInfo> GetSuttaList(string searchType, string matchPattern = "", string userClass = "U")
        {
            string query = "";
            if (searchType == "All" && clientSuttaInfo != null)
            {
                return clientSuttaInfo.QuerySuttaInfo(query);
            }
            nextStartingIndex = 0;
            List<SuttaInfo> suttaList = new List<SuttaInfo>();
            if (searchType == TaskCategories._Recent_ || (userClass == "U" && 
                (searchType == TaskCategories._Completed_ || searchType == TaskCategories._Uploaded_ || 
                searchType == TaskCategories._HTMLCompleted_ || searchType == TaskCategories._Assigned_))) 
                return GetRecentSuttaList(searchType, userClass);
            if (clientSuttaInfo != null)
            {
                switch(searchType)
                {
                    case TaskCategories._Current_:
                        query = "Status ne 'Completed' and Status ne 'Uploaded' and Status ne 'HTML'";
                        break;
                    case TaskCategories._New_:          // New
                        query = "Status eq 'Created'";
                        break;
                    case TaskCategories._Assigned_:     // Assigned
                        query = "Status eq 'Assigned'";
                        query += " or Status eq 'DataEntry' or Status eq 'Review' or Status eq 'Edit'";
                        break;
                    case TaskCategories._OnGoing_:      // Ongoing
                        query = "Status eq 'DataEntry' or Status eq 'Review' or Status eq 'Edit'";
                        break;
                    case TaskCategories._Completed_:    // Completed
                        query = "Status eq 'Completed'";
                        break;
                    case TaskCategories._Completion_Report_:
                        query = "Status eq 'Completed' or Status eq 'Uploaded'";
                        break;
                    case TaskCategories._Uploaded_:     // Uploaded
                        query = "Status eq 'Uploaded'";
                        break;
                    case TaskCategories._HTMLCompleted_:     // HTML Complete
                        query = "Status eq 'HTML'";
                        break;
                    case TaskCategories._HTMLToDo_:     // HTML Todo
                        query = "Status eq 'Completed' or Status eq 'Uploaded'";
                        break;
                    case TaskCategories._BookID_:
                        query = String.Format("BookID eq '{0}'", matchPattern);
                        break;
                    case TaskCategories._ExactMatch_:     // Exact match
                        query = String.Format("RowKey eq '{0}'", matchPattern);
                        break;
                    case TaskCategories._StartsWith_:     // Starting with
                        query = String.Format("RowKey ge '{0}' and RowKey le '{0}~'", matchPattern);
                        break;
                }
                suttaList = clientSuttaInfo.QuerySuttaInfo(query);
                CurrentSearchedDocNos.Clear();
                foreach (var sutta in suttaList) CurrentSearchedDocNos.Add(sutta.RowKey);
            }
            return suttaList;
        }
        List<string> CurrentSearchedDocNos = new List<string>();
        int nextStartingIndex = 0;
        const int _GetSuttaBatchSize_ = 10;
        public int GetSearchResultCount () { return CurrentSearchedDocNos.Count; }
        public List<SuttaInfo> GetRecentSuttaList(string searchType, string userClass = "U")
        {
            string query = "";
            List<SuttaInfo> suttaList = new List<SuttaInfo>();
            if (clientKeyValueData != null)
            {
                if (clientSuttaInfo == null) { return suttaList; }
                string userID = (userClass == "U") ? email : "admin";
                CurrentSearchedDocNos = clientKeyValueData.GetUserDocs(userID, searchType);// TaskCategories._Recent_);
                if (CurrentSearchedDocNos.Count > 0)
                {
                    int n = nextStartingIndex;
                    foreach (string doc in CurrentSearchedDocNos)
                    {
                        if (query.Length == 0) query = String.Format("RowKey eq '{0}'", doc);
                        else query += String.Format(" or RowKey eq '{0}'", doc);
                        ++nextStartingIndex;
                        if (++n >= _GetSuttaBatchSize_) break;
                    }
                    suttaList = clientSuttaInfo.QuerySuttaInfo(query);
                }
            }
            return suttaList;
        }
        public List<SuttaInfo> GetNextBatchSearchedSuttas()
        {
            List<SuttaInfo> suttaList = new List<SuttaInfo>();
            if (clientSuttaInfo != null &&
                CurrentSearchedDocNos != null && 
                CurrentSearchedDocNos.Count > 0 && 
                nextStartingIndex < CurrentSearchedDocNos.Count)
            {
                string query = "";
                int lastIndex = Math.Min(CurrentSearchedDocNos.Count, nextStartingIndex + _GetSuttaBatchSize_);
                for(int i = nextStartingIndex; i < lastIndex; ++i)
                {
                    if (query.Length == 0) query = String.Format("(RowKey eq '{0}')", CurrentSearchedDocNos[i]);
                    else query += String.Format(" or (RowKey eq '{0}')", CurrentSearchedDocNos[i]);
                    ++nextStartingIndex;
                }
                if (nextStartingIndex >= CurrentSearchedDocNos.Count) nextStartingIndex = 0;
                suttaList = clientSuttaInfo.QuerySuttaInfo(query);
            }
            return suttaList;
        }
        public SortedDictionary<string, string>? GetAllSuttaList()
        {
            if (clientSuttaInfo != null)
                return clientSuttaInfo.GetAllSuttaInfo();
            return null;
        }
        public List<SuttaInfo> GetPDFSuttaList(string pdfNo)
        {
            string query = String.Format("BookID eq '{0}'", pdfNo);
            List<SuttaInfo> list = new List<SuttaInfo>();
            if (clientSuttaInfo != null)
            {
                list = clientSuttaInfo.QuerySuttaInfo(query);
            }
            return list;
        }
        public List<string> GetPDFDocList(string pdfNo)
        {
            List<string> list = new List<string>();
            List<SuttaInfo> suttaInfoList = GetPDFSuttaList(pdfNo.ToUpper());
            if (suttaInfoList == null || suttaInfoList.Count == 0) return list;
            foreach (SuttaInfo suttaInfo in suttaInfoList)
            {
                if (suttaInfo.PagesSubmitted > 0)
                    list.Add(String.Format("{0}|{1}|{2}", suttaInfo.RowKey, suttaInfo.Title, suttaInfo.NoPages));
            }
            list.Sort();
            return list;
        }
        public async Task GetServerSuttaData(string docID, Action<Object> callback) //, Object mainThread = null)
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
            //clientSuttaPageData!.QueryTableRec(docID).Wait();
            List<SuttaPageData> listSuttaPageData = (List<SuttaPageData>)objResult;
            if (listSuttaPageData.Count() > 0)
            {
                string pgno;
                foreach (var item in listSuttaPageData)
                {
                    pgno = item.PageNo.ToString();
                    pageNos.Add(pgno);
                    Pages[pgno] = GetNISDataFromPage(item.PageData);
                }
            }
            if (callback != null) callback(DocID);
        }
        public List<NIS> GetNISDataFromPage(string page)
        {
            List<NIS> listNIS = new List<NIS>();
            string[] f = page.Split("*");
            // ဧဝံမေသုတန္တိ ^ဧဝံ မေ သုတံ အစရှိသောသုတ်သည် @?Remarks 
            // ရထဝိနီတသုတ္တံ ^ရထဝီနိတသုတ်တည်း @Footnote?Remarks 
            int recNo = 0;
            foreach (var nisRec in f)
            {
                if (nisRec.Length == 0) continue;
                NIS nis = new NIS();
                nis.RecNo = ++recNo;
                string[] f2 = nisRec.Split("^");
                // ဧဝံမေသုတန္တိ
                // ဧဝံ မေ သုတံ အစရှိသောသုတ်သည် @?Remarks 
                if (f2.Length > 1)
                {
                    int n = 0, pos2 = 0; char remarkSymbol = ' ';
                    string trans = "", footnote = "", remarks = "";
                    nis.Pali = f2[0].Trim();    // Pali text
                    string[] f3 = f2[1].Split('@');
                    // ဧဝံ မေ သုတံ အစရှိသောသုတ်သည်
                    // ?Remarks 
                    trans = f3[0].Trim();
                    if (f3.Length == 2)
                    {
                        // footnote exists
                        footnote = f3[1].Trim(); // ?Remarks
                        pos2 = footnote.IndexOfAny(new char[] { '!', '?' });
                        if (pos2 != -1) // remarks
                        {
                            remarkSymbol = footnote[pos2];
                            string[] f4 = footnote.Split(remarkSymbol);
                            footnote = f4[0].Trim();
                            remarks = f4[1].Trim();
                        }
                    }
                    nis.Trans = trans;
                    nis.Footnote = footnote;
                    nis.Remarks = remarks;
                }
                listNIS.Add(nis);
            }
            return listNIS;
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
                Team = email,
            };
            if (clientSuttaInfo != null)
            {
                clientSuttaInfo.AddSuttaInfo(suttaInfo);
                if (clientSuttaInfo.StatusCode != 204) ErrMsg = "New document create error.";
            }
        }
        public void AddNewSuttaInfo(List<SuttaInfo> suttaList)
        {
            if (clientSuttaInfo == null) return;
            clientSuttaInfo.InsertSuttaInfoRecBatch(suttaList).Wait();
            ErrMsg = "";
            if (clientSuttaInfo.StatusCode != 202)
            {
                ErrMsg = clientSuttaInfo.DBErrMsg;
            }
        }
        //***************************************************************************************
        //*** TaskAssignment functions
        //***************************************************************************************
        public List<string> UserTasks = new List<string> 
        {
            TaskCategories._DataEntry_, TaskCategories._Review_, TaskCategories._Edit_, 
            TaskCategories._EditUpload_, TaskCategories._HTML_ 
        };
        public void AddTaskAssignmentInfo(TaskAssignmentInfo taskAssignmentInfo)
        {
            if (clientTaskAssignmentInfo == null) return;
            clientTaskAssignmentInfo.AddTaskAssignmentInfo(taskAssignmentInfo);
            if (clientTaskAssignmentInfo.StatusCode != 204)
                ErrMsg = clientTaskAssignmentInfo.DBErrMsg; 
            else ErrMsg = "";
        }
        public TaskAssignmentInfo? GetTaskAssignmentInfo(string docNo)
        {
            TaskAssignmentInfo? taskAssignmentInfo = null;
            if (clientTaskAssignmentInfo == null) return taskAssignmentInfo;
            taskAssignmentInfo = clientTaskAssignmentInfo.GetTaskAssignmentInfo(docNo);
            if (clientTaskAssignmentInfo.StatusCode != 204)
                ErrMsg = clientTaskAssignmentInfo.DBErrMsg;
            else ErrMsg = "";
            return taskAssignmentInfo;
        }
        public void RemoveTaskAssignmentInfo(string docNo)
        {
            if (clientTaskAssignmentInfo != null)
            {
                TaskAssignmentInfo? taskAssignmentInfo = clientTaskAssignmentInfo.GetTaskAssignmentInfo(docNo);
                if (taskAssignmentInfo != null)
                {
                    List<string> listUsers = new List<string>() { "admin" };
                    var listUserTaskProgressInfo = JsonConvert.DeserializeObject<List<UserTaskProgressInfo>>(taskAssignmentInfo.AssigneeProgress);
                    if (listUserTaskProgressInfo != null)
                    {
                        foreach(var item in listUserTaskProgressInfo) listUsers.Add(item.userID);
                    }
                    if (listUsers.Count > 0)
                    {
                        clientKeyValueData?.RemoveUserDocByCategory(listUsers.ToArray(), "Recent", docNo);
                        clientKeyValueData?.RemoveUserDocByCategory(listUsers.ToArray(), "Assigned", docNo);
                        clientKeyValueData?.RemoveUserTask(listUsers.ToArray(), docNo);
                    }
                    clientTaskAssignmentInfo.RemoveTaskAssignmentInfo(docNo);
                }
            }
        }
        public void UpdateTaskAssignmentInfo(TaskAssignmentInfo taskAssignmentInfo, string docNo)
        {
            if (clientTaskAssignmentInfo != null && clientKeyValueData != null)
            {
                clientTaskAssignmentInfo.UpdateTaskAssignmentInfo(taskAssignmentInfo);
                //string assignedUsers = taskAssignmentInfo.AssigneeProgress;
                //string[] assignments = assignedUsers.Split(new char[] { '|' });
                //List<UserTaskProgressInfo>? userTaskProgressInfo = new List<UserTaskProgressInfo>();
                if (taskAssignmentInfo.AssigneeProgress != null)
                {
                    List<UserTaskProgressInfo>? userTaskProgressInfo = JsonConvert.DeserializeObject<List<UserTaskProgressInfo>>(taskAssignmentInfo.AssigneeProgress);
                    if (userTaskProgressInfo != null)
                    {
                        //clientKeyValueData.AddUserDocByCategory("admin", TaskCategories._Recent_, docNo);
                        foreach (UserTaskProgressInfo item in userTaskProgressInfo)
                        {
                            if (item.status == "Assigned")
                                clientKeyValueData.AddUserDocByCategory(item.userID, TaskCategories._Assigned_, docNo);
                        }
                    }
                }
            }
        }
        public bool HasUserEnteredData(string userID)
        {
            bool dataEntered = false;
            if (clientKeyValueData != null)
            {
                string qualifier = String.Format("(RowKey eq '{0}-Task-Assigned' or RowKey eq '{0}-Task-Recent' or RowKey eq '{0}-Task-Completed')", userID);
                clientKeyValueData.QueryKeyValueData(qualifier);
                List<KeyValueData> tasks = (List<KeyValueData>)clientKeyValueData.objResult;
                dataEntered = tasks.Count > 0;
            }
            return dataEntered;
        }
        public string UserTaskAssignment(string taskAssignments, string userID, string task, string startDate, string lastDate, int submitted, string status)
        {
            string userTaskInfo = String.Format("{0},{1},{2},{3},{4},{5}", userID, task, startDate, lastDate, submitted, status);
            if (taskAssignments != null && taskAssignments.Length == 0) { return userTaskInfo; }
            return taskAssignments + "|" + userTaskInfo;
        }
        //public string GetTaskStatus(TaskAssignmentInfo taskAssignmentInfo, string userProgress)
        //{
        //    string status = "";
        //    string[] f = userProgress.Split(',');
        //    if (f.Length == 6)
        //    {
        //        if (f[1] == "NewDoc") { status = f[5]; }
        //        else
        //        {
        //            string[] ff = taskAssignmentInfo.PageNos.Split(' ');
        //            // get total no of pages
        //            ff[1] = ff[1].Substring(1);
        //            ff[1] = ff[1].Substring(0, ff[1].Length - 1);
        //            // get no of submitted pages
        //            int n = Int32.Parse(f[4]);
        //            // if task has already started and no submitted pages equals total doc pages
        //            // then the task is complete
        //            if (f[2].Length > 0 && n > 0 && n == Int32.Parse(ff[1])) status = "Completed";
        //            else status = f[5];
        //        }
        //    }
        //    return status;
        //}
        public string GetTaskStatus(TaskAssignmentInfo taskAssignmentInfo, UserTaskProgressInfo userTaskProgressInfo)
        {
            string status = "";
            if (userTaskProgressInfo != null)
            {
                if (userTaskProgressInfo.task == "NewDoc") status = userTaskProgressInfo.status;
                else
                {
                    string[] ff = taskAssignmentInfo.PageNos.Split(' ');
                    // get total no of pages
                    ff[1] = ff[1].Substring(1);
                    ff[1] = ff[1].Substring(0, ff[1].Length - 1);
                    int n = Int32.Parse(ff[1]);
                    // if task has already started and # of submitted pages equals total doc pages
                    // then the task is complete
                    if (userTaskProgressInfo.startDate.Length > 0 && n > 0 && userTaskProgressInfo.submitted == n) status = "Completed";
                    else status = userTaskProgressInfo.status;
                }
            }
            return status;
        }
        public void UpdateSuttaInfo(string docNo, string status, string team)
        {
            if (clientSuttaInfo == null) { return; }
            clientSuttaInfo.UpdateSuttaInfo(docNo, status, team);
        }
        public string GetTeamMembers(string assigneeProgress)
        {
            string teamMembers = "", separator = "";
            var listUserTaskProgressInfo = JsonConvert.DeserializeObject<List<UserTaskProgressInfo>>(assigneeProgress);
            if (listUserTaskProgressInfo != null)
            {
                foreach (UserTaskProgressInfo userTaskProgressInfo in listUserTaskProgressInfo)
                {
                    separator = (teamMembers.Length > 0) ? "," : "";
                    teamMembers += separator + GetUserName(userTaskProgressInfo.userID.Trim());
                }
            }
            return teamMembers;
        }
        //***************************************************************************************
        //*** UserProfile functions
        //***************************************************************************************
        //public List<UserProfile> AllUserProfiles = new List<UserProfile>();
        public void ClearLoadedUserProfiles() { UserProfiles = new List<UserProfile>(); }
        public List<UserProfile> GetActiveUserProfiles()
        {
            return GetAllUserProfiles(true);
        }
        public List<UserProfile> GetAllUserProfiles(bool active = false)
        {
            bool inclDev = false;
            if (UserProfiles != null && UserProfiles.Count == 0)
            {
                if (clientUserProfile != null)
                {
                    UserProfiles = clientUserProfile.GetAllUsers(inclDev, active);
                    foreach (UserProfile user in UserProfiles)
                    {
                        if (user.LastLogin != null) user.LastLogin = user.LastLogin.Value.ToLocalTime();
                    }

                }
            }
            return UserProfiles!;
        }
        public void AddUserProfile(string email, string name, string nameM = "", string userClass = "U", int loginCount = 1)
        {
            if (clientUserProfile != null)
            {
                clientUserProfile.AddUserProfile(email, name, nameM, userClass, loginCount);
                ErrMsg = string.Empty;
                if (clientUserProfile.StatusCode != 204) { ErrMsg = "User profile add error."; }
            }
        }
        public void DelUserProfile(string email)
        {
            if (clientUserProfile != null)
            {
                ErrMsg = string.Empty;
                UserProfile userProfile = new UserProfile() { RowKey = email };
                clientUserProfile.DeleteTableRec(userProfile).Wait();
                if (clientUserProfile.StatusCode != 204) { ErrMsg = "User profile add error."; }
            }
        }
        public void UpdateUserProfile(UserProfile userProfile)
        {
            if (clientUserProfile != null && userProfile != null)
            {
                clientUserProfile.UpdateUserProfile(userProfile);
                ErrMsg = clientUserProfile.ErrMsg;
            }
        }
        public void UpdateUserProfile(string userID, string nameE, string nameM, string userClass = "U")
        {
            UserProfile userProfile = new UserProfile()
            {
                RowKey = userID,
                Name_E = nameE,
                Name_M = nameM,
                UserClass = userClass
            };
            if (clientUserProfile != null && userProfile != null)
            {
                clientUserProfile.UpdateUserProfile(userProfile);
                ErrMsg = clientUserProfile.ErrMsg;
            }
        }
        public string GetUserName(string email)
        {
            if (email == "dhammayaungchi2011@gmail.com") return "System Admin";
            List<UserProfile> AllUserProfiles = new List<UserProfile>();
            if (AllUserProfiles.Count == 0) AllUserProfiles = GetAllUserProfiles();
            var userName = (from user in AllUserProfiles
                              where user.RowKey == email select user.Name_E).ToList();
            return userName.Count == 0 ? "" : userName[0];
        }
        public string GetUserID(string userName)
        {
            if (userName == "System Admin") return "dhammayaungchi2011@gmail.com";
            List<UserProfile> AllUserProfiles = new List<UserProfile>();
            if (AllUserProfiles.Count == 0) AllUserProfiles = GetAllUserProfiles();
            var userID = (from user in AllUserProfiles
                            where user.Name_E == userName
                            select user.RowKey).ToList();
            if (userID.Count > 0)
            {
                return "";
            }
            return "";
        }
        //***************************************************************************************
        //*** ီDocType functions
        //***************************************************************************************
        public List<string> DocTypes = new List<string>();
        public List<string> DocSubTypes = new List<string>();
        //***************************************************************************************
        //*** ီSourceBook Info functions
        //***************************************************************************************
        private SortedDictionary<string, SourceBookInfo> dicSourceDocInfo = new SortedDictionary<string, SourceBookInfo>();
        public List<SourceBookInfo> listSourceBookInfo = new List<SourceBookInfo>();
        public SourceBookInfo? GetSourceBookInfo(string bookID)
        {
            SourceBookInfo? sourceBookInfo = null;
            if (clientSourceBookInfo == null) return sourceBookInfo;
;
            if (listSourceBookInfo.Count == 0)
                listSourceBookInfo = clientSourceBookInfo.GetSourceBookInfo().Result;
            var list = (from n in listSourceBookInfo
                        where n.RowKey == bookID select n).ToList();
            if (list.Count > 0) sourceBookInfo = list[0];
            return sourceBookInfo;
        }
        public SortedDictionary<string, SourceBookInfo> GetSourceBookInfo()
        {
            // always refresh
            dicSourceDocInfo.Clear();
            if (clientKeyValueData != null && dicSourceDocInfo.Count == 0)
            {
                string qualifier = "all";
                if ((listSourceBookInfo.Count > 0 && dicSourceDocInfo.Count > 0) || clientSourceBookInfo == null) return dicSourceDocInfo;
                //listSourceBookInfo = clientSourceBookInfo.GetSourceBookInfo(qualifier);
                listSourceBookInfo = clientSourceBookInfo.GetSourceBookInfo().Result;//.ConfigureAwait(false);
                if (listSourceBookInfo != null && listSourceBookInfo.Count > 0)
                {
                    foreach(SourceBookInfo sourceBookInfo in listSourceBookInfo) 
                    {
                        //if (sourceBookInfo.RowKey.Contains("MN-01"))
                        //{
                        //    qualifier = sourceBookInfo.RowKey;
                        //}
                        dicSourceDocInfo.Add(sourceBookInfo.RowKey, sourceBookInfo);
                    }
                }
            }
            return dicSourceDocInfo!;
        }
        public List<SourceBookInfo> AddSourceBookInfo(string[] recs)
        {
            List<SourceBookInfo> listPDF = new List<SourceBookInfo>();
            if (recs == null || recs.Length == 0) return listPDF;
            if (clientSourceBookInfo != null && clientKeyValueData != null)
            {
                foreach (string rec in recs)
                {
                    string[] f = rec.Trim().Split(',');
                    if (f.Length == 3)
                    {
                        SourceBookInfo sourceBookInfo = new SourceBookInfo()
                        {
                            RowKey = f[0],
                            BookFilename = f[1],
                            DocCount = 0,
                            DocNos = "",
                            Pages = (f[2].Length > 0) ? Convert.ToInt32(f[2]) : 0,
                            Completed = 0,
                            HTML = 0
                        };
                        clientSourceBookInfo.AddSourceBookInfo(sourceBookInfo);
                        listPDF.Add(sourceBookInfo);
                    }
                }
            }
            return listPDF;
        }
        public List<string> GetDocTypes()
        {
            if (clientKeyValueData != null && DocTypes.Count == 0)
            {
                KeyValueData kvd = clientKeyValueData.GetKeyValueData("DocTypes");
                if (kvd.Value.Length > 0)
                {
                    DocTypes = kvd.Value.Split(',').ToList();
                    for (int i = 0; i < DocTypes.Count; i++)
                    {
                        DocTypes[i] = DocTypes[i].Trim();
                    }
                }
            }
            return DocTypes;
        }
        public List<string> GetDocSubTypes()
        {
            if (clientKeyValueData != null && DocSubTypes.Count == 0)
            {
                KeyValueData kvd = clientKeyValueData.GetKeyValueData("DocSubTypes");
                if (kvd != null && kvd.Value != null && kvd.Value.Length > 0)
                {
                    DocSubTypes = kvd.Value.Split(',').ToList();
                    for (int i = 0; i < DocSubTypes.Count; i++)
                    {
                        DocSubTypes[i] = DocSubTypes[i].Trim();
                    }
                }
            }
            return DocSubTypes;
        }
        public void AddDocSubType(string subType)
        {
            if (subType == null || subType.Length == 0) return;
            DocSubTypes.Add(subType);
            if (clientKeyValueData != null)
            {
                clientKeyValueData.UpdateKeyValueData(new KeyValueData()
                {
                    PartitionKey = "KeyValueData",
                    RowKey = "DocSubTypes",
                    Value = String.Join(",", DocSubTypes),
                });
            }
        }
        public void UpdateSourceBookDocInfo(Dictionary<string, List<string>> dictBookDocList)
        {
            if (clientSourceBookInfo == null) { return; }
            //string separator;
            //int totalPages = 0;
            //dicSourceDocInfo.Add(sourceBookInfo.RowKey, sourceBookInfo);
            foreach (string bookID in dictBookDocList.Keys)
            {
                SourceBookInfo? sourceBookInfo = clientSourceBookInfo.GetSourceBookInfo(bookID);
                if (sourceBookInfo != null)
                {
                    // check to make sure this doc is already not in the list
                    //totalPages = 0;
                    List<string> listDoc = dictBookDocList[bookID];
                    //string book_pages = sourceBookInfo.DocNos;
                    List<string> listSrcDocs = new List<string>();
                    if (sourceBookInfo.DocNos.Trim().Length > 0)
                        listSrcDocs = sourceBookInfo.DocNos.Trim().Split(',').ToList();
                    foreach (string doc in listDoc)
                    {
                        if (!listSrcDocs.Contains(doc))
                        {
                            //separator = (sourceBookInfo.DocNos.Length == 0) ? "" : ",";
                            //totalPages += TotalPagesInDocs(listDoc);
                            //totalPages += PagesInDoc(doc);
                            //if (sourceBookInfo.DocNos.Length > 0) totalPages += TotalPagesInDocs(sourceBookInfo.DocNos.Split(',').ToList());
                            //if (totalPages > sourceBookInfo.Pages) sourceBookInfo.Pages = totalPages;
                            //sourceBookInfo.DocNos += separator + doc; // String.Join(',', listDoc);
                            //book_pages += separator + doc;
                            listSrcDocs.Add(doc);
                        }
                    }
                    sourceBookInfo.DocCount = listSrcDocs.Count();
                    sourceBookInfo.DocNos = String.Join(',', listSrcDocs);
                    dicSourceDocInfo[bookID] = sourceBookInfo;
                    clientSourceBookInfo.UpdateSourceBookInfo(sourceBookInfo).Wait();
                    ErrMsg = "";
                    if (clientSourceBookInfo.StatusCode != 204) ErrMsg = clientSourceBookInfo.DBErrMsg;
                }
            }
        }
        public void RemoveSourceBookDocNo(string bookID, string docNo)
        {
            if (clientSourceBookInfo == null || bookID == null || bookID.Length == 0 ||
                docNo == null || docNo.Length == 0) { return; }
            SourceBookInfo? sourceBookInfo = clientSourceBookInfo.GetSourceBookInfo(bookID);
            if (sourceBookInfo == null || !sourceBookInfo.DocNos.Contains(docNo)) { return; }
            List<string> listDoc = sourceBookInfo.DocNos.Split(',').ToList();
            if (listDoc == null || !listDoc.Contains(docNo)) return;
            listDoc.Remove(docNo);
            sourceBookInfo.DocNos = String.Join(",", listDoc);
            sourceBookInfo.DocCount--;
            clientSourceBookInfo.UpdateSourceBookInfo(sourceBookInfo).Wait();
        }
        public bool DocAlreadyExists(string docNo)
        {
            if (clientSuttaInfo == null) { return false; }
            SuttaInfo? suttaInfo = clientSuttaInfo.GetSuttaInfo(docNo);
            return (suttaInfo == null) ? false : true;
        }
        public void RemoveDocInfo(string bookID, string docNo)
        {
            if (clientSourceBookInfo == null || bookID == null || bookID.Length == 0 || 
                docNo == null || docNo.Length == 0) { return; }
            SourceBookInfo? sourceBookInfo = clientSourceBookInfo.GetSourceBookInfo(bookID);
            if (sourceBookInfo == null) { return; }
            if (sourceBookInfo.DocNos.Contains(docNo))
            {
                List<string> list = sourceBookInfo.DocNos.Split(',').ToList();
                list.Remove(docNo);
                sourceBookInfo.DocCount = list.Count;
                sourceBookInfo.DocNos = String.Join(",", list.ToArray());
                clientSourceBookInfo.UpdateSourceBookInfo(sourceBookInfo).Wait();
            }
            RemoveSuttaInfo(docNo);
        }
        public string GetSourceBook(string docID)
        {
            string book = "";
            if (clientSuttaInfo == null) return book;
            SuttaInfo? suttaInfo = clientSuttaInfo.GetSuttaInfo(docID);
            if (suttaInfo != null)
            {
                SourceBookInfo? sourceBookInfo = GetSourceBookInfo(suttaInfo.BookID);
                if (sourceBookInfo != null)
                {
                    book = sourceBookInfo.BookFilename;
                }
            }
            return book;
        }
        List<string> SourceBookIDList = new List<string>();
        public List<string> GetSourceBookIDs()
        {
            if (SourceBookIDList.Count > 0) return SourceBookIDList;
            SortedDictionary<string, SourceBookInfo> dictSourceBookInfo = GetSourceBookInfo();
            foreach (string key in dictSourceBookInfo.Keys)
            {
                SourceBookIDList.Add(key);
            }
            return SourceBookIDList;
        }
        //***************************************************************************************
        //*** ီActivityLog & TaskActivityLog functions
        //***************************************************************************************
        public List<ActivityLog> GetActivities(DateTime dt1, DateTime dt2)
        {
            List < ActivityLog > activities = new List < ActivityLog >();
            if (clientActivityLog != null)
            {
                activities = clientActivityLog.GetActivities(dt1, dt2);
            }
            return activities;
        }
        public void AddActivityLog(string userID, string activity, string desc)
        {
            if (clientActivityLog != null)
            {
                clientActivityLog.AddActivityLog(userID, activity, desc);
            }
        }
        public void AddTaskActivityLog(string docNo, string userID, string activity, int pages, int totalSubmittedPages,
                    int submittedPages, string desc = "")
        {
            if (clientTaskActivityLog != null && clientKeyValueData != null)
            {
                if (desc.Length == 0)
                {
                    //int totalSubmittedPagesAfterThisTask = totalSubmittedPages + submittedPages;
                    if (totalSubmittedPages >= pages) desc = "Completed";
                    else desc = String.Format("{0}%", (int)(totalSubmittedPages * 100 / pages));
                }
                // TaskActivityLog
                clientTaskActivityLog.AddTaskActivityLog(docNo, userID, activity, pages, totalSubmittedPages, submittedPages, desc);
                //string uid = (userClass != null && userClass == "U") ? userID : "admin";
                //if (userClass == "U") clientKeyValueData.AddUserDocByCategory(uid, TaskCategories._Assigned_, docNo);
                if (desc == "Completed")
                {
                    // Update KeyValueData4Task
                    clientKeyValueData.AddUserDocByCategory(userID, TaskCategories._Completed_, docNo);
                    //clientKeyValueData.UpdateKeyValueData4Task(userID, docNo);
                    //clientKeyValueData.RemoveKeyValueData4Task(userID, docNo);
                }
            }
        }
        public void AddDocNoToRecent()
        {
            if (clientKeyValueData != null)
            {
                clientKeyValueData.AddUserDocByCategory(email, TaskCategories._Recent_, DocID);
            }
        }
        public bool PayrollAdmin()
        {
            if (clientKeyValueData != null)
            {
                KeyValueData p = clientKeyValueData.GetKeyValueData("Payroll-Department");
                return p != null && p.Value != null && p.Value.Contains(email);
            }
            return false;
        }
        public void GetDocPages(Int32 pages, out int docPages, out int bookPages)
        {
            string subbkPages = pages.ToString();
            if (subbkPages.Length >= 3)
            {
                if (subbkPages.Length < 6)
                {
                    subbkPages = "000" + subbkPages;
                    subbkPages = subbkPages.Substring(subbkPages.Length - 6);
                }
            }
            docPages = Int32.Parse(subbkPages.Substring(0, 3));
            bookPages = Int32.Parse(subbkPages.Substring(3));
        }
        public Int32 CombineSubBkPages(int docPages, int bookPages)
        {
            if (bookPages < 100) docPages *= 10000; else docPages *= 1000;

            return docPages + bookPages; 
        }
        //***************************************************************************************
        //*** JSON functions
        //***************************************************************************************
        //public class NDESTask
        //{
        //    public string userID = "";
        //    public string task = "";
        //    public string startDate = "";
        //    public string lastDate = "";
        //    public int submitted= 0;
        //    public NDESTask(string userID, string task, string startDate, string lastDate, int submitted)
        //    {
        //        this.userID = userID; this.task = task; this.startDate = startDate;
        //        this.lastDate = lastDate; this.submitted = submitted;
        //    }
        //}
        // https://www.newtonsoft.com/json

        //const string jsonRecord = "{\"UserID\":\"user0@gmail.com\", \r\n\"Task:\"NewDoc\", \r\n\"StartDate\":\"\", \r\n\"LastUpdated\":\"\", \r\n\"Submitted\":0}";
        public void ResetDataTables()
        {
            if (email == null || email != "dhammayaungchi2011@gmail.com") return;
            if (clientActivityLog != null) clientActivityLog.DeleteAll(email);
            if (clientTaskActivityLog != null) clientTaskActivityLog.DeleteAll(email);
            if (clientSourceBookInfo != null) { clientSourceBookInfo.ResetSourceBookDocs(email); }
            if (clientSuttaInfo != null) { clientSuttaInfo.DeleteAll(email); }; 
            if (clientSuttaPageData != null) { clientSuttaPageData.DeleteAll(email); };
            if (clientTaskAssignmentInfo != null) { clientTaskAssignmentInfo.DeleteAll(email); }
            if (clientKeyValueData != null) clientKeyValueData.DeleteUserTasks(email).Wait();
        }
        //***************************************************************************************
        //*** Other functions
        //***************************************************************************************
        public void ExportAsHTML()
        {
            string type = "MN-";
            for(int i = 5; i <= 150; i++)
            {
                if (i >= 130) type = "DN-";
                SuttaInfo suttaInfo = new SuttaInfo()
                {
                    RowKey = type + i.ToString("D3"),
                    Title = "",
                    StartPage = 1,
                    EndPage = 100,
                    NoPages = 100,
                    Team = "AuzMaung",
                };
                if (clientSuttaInfo != null) clientSuttaInfo.AddSuttaInfo(suttaInfo);
            }
        }
        public string GetUKDateOnly(string dt)
        {
            if (dt.Length == 0) return dt;
            string[] f = dt.Split('/');
            string t = f[0];
            f[0] = f[1]; f[1] = t;
            dt = String.Join('/', f);
            int p = dt.IndexOf(' ');
            if (p == -1) return dt;
            return dt.Substring(0, p);
        }
        public string GetTodaysDate()
        {
            return GetUKDateOnly(DateTime.Today.ToShortDateString());
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

