using Autofac;

using Neudesic.Elements.Validation;
using System;
using System.Diagnostics.CodeAnalysis;
using Neudesic.Elements.IO.Serialization;
using Neudesic.SmartOffice.RoomService.Domain;
using Neudesic.SmartOffice.RoomService.Services;
using AutoMapper;
using Microsoft.Owin;
using Neudesic.Elements.Configuration;
using Neudesic.Elements.Web.Owin.Configuration;
using Neudesic.Elements;
using Neudesic.Elements.Data.Extensions.Azure.Cosmos;
using Newtonsoft.Json;
using System.Web.Http;

namespace Neudesic.SmartOffice.RoomService.Infrastructure.Initialization {
    /// <summary>
    /// Core DI container registration module. Registers domain implementation classes,
    /// infrastructure components, etc. 
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class CoreServicesRegistationModule
        : Module {
        private const string PLATFORM_DB_CONNECTION_NAME = "qualia-platform";

        protected override void Load(ContainerBuilder builder) {
            RegisterInfrastructureServices(builder);
            RegisterNeudesicElementsServices(builder);
            RegisterCustomOwinMiddleware(builder);
            RegisterDomainImplementationServices(builder);

            base.Load(builder);
        }

        private void RegisterDomainImplementationServices(ContainerBuilder builder) {



            #region COSMOSDB
            builder.Register(c => new Services.RoomService(c.Resolve<IRoomRepository>(), c.Resolve<IMapper>()))
                .As<IRoomService>()
                .SingleInstance();
            builder.Register(c => new CosmosDbRoomRepository(c.Resolve<IParameterizedConfigurationAdapter<CosmosDbConfiguration, string>>(), PLATFORM_DB_CONNECTION_NAME, c.Resolve<HttpConfiguration>().Formatters.JsonFormatter.SerializerSettings))
                .As<IRoomRepository>()
                .SingleInstance();
            #endregion COSMOSDB


        }

        private void RegisterCustomOwinMiddleware(ContainerBuilder builder) {

            // register app version middleware and its configuration
            builder.Register(c => new OwinSystemConfigurationAdapter())
                .As<IConfigurationAdapter<OwinConfiguration>>()
                .SingleInstance();

            // optionally allow app version header support
            builder.RegisterType<Neudesic.Elements.Web.Owin.Diagnostics.AppVersionHeaderOwinMiddleware>();

        }

        private void RegisterInfrastructureServices(ContainerBuilder builder) {
            #region COSMOSDB
            builder.Register(c => new SystemCosmosDbConfigurationAdapter("neudesic.elements.data/cosmosDb"))
                .AsImplementedInterfaces()
                .SingleInstance();
            #endregion COSMOSDB            

            builder.Register(c => new AssemblyInfoApplicationVersionRetrievalService(System.Reflection.Assembly.GetExecutingAssembly(), "{0}.{1}"))
                .As<IApplicationVersionRetrievalService>()
                .SingleInstance();
        }

        private void RegisterNeudesicElementsServices(ContainerBuilder builder) {

            // register validation services
            builder.Register(c => new DataAnnotationsValidationService())
                .As<IValidationService>()
                .SingleInstance();

            // register serialization services
            builder.Register(c => new SerializationService())
               .As<ISerializationService>()
               .SingleInstance();
        }
    }
}