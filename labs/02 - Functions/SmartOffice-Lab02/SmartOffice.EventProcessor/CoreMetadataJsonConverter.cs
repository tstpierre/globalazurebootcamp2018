using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartOffice.EventProcessor
{
    /// <summary>
    /// Implementation of <see cref="InjectableJsonConverter"/> that augments documents with content type and schema version information.
    /// </summary>
    public class CoreMetadataJsonConverter : InjectableJsonConverter {
        private Type _type;
        private string _contentTypeName;
        private string _schema;

        /// <summary>
        /// Creates a new instance of <see cref="CoreMetadataJsonConverter"/>.
        /// </summary>
        /// <param name="type">The type to convert for.</param>
        /// <param name="contentTypeName">The root content type name.</param>
        /// <param name="schema">The schema reference for the document.</param>
        public CoreMetadataJsonConverter(Type type, string contentTypeName, string schema) {
            _type = type;
            _contentTypeName = contentTypeName;
            _schema = schema;
        }
        /// <summary>
        /// Implementation of <see cref="InjectableJsonConverter.CanConvert(Type)"/>.
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType) {
            return objectType.Equals(_type);
        }
        /// <summary>
        /// Implementation of <see cref="InjectableJsonConverter.InjectableJsonConverter"/>.
        /// </summary>
        /// <param name="objectType">The type of the object.</param>
        /// <param name="jsonObject">The json object that was parased.</param>
        /// <returns></returns>
        protected override object CreateInstanceOfType(Type objectType, JObject jsonObject) {
            return Activator.CreateInstance(objectType);
        }
        /// <summary>
        /// Implementation of <see cref="InjectableJsonConverter.EnrichDocument(JsonWriter, object)"/>.
        /// </summary>
        /// <param name="writer">The document writer.</param>
        /// <param name="value">The value being written. </param>
        protected override void EnrichDocument(JsonWriter writer, object value) {
            writer.WritePropertyName("contentType");
            writer.WriteValue(_contentTypeName);
            writer.WritePropertyName("schema");
            writer.WriteValue(_schema);
        }
    }
}
