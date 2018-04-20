using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Neudesic.SmartOffice.RoomService.Infrastructure {
    /// <summary>
    /// Well known claims for the Isagenix token.
    /// </summary>
    internal static class WellKnownClaims {
        /// <summary>
        /// The safe user id (the guid) to use when referencing the user for any data storage.
        /// </summary>
        public static readonly string UserId = "isa_userid";
    }
}