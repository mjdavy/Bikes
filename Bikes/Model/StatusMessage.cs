using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bikes.Model
{
    public class StatusMessage
    {
        public StatusMessage(string status)
        {
            this.Status = status;
        }

        public string Status { get; set; }
    }
}
