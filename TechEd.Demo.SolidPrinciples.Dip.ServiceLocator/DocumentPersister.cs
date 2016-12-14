using System;
using System.Collections.Generic;
using System.Linq;

namespace TechEd.Demo.SolidPrinciples.Dip.ServiceLocator
{
    public static class DocumentPersister
    {
        private static readonly Dictionary<Func<string, bool>, IDocumentPersister> DocumentPersisters = new Dictionary<Func<string, bool>, IDocumentPersister>();

        public static void RegisterDocumentPersister(Func<string, bool> evaluator, IDocumentPersister documentPersister)
        {
            DocumentPersisters.Add(evaluator, documentPersister);
        }

        public static IDocumentPersister ForFileName(string filename)
        {
            return DocumentPersisters.First(x => x.Key(filename)).Value;
        }
    }
}