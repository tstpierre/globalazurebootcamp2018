using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Neudesic.SmartOffice.RoomService.Services {
    /// <summary>
    /// Application service that handles the mediation between HTTP protocols / models and the underlying domain model.
    /// </summary>
    public interface IRoomService {        
        /// <summary>
        /// Finds a sample by id.
        /// </summary>
        /// <param name="id">The id of the sample.</param>
        /// <returns></returns>
        Task<Models.Room>    FindRoomById(string id);
        /// <summary>
        /// Returns the list of all rooms.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Models.Room>> GetAllRooms();
    }
}