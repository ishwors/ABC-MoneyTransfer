namespace MoneyTransfer.Core.DTOs.ExchangeRate;

public class NrbApiResponse
{
    public Status Status { get; set; }
    public Errors Errors { get; set; }
    public Params Params { get; set; }
    public DataContainer Data { get; set; }
    public Pagination Pagination { get; set; }
}

public class Status
{
    public int Code { get; set; }
}

public class Errors
{
    public object Validation { get; set; }
}

public class Params
{
    public string Date { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public string Post_Type { get; set; }
    public string Per_Page { get; set; }
    public string Page { get; set; }
    public string Slug { get; set; }
    public string Q { get; set; }
}

public class DataContainer
{
    public List<PayloadItem> Payload { get; set; }
}

public class PayloadItem
{
    public string Date { get; set; }
    public string Published_On { get; set; }
    public string Modified_On { get; set; }
    public List<RateDetail> Rates { get; set; }
}

public class RateDetail
{
    public Currency Currency { get; set; }
    public string Buy { get; set; }
    public string Sell { get; set; }
}

public class Currency
{
    public string Iso3 { get; set; }
    public string Name { get; set; }
    public int Unit { get; set; }
}

public class Pagination
{
    public int Page { get; set; }
    public int Pages { get; set; }
    public int Per_Page { get; set; }
    public int Total { get; set; }
    public Links Links { get; set; }
}

public class Links
{
    public object Prev { get; set; }
    public object Next { get; set; }
}