using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.ExceptionHandling;
using NLog;

namespace Neudesic.SmartOffice.RoomService.Infrastructure.Diagnostics {
    #pragma warning disable 1998
    /// <summary>
    /// Default logger that utilizes NLog to log errors. 
    /// </summary>
    internal class NLogLogger : IExceptionLogger {

        private Logger m_logger;
        private string m_correlationHeaderName;        
        private static class CustomPropertyNames {
            public static readonly string RequestUri = "uri";
            public static readonly string CorrelationId = "correlationId";
            public static readonly string User = "user";
            public static readonly string IpAdress = "ipAddress";
        }
       
        /// <summary>
        /// Creates a new new instance of <see cref="NLogLogger"/>.
        /// </summary>
        public NLogLogger() {
            m_logger = LogManager.GetCurrentClassLogger();
            m_correlationHeaderName = ConfigurationManager.AppSettings["diagnostics.correlation.headerName"];
        }

        /// <summary>
        /// Implementation of <see cref="IExceptionLogger.LogAsync(ExceptionLoggerContext, CancellationToken)"/>.
        /// </summary>
        /// <param name="context">The exception context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>        
        public async Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken) {
            if (context.CatchBlock.IsTopLevel) {
                LogEventInfo info = new LogEventInfo { Exception = context.Exception, Level = LogLevel.Error };                
                info.Properties[CustomPropertyNames.RequestUri] = $"{context.Request.Method} {context.Request.RequestUri}";
                info.Properties[CustomPropertyNames.User] = ((ClaimsIdentity)context.RequestContext.Principal.Identity).Claims.FirstOrDefault(c => c.Type == WellKnownClaims.UserId)?.Value;
                if (context.Request.Headers.TryGetValues(m_correlationHeaderName, out IEnumerable<string> headerValues)) {
                    info.Properties[CustomPropertyNames.CorrelationId] = headerValues.FirstOrDefault();
                }
                m_logger.Log(info);
            }
            return;
        }
    }
    #pragma warning restore 1998
}