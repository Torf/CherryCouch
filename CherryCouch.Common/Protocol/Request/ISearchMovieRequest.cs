namespace CherryCouch.Common.Protocol.Request
{
    public interface ISearchMovieRequest
    {
        string ImdbId { get; }

        string Terms { get; }

        IRequestAuthorization Authorization { get; }
    }
}
