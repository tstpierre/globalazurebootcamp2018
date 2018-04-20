using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Neudesic.SmartOffice.RoomService.Models;
using Neudesic.SmartOffice.RoomService.Services;
using Neudesic.Elements.Validation;

namespace Neudesic.SmartOffice.RoomService.Controllers {
    /// <summary>
    /// Sample controller.
    /// </summary>    
    [RoutePrefix("room")]    
    public class RoomController 
        : ApiController {

        private IRoomService m_roomService;        
        private IValidationService m_validationService;

        /// <summary>
        /// Creates a new instance of <see cref="RoomController"/>.
        /// </summary>
        /// <param name="sampleService">The repository instance.</param>        
        /// <param name="validationService"></param>
        public RoomController(IRoomService sampleService,IValidationService validationService) {
            m_roomService = sampleService;            
            m_validationService = validationService;
        }


        #region ENABLE_HTTP_GET
        /// <summary>
        /// Retrieves a sample by query parameters.
        /// </summary>
        /// <param name="id">The id of the sample.</param>
        /// <returns>Returns an isntasnce of <see cref="Room"/></returns>
        /// <response code="200">The request completed successfully.</response>
        /// <response code="400">The request is malformed or otherwise invalid.</response>
        /// <response code="401">The caller is not correctly authenticated.</response>
        /// <response code="403">The caller is not authenticated, but not authorized to perform the action or access the requested resource.</response>
        /// <response code="500">An internal processing error occurred.</response>        
        [HttpGet]
        [Route("{id}")]        
        public async Task<IHttpActionResult> Get(string id) {
            
            if(string.IsNullOrEmpty(id)) {
                return BadRequest();
            }

            var retVal = await m_roomService.FindRoomById(id);

            if (retVal != null) {
                return Ok(retVal);
            }else {
                return NotFound();
            }
        }

        /// <summary>
        /// Retrieves a sample by query parameters.
        /// </summary>
        /// <param name="id">The id of the sample.</param>
        /// <returns>Returns an isntasnce of <see cref="Room"/></returns>
        /// <response code="200">The request completed successfully.</response>
        /// <response code="400">The request is malformed or otherwise invalid.</response>
        /// <response code="401">The caller is not correctly authenticated.</response>
        /// <response code="403">The caller is not authenticated, but not authorized to perform the action or access the requested resource.</response>
        /// <response code="500">An internal processing error occurred.</response>        
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Get() {


            var retVal = await m_roomService.GetAllRooms();

            if (retVal != null) {
                return Ok(retVal);
            }
            else {
                return NotFound();
            }
        }

        #endregion ENABLE_HTTP_GET



    }
}
