using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PrsServer.Models
{

    public class Requestline
    {

        [Key]
        public int Id { get; set; }

        public int RequestId { get; set; }
        [JsonIgnore]
        public virtual Request? Request { get; set; }

        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }

        public int Quantity { get; set; } = 1;
    }
}