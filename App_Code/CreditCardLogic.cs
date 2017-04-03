using System.Web.Script.Serialization;
using System.Web.UI.HtmlControls;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Types;

/// <summary>
/// Summary description for CreditCardLogic
/// </summary>
public class CreditCardLogic : PortalPage
{
    protected string GetPriorityPaymentsConfig(string entityId)
    {
        if (string.IsNullOrWhiteSpace(entityId))
            return null;

        using (var api = GetServiceAPIProxy())
        {
            ConciergeResult<PaymentProcessorSettings> priorityData = api.GetPaymentProcessorSettings(null, entityId);
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(priorityData.ResultValue);
        }
    }
}