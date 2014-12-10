using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;
using System.Text;
public partial class homepagecontrols_Membership : HomePageUserControl
{
    private readonly Dictionary<string, string> _exitingMembeships = new Dictionary<string, string>();
    public override void GenerateSearchesToBeRun(List<Search> searchesToRun)
    {
        base.GenerateSearchesToBeRun(searchesToRun);

        //PS-760
        /*Lets get membership organizations already used by this entity*/

        var exclude = new List<string>();
        // MS-5444
        const string renewalRangeColumn = "MembershipOrganization.NumberOfDaysPriorToExpirationToPromptForRenewal";
        const string expirationDateColumn = "ExpirationDate";
        using (var proxy = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            var s = new Search(msMembership.CLASS_NAME);
            s.AddCriteria(Expr.Equals("MembershipOrganization.MembersCanRenewThroughThePortal", true));
            s.AddOutputColumn("MembershipOrganization");
            // MS-5444
            s.AddOutputColumn(renewalRangeColumn);
            s.AddOutputColumn(expirationDateColumn);
            s.AddOutputColumn("Product.Name");
            s.AddOutputColumn("Type.Name");
            var tGroup = new SearchOperationGroup { FieldName = "TerminationDate" };
            tGroup.Criteria.Add(Expr.Equals("TerminationDate", null));
            tGroup.Criteria.Add(Expr.IsGreaterThan("TerminationDate", DateTime.Now));
            tGroup.GroupType = SearchOperationGroupType.Or;

            s.Criteria.Add(tGroup);
            s.Criteria.Add(Expr.Equals("ReceivesMemberBenefits", true));
            s.AddCriteria(Expr.Equals("Owner", ConciergeAPI.CurrentEntity.ID));

            var members = proxy.ExecuteSearch(s, 0, null).ResultValue;
            if (members.TotalRowCount > 0)
            {
                foreach (DataRow dr in members.Table.Rows)
                {
                    if (dr["MembershipOrganization"] == DBNull.Value)
                        continue;

                    var id = Convert.ToString(dr["MembershipOrganization"]);
                    if (string.IsNullOrEmpty(id) || exclude.Contains(id))
                        continue;

                    // MS-5444 Exclude current membership organization from renewal list only if member's expiration date is not within a renewal range.
                    var suppressRenew = true;
                    if (dr.Table.Columns.Contains(renewalRangeColumn) && dr[renewalRangeColumn] != DBNull.Value && 
                        dr.Table.Columns.Contains(expirationDateColumn) && dr[expirationDateColumn] != DBNull.Value)
                    {
                        var renewalRange = Convert.ToInt32(dr[renewalRangeColumn]);
                        var expirationDate = Convert.ToDateTime(dr[expirationDateColumn]);
                        suppressRenew = DateTime.Now.AddDays(renewalRange) < expirationDate;
                    }

                    if (suppressRenew)
                        exclude.Add(id);
                    
                    _exitingMembeships.Add(Convert.ToString(dr["ID"]), Convert.ToString(dr["Product.Name"]));
                }
            }
        }

        // now, we need the default membership organization
        var sDefaultMemOrg = new Search { Type = msMembershipOrganization.CLASS_NAME, ID = "MembershipOrganization" };
        //PS-760 

        sDefaultMemOrg.AddCriteria(Expr.Equals("IsActive", true));
        foreach (var id in exclude)
            sDefaultMemOrg.AddCriteria(Expr.DoesNotEqual("ID", id));

        sDefaultMemOrg.AddOutputColumn("Name");
        sDefaultMemOrg.AddOutputColumn(msMembershipOrganization.FIELDS.MembersCanJoinThroughThePortal);
        sDefaultMemOrg.AddOutputColumn(msMembershipOrganization.FIELDS.MembersCanRenewThroughThePortal);
        sDefaultMemOrg.AddOutputColumn(msMembershipOrganization.FIELDS.NumberOfDaysPriorToExpirationToPromptForRenewal);
        searchesToRun.Add(sDefaultMemOrg);



        var terminationGroup = new SearchOperationGroup { FieldName = "Membership.TerminationDate" };
        terminationGroup.Criteria.Add(Expr.Equals("Membership.TerminationDate", null));
        terminationGroup.Criteria.Add(Expr.IsGreaterThan("Membership.TerminationDate", DateTime.Now));
        terminationGroup.GroupType = SearchOperationGroupType.Or;

        //Chapter Membership
        Search sChapterMembership = new Search { Type = msChapterMembership.CLASS_NAME, ID = "ChapterMembership" };
        sChapterMembership.AddOutputColumn("Chapter.ID");
        sChapterMembership.AddOutputColumn("Chapter.Name");
        sChapterMembership.AddSortColumn("Chapter.Name");
        sChapterMembership.AddCriteria(Expr.Equals("Membership.Owner.ID", ConciergeAPI.CurrentEntity.ID));
        sChapterMembership.AddCriteria(Expr.Equals("IsCurrent", true));
        sChapterMembership.AddCriteria(terminationGroup);
        searchesToRun.Add(sChapterMembership);


        var sOrganizationalLayerMembership = MembershipLogic.GetSearchForOrganizationalLayerMemberships();
        sOrganizationalLayerMembership.AddCriteria(terminationGroup);
        searchesToRun.Add(sOrganizationalLayerMembership);

        //Section Membership
        Search sSectionMembership = new Search { Type = msSectionMembership.CLASS_NAME, ID = "SectionMembership" };
        sSectionMembership.AddOutputColumn("Section.ID");
        sSectionMembership.AddOutputColumn("Section.Name");
        sSectionMembership.AddSortColumn("Section.Name");
        sSectionMembership.AddCriteria(Expr.Equals("Membership.Owner.ID", ConciergeAPI.CurrentEntity.ID));
        sSectionMembership.AddCriteria(Expr.Equals("IsCurrent", true));
        sSectionMembership.AddCriteria(terminationGroup);
        sSectionMembership.UniqueResult = true;
        searchesToRun.Add(sSectionMembership);

        //Chapter Leadership
        Search sChapterLeadership = new Search { Type = "ChapterLeader", ID = "ChapterLeadership" };
        sChapterLeadership.AddOutputColumn("Chapter.ID");
        sChapterLeadership.AddOutputColumn("Chapter.Name");
        sChapterLeadership.AddSortColumn("Chapter.Name");
        sChapterLeadership.AddCriteria(Expr.Equals("Individual.ID", ConciergeAPI.CurrentEntity.ID));
        searchesToRun.Add(sChapterLeadership);

        //Section Leadership
        Search sSectionLeadership = new Search { Type = "SectionLeader", ID = "SectionLeadership" };
        sSectionLeadership.AddOutputColumn("Section.ID");
        sSectionLeadership.AddOutputColumn("Section.Name");
        sSectionLeadership.AddSortColumn("Section.Name");
        sSectionLeadership.AddCriteria(Expr.Equals("Individual.ID", ConciergeAPI.CurrentEntity.ID));
        searchesToRun.Add(sSectionLeadership);

        //Organizational Layer Leadership
        Search sOrganizationalLayerLeadership = new Search { Type = "OrganizationalLayerLeader", ID = "OrganizationalLayerLeadership" };
        sOrganizationalLayerLeadership.AddOutputColumn("OrganizationalLayer.ID");
        sOrganizationalLayerLeadership.AddOutputColumn("OrganizationalLayer.Name");
        sOrganizationalLayerLeadership.AddOutputColumn("OrganizationalLayer.Type.Name");
        sOrganizationalLayerLeadership.AddSortColumn("OrganizationalLayer.Type.Name");
        sOrganizationalLayerLeadership.AddSortColumn("OrganizationalLayer.Name");
        sOrganizationalLayerLeadership.AddCriteria(Expr.Equals("Individual.ID", ConciergeAPI.CurrentEntity.ID));
        searchesToRun.Add(sOrganizationalLayerLeadership);
    }



