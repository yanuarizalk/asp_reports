
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP_Web_Reports.Models
{
    [Table("SALE1")]
    public class SalesGraph
    {
        [Key]
        //[Column("YYYYMM")]
        public string YYYYMM { get; set; }
        public decimal YARN { get; set; }
        public decimal DENIM { get; set; }
        public decimal GREY { get; set; }
        public decimal COLOUR { get; set; }
        public decimal HOMELINEN { get; set; }
        public decimal TOWEL { get; set; }
        public decimal GARMENT { get; set; }

    }
}