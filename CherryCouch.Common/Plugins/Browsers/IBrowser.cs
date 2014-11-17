namespace CherryCouch.Common.Plugins.Browsers
{
    public interface IBrowser
    {
        /// <summary>
        /// Clears the current session, removing cookies and cache.
        /// </summary>
        void ClearSession();
    }
}