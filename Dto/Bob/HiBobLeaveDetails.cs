using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Bob
{
    public class HiBobLeaveDetails
    {
        public string requestId { get; set; }
        public string employeeEmail { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
    }
}
