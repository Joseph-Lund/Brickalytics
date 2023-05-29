namespace Brickalytics.Models
{
    public class Result<T> : Result
    {
    public T? Data { get; set; }
    }
    public class Result
    {
    public int Code { get; set; }
    public string? Message { get; set; }
    }
}