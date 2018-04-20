using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using SmartOffice.EventProcessor.Models;
using System;
using System.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace SmartOffice.EventProcessor
{
    public static class EventReader
    {
        private static DocumentClient _client;
        private static Uri _collectionUri;
        private static string _collectionName;
        private static string _databaseName;
        private static Dictionary<string, RoomState> _stateMap = new Dictionary<string, RoomState> {
            {"occupied", RoomState.Occupied },
            {"engaged", RoomState.Occupied },            
            {"locked", RoomState.Occupied },
            {"vacant", RoomState.Unoccupied },
        };

        //NOTE: doing this much in a static constructor is sub-optimal. it can lead to some VERY difficult to track errors 
        //      as a result of the type not being created correctly (i.e. plan on using WinDbg).
        static EventReader() {
            string serviceUri = Environment.GetEnvironmentVariable("CosmosDbEndpointUri", EnvironmentVariableTarget.Process);
            string accessKey = Environment.GetEnvironmentVariable("CosmosDbAccessKey", EnvironmentVariableTarget.Process);
            string databaseName = Environment.GetEnvironmentVariable("CosmosDbDatabaseName", EnvironmentVariableTarget.Process);
            string collectionName = Environment.GetEnvironmentVariable("CosmosDbCollectionName", EnvironmentVariableTarget.Process);

            _client = new DocumentClient(new Uri(serviceUri), accessKey, new Newtonsoft.Json.JsonSerializerSettings {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            _collectionUri = UriFactory.CreateDocumentCollectionUri(databaseName, collectionName);
            _databaseName = databaseName;
            _collectionName = collectionName;

        }

        [FunctionName("EventReader")]
        public static async Task Run([EventHubTrigger("deviceevents", Connection = "EventHubConnectionString")]EventData[] myEventHubMessages, TraceWriter log)
        {
            log.Info($"C# Event Hub trigger function processed a message: {myEventHubMessages.Length}");

            foreach (var message in myEventHubMessages) {

                using (StreamReader reader = new StreamReader(message.GetBodyStream())) {

                    string body = await reader.ReadToEndAsync();

                    dynamic bodyAsJsonObject = JObject.Parse(body);

                    string deviceId = bodyAsJsonObject.deviceId;
                    string deviceName = bodyAsJsonObject.deviceName;
                    RoomState mappedState = RoomState.Unknown;

                    _stateMap.TryGetValue(bodyAsJsonObject.value.ToString(), out mappedState);

                    var room = await GetOrCreateRoom(deviceId, deviceName);

                    await SetRoomState(room, mappedState, message.EnqueuedTimeUtc);
                }
            }
        }

        private static async Task<Room> GetOrCreateRoom(string deviceId, string deviceName) {

            var room = await GetRoomById(deviceId);

            if( room == null) {
                room = await CreateRoom(deviceId, deviceName);
            }

            return room;
        }

        private static async Task<Room> CreateRoom(string deviceId, string deviceName) {

            Room room = new Room {
                Id = deviceId,
                DisplayName = deviceName,
                Description = string.Empty,
            };

            var i = 0;

            var result = await _client.CreateDocumentAsync(_collectionUri, room);

            return room;
        }

        private static async Task<Room> GetRoomById(string roomId) {
            var documentUri = UriFactory.CreateDocumentUri(_databaseName, _collectionName, roomId);

            try {
                var result = await _client.ReadDocumentAsync<Room>(documentUri);
                return result.Document;
            }
            catch (DocumentClientException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound) { 
                // ignore, this is ok
                return null;
            }            
        }

        private static async Task<Room> SetRoomState(Room room,RoomState newState, DateTime stateChangeTime) {
            
            var documentUri = UriFactory.CreateDocumentUri(_databaseName, _collectionName, room.Id);

            room.State = newState;
            room.StateChangeTimestamp = stateChangeTime;

            await _client.ReplaceDocumentAsync(documentUri, room);

            return room;            

        }
    }
}
