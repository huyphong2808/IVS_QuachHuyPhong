using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Model
{
    public class MeasureModel
    {
        public int id { get; set; }
        [Required]
        public string code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }
}
