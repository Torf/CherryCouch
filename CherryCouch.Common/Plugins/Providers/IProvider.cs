using System.Collections.Generic;
using CherryCouch.Common.Protocol.Search;

namespace CherryCouch.Common.Plugins.Providers
{
    public interface IProvider
    {
        /// <summary>
        /// Gets the name of the provider.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Indicates if we're connected
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Indicates if the process is currently working
        /// </summary>
        bool IsWorking { get; }

        /// <summary>
        /// Logs or check the user is properly connected to the service
        /// </summary>
        /// <returns>Logic indicating success</returns>
        bool Login();

        /// <summary>
        /// Logout the user from the service
        /// </summary>
        void Logout();

        /// <summary>
        /// Terms for a set of terms
        /// </summary>
        /// <param name="terms">terms to search for</param>
        /// <returns>List of results retrieved</returns>
        List<ISearchResult> Search(string terms);
    }
}
