using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Neudesic.SmartOffice.RoomService.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Neudesic.SmartOffice.RoomService {

    [TestClass]
    public class BuildVerificationTestsTestFixture {

        private const string CREATED_RESOURCE_ID_CONTEXT_PROPERTY_NAME = "resources-created";
        private const string HTTP_CLIENT_CONTEXT_PROPERTY_NAME = "servercontext";
        private const string AUTHORIZATION_HEADER_NAME = "authorization";
        private const string BASE_URI_CONFIG_SETTING_NAME = "baseUri";
        private string _baseUri = null;
        private static Lazy<HttpClient> _client = new Lazy<HttpClient>(CreateHandler, LazyThreadSafetyMode.ExecutionAndPublication);

        private static HttpClient CreateHandler() {
            HttpClientHandler noRedirectHandler = new HttpClientHandler { AllowAutoRedirect = false };
            return new HttpClient(noRedirectHandler);
        }

        [TestMethod]
        public async Task Given_All_Basic_Test_Cases_Pass_When_Executed_Then_Build_Is_Verified() {

            // arrange

            string ownerId = (string)TestContext.Properties["known_good_user_system_id"];

            SampleWriteModel model = new SampleWriteModel { Metric = "integration-test-metric", Value = 42 };

            // create a new resource and verify that it was created
            var postResponse = await CreateResource(ownerId, model);
            var createdModel = await postResponse.Content.ReadAsAsync<SampleReadModel>();
            Assert.AreEqual(HttpStatusCode.Created, postResponse.StatusCode);
            Assert.IsNotNull(createdModel);

            this.RegisterCreatedResource(createdModel.Id);

            // get the created resource and verify that it provides a redirect
            var getResponse = await GetResource(ownerId, createdModel.Id);
            Assert.AreEqual(HttpStatusCode.Redirect, getResponse.StatusCode);

            // update the resource and verify that it was updated
            var putResponse = await UpdateResource(ownerId, createdModel.Id, new SampleWriteModel { Metric = "integration-test-metric", Value = 43 });
            Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);

            // delete the resource and verify that it was deleted
            var deleteResponse = await DeleteResource(ownerId, createdModel.Id);

            // assert
            Assert.AreEqual(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        }
        #region Test Properties
        public TestContext TestContext {
            get; set;
        }

        public HttpClient Client {
            get { return _client.Value; }
        }
        #endregion

        #region Cleanup & Initialization
        [TestInitialize]
        public async Task OnBeforeTest() {

            _baseUri = (string)TestContext.Properties[BASE_URI_CONFIG_SETTING_NAME];
        }


        [TestCleanup]
        public async Task OnAfterTest() {

            try {

                string ownerId = (string)TestContext.Properties["known_good_user_system_id"];

                if (TestContext.Properties.Contains(CREATED_RESOURCE_ID_CONTEXT_PROPERTY_NAME)) {
                    foreach (var id in ((List<string>)TestContext.Properties[CREATED_RESOURCE_ID_CONTEXT_PROPERTY_NAME])) {
                        await DeleteResource(ownerId, id);
                    }
                }

                ((HttpClient)TestContext.Properties[HTTP_CLIENT_CONTEXT_PROPERTY_NAME]).Dispose();

            }
            catch (Exception e) {
                TestContext.WriteLine($"An error occurred while removing resources created by the test:\n{e.Message}");
            }
        }
        #endregion

        #region Standard Helpers
        private async Task<HttpResponseMessage> GetResource(string ownerId, string id) {

            string authToken = await AuthenticationHelper.GetKnownAuthenticationToken(TestContext);

            var message = new HttpRequestMessage(HttpMethod.Get, $"{_baseUri}/{ownerId}/{id}");

            message.Headers.Add(AUTHORIZATION_HEADER_NAME, $"Bearer {authToken}");

            var getResponse = await Client.SendAsync(message);

            return getResponse;
        }

        private async Task<HttpResponseMessage> DeleteResource(string ownerId, string id) {

            string authToken = await AuthenticationHelper.GetKnownAuthenticationToken(TestContext);

            var message = new HttpRequestMessage(HttpMethod.Delete, $"{_baseUri}/{ownerId}/{id}");

            message.Headers.Add(AUTHORIZATION_HEADER_NAME, $"Bearer {authToken}");

            var deleteResponse = await Client.SendAsync(message);

            return deleteResponse;
        }

        private async Task<HttpResponseMessage> CreateResource(string ownerId, SampleWriteModel model, byte[] data = null) {

            string authToken = await AuthenticationHelper.GetKnownAuthenticationToken(TestContext);

            var message = new HttpRequestMessage(HttpMethod.Post, $"{_baseUri}/{ownerId}");

            message.Headers.Add(AUTHORIZATION_HEADER_NAME, $"Bearer {authToken}");
            message.Content = new ObjectContent(typeof(SampleWriteModel), model, new JsonMediaTypeFormatter());

            var postResponse = await Client.SendAsync(message);

            return postResponse;
        }

        private async Task<HttpResponseMessage> UpdateResource(string ownerId, string id, SampleWriteModel model) {

            string authToken = await AuthenticationHelper.GetKnownAuthenticationToken(TestContext);

            var message = new HttpRequestMessage(HttpMethod.Put, $"{_baseUri}/{ownerId}/{id}");

            message.Headers.Add(AUTHORIZATION_HEADER_NAME, $"Bearer {authToken}");
            message.Content = new ObjectContent(typeof(SampleWriteModel), model, new JsonMediaTypeFormatter());

            var putResponse = await Client.SendAsync(message);

            return putResponse;

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
