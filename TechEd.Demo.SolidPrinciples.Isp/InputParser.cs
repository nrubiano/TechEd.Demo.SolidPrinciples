using System;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace TechEd.Demo.SolidPrinciples.Isp
{
    public class InputParser
    {
        public virtual Document ParseInput(string input)
        {
            Document doc;
            try
            {
                var xdoc = XDocument.Parse(input);
                doc = new Document();
                doc.Title = xdoc.Root.Element("title").Value;
                doc.Text = xdoc.Root.Element("text").Value;
            }
            catch (Exception)
            {
                throw new InvalidInputFormatException();
            }

            return doc;
        }
    }

    public class JsonInputParser : InputParser
    {
        public override Document ParseInput(string input)
        {
            try
            {
                return JsonConvert.DeserializeObject<Document>(input);
            }
            catch (Exception)
            {
                return base.ParseInput(input);
            }
        }
    }
}