namespace Brickalytics.Models
{
    public class Result<T>
    {
    public int Code { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    }
}