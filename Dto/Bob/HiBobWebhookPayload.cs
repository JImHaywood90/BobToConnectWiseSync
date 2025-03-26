using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Bob
{
    public class HiBobWebhookPayload
    {
        public string eventType { get; set; }
        public string requestId { get; set; }
    }
}
