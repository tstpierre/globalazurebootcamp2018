using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neudesic.SmartOffice.RoomService.Domain {

    /// <summary>
    /// A repository that operates against instances of Sample.
    /// </summary>
    public interface IRoomRepository {
        #region ENABLE_HTTP_GET
        /// <summary>
        /// Retrieves a sample by its id.
        /// </summary>
        /// <param name="id">The id of the item.</param>
        /// <returns>Returns an instance of <see cref="Room"/> corresponding to the id.</returns>
        Task<Room>    FindRoomById(string id);
        /// <summary>
        /// Gets the list of all rooms.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Room>> GetAllRooms();
        #endregion ENABLE_HTTP_GET
    }
}
