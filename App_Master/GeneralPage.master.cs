using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Types;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using System.Data;
using System.Configuration;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Utilities;

/// <summary>
/// 
/// </summary>
public partial class App_Master_GeneralPage : System.Web.UI.MasterPage
{
    #region Properties

    public bool HideHomeBreadcrumb
    {
        get; set;
    }

    public bool LogoutRequested
    {
        get
        {
            bool result;
            return bool.TryParse(Request.QueryString["logout"], out result) && result;
        }
    }

    #endregion

    #region Version

    
    public string GetVersion()
    {

        return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        
    }

    #endregion

    protected void lbBackgroundUser_Click(object sender, EventArgs e)
    {
        Logout(!ConciergeAPI.HasBackgroundConsoleUser);
        Response.Redirect(ConciergeAPI.ConsoleReturnUrl);
    }

    protected void lblLogout_Click(object sender, EventArgs e)
    {
        Logout();
    }

    private void Logout(bool redirect)
    {
        ConciergeAPI.ClearSession();
        SessionManager.Set<object>("PortalLinks", null );  // force portal link reload, so non-public links don't show
        if (redirect)
            Response.Redirect(!string.IsNullOrWhiteSpace(ConciergeAPI.LogoutUrl)
                                  ? ConciergeAPI.LogoutUrl
                                  : "~/Login.aspx");
    }

    private void Logout()
    {
        Logout(true);
    }

    

    protected void Page_Load(object sender, EventArgs e)
    {
        if (LogoutRequested)
            Logout(true);

        if (!IsPostBack)
        {
           populateInfoBanner();
           setBannerGraphic();
            applySkinIfApplicable();
            applyCSSIfApplicable();
            populatePortalLinks();
            liOnlineStore.Visible = PortalConfiguration.Current.OnlineStorefrontEnabled;
            liUpcomingEvents.Visible = PortalConfiguration.Current.ShowUpcomingEventsTab;
            hlHome.Visible = !HideHomeBreadcrumb;

            if (isPageCustomizationAllowed())
                lbCustomize.Visible = true;

            if (ConciergeAPI.CurrentEntity == null)   // make it login
                lHomeText.Text = "Login";

            lblVersion.Text = GetVersion();

            makeScreenPrintableIfNecessary();
        }
    }

    private void makeScreenPrintableIfNecessary()
    {
        if (Request.QueryString["print"] != "true") return;
        SkinHeaderContent.Text = "";
        SkinFooterContent.Text = "";
        phTabs.Visible = false;
        phBottomFooter.Visible = false;

        // hide the drop shadow
        phNoDropShadowStyles.Visible = true;
        phDropShadowStyles.Visible = false;
    }

    private void applyCSSIfApplicable()
    {
        phNoDropShadowStyles.Visible = PortalConfiguration.Current.HideDropShadow;
        phDropShadowStyles.Visible = !phNoDropShadowStyles.Visible;

        lInlineStyles.Text = PortalConfiguration.Current.Css;
    }

    /// <summary>
    /// Determines whether page customization allowed.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if [is page customization allowed]; otherwise, <c>false</c>.
    /// </returns>
    private bool isPageCustomizationAllowed()
    {
        // page customization is allowed if there's a background, logged in user
        if (Request.Url.LocalPath.Contains("/admin/"))
            return false;

        //return true;
        
        // you can customize the portal if you're logged in from the console,
        // OR if there's an auto login (meaning, we're in development mode
        return ConciergeAPI.HasBackgroundConsoleUser ||
            ConfigurationManager.AppSettings["AutoLoginName"] != null;
    }

    private void setBannerGraphic()
    {
        if (PortalConfiguration.Current == null || PortalConfiguration.Current.PortalGraphicHeaderUrl == null)
        {
            SkinHeaderContent.Text = SkinHeaderContent.Text.Replace("{homepageurl}", "").Replace("{imageurl}",
                                                                                                 "/images/samples/bg_header_banner.gif");
            return;
        }

        // for now, the API won't generate a URL for an image/file. But that should change. 
        // So, we'll deal with both cases - the case in which we get a guid back (and we have to figure out the URL),
        // and the case where we get a URL

        // are we dealing with a GUID?
        string imageUrl;
        if (RegularExpressions.GuidRegex.IsMatch(PortalConfiguration.Current.PortalGraphicHeaderUrl))
            imageUrl = string.Format("{0}/{1}/{2}/{3}", ConfigurationManager.AppSettings["ImageServerUri"],
                                     PortalConfiguration.Current.AssociationID, PortalConfiguration.Current.PartitionKey,
                                     PortalConfiguration.Current.PortalGraphicHeaderUrl);
        else
            imageUrl = PortalConfiguration.Current.PortalGraphicHeaderUrl;

        string homePageUrl = "#";
        if (!string.IsNullOrWhiteSpace(PortalConfiguration.Current.AssociationHomePageUrl))
            homePageUrl = PortalConfiguration.Current.AssociationHomePageUrl;

        SkinHeaderContent.Text = SkinHeaderContent.Text.Replace("{homepageurl}", homePageUrl).Replace("{imageurl}",
                                                                                                      imageUrl);

    }