    public override List<string> GetFieldsNeededForMainSearch()
    {
        var fields = base.GetFieldsNeededForMainSearch();

        fields.Add("Membership");
        fields.Add("Membership.JoinDate");
        fields.Add("Membership.Status.Name");
        fields.Add("Membership.Type.Name");
        fields.Add("Membership.ExpirationDate");
        fields.Add("Membership.PrimaryChapter.Name");
        fields.Add("Membership.PrimaryChapter");
        fields.Add("Membership.ReceivesMemberBenefits");
        fields.Add("Membership.MembershipOrganization");
        fields.Add("Membership.MembershipOrganization.MembersCanRenewThroughThePortal");
        fields.Add("Membership.MembershipOrganization.NumberOfDaysPriorToExpirationToPromptForRenewal");
        fields.Add("Membership.TerminationDate");
        fields.Add("Membership.PrimaryChapter.Layer");
        fields.Add("Membership.PrimaryChapter.Layer.Name");
        fields.Add("Membership.PrimaryChapter.Layer.Type.Name");

        StringBuilder sbOrganizationalLayer = new StringBuilder("Membership.PrimaryChapter.Layer");
        for (int i = 0; i < PortalConfiguration.OrganizationalLayerTypes.Rows.Count - 1; i++)
        {
            sbOrganizationalLayer.Append(".{0}");
            string parentLayerColumn = string.Format(sbOrganizationalLayer.ToString(), "ParentLayer");
            string parentLayerName = string.Format("{0}.Name", parentLayerColumn);
            string parentLayerTypeName = string.Format("{0}.Type.Name", parentLayerColumn);

            fields.Add(parentLayerColumn);
            fields.Add(parentLayerName);
            fields.Add(parentLayerTypeName);
        }

        return fields;
    }

