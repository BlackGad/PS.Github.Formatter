using System;
using System.Linq;
using System.Xml.Linq;

namespace PS.Github.Formatter.Extensions
{
    public static class XExtensions
    {
        #region Static members

        public static string GetSummary(this XElement element)
        {
            var value = element?.Descendants()
                                .FirstOrDefault(e => string.Equals(e.Name.LocalName,
                                                                   "summary",
                                                                   StringComparison.InvariantCultureIgnoreCase))?
                                .Value
                                .Trim('\n');
            if (value != null) value = string.Join("\n", value.Split('\n').Select(s => s.Trim(' ')));
            return value;
        }

        #endregion
    }
}