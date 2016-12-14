using System;
using System.Configuration;
using System.IO;

namespace TechEd.Demo.SolidPrinciples.Dip.Manual
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourceFileName = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\Input Documents\\Document1.xml");
            var targetFileName = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\Output Documents\\Document1.json");
            //var sourceFileName = "http://chris.59north.com/Document1.xml";
            //var targetFileName = "https://teched.blob.core.windows.net/converted-documents/Document1.json";

            ConfigureStorage();

            var documentSerializer = new CamelCaseJsonSerializer();
            var inputParser = new JsonInputParser();

            var formatConverter = new FormatConverter(documentSerializer, inputParser);
            if (!formatConverter.ConvertFormat(sourceFileName, targetFileName))
            {
                Console.WriteLine("Conversion failed...");
                Console.ReadKey();
            }
        }

        private static void ConfigureStorage()
        {
            var blobStorage = new BlobDocumentStorage(ConfigurationManager.AppSettings["storageAccount"], ConfigurationManager.AppSettings["storageKey"]);
            var fileStorage = new FileDocumentStorage();
            var httpInputRetriever = new HttpInputRetriever();

            InputRetriever.RegisterInputRetriever(x => x.StartsWith("http"), httpInputRetriever);
            InputRetriever.RegisterInputRetriever(IsBlobstorageUrl, blobStorage);
            InputRetriever.RegisterInputRetriever(x => true, fileStorage);
            DocumentPersister.RegisterDocumentPersister(IsBlobstorageUrl, blobStorage);
            DocumentPersister.RegisterDocumentPersister(x => true, fileStorage);
        }
        private static bool IsBlobstorageUrl(string str)
        {
            var storageAccount = ConfigurationManager.AppSettings["storageAccount"];
            return str.StartsWith(string.Format("https://{0}.blob.core.windows.net/", storageAccount));
        }
    }
}
