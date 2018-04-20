using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Hosting;

namespace Neudesic.SmartOffice.RoomService.Infrastructure.Initialization
{
    /// <summary>
    /// For internal use only.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class AutofacInitializer
    {
        /// <summary>
        /// For internal use only.
        /// </summary>
        public static void Initialize()
        {
            var pluginFolder = new DirectoryInfo(HostingEnvironment.MapPath("~/bin"));
            var pluginAssemblies = pluginFolder.GetFiles("*.dll", SearchOption.TopDirectoryOnly);
            foreach (var pluginAssemblyFile in pluginAssemblies)
            {
                try {
                    var asm = Assembly.LoadFrom(pluginAssemblyFile.FullName);
                    BuildManager.AddReferencedAssembly(asm);
                } catch {
                    // ignore - native image
                }
            }
        }
    }
}