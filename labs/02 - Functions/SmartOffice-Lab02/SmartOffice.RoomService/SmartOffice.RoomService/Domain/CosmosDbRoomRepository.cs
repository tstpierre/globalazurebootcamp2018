using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Neudesic.Elements.Configuration;
using Neudesic.Elements.Data.Extensions.Azure.Cosmos;
using Neudesic.Elements.IO.Serialization;
using Neudesic.SmartOffice.RoomService.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Neudesic.SmartOffice.RoomService.Domain { 

    /// <summary>
    /// Implementation of <see cref="IRoomRepository"/> that stores content within Azure Blob Storage.
    /// </summary>    
    internal class CosmosDbRoomRepository
        : IRoomRepository {

        private const string CONTENT_TYPE_FORMAT_STRING = "application/vnd.room+json";

        private const string ID_SEPARATOR = "_";
        private CosmosDbConfiguration m_configuration;
        private DocumentClient m_client;
        private Uri m_collectionUri;


        /// <summary>
        /// Creates a new instance of <see cref="CosmosDbRoomRepository"/>.
        /// </summary>
        /// <param name="configurationAdapter">The configuration adapter instance to use to retrieve configuration data.</param>
        /// <param name="connectionName">The name of the connection to utilize.</param>
        public CosmosDbRoomRepository(IParameterizedConfigurationAdapter<CosmosDbConfiguration, string> configurationAdapter, string connectionName) : this(configurationAdapter, connectionName, null) {

        }

        /// <summary>
        /// Creates a new instance of <see cref="CosmosDbRoomRepository"/>.
        /// </summary>
        /// <param name="configurationAdapter">The configuration adapter instance to use to retrieve configuration data.</param>
        /// <param name="connectionName">The name of the connection to utilize.</param>
        /// <param name="serializerSettings">The serializer settings to use when communicating with Cosmos.</param>
        public CosmosDbRoomRepository(IParameterizedConfigurationAdapter<CosmosDbConfiguration, string> configurationAdapter, string connectionName, JsonSerializerSettings serializerSettings) {
            if (serializerSettings == null) {
                serializerSettings = CreateDefaultSerializationSettings();
            }

            m_configuration = configurationAdapter.Create(connectionName);

            InitializeDocumentClient(m_configuration, serializerSettings);
        }

        private void InitializeDocumentClient(CosmosDbConfiguration configuration, JsonSerializerSettings serializerSettings) {
            

            m_client = (DocumentClient)CosmosDbClientFactory.CreateDocumentClient(configuration, serializerSettings);

            Task.WaitAll(m_client.OpenAsync());

            m_collectionUri = UriFactory.CreateDocumentCollectionUri(configuration.DatabaseName, configuration.CollectionName);
        }

        public async Task<Room> FindRoomById(string id) {
            var documentUri = UriFactory.CreateDocumentUri(m_configuration.DatabaseName, m_configuration.CollectionName, id);            
            try {
                var result = await m_client.ReadDocumentAsync<Room>(documentUri);
                return result.Document;
            }
            // document client throws a hard exception if you try to load a non-existent document. only handle this particular case, otherwise
            // allow the exception to bubble up the callstack.
            catch (DocumentClientException e) when (e.StatusCode == HttpStatusCode.NotFound) {
                return null;
            }
        }

        
        public async Task<IEnumerable<Room>> GetAllRooms() {

            FeedOptions feedOptions = new FeedOptions { MaxItemCount = -1};

            string queryText = $"SELECT * FROM c WHERE c.contentType=\"{CONTENT_TYPE_FORMAT_STRING}\"";

            var query = m_client.CreateDocumentQuery(m_collectionUri, queryText, feedOptions).AsDocumentQuery();

            var queryResult = await query.ExecuteNextAsync<Room>();

            return queryResult.ToList();
        }

      
        #region Utility Methods

        private JsonSerializerSettings CreateDefaultSerializationSettings() {
            JsonSerializerSettings settings = new JsonSerializerSettings {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None,
                DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc,
            };            
            settings.Converters.Add(new CoreMetadataJsonConverter(typeof(Room), CONTENT_TYPE_FORMAT_STRING, "1.0.0"));
            return settings;
        }
        #endregion


    }
}