    private void applySkinIfApplicable()
    {
        //If no skin is defined return null
        if (PortalConfiguration.Current.PortalSkin == null)
            return;

        //If a skin is defined it is by definition valid and therefore null headers and footers are also valid.
        //So no null/empty checks here.
        this.SkinHeaderContent.Text = PortalConfiguration.Current.PortalSkin.Header;
        this.SkinFooterContent.Text = PortalConfiguration.Current.PortalSkin.Footer;
    }

    /// <summary>
    /// Populates the info banner at the top of the screen if we're logged in -
    /// otherwise, hides it
    /// </summary>
    private void populateInfoBanner()
    {
        if (ConciergeAPI.CurrentEntity == null)
        {
            phInfoBanner.Visible = false;
            return;
        }

      
        populateCurrentUser();
       
        // do we have a background, logged in user?
        lbBackgroundUser.Visible = false;

        //The back to console link should be visible if there is a background console user
        //OR a return URL as been specified since this link is customizable to people doing a SSO.
        if (ConciergeAPI.HasBackgroundConsoleUser || !string.IsNullOrWhiteSpace(ConciergeAPI.ConsoleReturnUrl))
        {
            if(!string.IsNullOrWhiteSpace(ConciergeAPI.ConsoleReturnText))
                lbBackgroundUser.Text = ConciergeAPI.ConsoleReturnText;
            
            lbBackgroundUser.Visible = true;
        }

        PortalPage pp = Page as PortalPage;
        if (pp == null || !pp.IsPrintable)
            hlPrintPage.Visible = false;
        else
        {
            var thisUrl = Request.Url.ToString();
            if (thisUrl.Contains("?"))
                thisUrl += "&";
            else
                thisUrl += "?";
            thisUrl += "print=true";
            hlPrintPage.NavigateUrl = thisUrl;
        }
    }

    private void populatePortalLinks()
    {
        System.Data.DataTable dt = getPortalLinkTable();

        if (dt.Rows.Count == 0) return; // nothing to do
        rptTabs.DataSource = dt;
        rptTabs.DataBind();
     
    }

    private DataTable getPortalLinkTable()
    {
        DataTable dt = SessionManager.Get<DataTable>("PortalLinks");
        if (dt != null) return dt;

        Search s = new Search("PortalLink");
        s.AddCriteria(Expr.Equals("IsActive", true));
        s.AddSortColumn("DisplayOrder");
        s.AddSortColumn("Name");
        s.AddOutputColumn( "Name" );
        s.AddOutputColumn("Type");
        s.AddOutputColumn("Event");
        s.AddOutputColumn("Competition");
        s.AddOutputColumn("Url");
        s.AddOutputColumn("ExpirationDate");
        s.AddOutputColumn("IsPublic");
        s.AddOutputColumn("MembersOnly");

         //Log in the anonymous user
        using (var serviceProxy = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            dt = serviceProxy.ExecuteSearch(s, 0, null).ResultValue.Table;
            
            //Do not logout because that will logout anyone doing impersonation resulting in a session expired error
            //serviceProxy.Logout();

       

        // remove expired links
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dr = dt.Rows[i];

                // ok, first, is it private?
                bool isPublicLink = dr["IsPublic"] != DBNull.Value ? (bool)dr["IsPublic"] : false;  
                bool isMembersOnly =   dr["MembersOnly"] != DBNull.Value ? (bool) dr["MembersOnly"] : false;
                if (ConciergeAPI.CurrentEntity == null)
                {
                    // if its NOT a public link, or it's a Members Only link, then remove it
                    if (!isPublicLink || isMembersOnly)
                    {
                        // remove it
                        dt.Rows.RemoveAt(i);
                        continue;
                    }
                }
                else
                    if (isMembersOnly)
                    {
                        // let's run a quick search here
                        Search ss = new Search(msIndividual.CLASS_NAME);
                        ss.AddCriteria(Expr.Equals("ID", ConciergeAPI.CurrentEntity.ID));
                        ss.AddOutputColumn("Membership.ReceivesMemberBenefits");


                        var dtMember = serviceProxy.ExecuteSearch(ss, 0, 1).ResultValue.Table;

                        if (dtMember == null || dtMember.Rows.Count == 0)
                        {
                            // remove it
                            dt.Rows.RemoveAt(i);
                            continue;
                        }

                        object o = dtMember.Rows[0]["Membership.ReceivesMemberBenefits"];
                        if (o == DBNull.Value || !Convert.ToBoolean(o)) // not a member
                        {
                            // remove it
                            dt.Rows.RemoveAt(i);
                            continue;
                        }
                    }
                if (Convert.IsDBNull(dr["ExpirationDate"])) // good
                    continue;
                DateTime dtExpiration = (DateTime) dr["ExpirationDate"];
                if (dtExpiration < DateTime.UtcNow)
                    dt.Rows.RemoveAt(i); // it's expired

               
            }



        }
        //dt.Columns.Add("Url");

