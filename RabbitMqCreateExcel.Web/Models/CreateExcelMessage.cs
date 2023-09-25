namespace RabbitMqCreateExcel.Web.Models
{
    public class CreateExcelMessage
    {
        public string? UserId { get; set; }
        public int FileId { get; set; }
    }
}
