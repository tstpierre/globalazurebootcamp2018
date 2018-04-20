using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neudesic.SmartOffice.RoomService.Models;

namespace Neudesic.SmartOffice.RoomService.Tests.Integration.Controllers {
    [TestClass]
    public class SampleControllerTestFixture {

        private const string CREATED_RESOURCE_ID_CONTEXT_PROPERTY_NAME = "resources-created";
        private const string TEST_SERVER_CONTEXT_PROPERTY_NAME = "servercontext";
        private const string AUTHORIZATION_HEADER_NAME = "authorization";

        // GET Tests
        [TestMethod]
        public async Task Given_Get_Called_When_Request_Is_Valid_And_Resource_Is_Present_Then_OK_Is_Returned() {

            // arrange            
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;            
            
            // act            
            SampleWriteModel model = new SampleWriteModel {
                Metric = "testmetric",
                Value = 1
            };

            var postResponse = await CreateResource(model);
            var createdModel = await postResponse.Content.ReadAsAsync<SampleReadModel>();
            RegisterCreatedResource(createdModel.Id);
            var getResponse = await GetResource(createdModel.Id);
            var actualModel = await getResponse.Content.ReadAsAsync<SampleReadModel>();

            // assert
            Assert.AreEqual(expectedStatusCode, getResponse.StatusCode);
            Assert.AreEqual(createdModel.Id, actualModel.Id);


        }

        [TestMethod]
        public async Task Given_Get_Called_When_Request_Is_Valid_And_Resource_Is_Not_Present_Then_NotFound_Is_Returned() {

            // arrange
            string              expectedInvalidId = Guid.NewGuid().ToString();            

            // act
            var getResponse = await GetResource(expectedInvalidId);

            // assert
            Assert.AreEqual(HttpStatusCode.NotFound, getResponse.StatusCode);            
        }



        // PUT tests
        [TestMethod]
        public async Task Given_Put_Called_When_Model_Is_Valid_Then_OK_Is_Returned() {
            
            // arrange                    
            SampleWriteModel model = new SampleWriteModel {
                Metric = "testmetric",
                Value = 1
            };

            var postResponse = await CreateResource(model);
            var createdModel = await postResponse.Content.ReadAsAsync<SampleReadModel>();
            RegisterCreatedResource(createdModel.Id);

            // act    
            var putResponse = await UpdateResource(createdModel.Id, new SampleWriteModel { Metric = "integration-test", Value = 42 });
            var updatedModel = await putResponse.Content.ReadAsAsync<SampleReadModel>();

            // assert
            Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);
            Assert.AreEqual(createdModel.Id, updatedModel.Id);
            Assert.AreEqual(42, updatedModel.Value);
        }

        [TestMethod]
        public async Task Given_Put_Called_When_Model_Is_Invalid_Then_BadRequest_Is_Returned() {
            // arrange                    
            
            // act    
            var putResponse = await UpdateResource("123", new SampleWriteModel { Metric = null , Value = 42 });
            
            // assert
            Assert.AreEqual(HttpStatusCode.BadRequest, putResponse.StatusCode);            
        }

        // POST tests
        [TestMethod]
        public async Task Given_Post_Called_When_Model_Is_Valid_Then_Created_Is_Returned() {

            // arrange
            SampleWriteModel model = new SampleWriteModel {
                Metric = "testmetric",
                Value = 1
            };

            // act
            var postResponse = await CreateResource(model);
            var createdModel = await postResponse.Content.ReadAsAsync<SampleReadModel>();
            RegisterCreatedResource(createdModel.Id);            

            // assert
            Assert.AreEqual(HttpStatusCode.Created, postResponse.StatusCode);
            
        }

        [TestMethod]
        public async Task Given_Post_Called_When_Model_Is_Invalid_Then_BadRequest_Is_Returned() {

            // arrange
            SampleWriteModel model = new SampleWriteModel {
                Metric = null,
                Value = 1
            };

            // act
            var postResponse = await CreateResource(model);
            var createdModel = await postResponse.Content.ReadAsAsync<SampleReadModel>();            

            // assert
            Assert.AreEqual(HttpStatusCode.BadRequest, postResponse.StatusCode);
        }

