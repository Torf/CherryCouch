using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CherryCouch.Common.Utils
{
    public static class Converter
    {
        public static int ConvertSize(string sizeWithUnit)
        {
            var regex = new Regex(@"([0-9\.]+) ?([MmGgTtKk])[oB]");

            var match = regex.Match(sizeWithUnit);

            double value;
            Double.TryParse(match.Groups[1].Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out value);

            switch (match.Groups[2].Value.ToUpper())
            {
                case "M":
                    break;
                case "G":
                    value = value * 1024.0;
                    break;
                case "T":
                    value = value * 1024.0 * 1024.0;
                    break;
                case "K":
                    value = value / 1024.0;
                    break;
            }

            return (int)Math.Round(value);
        }
    }
}
