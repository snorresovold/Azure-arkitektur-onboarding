using Azure.Core;

public class RequestObject
{
    public int Id { get; set; }
    public string ActivityCode { get; set; }
    public string ProjectCode { get; set; }
    public int EmployeeCode { get; set; }
    public int CustomerCode { get; set; }
    public string HourType { get; set; }
    public DateTime Date { get; set; }
    public bool IsLocked { get; set; }
    public string Comment { get; set; }
    public string InternalComment { get; set; }
    public DateTime LastChanged { get; set; }
    public bool ExcludedFromPayroll { get; set; }
    public bool IsTransferedToPayroll { get; set; }
    public bool IsInvoiced { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal HourlyCost { get; set; }
    public int Minutes { get; set; }
    public decimal BillableHours { get; set; }
    public decimal BillableAmount { get; set; }
    public int BreakTime { get; set; }

}
