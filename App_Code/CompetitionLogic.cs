using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Types;

/// <summary>
/// Summary description for CompetitionLogic
/// </summary>
public static class CompetitionLogic
{
    private const string GetCompetitionEntryInformationKeyTemplate = "CompetitionLogic::GetCompetitionEntryInformation_{0}_{1}";

    /// <summary>
    /// Clear Competition Setup information from the SessionManager.
    /// </summary>
    /// <param name="target">Selected Competition.</param>
    /// <param name="entityId">Entity to evaluate settings based on. If null, use current entity.</param>
    /// <returns></returns>
    public static void ClearCompetitionEntryInformation(this msCompetition target, string entityId = null)
    {
        if (target == null)
        {
            return;
        }

        ClearCompetitionEntryInformation(target.ID, entityId);
    }

    /// <summary>
    /// Clear Competition Setup information from the SessionManager.
    /// </summary>
    /// <param name="competitionId">Selected Competition.</param>
    /// <param name="entityId">Entity to evaluate settings based on. If null, use current entity.</param>
    /// <returns></returns>
    public static void ClearCompetitionEntryInformation(string competitionId, string entityId = null)
    {
        if (entityId == null)
        {
            entityId = ConciergeAPI.CurrentEntity != null ? ConciergeAPI.CurrentEntity.ID : null;
        }

        var key = string.Format(GetCompetitionEntryInformationKeyTemplate, competitionId, entityId);

        SessionManager.Set<CompetitionEntryInformation>(key, null);
    }

    /// <summary>
    /// Pull the Competition Setup information through the SessionManager.
    /// </summary>
    /// <param name="target">Selected Competition.</param>
    /// <param name="entityId">Entity to evaluate settings based on. If null, use current entity.</param>
    /// <returns></returns>
    public static CompetitionEntryInformation GetCompetitionEntryInformation(this msCompetition target, string entityId = null)
    {
        if (target == null)
        {
            return null;
        }

        return GetCompetitionEntryInformation(target.ID, entityId);
    }

    /// <summary>
    /// Pull the Competition Setup information through the SessionManager.
    /// </summary>
    /// <param name="competitionId">Selected Competition.</param>
    /// <param name="entityId">Entity to evaluate settings based on. If null, use current entity.</param>
    /// <returns></returns>
    public static CompetitionEntryInformation GetCompetitionEntryInformation(string competitionId, string entityId = null)
    {
        if (entityId == null)
        {
            entityId = ConciergeAPI.CurrentEntity != null ? ConciergeAPI.CurrentEntity.ID : null;
        }

        var key = string.Format(GetCompetitionEntryInformationKeyTemplate, competitionId, entityId);

        return SessionManager.Get(key, () =>
        {
            using (var proxy = ConciergeAPIProxyGenerator.GenerateProxy())
            {
                return proxy.GetCompetitionEntryInformation(competitionId, entityId).ResultValue;
            }
        });
    }
}