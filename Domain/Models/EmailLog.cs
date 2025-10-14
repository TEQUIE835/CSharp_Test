namespace PruebaDesempenio.Models;

public class EmailLog
{
    public int Id { get; set; }
    public string Recipient { get; set; }
    public DateTime SentDate { get; set; } = DateTime.Now;
    public bool SentSuccesfully { get; set; }
    public string? ErrorMessage { get; set; }
}