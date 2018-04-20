using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Web;
using Microsoft.Owin;
using Neudesic.SmartOffice.RoomService;
using Neudesic.SmartOffice.RoomService.Infrastructure;
using Neudesic.SmartOffice.RoomService.Infrastructure.Initialization;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("SmartOffice.RoomService")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Isagenix")]
[assembly: AssemblyProduct("Qualia SmartOffice.RoomService")]
[assembly: AssemblyCopyright("Copyright ©  2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("E6E69354-F322-429A-AA48-F163CE200B84")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: OwinStartup(typeof(Startup))]
[assembly: PreApplicationStartMethod(typeof(AutofacInitializer), "Initialize")]
[assembly: InternalsVisibleTo("SmartOffice.RoomService.Tests.Unit")]
[assembly: InternalsVisibleTo("SmartOffice.RoomService.Tests.Integration")]
[assembly: InternalsVisibleTo("SmartOffice.RoomService.Tests.Unit")]
[assembly: InternalsVisibleTo("SmartOffice.RoomService.Tests.Verification")]