    /// <summary>
    /// Delivers the search results to the widget
    /// </summary>
    /// <param name="results">The results.</param>
    public override void DeliverSearchResults(List<SearchResult> results)
    {
        base.DeliverSearchResults(results);
        if (!Visible) return;

        //Merge chapter membership and chapter leadership and create a dataview
        DataTable dtChapters = results.Single(x => x.ID == "ChapterMembership").Table.Clone();
        dtChapters.Merge(results.Single(x => x.ID == "ChapterMembership").Table);
        dtChapters.Merge(results.Single(x => x.ID == "ChapterLeadership").Table);

        //Merge section membership and section leadership and create a dataview
        DataTable dtSections = results.Single(x => x.ID == "SectionMembership").Table.Clone();
        dtSections.Merge(results.Single(x => x.ID == "SectionMembership").Table);
        dtSections.Merge(results.Single(x => x.ID == "SectionLeadership").Table);


      //H.Z. Only those membership organizations that are eligibile for portal usage should be used for a join link.
        var expression = string.Format("MembersCanJoinThroughThePortal=true");
        var rows = results.Single(x => x.ID == "MembershipOrganization").Table.Select(expression);
        if (rows.Any())
        {
            var t = rows.CopyToDataTable();
            drMembershipOrganization = t.Rows[0];
            //PS-760
            drMembershipOrganizations = t.Rows;

            rptMemOrgs.DataSource = t;
            rptMemOrgs.DataBind();
        }

        //Pivot organizational layer membership
        DataTable dtOrganizationaLayerMembership = new DataTable();
        dtOrganizationaLayerMembership.Columns.Add("ID", typeof(string));
        dtOrganizationaLayerMembership.Columns.Add("Name", typeof(string));
        dtOrganizationaLayerMembership.Columns.Add("Type.Name", typeof(string));
        dtOrganizationaLayerMembership.Columns.Add("FromPrimary", typeof(bool));
        dtOrganizationaLayerMembership.PrimaryKey = new DataColumn[] { dtOrganizationaLayerMembership.Columns["ID"] };

        //First add the primary chapter layers
        if (drMainRecord["Membership.PrimaryChapter.Layer"] != DBNull.Value && dtOrganizationaLayerMembership.Rows.Find(drMainRecord["Membership.PrimaryChapter.Layer"].ToString()) == null)
        {
            DataRow drOrganizationalLayerMembership = dtOrganizationaLayerMembership.NewRow();
            drOrganizationalLayerMembership["ID"] = drMainRecord["Membership.PrimaryChapter.Layer"].ToString();
            drOrganizationalLayerMembership["Name"] = drMainRecord["Membership.PrimaryChapter.Layer.Name"].ToString();
            drOrganizationalLayerMembership["Type.Name"] = drMainRecord["Membership.PrimaryChapter.Layer.Type.Name"].ToString();
            drOrganizationalLayerMembership["FromPrimary"] = true;
            dtOrganizationaLayerMembership.Rows.Add(drOrganizationalLayerMembership);
        }

        StringBuilder sbOrganizationalLayer = new StringBuilder("Membership.PrimaryChapter.Layer");
        for (int i = 0; i < PortalConfiguration.OrganizationalLayerTypes.Rows.Count - 1; i++)
        {
            sbOrganizationalLayer.Append(".{0}");
            string parentLayerColumn = string.Format(sbOrganizationalLayer.ToString(), "ParentLayer");
            string parentLayerName = string.Format("{0}.Name", parentLayerColumn);
            string parentLayerTypeName = string.Format("{0}.Type.Name", parentLayerColumn);

            //If the parent layer column doesn't exist (meaning we've reached the top of the heiarchy) or the layer has already been added just move to the next one
            if (!drMainRecord.Table.Columns.Contains(parentLayerColumn) || drMainRecord[parentLayerColumn] == DBNull.Value || dtOrganizationaLayerMembership.Rows.Find(drMainRecord[parentLayerColumn].ToString()) != null)
                continue;

            DataRow drOrganizationalLayerMembership = dtOrganizationaLayerMembership.NewRow();
            drOrganizationalLayerMembership["ID"] = drMainRecord[parentLayerColumn].ToString();
            drOrganizationalLayerMembership["Name"] = drMainRecord[parentLayerName].ToString();
            drOrganizationalLayerMembership["Type.Name"] = drMainRecord[parentLayerTypeName].ToString();
            drOrganizationalLayerMembership["FromPrimary"] = true;
            dtOrganizationaLayerMembership.Rows.Add(drOrganizationalLayerMembership);
        }

        dvPrimaryOrganizationalLayers = new DataView(dtOrganizationaLayerMembership, "FromPrimary = 1", "", DataViewRowState.CurrentRows);

        //Now add any secondary chapter layers - because the primary chapter layers are still in the data table we can do a dup check
        foreach (DataRow drChapterMembership in results.Single(x => x.ID == "OrganizationalLayerMembership").Table.Rows)
        {
            if (drChapterMembership["Chapter.Layer"] != DBNull.Value && dtOrganizationaLayerMembership.Rows.Find(drChapterMembership["Chapter.Layer"].ToString()) == null)
            {
                DataRow drOrganizationalLayerMembership = dtOrganizationaLayerMembership.NewRow();
                drOrganizationalLayerMembership["ID"] = drChapterMembership["Chapter.Layer"].ToString();
                drOrganizationalLayerMembership["Name"] = drChapterMembership["Chapter.Layer.Name"].ToString();
                drOrganizationalLayerMembership["Type.Name"] = drChapterMembership["Chapter.Layer.Type.Name"].ToString();
                drOrganizationalLayerMembership["FromPrimary"] = false;
                dtOrganizationaLayerMembership.Rows.Add(drOrganizationalLayerMembership);
            }

            sbOrganizationalLayer = new StringBuilder("Chapter.Layer");
            for (int i = 0; i < PortalConfiguration.OrganizationalLayerTypes.Rows.Count - 1; i++)
            {
                sbOrganizationalLayer.Append(".{0}");
                string parentLayerColumn = string.Format(sbOrganizationalLayer.ToString(), "ParentLayer");
                string parentLayerName = string.Format("{0}.Name", parentLayerColumn);
                string parentLayerTypeName = string.Format("{0}.Type.Name", parentLayerColumn);

                //If the parent layer column doesn't exist (meaning we've reached the top of the heiarchy) or the layer has already been added just move to the next one
                if (!drChapterMembership.Table.Columns.Contains(parentLayerColumn) || drChapterMembership[parentLayerColumn] == DBNull.Value || dtOrganizationaLayerMembership.Rows.Find(drChapterMembership[parentLayerColumn].ToString()) != null)
                    continue;

                DataRow drOrganizationalLayerMembership = dtOrganizationaLayerMembership.NewRow();
                drOrganizationalLayerMembership["ID"] = drChapterMembership[parentLayerColumn].ToString();
                drOrganizationalLayerMembership["Name"] = drChapterMembership[parentLayerName].ToString();
                drOrganizationalLayerMembership["Type.Name"] = drChapterMembership[parentLayerTypeName].ToString();
                drOrganizationalLayerMembership["FromPrimary"] = false;
                dtOrganizationaLayerMembership.Rows.Add(drOrganizationalLayerMembership);
            }
        }

        //Add the leadership (cannot merge because schema is different
        foreach (DataRow drOrganizationalLayerLeadership in results.Single(x => x.ID == "OrganizationalLayerLeadership").Table.Rows)
        {
            if (dtOrganizationaLayerMembership.Rows.Find(drOrganizationalLayerLeadership["OrganizationalLayer.ID"].ToString()) != null)
                continue;

            DataRow drOrganizationalLayerMembership = dtOrganizationaLayerMembership.NewRow();
            drOrganizationalLayerMembership["ID"] = drOrganizationalLayerLeadership["OrganizationalLayer.ID"].ToString();
            drOrganizationalLayerMembership["Name"] = drOrganizationalLayerLeadership["OrganizationalLayer.Name"].ToString();
            drOrganizationalLayerMembership["Type.Name"] = drOrganizationalLayerLeadership["OrganizationalLayer.Type.Name"].ToString();
            drOrganizationalLayerMembership["FromPrimary"] = false;
            dtOrganizationaLayerMembership.Rows.Add(drOrganizationalLayerMembership);
        }

        dvSecondaryOrganizationalLayers = new DataView(dtOrganizationaLayerMembership, "FromPrimary = 0", "", DataViewRowState.CurrentRows);

        //Now initialize - this cannot happen in initialize widget because it depends on search fields existing and they won't if the module is inactive and GetFieldsNeededForMainSearch is not run
        //PS-760
        //  setupJoinRenewLink();
        ViewMemberships.Visible = false;
        if (_exitingMembeships != null)
        {
            ViewMemberships.Visible = _exitingMembeships.Count > 0;
            rptMemOrgsView.DataSource = _exitingMembeships;
            rptMemOrgsView.DataBind();
        }
        //string MembershipId = Convert.ToString(drMainRecord["Membership"]);
        //if (!string.IsNullOrWhiteSpace(MembershipId))
        //{
        //    liViewMembership.Visible = true;
        //    //liViewAllMemberships.Visible = true;

        //    hlViewMembership.NavigateUrl += "?contextID=" + MembershipId;
        //}

        setupLabels();

        liSearchMembershipDirectory.Visible = isMembershipDirectoryAvailable();


        //Bind the view chapter links starting with chapters the current entity is a member of - get distinct rows with the ToTable method
        //Filter out the primary chapter because the link is already displayed
        string chapterMembershipFilter = buildChapterMembershipFilter();
        DataView dvChapters = new DataView(dtChapters, chapterMembershipFilter, "Chapter.Name", DataViewRowState.CurrentRows);
        rptChapterMembership.DataSource = dvChapters.ToTable(true, new[] { "Chapter.ID", "Chapter.Name" });
        rptChapterMembership.DataBind();

        //Bind the view section links starting with chapters the current entity is a member of - get distinct rows with the ToTable method
        DataView dvSections = new DataView(dtSections, "", "Section.Name", DataViewRowState.CurrentRows);
        rptSectionMembership.DataSource = dvSections.ToTable(true, new[] { "Section.ID", "Section.Name" });
        rptSectionMembership.DataBind();


        //Bind the view organizational layer links starting with chapters the current entity is a member of - get distinct rows with the ToTable method
        rptPrimaryOrganizationalLayers.DataSource = dvPrimaryOrganizationalLayers.ToTable(true, new[] { "ID", "Name", "Type.Name" });
        rptPrimaryOrganizationalLayers.DataBind();

        rptOrganizationalLayerMembership.DataSource = dvSecondaryOrganizationalLayers.ToTable(true, new[] { "ID", "Name", "Type.Name" });
        rptOrganizationalLayerMembership.DataBind();

    }

