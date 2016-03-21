using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Manifests.Command;
using MemberSuite.SDK.Manifests.Command.Views;
using MemberSuite.SDK.Manifests.Resource;
using MemberSuite.SDK.Manifests.Searching;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.Utilities;
using MemberSuite.SDK.Web;
using MemberSuite.SDK.Web.ControlManagers;
using MemberSuite.SDK.Web.Controls;
using Image = System.Web.UI.WebControls.Image;

/// <summary>
/// This is the base class for all pages in the portal and houses common functionality
/// used throughout the site.
/// </summary>
public abstract class PortalPage : Page, IControlHost
{
    private List<msPortalControlPropertyOverride> pageOverrides;

    protected List<msPortalControlPropertyOverride> PageOverrides
    {
        get
        {
            if (pageOverrides == null)
            {
                pageOverrides = PortalConfiguration.Current.ControlOverrides.FindAll(x => x.PageName.Equals(Request.Url.LocalPath, StringComparison.InvariantCultureIgnoreCase));
            }

            return pageOverrides;
        }
    }

    public PortalPage()
    {
        PreInit += new EventHandler(PortalPage_PreInit);
    }

    protected virtual int PAGE_SIZE
    {
        get { return 25; }
    } 

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
    
        // it's important that this is the first thing that the page does. Every subsequent API call
        // will depend on it. Many pages do not require a login (such as the Login screen, the view event screen,
        // etc - and so it's imperative that this is set
    }

    /// <summary>
    /// Checks to make sure that this page is being access properly.
    /// </summary>
    /// <returns></returns>
    protected virtual bool CheckSecurity()
    {
        return true;
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected virtual void Page_Load(object sender, EventArgs e)
    {
        // since we very frequently have to run an operation, once, when a page loads
        // we'll just introduce the InitializePage method for convienence
        checkToSeeIfUserIsLoggedIn();

        checkThrowManualException();

        //MS-1956
        if (PortalConfiguration.Current != null)
        {
            ControlContext.CurrentAssociationID = PortalConfiguration.Current.AssociationID;
            ControlContext.CurrentAssociationKey = PortalConfiguration.Current.PartitionKey;
        }

        InitializeTargetObject();

        if (!CheckSecurity())
            Response.Redirect("/AccessDenied.aspx");

        if (!IsPostBack)
        {
            InitializePage();
        }

        // set the custom fields - must happen in Page_Load so that the target object is defined (it may have a page layout specific to a type value)
        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            InstantiateCustomFields(proxy);
        }

        // finally, apply the page overrides
        if (!IsPostBack)
        {
            setupControlPropertyOverrides();
        }

        if (Master != null)
        {
            var master = Master as PortalMaster;
            if (master == null && Master.Master != null)
            {
                master = Master.Master as PortalMaster;
            }

            if (master != null)
            {
                master.AddCustomOverrideEligibleControls += AddCustomOverrideEligibleControls;
            }
        }
    }
    
    void PortalPage_PreInit(object sender, EventArgs e)
    {
        SetupAddressControl();
        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo( ConciergeAPI.CurrentLocale );
    }

    public static void SetupAddressControl()
    {
        // make sure addresses render properly
        if (PortalConfiguration.Current != null && // check to see if we've already got it first
            PortalConfiguration.CurrentConfig != null)
        {
            HttpContext.Current.Items[AddressControl.ADDRESSCONTROL_COMBOBOX] =
                PortalConfiguration.CurrentConfig.UseDropDownsForStatesAndCountries == ConsolePortalOptions.Portal ||
                PortalConfiguration.CurrentConfig.UseDropDownsForStatesAndCountries == ConsolePortalOptions.Both;
        }
    }

    protected virtual void checkThrowManualException()
    {
        if (Request.QueryString["throwException"] != null)
            throw new ApplicationException("A manual exception has been thrown.");
    }

    protected virtual void InstantiateCustomFields(IConciergeAPIService proxy)
    {
    }

     /// <summary>
     /// Initializes the target object for the page
     /// </summary>
     /// <remarks>Many pages have "target" objects that the page operates on. For instance, when viewing
     /// an event, the target object is an event. When looking up a directory, that's the target
     /// object. This method is intended to be overriden to initialize the target object for
     /// each page that needs it.</remarks>
    protected virtual void InitializeTargetObject()
    {
       
    }

    protected string ContextID
    {
        get {return Request.QueryString["contextID"];}
    }

    protected void GoToMissingRecordPage()
    {
        QueueBannerError("The specified record could not be located.");
        GoTo("/");
    }

    /// <summary>
    /// Initializes the page.
    /// </summary>
    /// <remarks>This method runs on the first load of the page, and does NOT
    /// run on postbacks. If you want to run a method on PostBacks, override the
    /// Page_Load event</remarks>
    protected virtual  void InitializePage()
    {
        PageStart = (SelectedPage - 1) * PAGE_SIZE;

        setupHyperLinks();
        dequeueBannerMessageIfPresent();
    }

    private void setupControlPropertyOverrides()
    {
        if (PortalConfiguration.Current == null ||
            PortalConfiguration.Current.ControlOverrides == null)
            return;

        if (PageOverrides.Count == 0)
            // no overrides
            return;

        Control refControl = this.Master;
        Control pageTitle = null;
        if (Master == null)
            refControl = Page;
        else if (Master.Master == null)
        {
            refControl = Master.FindControl("PageContent");
            pageTitle = Master.FindControl("PageTitle");
        }
        else
        {
            refControl = Master.Master.FindControl("PageContent");
            pageTitle = Master.Master.FindControl("PageTitle");

            // Check if the PageTitle content is wrapping another PageTitle (happens for the DataPage Master Page)
            var childControl = pageTitle.FindControl("PageTitle") as ContentPlaceHolder;
            if (childControl != null)
                pageTitle = childControl;
        }

        var relatedControls = _recursiveFind(refControl, PageOverrides);

        foreach (var o in PageOverrides)
        {
            Control c = null;
            if (relatedControls.ContainsKey(o.ControlName))
            {
                c = relatedControls[o.ControlName];
            }

            if (c == null && pageTitle != null ) // one last ditch effort - try the page title
                c = pageTitle.FindControl(o.ControlName);

            applyOverride(c, o);
        }

        var pto = PageOverrides.Find(x => x.ControlName == "__PageTitle");
        if (pto != null && pageTitle != null) // ok, there's a title
        {
            LiteralControl lc = null;
            foreach (var c in pageTitle.Controls)
                if (c is LiteralControl) // grab it
                {
                    lc = (LiteralControl)c;
                    break;
                }

            if (lc != null)
            {
                Page.Title = pto.Value;
                lc.Text = pto.Value;
            }
        }
    }

    /// <summary>
    /// Apply overrides to a control manually in case needed before the end of Page_Load.
    /// </summary>
    /// <param name="c"></param>
    protected virtual void ApplyControlOverrides(Control c)
    {
        foreach (var o in PageOverrides.Where(o => o.ControlName == c.ID))
        {
            applyOverride(c, o);
        }
    }

    private void applyOverride(Control c, msPortalControlPropertyOverride o)
    {
        if (c == null)
            return;

        var pi = c.GetType().GetProperty(o.PropertyName);
        if (pi == null)
            return;

        try
        {
            object val = o.Value;
            if (pi.PropertyType == typeof (bool)) // try to cast
                val = Convert.ToBoolean(o.Value);

            if (pi.PropertyType == typeof (int))
                val = Convert.ToInt32(o.Value);

            if (pi.PropertyType == typeof (long))
                val = Convert.ToInt64(o.Value);

            if (pi.PropertyType == typeof (Unit))
                val = Unit.Parse(o.Value);

            if (pi.PropertyType.IsEnum)
                val = Enum.Parse(pi.PropertyType, o.Value);

            pi.SetValue(c, val, null);
        }
        catch
        {
        }
    }

    private Dictionary<string, Control> _recursiveFind(Control rootControl, List<msPortalControlPropertyOverride> overrideList)
    {
        var controlList = new Dictionary<string, Control>();
        var overrideNames = overrideList.Select(o => o.ControlName).ToList();

        if (rootControl != null && overrideList.Count > 0)
        {
            _recursiveFind(rootControl, overrideNames, controlList);
        }

        return controlList;
    }

    private void _recursiveFind(Control parentControl, List<string> overrideNames, Dictionary<string, Control> controlList)
    {
        // If this control is one of the ones to override, add it to the Dictionary
        if (overrideNames.Contains(parentControl.ID))
        {
            if (!controlList.ContainsKey(parentControl.ID))
            {
                controlList.Add(parentControl.ID, parentControl);
            }
        }

        // If the Dictionary lenth is the same as the Overrides list, we have found all of our controls
        //if (controlList.Count >= overrideNames.Count) - this fails because some overrides are custom (like __PageTitle)
        //{
        //    return;
        //}

        foreach (Control cc in parentControl.Controls)
        {
            _recursiveFind(cc, overrideNames, controlList);
        }
    }


    protected virtual void setupHyperLinks()
    {
        
    }

    #region Events

    /// <summary>
    /// Notifies the server control that caused the postback that it should handle an incoming postback event.
    /// </summary>
    /// <param name="sourceControl">The ASP.NET server control that caused the postback. This control must implement the <see cref="T:System.Web.UI.IPostBackEventHandler"/> interface.</param>
    /// <param name="eventArgument">The postback argument.</param>
    protected override void RaisePostBackEvent(IPostBackEventHandler sourceControl, string eventArgument)
    {
        // We want the postback event to get raised normally - however, if there is an error in an API call, we
        // need to catch it and display a simple banner message, and return control to the user
        // this keeps us from having to write logic to check to Success of an API call every time we make a call

        try
        {
            base.RaisePostBackEvent(sourceControl, eventArgument);
        }
        catch (ConciergeClientException ex)
        {
            DisplayBannerMessage(true, ex.Message);
        }
    }

    #endregion


    #region Security-related methods

    /// <summary>
    /// Checks to see if user is logged in.
    /// </summary>
    private void checkToSeeIfUserIsLoggedIn()
    {
        if (IsPublic)
            return;     // it doesn't matter

        if (ConciergeAPI.CurrentUser == null || ConciergeAPI.CurrentEntity == null)   // we're not logged in
        {
            // is there an auto login set?
            // the auto login will automatically try to log in the user specified in the config
            // this is helpful during development, where you don't want to constantly have to
            // relogin for each compile

 
            if (ConfigurationManager.AppSettings["AutoLoginName"] != null)
            {
                using (var api = GetConciegeAPIProxy())
                    ConciergeAPI.SetSession(
                        api.LoginToPortal(ConfigurationManager.AppSettings["AutoLoginName"],
                                          ConfigurationManager.AppSettings["AutoLoginPassword"]).ResultValue);
             MultiStepWizards.ClearAll();    
            return;
            }
 

            GoTo("/Login.aspx?redirectURL=" + Server.UrlEncode(  Request.Url.PathAndQuery ) );
            return;
        }

        if (CheckMustChangePassword && ConciergeAPI.CurrentUser.MustChangePassword)
        {
            GoTo("~/profile/ChangePassword.aspx");
            return;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this page is public, meaning you don't
    /// have to be logged in to access it.
    /// </summary>
    /// <value><c>true</c> if this instance is public; otherwise, <c>false</c>.</value>
    protected virtual bool IsPublic { get { return false; } }

    /// <summary>
    /// Gets a value indicating whether this page is printable.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is printable; otherwise, <c>false</c>.
    /// </value>
    public virtual bool IsPrintable { get { return true ; } }

    protected virtual bool CheckMustChangePassword { get { return true; } }

    #endregion

    #region API-related Methods

    /// <summary>
    /// Determines whether [is module active] [the specified module to check].
    /// </summary>
    /// <param name="moduleToCheck">The module to check.</param>
    /// <returns>
    /// 	<c>true</c> if [is module active] [the specified module to check]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsModuleActive(string moduleToCheck)
    {
        bool result = ConciergeAPI.IsModuleActive(moduleToCheck);
        return result;
    }

    /// <summary>
    /// Gets the conciege API proxy.
    /// </summary>
    /// <returns></returns>
    /// <remarks>We</remarks>
    public IConciergeAPIService GetConciegeAPIProxy()
    {
        return ConciergeAPIProxyGenerator.GenerateProxy();
    }

    public msEntity CurrentEntity {get { return ConciergeAPI.CurrentEntity; }}
    public msPortalUser CurrentUser { get { return ConciergeAPI.CurrentUser; } }
    public msAssociation CurrentAssociation { get { return ConciergeAPI.CurrentAssociation; } }

    protected Address CurrentEntityPreferredAddress
    {
        get {
            if (CurrentEntity == null || CurrentEntity.Addresses == null || CurrentEntity.Addresses.Count == 0 ) return null;
            var preferred = CurrentEntity.Addresses.Find(x => x.Type == CurrentEntity.PreferredAddressType);
            if (preferred != null) return preferred.Address ;
            return CurrentEntity.Addresses[0].Address ;
        }
        
    }

    public int SelectedPage
    {
        get 
        { 
            int i;

            return !Int32.TryParse(Request.QueryString["page"], out i) || i < 1 ? 1 : i;
        }
    }

    public int PageStart { get; set; }

    #endregion

    #region Banner Messaging

    /* There is an informational banner on the GeneralPage.master master page that
     * we use for messages or general errors. The methods in that region manage
     * the banner. 
     * 
     * The way it works is that a Session variable called BannerMessage is set.
     * Every time a page renders, it looks for this variable and if it finds it,
     * it unhides the banner and displays the message. If the BannerMessageType
     * is set to "Error", it will change the background color to red. Since the Panel
     * has its viewstate disabled, it will automatically be hidden the next time the page
     * is called.
     * */

    /// <summary>
    /// Puts an informational error in the queue to be displayed the next time the page renders
    /// </summary>
    /// <param name="msg">The first error message.</param>
    public void QueueBannerError(string msg)
    {
        SessionManager.Set("BannerMessage", msg);
        SessionManager.Set("BannerMessageType", "Error");
    }

    /// <summary>
    /// Puts an informational message in the queue to be displayed the next time the page renders
    /// </summary>
    /// <param name="msg">The first error message.</param>
    public void QueueBannerMessage(string msg)
    {
        SessionManager.Set("BannerMessage", msg);
        SessionManager.Set("BannerMessageType", "Success");
    }

    public void QueueBannerMessage(string msg, params object[] args)
    {
        QueueBannerMessage(string.Format(msg, args));
    }

    /// <summary>
    /// Dequeues the banner message if it's present and displays it
    /// </summary>
    private void dequeueBannerMessageIfPresent()
    {
        var msg = SessionManager.Get<string>("BannerMessage");
        if (msg == null) return;    // no message
        bool isError = (string) SessionManager.Get<string>("BannerMessageType") == "Error";

        DisplayBannerMessage(isError, msg);
    }

    public void DisplayBannerMessage(bool isError, string msg, params object[] args)
    {
        DisplayBannerMessage(isError, string.Format(msg, args));
    }

    /// <summary>
    /// Displays the banner message.
    /// </summary>
    /// <param name="isError">if set to <c>true</c> [is error].</param>
    /// <param name="msg">The MSG.</param>
    public void DisplayBannerMessage(bool isError, string msg)
    {
        if (Master == null) return;

        var master = Master;
        if (master.Master != null)
        {
            master = master.Master;
        }

        // let's pull the message from the
        var pnl = master.FindControl("pnlMessage") as Panel;
        var lMessage = master.FindControl("lMessage") as Literal;

        if (pnl == null || lMessage == null) return; // we should be able to find it, but we can't

        pnl.Visible = true;
        lMessage.Text = msg;
        if (isError)
            pnl.BackColor = Color.Red;

        SessionManager.Set<object>("BannerMessage", null); // remove it from the queue
    }

    #endregion

    #region API Helpers
    
    // Leaving here to prevent mass updates to pages calling this
    protected T LoadObjectFromAPI<T>(string id) where T:msAggregate 
    {
        return APIExtensions.LoadObjectFromAPI<T>(id);
    }

    protected MemberSuiteObject CreateNewObject(string className)
    {
        return MemberSuiteObject.FromClassMetadata(MetadataLogic.DescribeObject(className));
    }

    protected T CreateNewObject<T>() where T : msAggregate, new()
    {
        var instance = new T();
        return CreateNewObject(instance.ClassType).ConvertTo<T>();
    }



    protected MemberSuiteObject SaveObject(MemberSuiteObject msoObjectToSave)
    {
        using (var api = GetConciegeAPIProxy())
        {
            var result = api.Save(msoObjectToSave);
            return result.ResultValue;
        }
    }

    protected T SaveObject<T>(T msoObjectToSave) where T : msAggregate
    {
        using (var api = GetConciegeAPIProxy())
        {
            var result = api.Save(msoObjectToSave);
            return result.ResultValue == null ? null : result.ResultValue.ConvertTo<T>();
        }
    }

    /// <summary>
    /// Gets all objects from the API of a certain type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objectType">Type of the object.</param>
    /// <returns></returns>
    protected List<T> GetAllObjects<T>( string objectType ) where T:msAggregate 
    {
        using (var api = GetServiceAPIProxy())
        {
            return GetAllObjects<T>(api, objectType);
        }
    }

    /// <summary>
    /// Gets all objects from the API of a certain type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objectType">Type of the object.</param>
    /// <returns></returns>
    protected List<T> GetAllObjects<T>(IConciergeAPIService proxy, string objectType) where T : msAggregate
    {
        List<MemberSuiteObject> result = new List<MemberSuiteObject>();
        int totalCount = Int32.MaxValue;

        //Add an emergency break
        Search s = new Search(objectType);
        while (result.Count < totalCount)
        {
            
            var queryResult = proxy.GetObjectsBySearch( s, null, result.Count, null).ResultValue ;
            result.AddRange(queryResult.Objects);

            totalCount = queryResult.TotalRowCount;
        }

        return (from r in result select r.ConvertTo<T>()).ToList();
    }

    /// <summary>
    /// Describes the products, getting the price for the current customer
    /// </summary>
    /// <param name="productsToDescribe">The products to describe.</param>
    /// <returns></returns>
    protected List<ProductInfo> DescribeProducts(List<string> productsToDescribe)
    {
        return DescribeProducts(CurrentEntity.ID, productsToDescribe);
    }

    protected List<ProductInfo> DescribeProducts(string targetEntityId, List<string> productsToDescribe)
    {
        using (var api = GetConciegeAPIProxy())
        {
            var result = api.DescribeProducts(targetEntityId, productsToDescribe);
            return result.ResultValue;
        }
    }

    protected Search GetOrganizationalLayerLeaderSearch(string organizationalLayerId)
    {
        Search sLeaders = GetBaseLeaderSearch();
        sLeaders.Type = "OrganizationalLayerLeader";
        sLeaders.ID = "OrganizationalLayerLeader";
        sLeaders.AddCriteria(Expr.Equals("OrganizationalLayer", organizationalLayerId));

        return sLeaders;
    }

    protected Search GetChapterLeaderSearch(string chapterId)
    {
        Search sLeaders = GetBaseLeaderSearch();
        sLeaders.Type = "ChapterLeader";
        sLeaders.ID = "ChapterLeader";
        sLeaders.AddCriteria(Expr.Equals("Chapter", chapterId));

        return sLeaders;
    }

    protected Search GetSectionLeaderSearch(string sectionId)
    {
        Search sLeaders = GetBaseLeaderSearch();
        sLeaders.Type = "SectionLeader";
        sLeaders.ID = "SectionLeader";
        sLeaders.AddCriteria(Expr.Equals("Section", sectionId));

        return sLeaders;
    }

    protected Search GetBaseLeaderSearch()
    {
        Search sLeaders = new Search();
        sLeaders.AddOutputColumn("CanViewMembers");
        sLeaders.AddOutputColumn("CanCreateMembers");
        sLeaders.AddOutputColumn("CanManageDocuments");
        sLeaders.AddOutputColumn("CanDownloadRoster");
        sLeaders.AddOutputColumn("CanMakePayments");
        sLeaders.AddOutputColumn("CanManageCommittees");
        sLeaders.AddOutputColumn("CanManageEvents");
        sLeaders.AddOutputColumn("CanManageLeaders");
        sLeaders.AddOutputColumn("CanUpdateContactInfo");
        sLeaders.AddOutputColumn("CanUpdateInformation");
        sLeaders.AddOutputColumn("CanUpdateMembershipInfo");
        sLeaders.AddOutputColumn("CanViewAccountHistory");
        sLeaders.AddOutputColumn("CanModerateDiscussions");
        sLeaders.AddOutputColumn("CanLoginAs");
        sLeaders.AddCriteria(Expr.Equals("Individual", ConciergeAPI.CurrentEntity.ID));
        sLeaders.AddSortColumn("Individual");

        return sLeaders;
    }

    protected msMembershipLeader ConvertLeaderSearchResult(SearchResult searchResult)
    {
        if (searchResult == null || searchResult.Table == null || searchResult.Table.Rows.Count == 0)
            return null;

        return MemberSuiteObject.FromDataRow(searchResult.Table.Rows[0]).ConvertTo<msMembershipLeader>();
    }

    protected void ParseSearchCriteria(List<FieldMetadata> targetCriteriaFields, MemberSuiteObject criteria, SearchBuilder sb)
    {
        foreach (var fieldMetadata in targetCriteriaFields)
        {
            string fieldName = Formats.GetSafeFieldName(fieldMetadata.Name);
            bool found = false;

            if (_isValidRange(criteria.SafeGetValue(fieldName + "__from")))
            {
                // let's create a greater than
                sb.AddOperation(Expr.IsGreaterThanOrEqualTo(fieldMetadata.Name, criteria[fieldName + "__from"]), SearchOperationGroupType.And);
                found = true;
            }

            if (_isValidRange(criteria.SafeGetValue(fieldName + "__to")))
            {
                // let's create a less than
                sb.AddOperation(Expr.IsLessThanOrEqual(fieldMetadata.Name, criteria[fieldName + "__to"]), SearchOperationGroupType.And);
                found = true;
            }

            if (found || !criteria.Fields.ContainsKey(fieldName))
                continue;

            var fieldValue = criteria.Fields[fieldName];
            if (fieldValue == null)
                continue;

            SearchOperation so = new Equals(); //default
            so.ValuesToOperateOn = new List<object> { fieldValue };

            if (fieldValue is string)
            {
                string fieldValueAsString = (string)fieldValue;
                if (String.IsNullOrWhiteSpace(fieldValueAsString)) continue;

                if (fieldMetadata.DataType == FieldDataType.Boolean)
                {
                    bool b = false;
                    if (Boolean.TryParse(fieldValueAsString, out b))
                        so.ValuesToOperateOn = new List<object>() { b };
                }
                else
                {
                    so = new Contains(); //string should use contains instead of equals
                    so.ValuesToOperateOn = new List<object>() { fieldValueAsString.Trim() };
                }
            }

            if (fieldValue is List<string>)
            {
                List<string> fieldValueAsList = fieldValue as List<string>;
                if (fieldValueAsList.Count == 0)
                    continue;

                so = new ContainsOneOfTheFollowing(); //Lists just look for one hit
                List<object> values = new List<object>();
                values.AddRange(fieldValueAsList);

                so.ValuesToOperateOn = values;
            }

            so.FieldName = fieldMetadata.Name;

            sb.AddOperation(so, SearchOperationGroupType.And);
        }
    }

    private bool _isValidRange(object valueoCheck)
    {
        if (valueoCheck == null)
            return false;

        string s = valueoCheck as string;
        if (s != null && String.IsNullOrWhiteSpace(s))
            return false;

        return true;
    }

    #endregion

    #region Navigation

    protected void GoHome()
    {
        GoTo("/default.aspx");
    }

    /// <summary>
    /// Navigates home, displaying a banner success message
    /// </summary>
    /// <param name="successMessage">The success message.</param>
    protected void GoHome(string successMessage)
    {
        QueueBannerMessage(successMessage);
        GoHome();

    }

    protected string GetSearchResult( DataRow dr, string fieldName)
    {
        return GetSearchResult(dr, fieldName,  null );
    }

    protected string GetSearchResult( DataRow dr, string fieldName, string formatString)
    {
        if (dr == null) return null;
        if (!dr.Table.Columns.Contains(fieldName)) return "(column not in search)";

        object obj = dr[fieldName];
        if (obj == DBNull.Value) return null;

        if (formatString == null) return Convert.ToString(obj);

        return String.Format("{0:" + formatString + "}", obj);
    }

    #endregion


    #region ControlHost Implementation

    // These methods are required for dynamic controls to render
    // properly

    public object Resolve(ControlMetadata cMeta)
    {
        return null;
    }

    public string ResolveResource(string resourceName)
    {
        return ResolveResource(resourceName, false);
    }

    public string ResolveResource(string resourceName, bool returnNullIfNothingFound)
    {
        //Check for a local resource file first so that any settings there take precedence
        try
        {
            var localResourceObject = GetLocalResourceObject(resourceName);
            if (localResourceObject != null)
            {
                string result = localResourceObject.ToString();
                if (!String.IsNullOrWhiteSpace(result))
                    return result;
            }
        }
        catch{}

        //There was no local resource file or no resource matching the specified name so check from the API
        return ConciergeAPI.ResolveResource(resourceName, returnNullIfNothingFound);
    }

    public string ResolveComplexExpression(string complexExpression)
    {
        return complexExpression;
    }


    /// <summary>
    /// Gets the field metadata for the field that is bound
    /// to this control, if possible.
    /// </summary>
    /// <returns></returns>
    public FieldMetadata GetBoundFieldFor(ControlMetadata controlSpec)
    {

        return null;
    }


    public IConciergeAPIService GetServiceAPIProxy()
    {
        return GetConciegeAPIProxy();
    }

    public void SetModelValue(ControlMetadata ControlMetadata, object valueToSet)
    {
         
    }

    public void SetModelValue(ControlMetadata ControlMetadata, object valueToSet, bool onlyIfMemberSuiteObject)
    {
        
    }

    public object ResolveAcceptableValues(ControlMetadata ControlMetadata)
    {
        return null;
    }

    public SearchManifest DescribeSearch(string searchType, string searchContext)
    {
        using (var api = GetConciegeAPIProxy())
        {
            var result = api.DescribeSearch(searchType, searchContext);
            return result.ResultValue;
        }
    }

    public TimeZoneInfo GetCurrentTimeZone()
    {
        if ( CurrentUser == null )
            return TimeZoneInfo.Local;

        var tzo = TimeZoneInfo.FindSystemTimeZoneById( CurrentUser.TimeZone );
        if ( tzo!= null )
            return tzo;

        return TimeZoneInfo.Local;

    }

    #endregion

    /// <summary>
    /// Redirects to another page
    /// </summary>
    /// <param name="newPageLocation">The new page location.</param>
    /// <remarks>All redirects in the portal should use the base method. It provides an important layer
    /// of abstraction that can enable, among other things, redirect substitution based on 
    /// information in the association settings.</remarks>
    protected void GoTo(string newPageLocation)
    {
        GoTo(newPageLocation,  null );
    }

    /// <summary>
    /// Redirects to another page
    /// </summary>
    /// <param name="newPageLocation">The new page location.</param>
    /// <remarks>All redirects in the portal should use the base method. It provides an important layer
    /// of abstraction that can enable, among other things, redirect substitution based on 
    /// information in the association settings.</remarks>
    protected void GoTo(string newPageLocation, string successMessage)
    {
        if (successMessage != null)
            QueueBannerMessage(successMessage);

        Response.Redirect(newPageLocation);
    }

    /// <summary>
    /// Refreshed the current page using the Request.Url. 
    /// </summary>
    /// <remarks>
    /// Will refresh the entire page - no AJAX
    /// </remarks>
    protected void Refresh()
    {
        Page.Response.Redirect(Page.Request.Url.ToString(), true);
    }

    #region Javascript
    /// <summary>
    /// Registers the javascript confirmation box.
    /// </summary>
    /// <param name="wc">The wc.</param>
    /// <param name="confirmationText">The confirmation text.</param>
    /// <param name="args">The args.</param>
    public void RegisterJavascriptConfirmationBox(WebControl wc, string confirmationText)
    {
        if (!wc.Visible)
            return;					// don't do anything to invisible controls


       
        confirmationText = confirmationText.Replace("'", @"\'");

        wc.Attributes["onclick"] += "; return confirm('" + confirmationText + "');";

    }
    #endregion

    public static string GetImageUrl(string imageID)
    {
        return String.Format("{0}/{1}/{2}/{3}", ConfigurationManager.AppSettings["ImageServerUri"],
                                       PortalConfiguration.Current.AssociationID,
                                       PortalConfiguration.Current.PartitionKey,
                                       imageID);
    }

    public static bool setProfileImage(Image image, DataRow dataRow)
    {
        if (!dataRow.Table.Columns.Contains("Image") || dataRow["Image"] == DBNull.Value)
        {
            image.Visible = false;
            return false;
        }

        return setProfileImage(image, dataRow["Image"].ToString());
    }

    public static bool setProfileImage(Image image, DataRowView dataRowView)
    {
        if (!dataRowView.Row.Table.Columns.Contains("Image") || dataRowView["Image"] == DBNull.Value)
        {
            image.Visible = false;
            return false;
        }

        return setProfileImage(image, dataRowView["Image"].ToString());
    }

    public static bool setProfileImage(Image image, msEntity entity)
    {
        return setProfileImage(image, entity.Image);
    }

    public static bool setProfileImage(Image image, string imageId)
    {
        if (String.IsNullOrWhiteSpace(imageId))
        {
            image.Visible = false;
            return false;
        }

        string imageUrl = GetImageUrl(imageId);

        if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            return false;

        image.ImageUrl = imageUrl;
        //img.BorderWidth = Unit.Empty;

        //If a new image has been uploaded the browser will still have the old one cached.
        //We can force the browser to retrieve the new image from the MemberSuite Image content server by changing the URL.  
        //By changing the case of the URL the browser will accurately interpret it as a new URL with a potentially 
        //different image and will initiate the request to the MemberSuite Image content server for the new image content.  
        //The MemberSuite Image content server is case insensitive and so will return the same image content regardless of case.
        
        if (SessionManager.Get<bool>("ImageUrlUpper"))
            image.ImageUrl = image.ImageUrl.ToUpper();
        
        return true;
    }

    protected ClassMetadata createClassMetadata(List<FieldMetadata> fields)
    {
        var result = new ClassMetadata { Fields = new List<FieldMetadata>() };
        foreach (var field in fields)
        {
            var newField = field.Clone();
            newField.Name = Formats.GetSafeFieldName(newField.Name);
            result.Fields.Add(newField);
        }
        return result;
    }

    protected DataEntryViewMetadata createViewMetadata(List<FieldMetadata> fields)
    {
        return createViewMetadata(fields, false);
    }

    protected DataEntryViewMetadata createViewMetadata(List<FieldMetadata> fields, bool isForSearch)
    {
        DataEntryViewMetadata result = new DataEntryViewMetadata();
        ViewMetadata.ControlSection baseSection = new ViewMetadata.ControlSection();
        baseSection.SubSections = new List<ViewMetadata.ControlSection>();

        var currentSection = new ViewMetadata.ControlSection();
        currentSection.LeftControls = new List<ControlMetadata>();
        baseSection.SubSections.Add(currentSection);
        foreach (var field in fields)
        {
            if (field.DisplayType == FieldDisplayType.Separator)
            {
                if (currentSection.LeftControls != null && currentSection.LeftControls.Count > 0)
                // we have to create a new section
                {
                    currentSection = new ViewMetadata.ControlSection();
                    currentSection.LeftControls = new List<ControlMetadata>();
                    baseSection.SubSections.Add(currentSection);
                }

                currentSection.Name = Guid.NewGuid().ToString();


                currentSection.Label = field.PortalPrompt ?? field.Label; // set the section label

                continue;
            }


            // // MS-1251 - REQUIREMENT to have to/from fields for numeric fields

            var control = ControlMetadata.FromFieldMetadata(field);

            if (isForSearch)
                control.PortalPrompt = null;
            switch (control.DataType)
            {
                case FieldDataType.Integer:
                case FieldDataType.Decimal:
                case FieldDataType.Date:
                case FieldDataType.DateTime:

                    // we need to use a range
                    var controlTo = control.Clone();
                    control.Label += " - Range Start";
                    control.Name += "__from";
                    control.DataSourceExpression += "__from";

                    currentSection.LeftControls.Add(control);


                    controlTo.Label += " - Range End";
                    controlTo.Name += "__to";
                    controlTo.DataSourceExpression += "__to";


                    currentSection.LeftControls.Add(controlTo);
                    break;

                default:
                    currentSection.LeftControls.Add(control);
                    break;
            }


        }
        result.Sections = new List<ViewMetadata.ControlSection> { baseSection };
        return result;
    }

    protected void CheckForDemographicsAndRedirectIfNecessary( msOrderLineItem lineItem, string redirectUrl)
    {
        using (var api = GetConciegeAPIProxy())
        {
           
            var pp = api.PreProcessOrder(MultiStepWizards.PlaceAnOrder.ShoppingCart).ResultValue;

            if (pp == null) return; //defensive programming
            if (pp.FinalizedOrder == null) return;
            if (pp.ProductDemographics == null || pp.ProductDemographics.Count == 0)
                return;

            // we have to account for line items that may be inserted (like taxes/discounts), so let's find
            // out the index of our new lineitem
            var fo = pp.FinalizedOrder.ConvertTo<msOrder>();

            if (fo.LineItems == null) return;
            var i = fo.LineItems.FindIndex(x => x.OrderLineItemID == lineItem.OrderLineItemID);
            if (i < 0) return;

            if (pp.ProductDemographics.Count <= i) return;

            var list = pp.ProductDemographics[i];
            if (list != null && list.Count > 0)   // we have demographics!
            {
                MultiStepWizards.PlaceAnOrder.EditOrderLineItem = lineItem;
                MultiStepWizards.PlaceAnOrder.EditOrderLineItemProductDemographics = list;
                MultiStepWizards.PlaceAnOrder.EditOrderLineItemProductName = api.GetName(lineItem.Product).ResultValue;
                MultiStepWizards.PlaceAnOrder.EditOrderLineItemRedirectUrl = redirectUrl;

                GoTo("/orders/EditOrderLineItem.aspx", "Item successfully added to cart.");
            }

            return;
 
        }
    }

    protected List<LoginResult.AccessibleEntity> getRelatedOrganizations(string entityId)
    {
        using (IConciergeAPIService proxy = GetConciegeAPIProxy())
        {
            var result = proxy.GetAccessibleEntitiesForEntity(entityId);
            if (result == null || result.ResultValue == null || result.ResultValue.Count == 0)
                return null;

            return result.ResultValue.Where(x => x.Type == msOrganization.CLASS_NAME && !x.ID.Equals(ConciergeAPI.CurrentEntity.ID, StringComparison.CurrentCultureIgnoreCase)).ToList();
        }
    }

    protected void SetCurrentPageFromResults(SearchResult searchResult, HyperLink firstPageLink, HyperLink previousPageLink, HyperLink nextPageLink, Literal lNumPages, Literal lNumResults, Literal lStartResult, Literal lEndResult, Literal lCurrentPage)
    {
        int pageCount = searchResult.TotalRowCount / PAGE_SIZE;
        if (searchResult.TotalRowCount % PAGE_SIZE > 0)
            pageCount++;

        if (searchResult.TotalRowCount == 0)
            pageCount = 1;

        int endRec = PageStart + PAGE_SIZE;
        if (endRec > searchResult.TotalRowCount)
            endRec = searchResult.TotalRowCount;

        if(lNumPages != null) lNumPages.Text = (pageCount).ToString("N0");
        if (lNumResults != null) lNumResults.Text = searchResult.TotalRowCount.ToString("N0");
        if (lStartResult != null) lStartResult.Text = (PageStart + 1).ToString("N0");
        if (lEndResult != null) lEndResult.Text = endRec.ToString("N0");
        if (lCurrentPage != null) lCurrentPage.Text = ((PageStart / PAGE_SIZE) + 1).ToString("N0"); ;

        string currentUrl = Request.Url.LocalPath;
        currentUrl += Formats.GetQueryString(new NameValueCollection(Request.QueryString), new[] { "page" });

        if (firstPageLink != null)
        {
            firstPageLink.NavigateUrl = String.Format("{0}page=1", currentUrl);
            firstPageLink.Visible = previousPageLink.Visible = PageStart != 0;
        }

        if (previousPageLink != null)
        {
            previousPageLink.NavigateUrl = String.Format("{0}page={1}", currentUrl, (SelectedPage - 1));
        }

        if (nextPageLink != null)
        {
            nextPageLink.NavigateUrl = String.Format("{0}page={1}", currentUrl, (SelectedPage + 1));
            nextPageLink.Visible = endRec < searchResult.TotalRowCount;
        }
    }

    protected virtual void AddCustomOverrideEligibleControls(List<msPortalControlPropertyOverride> eligibleControls)
    {
    }
}
