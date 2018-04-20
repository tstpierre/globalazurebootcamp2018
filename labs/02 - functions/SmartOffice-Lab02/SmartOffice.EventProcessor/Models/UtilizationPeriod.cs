using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartOffice.EventProcessor.Models
{
    public class UtilizationPeriod
    {
        public byte DayOfWeek { get; set; }
        public byte HourOfDay { get; set; }
        public decimal Utilization { get; set; }
    }
}
