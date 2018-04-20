using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neudesic.SmartOffice.RoomService.Models
{
    public class Room
    {        

        public string       Id { get; set; }
        public string       DisplayName { get; set; }
        public string       Description { get; set; }
        public RoomState    State { get; set; }

        public DateTime     StateChangeTimestamp { get; set; }

        public IEnumerable<UtilizationPeriod> Utilization { get; set; } = new List<UtilizationPeriod>();
    }
}
