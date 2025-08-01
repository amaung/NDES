﻿using Azure;
using Azure.Data.Tables;

namespace Tipitaka_DBTables
{
    public record UserProfile : ITableEntity
    {
        public string RowKey { get; set; } = default!; // email
        public string PartitionKey { get; set; } = "UserProfile";
        public string Name_E { get; set; } = default!;
        public string Name_M { get; set; } = default!;
        //public string Password { get; set; } = default!;
        public string UserClass { get; set; } = default!;
        //public string Status { get; set; } = default!;
        public int LoginCount {  get; set; } = default!;
        public DateTimeOffset JoinedDate { get; init; } = default!;
        public string? LastDate { get; set; } = default!;
        public DateTimeOffset? LastLogin { get; set; } = default!;
        public ETag ETag { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = default!;
    }
    public record ActivityLog : ITableEntity
    {
        public string PartitionKey { get; set; } = "ActivityLog";
        public string RowKey { get; set; } = default!;
        public string UserID { get; set; } = default!;
        public string Activity { get; set; } = default!;
        public string Description { get; set; } = default!;
        public ETag ETag { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = default!;
    }
    public record TaskActivityLog : ITableEntity
    {
        public string PartitionKey { get; set; } = "TaskActivityLog";
        public string RowKey { get; set; } = default!; // Datetime
        public string UserID { get; set; } = default!;
        public string Author { get; set; } = default!; // added 
        public string DocNo { get; set; } = default!;
        public string Activity { get; set; } = default!;
        public int Pages {  get; set; } = default!;
        public int TotalSubmitted {  get; set; } = default!;
        public int SubmittedPages { get; set; } = default;
        public string Description { get; set; } = default!;
        public ETag ETag { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = default!;
    }
    public record SuttaInfo : ITableEntity
    {
        public string PartitionKey { get; set; } = "SuttaInfo";
        public string RowKey { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string BookID { get; set; } = default!;
        public int StartPage { get; set; } = default!;
        public int EndPage { get; set; } = default!;
        public int NoPages { get; set; } = default!;
        public int PagesSubmitted { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string Team { get; set; } = default!;
        public ETag ETag { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = default!;
        public void Copy(SuttaInfo other)
        {
            this.PartitionKey = other.PartitionKey;
            this.RowKey = other.RowKey;
            this.Title = other.Title;
            this.StartPage = other.StartPage;
            this.EndPage = other.EndPage;
            this.NoPages = other.NoPages;
            this.PagesSubmitted = other.PagesSubmitted;
            this.Status = other.Status;
            this.Team = other.Team;
        }
    }
    public record SuttaPageData : ITableEntity
    {
        public string PartitionKey { get; set; } = "SuttaPageData";
        public string RowKey { get; set; } = default!;
        public int PageNo { get; set; } = default!;
        public string PageData { get; set; } = default!;
        public int NISRecCount { get; set; } = default!;
        public int NISRecLen { get; set; } = default!;
        public string UserID { get; set; } = default!;
        public ETag ETag { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = default!;
    }
    public record TaskAssignmentInfo : ITableEntity
    {
        public string PartitionKey { get; set; } = "TaskAssignmentInfo";
        public string RowKey { get; set; } = default!;
        public string DocTitle { get; set; } = default!;
        public string PageNos { get; set; } = default!;
        public int PagesSubmitted { get; set; } 
        public string AssigneeProgress { get; set; } = default!;
        public string StartDate { get; set; } = default!; 
        public string LastDate { get; set; } = default!;
        public int CorrectionCount { get; set; } = default!;
        public string Status { get; set; } = default!;
        public ETag ETag { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = default!;
    }
    public record CorrectionLog : ITableEntity
    {
        public string PartitionKey { get; set; } = "CorrectionLog";
        public string RowKey { get; set; } = default!;
        public string UserID { get; set; } = default!;
        public string Task { get; set; } = default!;
        public int PageNo { get; set; } = default!;
        public string NISRec {  get; set; } = default!;
        public string OrigText { get; set; } = default!;
        public string EditedText { get; set; } = default!;
        public ETag ETag { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = default!;
    }
    public record UserPageActivity : ITableEntity
    {
        public string PartitionKey { get; set; } = "UserPageActivity"!;
        public string RowKey { get; set; } = default!;
        public string DocID { get; set; } = default!;
        public string Activity { get; set; } = default!;
        public string PageRange { get; set; } = default!;
        public int Pages { get; set; } = default!;
        public ETag ETag { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = default!;
    }
    public record KeyValueData : ITableEntity
    {
        public string PartitionKey { get; set; } = "KeyValueData";
        public string RowKey { get; set; } = default!;
        public string Value { get; set; } = default!;
        public ETag ETag { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = default!;
    }
    public record SourceBookInfo : ITableEntity
    {
        public string PartitionKey { get; set; } = "SourceBookInfo";
        public string RowKey { get; set; } = default!;  // PDF Book ID
        public string BookFilename { get; set; } = default!;
        public int DocCount { get; set; } = default!;
        public string DocNos { get; set; } = default!;
        public int Pages { get; set; } = default!;
        public int Completed { get; set; } = default!;
        public int HTML { get; set; } = default!;
        public ETag ETag { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = default!;
    }
    public record Timesheet : ITableEntity
    {
        public string PartitionKey { get; set; } = "Timesheet";
        public string RowKey { get; set; } = default!;  // userID $ date $ tickcount
        public string Date { get; set; } = default!;
        public string UserID { get; set; } = default!;
        public DateTime StartTime { get; set; } = default!;
        public DateTime EndTime { get; set; } = default!;
        public string DocNo { get; set; } = default!;
        public string Task { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int StartPage { get; set; } = default!;
        public int EndPage { get; set; } = default!;
        public string Status { get; set; } = default!;
        public ETag ETag { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = default!;
    }
    public class UserTaskProgressInfo
    {
        public string userID = "";
        public string author = "";
        public string task = "";
        public string startDate = "";
        public string lastDate = "";
        public int submitted = 0;
        public int corrections = 0;
        public string status = "";
        public UserTaskProgressInfo() { }
        public UserTaskProgressInfo(UserTaskProgressInfo u)
        {
            this.userID = u.userID; this.author = u.author; this.task = u.task; this.startDate = u.startDate;
            this.lastDate = u.lastDate; this.submitted = u.submitted;
            this.corrections = u.corrections; this.status = u.status;
        }
    }
    public static class TaskCategories
    {
        public const string _New_ = "New";
        public const string _Assigned_ = "Assigned";
        public const string _OnGoing_ = "OnGoing";
        public const string _Completed_ = "Completed"; // changed from Data Completed 4/3/2025
        public const string _Completion_Report_ = "Completion Report";
        public const string _Uploaded_ = "Uploaded";
        public const string _HTMLCompleted_ = "HTML Completed";
        public const string _HTMLToDo_ = "HTML ToDo";
        public const string _ExactMatch_ = "Exact match";
        public const string _StartsWith_ = "Starts with";
        public const string _Current_ = "Current";
        public const string _Recent_ = "Recent";
        public const string _BookID_ = "Book ID";
        // user tasks
        public const string _DataEntry_ = "DataEntry";
        public const string _Review_ = "Review";
        public const string _Review1_ = "Review1";
        public const string _Review2_ = "Review2";
        public const string _Review3_ = "Review3";
        public const string _Review4_ = "Review4";
        public const string _Edit_ = "Edit";
        public const string _EditUpload_ = "Edit-Upload";
        public const string _HTML_ = "HTML";
        public const string _DocTask_ = "Doc-Task Related";
        public const string _ProgectManagement_ = "Progect Management";
        public const string _Others_ = "Others";
    }
    public class DocReportInfo
    {
        public int srNo { get; set; }
        public string date { get; set; } = "";
        public string docNo { get; set; } = "";
        public string docTitle { get; set; } = "";
        public string pages { get; set; } = "";
        public string sourceFileCode { get; set; } = "";
        public string sourceFile { get; set; } = "";
    }
}