    private void setupLabels()
    {
        lblMembershipStatus.Text = drMainRecord.Field<string>("Membership.Status.Name");

        DateTime? exp = drMainRecord.Field<DateTime?>("Membership.ExpirationDate");
        if (exp != null)
            lblMembershipExpiration.Text = exp.Value.ToShortDateString();
        else
            lblMembershipExpiration.Text = "-";

        DateTime? joinDate = drMainRecord.Field<DateTime?>("Membership.JoinDate");
        if (joinDate != null)
            lblMembershipJoinDate.Text = joinDate.Value.ToShortDateString();
        else
            lblMembershipJoinDate.Text = "-";

        lblMembershipMembershipType.Text = drMainRecord.Field<string>("Membership.Type.Name");

        hlChapter.Text = drMainRecord.Field<string>("Membership.PrimaryChapter.Name");

        if (hlChapter.Text != string.Empty)   // THERE'S A CHAPTER
        {
            trChapter.Visible = true;
            hlChapter.NavigateUrl += "?contextID=" + drMainRecord.Field<System.Guid>("Membership.PrimaryChapter").ToString();
        }

    }

    private bool isMembershipDirectoryAvailable()
    {
        if (!PortalConfiguration.Current.MembershipDirectoryEnabled)
            return false;

        //If the directory is enabled and not restricted to members it's available and no need to check membership status
        if (!PortalConfiguration.Current.MembershipDirectoryForMembersOnly)
            return true;

        //Directory is for members only
        return isActiveMember();
    }

