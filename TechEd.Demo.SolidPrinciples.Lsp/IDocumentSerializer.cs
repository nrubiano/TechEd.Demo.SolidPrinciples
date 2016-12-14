using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TechEd.Demo.SolidPrinciples.Lsp
{
    public interface IDocumentSerializer
    {
        string Serialize(Document document);
    }

    public class JsonDocumentSerializer : IDocumentSerializer
    {
        public string Serialize(Document document)
        {
            return JsonConvert.SerializeObject(document);
        }
    }

    public class CamelCaseJsonSerializer : IDocumentSerializer
    {
        public string Serialize(Document document)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return JsonConvert.SerializeObject(document, settings);
        }
    }
}