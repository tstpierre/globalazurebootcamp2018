using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neudesic.SmartOffice.RoomService.Domain {
    public class UtilizationPeriod
    {
        public byte DayOfWeek { get; set; }
        public byte HourOfDay { get; set; }
        public decimal Utilization { get; set; }
    }
}
