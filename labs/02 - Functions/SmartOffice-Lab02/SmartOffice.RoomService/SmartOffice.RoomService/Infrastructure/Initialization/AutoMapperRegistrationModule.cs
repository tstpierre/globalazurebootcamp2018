using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Autofac.Core;
using AutoMapper;

namespace Neudesic.SmartOffice.RoomService.Infrastructure.Initialization {
    /// <summary>
    /// Registration module for initializing Automapper and registering the mapping service with the container.
    /// </summary>
    public class AutoMapperRegistrationModule
        : Module {
        /// <summary>
        /// Implementation of <see cref="Module.Load(ContainerBuilder)"/>.
        /// </summary>
        /// <param name="builder">The builder.</param>
        protected override void Load(ContainerBuilder builder) {

            IMapper mapper = CreateMapper();

            builder.Register(c => mapper).As<IMapper>();

        }
        private static IMapper CreateMapper() {

            MapperConfiguration cfg = new MapperConfiguration(c => {
                c.CreateMap<Domain.Room, Models.Room>();
                c.CreateMap<Domain.RoomState, Models.RoomState>();
                c.CreateMap<Domain.UtilizationPeriod, Models.UtilizationPeriod>();
            });

            IMapper mapper = new Mapper(cfg);

            return mapper;
        }
    }
}