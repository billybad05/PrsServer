using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace PrsServer.Models {

    public class Request {

        [Key]
        public int Id { get; set; }

        [StringLength(80)]
        public string Description { get; set; }

        [StringLength(80)]
        public string Justification { get; set; }

        [StringLength(80)]
        public string? RejectionReason { get; set; }

        [StringLength(20)]
        public string DeliveryMode { get; set; } = "Pickup";

        [StringLength(10)]
        public string Status { get; set; } = "NEW";

        [Column(TypeName = "decimal(11,2)")]
        public decimal Total { get; set; } = 0m;

        public int UserId { get; set; }
        public virtual User? User { get; set; }

        public virtual ICollection<Requestline>? Requestlines { get; set; }
    }
}
