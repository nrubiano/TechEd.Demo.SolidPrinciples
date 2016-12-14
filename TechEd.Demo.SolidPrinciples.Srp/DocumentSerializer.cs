using Newtonsoft.Json;

namespace TechEd.Demo.SolidPrinciples.Srp
{
    public class DocumentSerializer
    {
        public string Serialize(Document document)
        {
            return JsonConvert.SerializeObject(document);
        }
    }
}