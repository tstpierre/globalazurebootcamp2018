using Autofac;
using Autofac.Integration.WebApi;
using IdentityServer3.AccessTokenValidation;
using Owin;
using Neudesic.SmartOffice.RoomService.Config;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Neudesic.SmartOffice.RoomService.Infrastructure;
using Neudesic.Elements.Web.Configuration;
using Neudesic.Elements.Web.Owin.Diagnostics;
using System.Net;
using Newtonsoft.Json.Serialization;
using Neudesic.SmartOffice.RoomService.Infrastructure.Diagnostics;
using Neudesic.SmartOffice.RoomService.Infrastructure.Messaging;
using Neudesic.Elements.IO.Serialization;

namespace Neudesic.SmartOffice.RoomService
{
    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appBuilder"></param>
        public void Configuration(IAppBuilder appBuilder) {

            ServicePointManager.DefaultConnectionLimit = 100; // per best practice

            IContainer container;
            HttpConfiguration configuration = new HttpConfiguration();

            configuration.MapHttpAttributeRoutes();            
            configuration.Services.Replace(typeof(IExceptionHandler), new ContentNegotiatedExceptionHandler());
            configuration.Services.Add(typeof(IExceptionLogger), new NLogLogger());
            configuration.MessageHandlers.Add(new LanguageMessageHandler());
            InitializeJsonSerializerSettings(configuration);

            container = BuildContainer(configuration);

            SwaggerConfig.Register(configuration);
            configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);            
            appBuilder.UseAutofacMiddleware(container);
            appBuilder.UseAutofacWebApi(configuration);
            appBuilder.UseWebApi(configuration);

        }


        private void InitializeJsonSerializerSettings(HttpConfiguration config) {
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto;
            config.Formatters.JsonFormatter.SerializerSettings.SerializationBinder = new PolymorphicSerializationBinder( new System.Collections.Generic.Dictionary<string, System.Type>());
        }


        private IContainer BuildContainer(HttpConfiguration configuration) {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(configuration);

            ExecuteAutofacModules(builder);

            var container = builder.Build();

            return container;
        }
        private void ExecuteAutofacModules(ContainerBuilder builder) {

            var assemblies = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray();
            builder.RegisterAssemblyModules(assemblies);
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
        }

    }
}