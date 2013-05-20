using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.DuplicateDetection;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Types;

public partial class profile_CreateAccount_DuplicateCheck : PortalPage
{
    #region Fields

    private NewUserRequest targetRequest;
    private DataView dvDuplicates;

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether this page is public, meaning you don't
    /// have to be logged in to access it.
    /// </summary>
    /// <value><c>true</c> if this instance is public; otherwise, <c>false</c>.</value>
    protected override bool IsPublic
    {
        get
        {
            return true;
        }
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the target object for the page
    /// </summary>
    /// <remarks>Many pages have "target" objects that the page operates on. For instance, when viewing
    /// an event, the target object is an event. When looking up a directory, that's the target
    /// object. This method is intended to be overriden to initialize the target object for
    /// each page that needs it.</remarks>
    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        targetRequest = MultiStepWizards.CreateAccount.Request;
        if (targetRequest == null)
            GoTo("~/profile/CreateAccount_BasicInfo.aspx");
    }

    /// <summary>
    /// Initializes the page.
    /// </summary>
    /// <remarks>This method runs on the first load of the page, and does NOT
    /// run on postbacks. If you want to run a method on PostBacks, override the
    /// Page_Load event</remarks>
    protected override void InitializePage()
    {
        using (IConciergeAPIService proxy = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            executePotentialDuplicatesSearch(proxy);
        }

        if ( dvDuplicates == null || dvDuplicates.Count < 1)
            GoTo("~/profile/CreateAccount_CreateUser.aspx");

        gvDuplicates.DataSource = dvDuplicates;
        gvDuplicates.DataBind();
    }

    private void executePotentialDuplicatesSearch(IConciergeAPIService serviceProxy)
    {
        var config = serviceProxy.GetAssociationConfiguration().ResultValue;;
        if (config != null && config.SafeGetValue<bool>("DisableDuplicateCheckPortal"))    //dupe check disabled
            return;

        //Get the City/State from the Postal Code - This is better for duplicate matching because it removes zip+4 from the equation
        Address address = new Address();
        address.PostalCode = targetRequest.PostalCode;

        try
        {
            using (IConciergeAPIService proxy = GetConciegeAPIProxy())
            {
                var apiResult = proxy.PopulateCityStateFromPostalCode(address);
                address = apiResult.ResultValue;
            }
        }
        catch 
        {
            // don't want to do anything if this fails
        }

        MemberSuiteObject mso = new MemberSuiteObject();
        mso.Fields.Add("FirstName", targetRequest.FirstName);
        mso.Fields.Add("LastName", targetRequest.LastName);
        mso.Fields.Add("City", address.City);
        mso.Fields.Add("State", address.State);
        mso.ClassType = "Individual";

        Search duplicateSearch = new Search();
        duplicateSearch.AddOutputColumn("ID");
        duplicateSearch.AddOutputColumn("FirstName");
        duplicateSearch.AddOutputColumn("LastName");
        duplicateSearch.AddOutputColumn("EmailAddress");
        duplicateSearch.AddOutputColumn("_Preferred_Address_Line1");
        duplicateSearch.AddOutputColumn("_Preferred_Address_City");
        duplicateSearch.AddOutputColumn("_Preferred_Address_State");
        duplicateSearch.AddOutputColumn("_Preferred_Address_PostalCode");
        duplicateSearch.AddSortColumn("FirstName");
        duplicateSearch.AddSortColumn("LastName");

        SearchResult duplicateResult;


        duplicateResult = serviceProxy.FindPotentialDuplicates(mso, null, duplicateSearch, 0, null).ResultValue;

        hideInfo(duplicateResult.Table);
        dvDuplicates = new DataView(duplicateResult.Table);

    }

    private void hideInfo(DataTable dataTable)
    {
        DataColumn dcAddress = dataTable.Columns.Add("AddressClean", typeof (string));
        DataColumn dcEmail = dataTable.Columns.Add("EmailClean", typeof(string));
        int charCount;

        foreach (DataRow row in dataTable.Rows)
        {
            string address = Convert.ToString( row["_Preferred_Address_Line1"] );
            if(!string.IsNullOrEmpty(address))
            {
                charCount = 10;

                if (address.Length < charCount)
                    charCount = address.Length/2;
                row[dcAddress] = address.Substring(0, charCount) + "XXXXX";
            }

            string emailAddress =  Convert.ToString( row["EmailAddress"] );
            if (!string.IsNullOrEmpty(emailAddress))
            {
                int atLocation = emailAddress.IndexOf("@");
                charCount = atLocation + 3;

                if (emailAddress.Length < charCount)
                    charCount = atLocation + 1;
                row[dcEmail] = emailAddress.Substring(0, charCount) + "XXXXX";
            }
        }
    }

    #endregion


    #region Event Handlers

    protected void btnNewAccount_Click(object sender, EventArgs e)
    {
        GoTo("~/profile/CreateAccount_CreateUser.aspx");
    }
    protected void gvDuplicates_RowCommand(Object sender, GridViewCommandEventArgs e)
    {
        if(e.CommandName.ToLower() == "select")
        {
            if(MultiStepWizards.CreateAccount.InitiatedByLeader)
            {
                MultiStepWizards.CreateAccount.InitiatedByLeader = false;
                GoTo(string.Format("~/membership/Join.aspx?entityID={0}", e.CommandArgument));
                return;
            }

            msPortalUser portalUser = null;

            using(IConciergeAPIService proxy = GetConciegeAPIProxy())
            {
                portalUser = proxy.GetOrCreatePortalUserForEntity(e.CommandArgument.ToString()).ResultValue;
            }

            if(portalUser == null)
            {
                GoTo("~/profile/UnableToCreateAccount.aspx");
                return;
            }

            GoTo(string.Format("~/profile/EmailLoginInformation.aspx?contextID={0}",portalUser.ID));
            return;
        }
    }

    #endregion

}