using System.Diagnostics.CodeAnalysis;
using System.Web.Http;

namespace Neudesic.SmartOffice.RoomService.Config
{

    [ExcludeFromCodeCoverage]
    internal class WebApiConfig
    {
        public static void Register(HttpConfiguration config){
            config.MapHttpAttributeRoutes();
        }
    }
}