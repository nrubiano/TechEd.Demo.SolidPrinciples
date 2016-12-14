using System;
using System.IO;
using System.Net;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace TechEd.Demo.SolidPrinciples.Dip.Manual
{
    public interface IInputRetriever
    {
        string GetData(string fileName);
    }
    public interface IDocumentPersister
    {
        void PersistDocument(string serializedDocument, string targetFileName);
    }
    public abstract class DocumentStorage : IInputRetriever, IDocumentPersister
    {
        public abstract string GetData(string fileName);
        public abstract void PersistDocument(string serializedDocument, string targetFileName);
    }

    public class FileDocumentStorage : DocumentStorage
    {
        public override string GetData(string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException("Could not find a file with that name...");

            using (var stream = File.OpenRead(fileName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
        public override void PersistDocument(string serializedDocument, string targetFileName)
        {
            try
            {
                using (var stream = File.Open(targetFileName, FileMode.Create, FileAccess.Write))
                using (var sw = new StreamWriter(stream))
                {
                    sw.Write(serializedDocument);
                    sw.Close();
                }
            }
            catch (Exception)
            {
                throw new AccessViolationException();
            }
        }
    }

    public class BlobDocumentStorage : DocumentStorage
    {
        private readonly CloudBlobClient _blobClient;

        public BlobDocumentStorage(string storageAccount, string storageKey)
        {
            var account = new CloudStorageAccount(new StorageCredentials(storageAccount, storageKey), true);
            _blobClient = account.CreateCloudBlobClient();
        }

        public override string GetData(string fileName)
        {
            if (!fileName.StartsWith(_blobClient.BaseUri.ToString()))
            {
                throw new InvalidTargetException();
            }

            var client = new WebClient();
            var input = client.DownloadString(fileName);

            return input;
        }
        public override void PersistDocument(string serializedDocument, string targetFileName)
        {
            if (!targetFileName.StartsWith(_blobClient.BaseUri.ToString()))
            {
                throw new InvalidTargetException();
            }

            var uri = new Uri(targetFileName);
            var containerName = uri.AbsolutePath.Substring(1, uri.AbsolutePath.IndexOf('/', 1) - 1);
            var container = _blobClient.GetContainerReference(containerName);
            container.CreateIfNotExists(BlobContainerPublicAccessType.Container);
            var blob = container.GetBlockBlobReference(targetFileName.Replace(_blobClient.BaseUri + containerName + "/", ""));
            blob.UploadText(serializedDocument);
        }
    }

    public class HttpInputRetriever : IInputRetriever
    {
        public string GetData(string fileName)
        {
            if (!fileName.StartsWith("http"))
            {
                throw new InvalidTargetException();
            }

            var client = new WebClient();
            var input = client.DownloadString(fileName);

            return input;
        }
    }
}
