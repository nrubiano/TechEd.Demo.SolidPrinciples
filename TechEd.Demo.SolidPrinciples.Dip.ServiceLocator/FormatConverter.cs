using System;
using System.IO;

namespace TechEd.Demo.SolidPrinciples.Dip.ServiceLocator
{
    public class FormatConverter
    {
        private readonly IDocumentSerializer _documentSerializer;
        private readonly InputParser _inputParser;

        public FormatConverter()
        {
            _documentSerializer = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IDocumentSerializer>();
            _inputParser = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<InputParser>();
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
