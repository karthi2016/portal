using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Types;

[Bindable(true)]
public partial class controls_MembershipWidget : System.Web.UI.UserControl
{
    #region Properties

    #region Bindable Properties

    [Bindable(true)]
    public DateTime? JoinDate
    {
        get;
        set;
    }

    [Bindable(true)]
    public DateTime? ExpirationDate
    {
        get;
        set;
    }


    [Bindable(true)]
    public DateTime? TerminationDate
    {
        get;
        set;
    }

    [Bindable(true)]
    public bool ReceivesMemberBenefits
    {
        get;
        set;
    }

    [Bindable(true)]
    public string MembershipId
    {
        get;
        set;
    }

    [Bindable(true)]
    public string MembershipTypeName
    {
        get;
        set;
    }

    [Bindable(true)]
    public string MembershipStatusName
    {
        get;
        set;
    }

    [Bindable(true)]
    public string PrimaryChapterName
    {
        get;
        set;
    }

    [Bindable(true)]
    public string PrimaryChapterId
    {
        get;
        set;
    }

    [Bindable(true)]
    public string DefaultMembershipOrganizationName
    {
        get;
        set;
    }

    [Bindable(true)]
    public string DefaultMembershipOrganizationId
    {
        get;
        set;
    }

    [Bindable(true)]
    public bool DefaultMembersCanJoinThroughThePortal
    {
        get;
        set;
    }

    [Bindable(true)]
    public string MembershipOrganizationId
    {
        get;
        set;
    }

    [Bindable(true)]
    public int NumberOfDaysPriorToExpirationToPromptForRenewal
    {
        get; set;
    }

    [Bindable(true)]
    public bool MembersCanRenewThroughThePortal
    {
        get;
        set;
    }

    [Bindable(true)]
    public DataView ChapterMembership
    {
        get;
        set;
    }

    [Bindable(true)]
    public DataView ChapterLeadership
    {
        get;
        set;
    }

    [Bindable(true)]
    public DataView SectionMembership
    {
        get;
        set;
    }

    [Bindable(true)]
    public DataView SectionLeadership
    {
        get;
        set;
    }

    #endregion

    protected string MembershipStatusLabelText
    {
        get
        {
            if (string.IsNullOrWhiteSpace(MembershipStatusName))
                return "-";

            return MembershipStatusName;
        }
    }

    protected string JoinDateLabelText
    {
        get
        {
            if (JoinDate == null)
                return "Not a Member";

            return JoinDate.Value.ToString("d");
        }
    }

    protected string ExpirationLabelText
    {
        get
        {
            if (ExpirationDate == null) 
                return "-";

            return ExpirationDate.Value.ToString("d");
        }
    }

    protected string MembershipTypeLabelText
    {
        get
        {
            if (string.IsNullOrWhiteSpace(MembershipTypeName))
                return "-";

            return MembershipTypeName;
        }
    }

    protected string PrimaryChapterLabelText
    {
        get
        {
            if (string.IsNullOrWhiteSpace(PrimaryChapterName))
                return "-";

            return PrimaryChapterName;
        }
    }

    protected string PrimaryChapterNavigateUrl
    {
        get
        {
            string result =  "~/chapters/ViewChapter.aspx";

            if (!string.IsNullOrWhiteSpace(PrimaryChapterId))
                result = string.Format("{0}?contextID={1}", result, PrimaryChapterId);

            return result;
        }
    }

    #endregion

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        setupJoinRenewLink();

        if (!string.IsNullOrWhiteSpace(MembershipId))
        {
            liViewMembership.Visible = true;
            //liViewAllMemberships.Visible = true;

            hlViewMembership.NavigateUrl += "?contextID=" + MembershipId;
        }

        liSearchMembershipDirectory.Visible = isMembershipDirectoryAvailable();

        if (ChapterMembership != null)
        {
            //Bind the view chapter links starting with chapters the current entity is a member of - get distinct rows with the ToTable method
            //Filter out the primary chapter because the link is already displayed
            ChapterMembership.RowFilter = buildChapterMembershipFilter();
            rptChapterMembership.DataSource = ChapterMembership.ToTable(true, new[] {"Chapter.ID", "Chapter.Name"});
            rptChapterMembership.DataBind();
        }

        if (ChapterLeadership != null)
        {
            //Now filter the ones we just databound with the chapter membership from the chapter leadership
            ChapterLeadership.RowFilter = buildChapterLeadershipFilter();
            rptChapterLeadership.DataSource = ChapterLeadership.ToTable(true, new[] {"Chapter.ID", "Chapter.Name"});
            rptChapterLeadership.DataBind();
        }

