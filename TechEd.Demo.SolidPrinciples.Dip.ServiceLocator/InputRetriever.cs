using System;
using System.Collections.Generic;
using System.Linq;

namespace TechEd.Demo.SolidPrinciples.Dip.ServiceLocator
{
    public static class InputRetriever
    {
        private static readonly Dictionary<Func<string, bool>, IInputRetriever> InputRetrievers = new Dictionary<Func<string, bool>, IInputRetriever>();

        public static void RegisterInputRetriever(Func<string, bool> evaluator, IInputRetriever inputRetriever)
        {
            InputRetrievers.Add(evaluator, inputRetriever);
        }

        public static IInputRetriever ForFileName(string filename)
        {
            return InputRetrievers.First(x => x.Key(filename)).Value;
        }
    }
}
