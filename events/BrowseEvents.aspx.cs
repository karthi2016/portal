using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class events_BrowseEvents : PortalPage
{
    const int MAX_DESCRIPTION_CHARS = 600;
    protected msEventCategory targetCategory;

    #region Fields

    protected DataView dvEvents;

    #endregion

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        if (ContextID == "null")
        {
            targetCategory = new msEventCategory();
            targetCategory.Name = "All Other Events";
            targetCategory.ID = "null";
        }
        else
            targetCategory = LoadObjectFromAPI<msEventCategory>(ContextID);
    }

    #region Intitialize Page

    protected override void InitializePage()
    {
        base.InitializePage();

        
        //Execute a search for events in the future and databind
        loadDataFromConcierge();


        // ok, first  - do we have a category? If not, let's see how many categories we've got
        if (targetCategory == null)
        {
            List<string> distinctCategories = new List<string>();
            foreach (DataRow dr in dvEvents.Table.Rows)
            {
                string category = Convert.ToString(dr["Category"]);
                if (!distinctCategories.Contains(category))
                    distinctCategories.Add(category);
            }

            if (distinctCategories.Count > 1) // we have to give them a choice!
            {
                rptEvents.Visible = false;
                var msEventCategories = _getCategoryListFor(distinctCategories);

                // sort 'em
                msEventCategories.Sort((x, y) => string.Compare(x.Name, y.Name));

                // now display them as a choice
                rptEventCategory.DataSource = msEventCategories;
                rptEventCategory.DataBind();
                pnlSelectType.Visible = true;
                lNoUpcomingEvents.Visible = false;
                return;
            }

        }



        if (dvEvents.Table.Rows.Count > 0)
        {
            lNoUpcomingEvents.Visible = false;
            rptEvents.DataSource = dvEvents;
            rptEvents.DataBind();
        }
        else
        {
            rptEvents.Visible = false;
            lNoUpcomingEvents.Visible = true;
        }

        if (targetCategory != null)
            PageTitleExtension.Text = string.Format(" - {0}", targetCategory.Name);
    }

    private List<msEventCategory> _getCategoryListFor(List<string> distinctCategories)
    {
        if (distinctCategories == null) throw new ArgumentNullException("distinctCategories");
        List<msEventCategory> categories = new List<msEventCategory>();

        foreach (var dc in distinctCategories)
        {
             

            if (dc == "")
            {
                msEventCategory c2 = new msEventCategory();
                c2.Name = "All Other Events";
                c2.Description = " - These are events that have not been assigned a category.";
                c2.ID = "null";
                categories.Add(c2);
                continue;
            }

            categories.Add(LoadObjectFromAPI<msEventCategory>(dc)); // just get it from the db
        }

        return categories;
    }

    #endregion

    protected override bool IsPublic
    {
        get
        {
            return true;
        }
    }
    #region Methods


    /// <summary>
    /// Executes a search against the Concierge API for events with a future start date that are set to be visible in the portal.
    /// </summary>
    /// <returns></returns>
    private void loadDataFromConcierge()
    {
     
        Search s = new Search { Type = msEvent.CLASS_NAME };

        s.AddOutputColumn("ID");
        s.AddOutputColumn("Name");
        s.AddOutputColumn("StartDate");
        s.AddOutputColumn("EndDate");
        s.AddOutputColumn("Category");
        s.AddOutputColumn("ShortSummary");
        s.AddOutputColumn("VisibleInPortal");
        s.AddOutputColumn("PostToWeb");
        s.AddOutputColumn("RemoveFromWeb");
        s.AddOutputColumn("Location_City");
        s.AddOutputColumn("Location_State");
        s.AddOutputColumn("DisplayStartEndDateTimesAs");
        s.AddOutputColumn("InviteOnly");
        s.AddOutputColumn("Url"); // CORE-1017
        s.AddSortColumn("StartDate", false );

        if (targetCategory != null)
            if (targetCategory.ID != "null")
                s.AddCriteria(Expr.Equals("Category", targetCategory.ID));
            else
                s.AddCriteria(Expr.IsBlank("Category"));

        // MS-1366 - If there's a background user, then we can show the non-visible - otherwise no
        if (!ConciergeAPI.HasBackgroundConsoleUser) // there's no background user
            s.AddCriteria(Expr.Equals("VisibleInPortal", 1));

        s.AddCriteria(Expr.DoesNotEqual("IsClosed", true));

           
        // it's not over
        s.AddCriteria(Expr.IsGreaterThan("EndDate", DateTime.Now));

        var searchResult = APIExtensions.GetSearchResult(s, 0, null);
        var dt = searchResult.Table;

        // now, let's pull upcoming exhibit shows that are NOT tied to an event
        if (IsModuleActive("Exhibits") || ConciergeAPI.CurrentEntity == null) // put this in as a hack since you can't check modules when not logged in
        {
            Search se = new Search(msExhibitShow.CLASS_NAME);
            se.AddOutputColumn("ID");
            se.AddOutputColumn("Name");
            se.AddOutputColumn("StartDate");
            se.AddOutputColumn("EndDate");
            se.AddOutputColumn("ShortSummary");
            se.AddOutputColumn("Category");
            se.AddOutputColumn("VisibleInPortal");
            se.AddOutputColumn("PostToWeb");
            se.AddOutputColumn("RemoveFromWeb");

            se.AddSortColumn("StartDate", false);

            if (!ConciergeAPI.HasBackgroundConsoleUser) // there's no background user
                se.AddCriteria(Expr.Equals("VisibleInPortal", 1));

           // it's not over
            se.AddCriteria(Expr.IsGreaterThan("EndDate", DateTime.Now));
            se.AddCriteria(Expr.IsBlank(msExhibitShow.FIELDS.Event)); // only shows with no events tied to them!

            if (targetCategory != null)
                if (targetCategory.ID != "null")
                    se.AddCriteria(Expr.Equals("Category", targetCategory.ID));
                else
                    se.AddCriteria(Expr.IsBlank("Category"));

            var shows = APIExtensions.GetSearchResult(se, 0, null).Table;
            
            foreach (DataRow dr in shows.Rows) // this ONLY works because the columns are the same!
                dt.ImportRow(dr);


        }

        DataTable dtInvitations= null;

        if ( ConciergeAPI.CurrentEntity != null )
        {
            Search sInvitees = new Search("EventInvitee");
            sInvitees.AddOutputColumn("Event");
            sInvitees.AddCriteria(Expr.Equals("Invitee", ConciergeAPI.CurrentEntity.ID));
            dtInvitations = APIExtensions.GetSearchResult(sInvitees, 0, null).Table;
        }


        // MS-1366 - let's make sure if we show non-visible records we marked it as so
        foreach (DataRow dr in searchResult.Table.Rows)
            if (!Convert.ToBoolean(dr["VisibleInPortal"]))
                dr["Name"] = dr["Name"] + " [HIDDEN]";

        // now, let's remove all that shouldn't be posted
        for (int i = dt.Rows.Count - 1; i >= 0; i--)
        {
            DataRow dr = dt.Rows[i];
            if (!_checkPostRemoveDate(dr))
                if (ConciergeAPI.HasBackgroundConsoleUser)
                    dr["Name"] += " [UNPOSTED]";
                else
                {
                    dt.Rows.RemoveAt(i);
                    continue;
                }

            if (!_canSeeEvent(dr, dtInvitations))
                dt.Rows.RemoveAt(i);
        }

        dvEvents = dt.DefaultView;
        dvEvents.Sort = "StartDate";

    }

    private bool _canSeeEvent(DataRow dr, DataTable dtInvitations)
    {
        // let's check to see if this event is invite only, and if we're invited

        bool inviteOnly = dr["InviteOnly"] != DBNull.Value && Convert.ToBoolean( dr["InviteOnly"]);
        if ( ! inviteOnly ) return true;    // you can see if

        if ( dtInvitations == null ) return false;  // this person's not logged in


        foreach (DataRow drInvi in dtInvitations.Rows)
            if ( Convert.ToString( drInvi["Event"] )  == Convert.ToString( dr["ID"])) // invited!
                return true;

        return false;
    }

    private bool _checkPostRemoveDate(DataRow dr)
    {
        // quick examine the posttoweb/remove from web dates
        DateTime? post = (DateTime?) (dr["PostToWeb"] != DBNull.Value ? dr["PostToWeb"] : null);
        DateTime? remove = (DateTime?)(dr["RemoveFromWeb"] != DBNull.Value ? dr["RemoveFromWeb"] : null);

        if (post == null && remove == null) return true;    // common case

        if (post.HasValue && post.Value > DateTime.Now) return false ;    // we're before the post date
        if (remove.HasValue && remove.Value < DateTime.Now) return false ;    // we're after the remove date

        return true;   // we're good 
    }

    #endregion

    protected void rptEvents_OnItemDataBound(object sender, RepeaterItemEventArgs e)
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
                HyperLink hlEventName = (HyperLink)e.Item.FindControl("hlEventName");
                Literal lEventTime = (Literal)e.Item.FindControl("lEventTime");
                Literal lEventLocation = (Literal)e.Item.FindControl("lEventLocation");
                Literal lEventDescription = (Literal)e.Item.FindControl("lEventDescription");
                PlaceHolder phDescription = (PlaceHolder)e.Item.FindControl("phDescription");
                HyperLink hlMore = (HyperLink)e.Item.FindControl("hlMore");

                
                string id = Convert.ToString(drv["ID"]);

                bool isExhibitShow = id.ToUpper().Contains("-00BE-");   // we know this is the guid hint
                DateTime start = Convert.ToDateTime( drv["StartDate"]);
                DateTime end = Convert.ToDateTime( drv["EndDate"]);
                string city = Convert.ToString( drv["Location_City"]);
                string state = Convert.ToString( drv["Location_State"]);
                string url = Convert.ToString(drv["Url"]); // CORE-1017
                string strOverrideDisplay = null;
                
                if ( drv.Row.Table.Columns.Contains( "DisplayStartEndDateTimesAs"))
                    strOverrideDisplay = Convert.ToString(drv["DisplayStartEndDateTimesAs"]);

                 string description = Convert.ToString( drv["ShortSummary"]);
                 if (description != null)
                     description = description.Replace("\n", "<BR/>");

                // now, set the controls
                 hlEventName.Text = "<h3>" + Convert.ToString(drv["Name"]) + "</h3>";
                if (isExhibitShow)
                    hlEventName.NavigateUrl = "/exhibits/ViewShow.aspx?contextID=" + id;
                else
                {
                    if (!string.IsNullOrWhiteSpace(url)) // CORE-1017
                        hlEventName.NavigateUrl = url;
                    else
                        hlEventName.NavigateUrl = "/events/ViewEvent.aspx?contextID=" + id;
                }

                if (!string.IsNullOrWhiteSpace(strOverrideDisplay))
                    lEventTime.Text = strOverrideDisplay;
                else
                {
                    if (start.Date != end.Date) // multiple days
                        lEventTime.Text = string.Format("{0:D} - {1:D}", start, end);
                    else // same day
                    {
                        // let's convert
                        lEventTime.Text = string.Format("{0:D} ({1:t}-{2:t})", start, start, end );
                     }
                }

                if ( ! string.IsNullOrWhiteSpace( city ) || ! string.IsNullOrWhiteSpace( state )) 
                    lEventLocation.Text = string.Format("{0}, {1}", city, state);

                if (!string.IsNullOrWhiteSpace(description))
                {
                    phDescription.Visible = true;

                  
                    lEventDescription.Text = description;
                    hlMore.NavigateUrl = hlEventName.NavigateUrl;
                }
                break;
        }
    }
}