        if (SectionMembership != null)
        {
            //Bind the view section links starting with chapters the current entity is a member of - get distinct rows with the ToTable method
            rptSectionMembership.DataSource = SectionMembership.ToTable(true, new[] {"Section.ID", "Section.Name"});
            rptSectionMembership.DataBind();
        }

        if (SectionLeadership != null)
        {
            //Now filter the ones we just databound with the section membership from the section leadership and bind to distinct rows with the ToTable method
            SectionLeadership.RowFilter = buildSectionLeadershipFilter();
            rptSectionLeadership.DataSource = SectionLeadership.ToTable(true, new[] {"Section.ID", "Section.Name"});
            rptSectionLeadership.DataBind();
        }
    }

    private bool isMembershipDirectoryAvailable()
    {
         
        if (!PortalConfiguration.Current.MembershipDirectoryEnabled)
            return false;

        if (PortalConfiguration.Current.MembershipDirectoryIsPublic)
            return true;

        //If the directory is enabled and not restricted to members it's available and no need to check membership status
        if (!PortalConfiguration.Current.MembershipDirectoryForMembersOnly)
            return true;

        //Directory is for members only
        //Check there is a membership
        if (string.IsNullOrWhiteSpace(MembershipId))
            return false;

        //Check the membership indicates membership benefits
        if (!ReceivesMemberBenefits)
            return false;

        //At this point if the termination date is null the member should be able to see the restricted directory
        if (TerminationDate == null)
            return true;

        //There is a termination date so check if it's future dated
        return TerminationDate > DateTime.Now;
    }

    private string buildChapterMembershipFilter()
    {
        return string.IsNullOrWhiteSpace(PrimaryChapterId) ? null : string.Format("Chapter.ID <> '{0}'", PrimaryChapterId);
    }

    private string buildChapterLeadershipFilter()
    {
        StringBuilder filterBuilder = new StringBuilder("Chapter.ID NOT IN (");
        bool returnFilter = false;

        if (!string.IsNullOrWhiteSpace(PrimaryChapterId))
        {
            filterBuilder.AppendFormat("Convert('{0}','System.Guid'),", PrimaryChapterId);
            returnFilter = true;
        }

        foreach (DataRowView chapterMembershipRow in ChapterMembership)
        {
            filterBuilder.AppendFormat("Convert('{0}','System.Guid'),", chapterMembershipRow["Chapter.ID"]);
            returnFilter = true;
        }

        if (!returnFilter)
            return null;

        filterBuilder.Remove(filterBuilder.Length - 1, 1);
        filterBuilder.Append(")");
        return filterBuilder.ToString();
    }

    private string buildSectionLeadershipFilter()
    {
        StringBuilder filterBuilder = new StringBuilder("Section.ID NOT IN (");
        bool returnFilter = false;

        foreach (DataRowView sectionMembershipRow in SectionMembership)
        {
            filterBuilder.AppendFormat("Convert('{0}','System.Guid'),", sectionMembershipRow["Section.ID"]);
            returnFilter = true;
        }

        if (!returnFilter)
            return null;

        filterBuilder.Remove(filterBuilder.Length - 1, 1);
        filterBuilder.Append(")");
        return filterBuilder.ToString();
    }

    private void setupJoinRenewLink()
    {
        liJoinRenew.Visible = false; // start by not shoing it

        //if (string.IsNullOrWhiteSpace(MembershipId))
        //{
        //    // there's no membership - is there a default organization?
        //    if (!string.IsNullOrWhiteSpace(DefaultMembershipOrganizationId) && DefaultMembersCanJoinThroughThePortal)
        //    {
        //        hlPurchaseMembership.Text = string.Format("Join {0}!", DefaultMembershipOrganizationName);
        //        hlPurchaseMembership.NavigateUrl = string.Format("/membership/Join.aspx?contextID={0}",
        //                                           DefaultMembershipOrganizationId);
        //        liJoinRenew.Visible = true;
        //    }

        //    return;

        //}


        // ok, so there's a membership
        // the question is - can the person renew?
        // well, if they have no expiration, then absolutely not
        if (ExpirationDate == null) return;

        // and if the membership org doesn't allow it, then no
        if (!MembersCanRenewThroughThePortal) return;

        // and if the difference between today and the expiration is greater than the renewal lag, then no either
        TimeSpan tsTimeTillExpiration = ExpirationDate.Value - DateTime.Today;

        if (tsTimeTillExpiration.TotalDays > NumberOfDaysPriorToExpirationToPromptForRenewal) return;


        // ok, we can renew
        liJoinRenew.Visible = true;
        //hlPurchaseMembership.Text = "Renew Your Membership";
        //hlPurchaseMembership.NavigateUrl = "/membership/Join.aspx?contextID=" + MembershipOrganizationId;

    }
}