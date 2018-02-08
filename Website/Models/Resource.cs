using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Models
{
    [Table("AspNetResources")]
    public class Resource
    {
        [Key]
        [MaxLength(32)]
        [DataType("nvarchar")]
        public string ResouceKey { get; set; }

        [MaxLength(50)]
        public string ModuleName { get; set; }

        [MaxLength(128)]
        public string DisplayName { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }
    }
}