    private string buildChapterMembershipFilter()
    {
        string PrimaryChapterId = Convert.ToString(drMainRecord["Membership.PrimaryChapter"]);
        return string.IsNullOrWhiteSpace(PrimaryChapterId) ? null : string.Format("Chapter.ID <> '{0}'", PrimaryChapterId);
    }

    private void setupJoinRenewLink()
    {
        // if (drMembershipOrganization == null) return;

        // string memOrgID = Convert.ToString(drMembershipOrganization["ID"]);

        //// liJoinRenew.Visible = false; // start by not shoing it
        // string MembershipId = Convert.ToString(drMainRecord["Membership"]);

        // if (string.IsNullOrWhiteSpace(MembershipId))
        // {
        //     // there's no membership - is there a default organization?            
        //     if (drMembershipOrganization.Field<bool>(msMembershipOrganization.FIELDS.MembersCanJoinThroughThePortal))
        //     {
        //         hlPurchaseMembership.Text = string.Format("Join {0}!", drMembershipOrganization.Field<string>("Name"));
        //         hlPurchaseMembership.NavigateUrl = string.Format("/membership/Join.aspx?contextID={0}",
        //                                            memOrgID);
        //         liJoinRenew.Visible = true;
        //     }

        //     return;

        // }


        //// ok, so there's a membership
        //// the question is - can the person renew?
        //// well, if they have no expiration, then absolutely not
        //var ExpirationDate = drMainRecord.Field<DateTime?>("Membership.ExpirationDate");
        //if (ExpirationDate == null) return;

        //// and if the membership org doesn't allow it, then no
        //if (!drMembershipOrganization.Field<bool>(msMembershipOrganization.FIELDS.MembersCanRenewThroughThePortal)) return;

        //// and if the difference between today and the expiration is greater than the renewal lag, then no either
        //TimeSpan tsTimeTillExpiration = ExpirationDate.Value - DateTime.Today;

        //if (tsTimeTillExpiration.TotalDays > drMembershipOrganization.Field<int>("NumberOfDaysPriorToExpirationToPromptForRenewal")) return;


        //// ok, we can renew
        //liJoinRenew.Visible = true;
        //hlPurchaseMembership.Text = "Renew Your Membership";
        //hlPurchaseMembership.NavigateUrl = "/membership/Join.aspx?contextID=" + memOrgID;

    }

    #region Bindable Properties





    public DataTable dtChapters
    {
        get;
        set;
    }


    public DataTable dtSections
    {
        get;
        set;
    }

    public DataView dvPrimaryOrganizationalLayers
    {
        get;
        set;
    }

    public DataView dvSecondaryOrganizationalLayers
    {
        get;
        set;
    }


    protected DataRow drMembershipOrganization { get; set; }
    //PS-760
    protected DataRowCollection drMembershipOrganizations { get; set; }
    #endregion
}
