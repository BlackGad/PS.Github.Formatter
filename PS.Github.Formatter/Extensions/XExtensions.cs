using System;
using System.Linq;
using System.Xml.Linq;

namespace PS.Github.Formatter.Extensions
{
    public static class XExtensions
    {
        #region Static members

        public static string GetParameter(this XElement element, string name)
        {
            var paramElement = element?
                .Descendants()
                .FirstOrDefault(e => string.Equals(e.Name.LocalName, "param", StringComparison.InvariantCultureIgnoreCase) &&
                                     e.Attributes().Any(a => string.Equals(a.Value, name, StringComparison.InvariantCultureIgnoreCase)));
            return CleanupElementValue(paramElement);
        }

        public static string GetReturns(this XElement element)
        {
            var paramElement = element?.Descendants()
                                       .FirstOrDefault(e => string.Equals(e.Name.LocalName,
                                                                          "returns",
                                                                          StringComparison.InvariantCultureIgnoreCase));
            return CleanupElementValue(paramElement);
        }

        public static string GetSummary(this XElement element)
        {
            var summaryElement = element?.Descendants()
                                         .FirstOrDefault(e => string.Equals(e.Name.LocalName,
                                                                            "summary",
                                                                            StringComparison.InvariantCultureIgnoreCase));
            return CleanupElementValue(summaryElement);
        }

        private static string CleanupElementValue(XElement summaryElement)
        {
            var value = summaryElement?.Value.Trim(' ');
            if (value != null)
                value = string.Join("\n",
                                    value.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                         .Select(s => s.Trim(' ')));
            return value;
        }

        #endregion
    }
}