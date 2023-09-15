public class TimeTrackingEntry
{
    public TimeTrackingEntry(string Consultant, string Date, string Account, string ActivityCode, string Hours, string Comment)
    {
        this.Consultant = Consultant;
        this.Date = Date;
        this.Account = Account;
        this.ActivityCode = ActivityCode;
        this.Hours = Hours;
        this.Comment = Comment;
    }

    public string Consultant { get; set; }

    public string Date { get; set; }

    public string Account { get; set; }

    public string ActivityCode { get; set; }

    public string Hours { get; set; }

    public string Comment { get; set; }
}
