using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class events_Register_SelectMember : PortalPage
{
    #region Fields

    protected msEvent targetEvent;
    protected msEntity targetEntity;

    protected DataView dvMembers;

    protected msChapter targetChapter;
    protected msSection targetSection;
    protected msOrganizationalLayer targetOrganizationalLayer;

    #endregion

    #region Properties

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

        targetEvent = LoadObjectFromAPI<msEvent>(ContextID);
        if (targetEvent == null) GoToMissingRecordPage();

        loadEventOwners();
    }

    /// <summary>
    /// Initializes the page.
    /// </summary>
    /// <remarks>This method runs on the first load of the page, and does NOT
    /// run on postbacks. If you want to run a method on PostBacks, override the
    /// Page_Load event</remarks>
    protected override void InitializePage()
    {
        base.InitializePage();

        if (targetChapter != null)
            setOwnerBackLinks(targetChapter.ID, targetChapter.Name, "~/chapters/ViewChapter.aspx", "~/chapters/ManageChapterEvents.aspx");

        if (targetSection != null)
            setOwnerBackLinks(targetSection.ID, targetSection.Name, "~/sections/ViewSection.aspx", "~/sections/ManageSectionEvents.aspx");

        if (targetOrganizationalLayer != null)
            setOwnerBackLinks(targetOrganizationalLayer.ID, targetOrganizationalLayer.Name, "~/organizationallayers/ViewOrganizationalLayer.aspx", "~/organizationallayers/ManageOrganizationalLayerEvents.aspx");


        loadDataFromConcierge();

        ddlMembers.DataSource = dvMembers.ToTable(true, new []{"ID", "Name"});
        ddlMembers.DataBind();

        CustomTitle.Text = string.Format("{0} Registration - Select Member", targetEvent.Name);
    }

    #endregion

    #region Methods

    protected void loadDataFromConcierge()
    {
        DataTable dtMembers = new DataTable();
        dtMembers.Columns.Add("ID", typeof (string));
        dtMembers.Columns.Add("Name", typeof(string));

        SearchOperationGroup terminationGroup = new SearchOperationGroup { FieldName = "Membership.TerminationDate" };
        terminationGroup.Criteria.Add(Expr.Equals("Membership.TerminationDate", null));
        terminationGroup.Criteria.Add(Expr.IsGreaterThan("Membership.TerminationDate", DateTime.Now));
        terminationGroup.GroupType = SearchOperationGroupType.Or;

        if(targetChapter != null)
        {
            Search sChapterMembers = new Search(msChapterMembership.CLASS_NAME);
            sChapterMembers.AddOutputColumn("Membership.Owner.ID");
            sChapterMembers.AddOutputColumn("Membership.Owner.Name");
            sChapterMembers.AddOutputColumn("Membership.Owner.LocalID");
            sChapterMembers.AddOutputColumn("Membership.Owner.EmailAddress");
            sChapterMembers.AddSortColumn("Membership.Owner.Name");
            sChapterMembers.AddCriteria(Expr.Equals("Chapter", targetChapter.ID));
            sChapterMembers.AddCriteria(Expr.Equals("IsCurrent", true));
            sChapterMembers.AddCriteria(terminationGroup);

            SearchResult srChapterMembers = APIExtensions.GetSearchResult(sChapterMembers, 0, null);
            foreach (DataRow drChapterMember in srChapterMembers.Table.Rows)
            {
                DataRow memberRow = dtMembers.NewRow();
                memberRow["ID"] = drChapterMember["Membership.Owner.ID"].ToString();
                memberRow["Name"] = string.Format("{0} (#{1})",drChapterMember["Membership.Owner.Name"], drChapterMember["Membership.Owner.LocalID"]);

                if (drChapterMember["Membership.Owner.EmailAddress"] != DBNull.Value && !string.IsNullOrWhiteSpace((string)drChapterMember["Membership.Owner.EmailAddress"]))
                    memberRow["Name"] = string.Format("{0} - {1}", memberRow["Name"], drChapterMember["Membership.Owner.EmailAddress"]);

                dtMembers.Rows.Add(memberRow);
            }
        }

        if (targetSection != null)
        {
            Search sSectionMembers = new Search(msSectionMembership.CLASS_NAME);
            sSectionMembers.AddOutputColumn("Membership.Owner.ID");
            sSectionMembers.AddOutputColumn("Membership.Owner.Name");
            sSectionMembers.AddOutputColumn("Membership.Owner.LocalID");
            sSectionMembers.AddOutputColumn("Membership.Owner.EmailAddress");
            sSectionMembers.AddSortColumn("Membership.Owner.Name");
            sSectionMembers.AddCriteria(Expr.Equals("Section", targetSection.ID));
            sSectionMembers.AddCriteria(Expr.Equals("IsCurrent", true));
            sSectionMembers.AddCriteria(terminationGroup);

            SearchResult srSectionMembership = APIExtensions.GetSearchResult(sSectionMembers, 0, null);
            foreach (DataRow drChapterMember in srSectionMembership.Table.Rows)
            {
                DataRow memberRow = dtMembers.NewRow();
                memberRow["ID"] = drChapterMember["Membership.Owner.ID"].ToString();
                memberRow["Name"] = string.Format("{0} (#{1})", drChapterMember["Membership.Owner.Name"], drChapterMember["Membership.Owner.LocalID"]);

                if (drChapterMember["Membership.Owner.EmailAddress"] != DBNull.Value && !string.IsNullOrWhiteSpace((string)drChapterMember["Membership.Owner.EmailAddress"]))
                    memberRow["Name"] = string.Format("{0} - {1}", memberRow["Name"], drChapterMember["Membership.Owner.EmailAddress"]);

                dtMembers.Rows.Add(memberRow);
            }
        }

        if (targetOrganizationalLayer != null)
        {
            Search sOrganizationalLayerMembers = new Search(msChapterMembership.CLASS_NAME);
            sOrganizationalLayerMembers.AddOutputColumn("Membership.Owner.ID");
            sOrganizationalLayerMembers.AddOutputColumn("Membership.Owner.Name");
            sOrganizationalLayerMembers.AddOutputColumn("Membership.Owner.LocalID");
            sOrganizationalLayerMembers.AddOutputColumn("Membership.Owner.EmailAddress");
            sOrganizationalLayerMembers.AddSortColumn("Membership.Owner.Name");

            //Get all members for the total/active member counts - determine later if they are active or not
            //Setup the clause to recursively find members who have a membership somewhere nested under the current organizational layer
            SearchOperationGroup organizationalLayerMembershipClause = new SearchOperationGroup { GroupType = SearchOperationGroupType.Or };
            organizationalLayerMembershipClause.Criteria.Add(Expr.Equals("Chapter.Layer", targetOrganizationalLayer.ID));
            //Add the recursive query for all the parent organizational layers
            StringBuilder sbOrganizationalLayer = new StringBuilder("Chapter.Layer");
            //Add Organizational Layers
            for (int i = 0; i < PortalConfiguration.OrganizationalLayerTypes.Rows.Count - 1; i++)
            {
                sbOrganizationalLayer.Append(".{0}");
                organizationalLayerMembershipClause.Criteria.Add(Expr.Equals(string.Format(sbOrganizationalLayer.ToString(), "ParentLayer"), targetOrganizationalLayer.ID));
            }

            sOrganizationalLayerMembers.AddCriteria(organizationalLayerMembershipClause);
            sOrganizationalLayerMembers.AddCriteria(terminationGroup);
            sOrganizationalLayerMembers.AddCriteria(Expr.Equals("IsCurrent",true));
            

            SearchResult srOrganizationalLayerMembers = APIExtensions.GetSearchResult(sOrganizationalLayerMembers, 0, null);
            foreach (DataRow drChapterMember in srOrganizationalLayerMembers.Table.Rows)
            {
                DataRow memberRow = dtMembers.NewRow();
                memberRow["ID"] = drChapterMember["Membership.Owner.ID"].ToString();
                memberRow["Name"] = string.Format("{0} (#{1})", drChapterMember["Membership.Owner.Name"], drChapterMember["Membership.Owner.LocalID"]);

                if (drChapterMember["Membership.Owner.EmailAddress"] != DBNull.Value && !string.IsNullOrWhiteSpace((string)drChapterMember["Membership.Owner.EmailAddress"]))
                    memberRow["Name"] = string.Format("{0} - {1}", memberRow["Name"], drChapterMember["Membership.Owner.EmailAddress"]);

                dtMembers.Rows.Add(memberRow);
            }
        }
        
        dvMembers = new DataView(dtMembers);
    }

    protected void loadEventOwners()
    {
        if (!string.IsNullOrWhiteSpace(targetEvent.Chapter))
            targetChapter = LoadObjectFromAPI<msChapter>(targetEvent.Chapter);

        if (!string.IsNullOrWhiteSpace(targetEvent.Section))
            targetSection = LoadObjectFromAPI<msSection>(targetEvent.Section);

        if (!string.IsNullOrWhiteSpace(targetEvent.OrganizationalLayer))
            targetOrganizationalLayer = LoadObjectFromAPI<msOrganizationalLayer>(targetEvent.OrganizationalLayer);
    }

    protected override bool CheckSecurity()
    {
        if(!base.CheckSecurity()) return false;

        if (ConciergeAPI.HasBackgroundConsoleUser)
            return true;

        if (targetChapter != null)
            return targetEvent.VisibleInPortal && canManageEvents(targetChapter.Leaders);

        if (targetSection != null)
            return targetEvent.VisibleInPortal && canManageEvents(targetSection.Leaders);

        if (targetOrganizationalLayer != null)
            return targetEvent.VisibleInPortal && canManageEvents(targetOrganizationalLayer.Leaders);

        //Default to false for now because currently only leaders can create events in the portal
        return false;
    }

    protected bool canManageEvents(List<msMembershipLeader> leaders)
    {
        if (leaders == null)
            // no leaders to speak of
            return false;

        var leader = leaders.Find(x => x.Individual == CurrentEntity.ID);
        return leader != null && leader.CanManageEvents;
    }

    protected void setOwnerBackLinks(string ownerId, string ownerName, string viewUrl, string manageEventsUrl)
    {
        hlEventOwner.NavigateUrl = string.Format("{0}?contextID={1}", viewUrl, ownerId);
        hlEventOwner.Text = string.Format("{0} >", ownerName);
        hlEventOwner.Visible = true;

        hlEventOwnerTask.Text = string.Format("Back to Manage {0} Events", ownerName);
        hlEventOwnerTask.NavigateUrl = string.Format("{0}?contextID={1}", manageEventsUrl, ownerId);
        liEventOwnerTask.Visible = true;
    }

    #endregion

    #region Event Handlers


    protected void btnContinue_Click(object sender, EventArgs e)
    {
        string nextUrl = string.Format("~/events/Register_SelectFee.aspx?contextID={0}&entityID={1}", ContextID,
                                       ddlMembers.SelectedValue);
        GoTo(nextUrl);
    }

    #endregion
}