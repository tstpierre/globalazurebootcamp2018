using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Autofac;
using Autofac.Integration.WebApi;
using IdentityServer3.AccessTokenValidation;
using Neudesic.SmartOffice.RoomService.Controllers;
using Neudesic.SmartOffice.RoomService.Infrastructure;
using Neudesic.SmartOffice.RoomService.Infrastructure.Diagnostics;
using Neudesic.SmartOffice.RoomService.Infrastructure.Messaging;
using Neudesic.Elements.Web.Configuration;
using Neudesic.Elements.Web.Owin.Diagnostics;
using Owin;

namespace Neudesic.SmartOffice.RoomService.Tests.Integration {
    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TestFixtureStartup {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appBuilder"></param>
        public void Configuration(IAppBuilder appBuilder) {
            IContainer container;
            HttpConfiguration configuration = new HttpConfiguration();

            configuration.MessageHandlers.Add(new LanguageMessageHandler());
            configuration.Services.Replace(typeof(IExceptionHandler), new ContentNegotiatedExceptionHandler());
            configuration.Services.Add(typeof(IExceptionLogger), new NLogLogger());
            configuration.MapHttpAttributeRoutes();
            container = BuildContainer(configuration);
            configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            ConfigureOAuth(appBuilder);
            appBuilder.UseAutofacMiddleware(container);
            appBuilder.UseAutofacWebApi(configuration);
            appBuilder.UseWebApi(configuration);
            appBuilder.UseMiddlewareFromContainer<AppVersionHeaderOwinMiddleware>();
        }

        private IContainer BuildContainer(HttpConfiguration configuration) {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(configuration);
            ExecuteAutofacModules(builder);
            var container = builder.Build();
            return container;
        }
        private void ExecuteAutofacModules(ContainerBuilder builder) {
            List<Assembly> assemblies = new List<Assembly>();

            var testFolder = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            var assemblyFiles = testFolder.GetFiles("*.dll", SearchOption.TopDirectoryOnly);
            foreach (var assembly in assemblyFiles) {
                try {
                    var asm = Assembly.LoadFrom(assembly.FullName);
                    assemblies.Add(asm);
                    Debug.WriteLine(asm.FullName);
                }
                catch {
                    //ignore - native image
                }
            }
            builder.RegisterAssemblyModules(assemblies.ToArray());
            builder.RegisterApiControllers(Assembly.GetAssembly(typeof(RoomController)));
        }

        private void ConfigureOAuth(IAppBuilder appBuilder) {

            OAuthConfigurationSection section;

            section = (OAuthConfigurationSection)ConfigurationManager.GetSection("neudesic.elements.web/security/oAuth");

            appBuilder.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions {
                Authority = section.Authority,
                RequiredScopes = section.RequiredScopes.Split(' ')
            });

        }
    }
}
