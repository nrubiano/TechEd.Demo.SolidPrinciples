using System;
using System.IO;

namespace TechEd.Demo.SolidPrinciples.Dip.Manual
{
    public class FormatConverter
    {
        private readonly IDocumentSerializer _documentSerializer;
        private readonly InputParser _inputParser;

        public FormatConverter(IDocumentSerializer documentSerializer, InputParser inputParser)
        {
            _documentSerializer = documentSerializer;
            _inputParser = inputParser;
        }

        public bool ConvertFormat(string sourceFileName, string targetFileName)
        {
            string input;
            try
            {
                var inputRetriever = InputRetriever.ForFileName(sourceFileName);
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
                var documentPersister = DocumentPersister.ForFileName(targetFileName);
                documentPersister.PersistDocument(serializedDoc, targetFileName);
            }
            catch (AccessViolationException)
            {
                return false;
            }

            return true;
        }
    }
}
