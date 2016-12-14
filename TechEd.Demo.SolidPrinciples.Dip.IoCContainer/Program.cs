using System;
using System.Configuration;
using System.IO;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace TechEd.Demo.SolidPrinciples.Dip.IoCContainer
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourceFileName = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\Input Documents\\Document1.xml");
            var targetFileName = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\Output Documents\\Document1.json");
            //var sourceFileName = "http://chris.59north.com/Document1.xml";
            //var targetFileName = "https://teched.blob.core.windows.net/converted-documents/Document1.json";
            
            //var container = GetContainer();
            var container = ConfigureDeclarativelyContainer();
            ConfigureStorage();

            var formatConverter = container.Resolve<FormatConverter>();
            if (!formatConverter.ConvertFormat(sourceFileName, targetFileName))
            {
                Console.WriteLine("Conversion failed...");
                Console.ReadKey();
            }
        }

        private static IUnityContainer GetContainer()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterType<IDocumentSerializer, CamelCaseJsonSerializer>();
            container.RegisterType<InputParser, JsonInputParser>();

            return container;
        }
        private static IUnityContainer ConfigureDeclarativelyContainer()
        {
            IUnityContainer container = new UnityContainer();
            var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            section.Configure(container);
            return container;
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
