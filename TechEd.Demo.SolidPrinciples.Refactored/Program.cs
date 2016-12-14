using System;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace TechEd.Demo.SolidPrinciples.Refactored
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourceFileName = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\Input Documents\\Document1.xml");
            var targetFileName = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\Output Documents\\Document1.json");

            var input = GetInput(sourceFileName);
            var doc = GetDocument(input);
            var serializedDoc = SerializeDocument(doc);
            PersistDocument(serializedDoc, targetFileName);
        }

        private static string GetInput(string sourceFileName)
        {
            using (var stream = File.OpenRead(sourceFileName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
        private static Document GetDocument(string input)
        {
            var xdoc = XDocument.Parse(input);
            var doc = new Document();
            doc.Title = xdoc.Root.Element("title").Value;
            doc.Text = xdoc.Root.Element("text").Value;

            return doc;
        }
        private static string SerializeDocument(Document doc)
        {
            return JsonConvert.SerializeObject(doc);
        }
        private static void PersistDocument(string serializedDoc, string targetFileName)
        {
            using (var stream = File.Open(targetFileName, FileMode.Create, FileAccess.Write))
            using (var sw = new StreamWriter(stream))
            {
                sw.Write(serializedDoc);
                sw.Close();
            }
        }
    }
}
