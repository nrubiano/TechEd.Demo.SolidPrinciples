using System;
using System.Configuration;
using System.IO;

namespace TechEd.Demo.SolidPrinciples.Isp
{
    public class FormatConverter
    {
        private readonly IDocumentSerializer _documentSerializer;
        private readonly InputParser _inputParser;

        public FormatConverter()
        {
            //_documentSerializer = new JsonDocumentSerializer();
            _documentSerializer = new CamelCaseJsonSerializer();
            _inputParser = new JsonInputParser();
        }

        public bool ConvertFormat(string sourceFileName, string targetFileName)
        {
            string input;
            try
            {
                var inputRetriever = GetInputRetrieverForFileName(sourceFileName);
                input = inputRetriever.GetData(sourceFileName);
            }
            catch (FileNotFoundException)
            {
                return false;
            }

            var doc = _inputParser.ParseInput(input);
            var serializedDoc = _documentSerializer.Serialize(doc);

            try
            {
                var documentPerister = GetDocumentPersisterForFileName(targetFileName);
                documentPerister.PersistDocument(serializedDoc, targetFileName);
            }
            catch (AccessViolationException)
            {
                return false;
            }

            return true;
        }

        private IInputRetriever GetInputRetrieverForFileName(string fileName)
        {
            if (IsBlobstorageUrl(fileName))
                return new BlobDocumentStorage(ConfigurationManager.AppSettings["storageAccount"], ConfigurationManager.AppSettings["storageKey"]);

            if (fileName.StartsWith("http"))
                return new HttpInputRetriever();

            return new FileDocumentStorage();
        }
        private IDocumentPersister GetDocumentPersisterForFileName(string fileName)
        {
            if (IsBlobstorageUrl(fileName))
                return new BlobDocumentStorage(ConfigurationManager.AppSettings["storageAccount"], ConfigurationManager.AppSettings["storageKey"]);

            return new FileDocumentStorage();
        }

        private bool IsBlobstorageUrl(string str)
        {
            var storageAccount = ConfigurationManager.AppSettings["storageAccount"];
            return str.StartsWith(string.Format("https://{0}.blob.core.windows.net/", storageAccount));
        }
    }
}