        // DELETE tests
        [TestMethod]
        public async Task Given_Delete_Called_When_Request_Is_Valid_Then_NoContent_Is_Returned() {

            // arrange
            var postResponse = await CreateResource(new SampleWriteModel { Metric = "integration-test", Value = 1 });
            var createdModel = await postResponse.Content.ReadAsAsync<SampleReadModel>();

            // act
            var deleteResponse = await DeleteResource(createdModel.Id);

            // assert
            Assert.AreEqual(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        }
        #region Test Properties
        public TestContext TestContext {
            get;set;
        }

        public TestServer TestServer {
            get { return (TestServer)TestContext.Properties[TEST_SERVER_CONTEXT_PROPERTY_NAME]; }
        }
        #endregion

        #region Cleanup & Initialization
        [TestInitialize]
        public async Task OnBeforeTest() {
            TestContext.Properties.Add(TEST_SERVER_CONTEXT_PROPERTY_NAME, TestServer.Create<TestFixtureStartup>());
        }

        [TestCleanup]
        public async Task OnAfterTest() {
            
            try {
                
                if (TestContext.Properties.Contains(CREATED_RESOURCE_ID_CONTEXT_PROPERTY_NAME)) {
                    foreach (var id in ((List<string>)TestContext.Properties[CREATED_RESOURCE_ID_CONTEXT_PROPERTY_NAME])) {
                        await DeleteResource(id);
                    }
                }

                TestServer.Dispose();

            } catch (Exception e) {
                TestContext.WriteLine($"An error occurred while removing resources created by the test:\n{e.Message}");
            }
        }
        #endregion
        
        #region Standard Helpers
        private async Task<HttpResponseMessage> GetResource(string id) {

            string authToken = await AuthenticationHelper.GetKnownAuthenticationToken();

            var getResponse = await TestServer.CreateRequest($"sample/{id}")
                .AddHeader(AUTHORIZATION_HEADER_NAME, $"Bearer {authToken}")
                .GetAsync();

            return getResponse;            
        }

        private async Task<HttpResponseMessage> DeleteResource(string id) {

            string authToken = await AuthenticationHelper.GetKnownAuthenticationToken();

            var deleteResponse = await TestServer.CreateRequest($"sample/{id}")
                .AddHeader(AUTHORIZATION_HEADER_NAME, $"Bearer {authToken}")
                .And(message => {
                    message.Method = HttpMethod.Delete;
                }).SendAsync("DELETE");

            return deleteResponse;
            
        }

        private async Task<HttpResponseMessage> CreateResource(SampleWriteModel model) {

            string authToken = await AuthenticationHelper.GetKnownAuthenticationToken();

            var postResponse = await TestServer.CreateRequest($"sample/")
            .AddHeader(AUTHORIZATION_HEADER_NAME, $"Bearer {authToken}")
            .And(message => {
                message.Content = new ObjectContent(typeof(SampleWriteModel), model, new JsonMediaTypeFormatter());
            }).PostAsync();                

            return postResponse;
            
        }

        private async Task<HttpResponseMessage> UpdateResource(string id, SampleWriteModel model) {

            string authToken = await AuthenticationHelper.GetKnownAuthenticationToken();

            var postResponse = await TestServer.CreateRequest($"sample/{id}")
            .AddHeader(AUTHORIZATION_HEADER_NAME, $"Bearer {authToken}")
            .And(message => {
                message.Content = new ObjectContent(typeof(SampleWriteModel), model, new JsonMediaTypeFormatter());
            }).SendAsync("PUT");

            return postResponse;

        }

        private void RegisterCreatedResource(string id) {
            if (!TestContext.Properties.Contains(CREATED_RESOURCE_ID_CONTEXT_PROPERTY_NAME)) {
                TestContext.Properties.Add(CREATED_RESOURCE_ID_CONTEXT_PROPERTY_NAME, new List<string>());
            }

            ((List<string>)TestContext.Properties[CREATED_RESOURCE_ID_CONTEXT_PROPERTY_NAME]).Add(id);
        }
        #endregion
    }
}
