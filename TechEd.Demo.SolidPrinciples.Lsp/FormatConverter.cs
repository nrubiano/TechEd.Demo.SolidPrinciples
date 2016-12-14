using System;
using System.Configuration;
using System.IO;

namespace TechEd.Demo.SolidPrinciples.Lsp
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
                var documentStorage = GetDocumentStorageForFileName(sourceFileName);
                input = documentStorage.GetData(sourceFileName);
            }
            catch (FileNotFoundException)
            {
                return false;
            }

            var doc = _inputParser.ParseInput(input);
            var serializedDoc = _documentSerializer.Serialize(doc);

            try
            {
                var documentStorage = GetDocumentStorageForFileName(targetFileName);
                documentStorage.PersistDocument(serializedDoc, targetFileName);
            }
            catch (AccessViolationException)
            {
                return false;
            }

            return true;
        }

        private DocumentStorage GetDocumentStorageForFileName(string fileName)
        {

            if (IsBlobstorageUrl(fileName))
                return new BlobDocumentStorage(ConfigurationManager.AppSettings["storageAccount"], ConfigurationManager.AppSettings["storageKey"]);

            if (fileName.StartsWith("http"))
                return new HttpInputRetriever();

            return new FileDocumentStorage();
        }

        private bool IsBlobstorageUrl(string str)
        {
            var storageAccount = ConfigurationManager.AppSettings["storageAccount"];
            return str.StartsWith(string.Format("https://{0}.blob.core.windows.net/", storageAccount));
        }
    }
}
