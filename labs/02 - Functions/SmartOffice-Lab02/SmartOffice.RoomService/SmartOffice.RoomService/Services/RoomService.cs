using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using Neudesic.SmartOffice.RoomService.Domain;
using Neudesic.SmartOffice.RoomService.Models;
using Neudesic.SmartOffice.RoomService.Infrastructure;

namespace Neudesic.SmartOffice.RoomService.Services {
    /// <summary>
    /// Default implementation of <see cref="IRoomService"/>.
    /// </summary>
    internal class RoomService : IRoomService {

        private IRoomRepository m_roomRepository;
        private IMapper m_typeMappingService;        

        public RoomService(IRoomRepository sampleRepository, IMapper typeMappingService) {
            m_roomRepository = sampleRepository;
            m_typeMappingService = typeMappingService;            
        }        

        #region HTTP_GET_ENABLED
        public async Task<Models.Room> FindRoomById(string id) {

            var searchResult = await m_roomRepository.FindRoomById(id);
            var retVal = m_typeMappingService.Map<Models.Room>(searchResult);

            return retVal;
        }

        public async Task<IEnumerable<Models.Room>> GetAllRooms() {

            var searchResult = await m_roomRepository.GetAllRooms();
            var retVal = m_typeMappingService.Map<IEnumerable<Models.Room>>(searchResult);

            return retVal;
        }
        #endregion HTTP_GET_ENABLED


    }
}