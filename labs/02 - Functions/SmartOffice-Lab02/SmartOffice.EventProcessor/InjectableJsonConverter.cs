using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartOffice.EventProcessor
{
    /// <summary>
    /// Custom converter to convert objects to and from JSON
    /// </summary>    
    public abstract class InjectableJsonConverter : JsonConverter {
        /// <summary>
        /// Allows implementors to create instances of the specified type. 
        /// </summary>
        /// <param name="objectType">The type of the object to create.</param>
        /// <param name="jsonObject">The underlying JSON object.</param>
        /// <returns></returns>
        protected abstract object CreateInstanceOfType(Type objectType, JObject jsonObject);
        /// <summary>
        /// Allows implementors to specify if the converter implementation is able to convert the requested type.
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public abstract override bool CanConvert(Type objectType);
        /// <summary>
        /// Indicates if the converter can serialize in addition to deserializing. Returns <c>true</c>.
        /// </summary>
        public override bool CanWrite {
            get { return true; }
        }

        /// <summary>
        /// Implementation of <see cref="JsonConverter.ReadJson(JsonReader, Type, object, JsonSerializer)"/>.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="objectType">The type of object.</param>
        /// <param name="existingValue">Not used. </param>
        /// <param name="serializer">The <see cref="JsonSerializer"/> instance.</param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            // load the json string
            var jsonObject = JObject.Load(reader);

            // instantiate the appropriate object based on the json string
            var target = CreateInstanceOfType(objectType, jsonObject);

            // populate the properties of the object
            serializer.Populate(jsonObject.CreateReader(), target);

            // return the object
            return target;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        protected virtual void EnrichDocument(JsonWriter writer, object value) {

        }
        /// <summary>
        /// Implementation of <see cref="JsonConverter.WriteJson(JsonWriter, object, JsonSerializer)"/>.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="value">The value being serialized.</param>
        /// <param name="serializer">The <see cref="JsonSerializer"/> instance.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {

            //https://stackoverflow.com/questions/29719509/json-net-throws-stackoverflowexception-when-using-jsonconvert
            JsonObjectContract contract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(value.GetType());

            writer.WriteStartObject();
            EnrichDocument(writer, value);
            foreach (var property in contract.Properties) {
                writer.WritePropertyName(property.PropertyName);
                serializer.Serialize(writer, property.ValueProvider.GetValue(value));   //KEY MODIFICATION - this is what allows it to preserve converters and operate as intended
            }
            writer.WriteEndObject();
        }
    }
}
