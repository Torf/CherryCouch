namespace CherryCouch.Common.Protocol.Scraper
{
    public enum ScrapingConverterEnum
    {
        None,
        ConcatBefore,
        ConcatAfter,
        RegexExtract,
        ConvertUnitFileSize,
        ConvertDoubleToInt,
        CustomMethod
    }
}