        //if ( dt.Rows.Count > 0 )    // we have to 
        //{
        //    DataRow dr = dt.NewRow();
        //    dr["Url"] = "/default.aspx";
        //    dr["Name"] = "Home";
        //    dt.Rows.InsertAt( dr, 0 );
        //}

        SessionManager.Set("PortalLinks", dt );

        return dt;
            
       
    }

    private void populateCurrentUser()
    {
        lCurrentUserID.Text = ConciergeAPI.CurrentEntity.LocalID.ToString();
        lCurrentUserName.Text = ConciergeAPI.CurrentEntity.Name;

        //MS-1151
        //Moved logic to MyProfile.ascx.cs
        //var accessibleEntities = ConciergeAPI.AccessibleEntities;
        //if (accessibleEntities == null || accessibleEntities.Count == 0) return;    // done

        //// now, we have accessible entities!
        //lCurrentUserName.Visible = false;
        //ddlMultipleUsers.Visible = true;

        //foreach( var e in accessibleEntities )
        //    ddlMultipleUsers.Items.Add(new ListItem(string.Format("{0} ({1})", e.Name, e.Type), e.ID));

        //// set the value
        //ListItem li = ddlMultipleUsers.Items.FindByValue(ConciergeAPI.CurrentEntity.ID);
        //if (li == null)   // this shouldn't happen
        //{
        //    li = new ListItem(ConciergeAPI.CurrentEntity.Name, ConciergeAPI.CurrentEntity.ID); 
        //    ddlMultipleUsers.Items.Add(li);
        //}

        //ddlMultipleUsers.ClearSelection();
        //li.Selected = true;
        //ddlMultipleUsers.SelectedValue = ConciergeAPI.CurrentEntity.ID;

    }

    //MS-1151
    //Moved logic to MyProfile.ascx.cs
    //protected void ddlMultiplesUsers_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    msEntity newEntity = null ;
    //    using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
    //    {
    //        newEntity = api.Get(ddlMultipleUsers.SelectedValue).ResultValue.ConvertTo<msEntity>();


    //        if (newEntity == null) return; // entity was erased?

    //        ConciergeAPI.CurrentEntity = newEntity;

    //        // record that this was the last one logged in
    //        // let's re-load it from the database
    //        var pu = api.Get(ConciergeAPI.CurrentUser.ID).ResultValue.ConvertTo<msPortalUser>();
    //        pu.LastLoggedInAs = newEntity.ID;
    //        api.Save(pu);
    //    }

    //    MultiStepWizards.ClearAll();
    //    Response.Redirect("/default.aspx");
    //}

    protected void rptTabs_DataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView drv = (DataRowView)e.Item.DataItem;

        if (Page.IsPostBack)
            return;				// only do this if there's a postback - otherwise, preserve ViewState

        switch (e.Item.ItemType)
        {
            case ListItemType.Header:
                break;

            case ListItemType.Footer:
                break;

            case ListItemType.AlternatingItem:
                goto case ListItemType.Item;

            case ListItemType.Item:
                HyperLink hlPortalLink = (HyperLink)e.Item.FindControl("hlPortalLink");

                hlPortalLink.Text = string.Format("<span>{0}</span>", drv["Name"]);

                string type = Convert.ToString(drv["Type"]);

                switch (type)
                {
                    case "Html":
                        hlPortalLink.NavigateUrl = "/portalLink.aspx?contextID=" + drv["ID"];
                        break;

                    case "Event":
                        hlPortalLink.NavigateUrl = "/events/ViewEvent.aspx?contextID=" + drv["Event"];
                        break;

                    case "Competition":
                        hlPortalLink.NavigateUrl = "/Competitions/ViewCompetition.aspx?contextID=" + drv["Competition"];
                        break;

                    case "Link":
                        hlPortalLink.NavigateUrl = (string)drv["Url"];
                        break;

                }


                break;
        }
    }

    protected void lbCustomize_Click(object sender, EventArgs e)
    {
        // our job here is to get a list of controls on this page that have a Text or EmptyText property, and then pass control over to the
        // page customization logic
        List<msPortalControlPropertyOverride> eligibleControls = new List<msPortalControlPropertyOverride>();
        List<string> allControls = new List<string>();
        
        
        _populateEligibleControls(allControls, eligibleControls);


        MultiStepWizards.CustomizePage.ControlsEligibleForQuickOverride = eligibleControls;
        MultiStepWizards.CustomizePage.AllEligibleControls= allControls ;
        MultiStepWizards.CustomizePage.PageName = Request.Url.LocalPath;
        MultiStepWizards.CustomizePage.Referrer = Request.Url.ToString();

        Response.Redirect("/admin/ViewControlPropertyOverrides.aspx");

    }

    private void _populateEligibleControls(List<string> allControls, List<msPortalControlPropertyOverride> eligibleControls)
    {
        _populateEligibleControls( allControls, eligibleControls, this.Page);

        // now, populate the title
        LiteralControl lc = null;
        bool doNotAddControl = false;
        foreach (var c in PageTitle.Controls)
        {
            if (c is LiteralControl)
                lc = (LiteralControl)c;

            if (c is Literal) // there's already a literal
                doNotAddControl = true;
            }

        if (lc != null && ! doNotAddControl )
        {
            var ppo = new msPortalControlPropertyOverride
                          {
                              PageName = Request.Url.LocalPath,
                              ControlName = "__PageTitle",
                              PropertyName = "Text",
                              Value = lc.Text
                          };
            ppo["Type"] = typeof(LiteralControl).Name;

            eligibleControls.Add(ppo);
        }
    }


    /// <summary>
    /// Recursively populates the eligible controls.
    /// </summary>
    /// <param name="allControls"></param>
    /// <param name="eligibleControls">The eligible controls.</param>
    /// <param name="controlToStart">The control to start.</param>
    private void _populateEligibleControls( List<string> allControls, List<msPortalControlPropertyOverride> eligibleControls, Control controlToStart)
    {
        /* We won't bother for performance reasons
        if (eligibleControls == null) throw new ArgumentNullException("eligibleControls");
        if (controlToStart == null) throw new ArgumentNullException("controlToStart");
         * */

        foreach (Control c in controlToStart.Controls)
        {
            if ( _isOverrideable( c  )) // only override named controls
            {
                var piText = c.GetType().GetProperty("Text");

                if (piText != null)
                {
                    var ppo = new msPortalControlPropertyOverride
                                                              {
                                                                  PageName = Request.Url.LocalPath,
                                                                  ControlName = c.ID,
                                                                  PropertyName = "Text",
                                                                  Value = Convert.ToString(piText.GetValue(c, null))
                                                              };
                    ppo["Type"] = c.GetType().Name;

                    eligibleControls.Add( ppo );
                }
            }

            if ( ! string.IsNullOrWhiteSpace( c.ID ) )
                allControls.Add(c.ID);

            if ( _shouldDrillDownOn( c ) )
            _populateEligibleControls( allControls, eligibleControls, c);
        }

      

        return;
    }

    private bool _shouldDrillDownOn(Control control)
    {
        if (control is GridView ||
            control is Repeater ||
            control is ListControl ||
            control is DataList ||
            control is DataGrid )
            return false;

        return true;
    }

    private bool _isOverrideable(Control control)
    {
        if (string.IsNullOrWhiteSpace(control.ID))
            return false;

        if (
            control.ID == "lbCustomize" ||
            control.ID == "SkinHeaderContent" ||
            control.ID == "SkinFooterContent" ||
            control.ID == "hlHome" ||
            control.ID == "lblLogout" ||
            control.ID == "lblVersion" ||
            control.ID == "lCurrentUserID" ||
            control.ID == "lCurrentUserName" ||
            control.ID == "lHomeText" ||
            control.ID == "lMessage" ||
            control.ID == "lbBackgroundUser")

            return false;

        if (control is BaseValidator)
            return false;

        // purposely don't include labels, as they are typically dynamic
        return  control is Literal || control is LinkButton || control is Button || control is HyperLink || control is CheckBox   ;

            


    }
}
