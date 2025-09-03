using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsthmaApp.Common
{
    public class Filter
    {
        public string? SearchQuery { get; set; }
        public Guid? UserId { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsActive { get; set; }
        public Guid? DoctorId { get; set; }
    }
}
