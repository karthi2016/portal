using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Types;

/// <summary>
/// Summary description for APIExtensions
/// </summary>
public static class APIExtensions
{
    public const int MaxResultsDefault = 500;

    public static void AppendAdditional(this IConciergeAPIService serviceProxy, Search searchToRun, SearchResult result)
    {
        // Stop the user right away if the Search they are running did not contain a Sort. Since we are calling more than
        // once, we need to ensure that the data is going to be consistent.
        if (searchToRun.SortColumns.Count == 0)
        {
            throw new ApplicationException(
                string.Format(
                    "Search {0} does not contain a Sort and may return incorrectly when there are more than {1} results.",
                    searchToRun.ID ?? searchToRun.Type,
                    MaxResultsDefault));
        }

        // Start at the initial count of the result
        var i = result.Table.Rows.Count;

        ConciergeResult<SearchResult> addlResult;
        do
        {
            addlResult = serviceProxy.ExecuteSearch(searchToRun, i, MaxResultsDefault);

            if (addlResult.ResultValue == null)
            {
                throw new ApplicationException(
                    string.Format(
                        "Error running Search {0}: {1}",
                        searchToRun.ID ?? searchToRun.Type,
                        addlResult.FirstErrorMessage));
            }

            result.Table.Merge(addlResult.ResultValue.Table);

            i += MaxResultsDefault;
        } while (addlResult.ResultValue.TotalRowCount > i);
    }

    /// <summary>
    /// Executes a search against the Concierge API
    /// </summary>
    /// <param name="searchToRun">The search to run.</param>
    /// <param name="startRecord">The start record.</param>
    /// <param name="maximumNumberOfRecordsToReturn">The maximum number of records to return.</param>
    /// <returns></returns>
    public static SearchResult GetSearchResult(Search searchToRun, int startRecord, int? maximumNumberOfRecordsToReturn)
    {
        using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            return api.GetSearchResult(searchToRun, startRecord, maximumNumberOfRecordsToReturn);
        }
    }

    /// <summary>
    /// Executes a search against the Concierge API
    /// </summary>
    /// <param name="serviceProxy">API Proxy</param>
    /// <param name="searchToRun">The search to run.</param>
    /// <param name="startRecord">The start record.</param>
    /// <param name="maximumNumberOfRecordsToReturn">The maximum number of records to return.</param>
    /// <returns></returns>
    public static SearchResult GetSearchResult(this IConciergeAPIService serviceProxy, Search searchToRun, int startRecord, int? maximumNumberOfRecordsToReturn)
    {
        var result = serviceProxy.ExecuteSearch(searchToRun, startRecord, maximumNumberOfRecordsToReturn);

        if (maximumNumberOfRecordsToReturn == null && result.ResultValue != null && result.ResultValue.TotalRowCount > MaxResultsDefault)
        {
            serviceProxy.AppendAdditional(searchToRun, result.ResultValue);
        }

        return result.ResultValue;
    }

    /// <summary>
    /// Executes multiple searches against the Concierge API
    /// </summary>
    /// <param name="searchesToRun">List of searches to run.</param>
    /// <param name="startRecord">The start record.</param>
    /// <param name="maximumNumberOfRecordsToReturn">The maximum number of records to return.</param>
    /// <returns></returns>
    public static List<SearchResult> GetMultipleSearchResults(List<Search> searchesToRun, int startRecord, int? maximumNumberOfRecordsToReturn)
    {
        using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            return api.GetMultipleSearchResults(searchesToRun, startRecord, maximumNumberOfRecordsToReturn);
        }
    }

    /// <summary>
    /// Executes multiple searches against the Concierge API
    /// </summary>
    /// <param name="serviceProxy">API Proxy</param>
    /// <param name="searchesToRun">List of searches to run.</param>
    /// <param name="startRecord">The start record.</param>
    /// <param name="maximumNumberOfRecordsToReturn">The maximum number of records to return.</param>
    /// <returns></returns>
    public static List<SearchResult> GetMultipleSearchResults(this IConciergeAPIService serviceProxy, List<Search> searchesToRun, int startRecord, int? maximumNumberOfRecordsToReturn)
    {
        var results = serviceProxy.ExecuteSearches(searchesToRun, startRecord, maximumNumberOfRecordsToReturn);

        if (maximumNumberOfRecordsToReturn == null)
        {
            foreach (var result in results.ResultValue)
            {
                if (result != null && result.TotalRowCount > MaxResultsDefault)
                {
                    serviceProxy.AppendAdditional(searchesToRun.FirstOrDefault(s => s.ID == result.ID), result);
                }
            }
        }

        return results.ResultValue;
    }

    public static T LoadObjectFromAPI<T>(this IConciergeAPIService proxy, string id) where T : msAggregate
    {
        return proxy.LoadObjectFromAPI(id).ConvertTo<T>();
    }

    public static T LoadObjectFromAPI<T>(string id) where T : msAggregate
    {
        using (var proxy = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            return proxy.LoadObjectFromAPI(id).ConvertTo<T>();
        }
    }

    public static MemberSuiteObject LoadObjectFromAPI(string id)
    {
        using (var proxy = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            return proxy.LoadObjectFromAPI(id);
        }
    }

    public static MemberSuiteObject LoadObjectFromAPI(this IConciergeAPIService proxy, string id)
    {
        if (id == null) return null;
        return proxy.Get(id).ResultValue;
    }
}