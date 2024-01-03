using Azure;
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
        //public string Remarks { get; set; } = default!;
        public DateTimeOffset LastLogin { get; set; } = default!;
        public ETag ETag { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = default!;
    }
    public record UserDocumentLog : ITableEntity
    {
        public string PartitionKey { get; set; } = default!;
        public string RowKey { get; set; } = default!; // UserID + DocID
        public string Action { get; set; } = default!;
        public string PageRanges {  get; set; } = default!;
        public int Pages { get; init; } = default;
        public ETag ETag { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = default!;
    }

    public record ActivityLog : ITableEntity
    {
        public string PartitionKey { get; set; } = default!;
        public string RowKey { get; set; } = default!;
        public string DocID { get; init; } = default!;
        public string Activity { get; init; } = default!;
        public string PageRanges { get; set; } = default!;
        public int Pages { get; init; } = default;
        public ETag ETag { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = default!;
    }
    public record SuttaInfo : ITableEntity
    {
        public string PartitionKey { get; set; } = "SuttaInfo";
        public string RowKey { get; set; } = default!;
        public string Title { get; set; } = default!;
        public int StartPage { get; set; } = default!;
        public int EndPage { get; set; } = default!;
        public int NoPages { get; set; } = default!;
        public int NISRecCount { get; set; } = default!;
        public string SubmittedBy { get; set; } = default!;
        public ETag ETag { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = default!;
        public void Copy(SuttaInfo other)
        {
            this.Title = other.Title;
            this.StartPage = other.StartPage;
            this.EndPage = other.EndPage;
            this.NoPages = other.NoPages;
            this.NISRecCount = other.NISRecCount;
            this.SubmittedBy = other.SubmittedBy;
        }
    }
    public record SuttaPageData : ITableEntity
    {
        public string PartitionKey { get; set; } = "SuttaPageData";
        public string RowKey { get; set; } = default!;
        public int PageNo { get; set; } = default!;
        public string PageData { get; set; } = default!;
        public string UserID { get; set; } = default!;
        public ETag ETag { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = default!;
    }
    public record UserPageActivity : ITableEntity
    {
        public string PartitionKey { get; set; } = "UserPageActivity"!;
        public string RowKey { get; set; } = default!;
        public string DocID { get; set; }
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
}