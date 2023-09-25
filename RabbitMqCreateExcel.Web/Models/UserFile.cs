using RabbitMqCreateExcel.Web.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace RabbitMqCreateExcel.Web.Models
{
    public class UserFile
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string? FilePath { get; set; } 
        public DateTime? CreatedDate { get; set; }
        public FileStatus FileStatus { get; set; }
        [NotMapped]
        public string GetCreatedDate => CreatedDate.HasValue ? CreatedDate.Value.ToShortDateString() : "-";
    }
}
