using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Results;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

/// <summary>
/// Summary description for DocumentsLogic
/// </summary>
public class DocumentsLogic
{
	public DocumentsLogic()
	{
		//
		// TODO: Add constructor logic here
		//
	}
 

    public static bool CanAccess(string fileCabinetID, string folderID, string entityID )
    {
        // ok, first, let's see what kind of file cabinet it is
        using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            var msoFileCabinet = api.Get(fileCabinetID).ResultValue;
            switch (msoFileCabinet.ClassType)
            {
                case msCommitteeFileCabinet.CLASS_NAME:
                   if (CommitteeLogic.IsMember(api, msoFileCabinet.SafeGetValue<string>(
                                                                     msCommitteeFileCabinet.FIELDS.Committee),
                                                                 ConciergeAPI.CurrentEntity.ID))
                           return true;
                  
                    break;

                case msSectionFileCabinet.CLASS_NAME:
                    return CanViewMembershipDocuments(api, msoFileCabinet.SafeGetValue<string>("Section"), entityID);
                case msChapterFileCabinet.CLASS_NAME:
                    return CanViewMembershipDocuments(api, msoFileCabinet.SafeGetValue<string>("Chapter"), entityID);
                case msOrganizationalLayerFileCabinet.CLASS_NAME:
                    return CanViewMembershipDocuments(api, msoFileCabinet.SafeGetValue<string>("OrganizationalLayer"), entityID);
                    

            }

        
            // ok - now, am I entitled to the file folder?
            // I'm going to check the folder directly, and then all of the parent folders
            var entitlements = api.ListEntitlements(ConciergeAPI.CurrentEntity.ID, msFileFolderEntitlement.CLASS_NAME).ResultValue ;

            if (entitlements.Exists(x => x.Context == folderID))
                return true;

            // ok, what about any parent folders?
            var folderInfo = api.DescribeFolder(folderID).ResultValue;
            if (folderInfo.ParentFolders != null)
                foreach (var pf in folderInfo.ParentFolders)
                    if (entitlements.Exists(x => x.Context == pf.FolderID))   // if you have access to the parent, you're good
                        return true;

           
        }

        return false;   // no access
         
    }

    public static bool CanWriteTo(msFileFolder folder, string entityID)
    {
        // assuming that the user has access, checks to see if the person can write to the folder
        string fileCabinetType;
        using( var api = ConciergeAPIProxyGenerator.GenerateProxy())
        fileCabinetType = api.DetermineObjectType( folder.FileCabinet ).ResultValue;
         
        // for non-association file cabinets, if it's not protected, it's good!
        if ( fileCabinetType != msAssociationFileCabinet.CLASS_NAME && !folder.OnlyLeadersCanUploadFiles)
            return CanAccess( folder.FileCabinet, folder.ID, entityID );    // we're good
        
        return HasAdministrativeAccessTo(folder, entityID);
      
    }

    public static void SetupContextAndParentFolders(IConciergeAPIService api, msFileFolder targetFolder, HyperLink hlFolderContext,
                                                    FolderInfo foldersAndFiles, Repeater rptParentFolders, bool includeTargetFolder)
    {

        var mso = GetFileCabinetContext(api, targetFolder);

        if (mso != null)
        {
            hlFolderContext.Text = mso.SafeGetValue<string>("Name");
            hlFolderContext.NavigateUrl = NavigationLogic.GetUrlFor(mso);
        }
        else
        {
            hlFolderContext.Text = "My Digital Library";
            hlFolderContext.NavigateUrl = "DigitalLibrary.aspx";
        }

        // now, the parent folders
        List<PartialFolderInfo> parentFolders = new List<PartialFolderInfo>();

        if (foldersAndFiles.ParentFolders != null)
        {
         
            // let's go through the parent folders and find the first one we have access to - and then
            // we'll start with that
            // we don't want to start with the root, because we may not have access
            bool accessEstablished = false;
            foreach (var folder in foldersAndFiles.ParentFolders)
            {
                if (!accessEstablished &&
                    !CanAccess(targetFolder.FileCabinet, folder.FolderID, ConciergeAPI.CurrentEntity.ID))
                    // do we have access?
                    continue; // no access yet

                accessEstablished = true; // ok, we can access from now on
                parentFolders.Add(folder);

            }
        }

        if (includeTargetFolder)
            parentFolders.Add(new PartialFolderInfo { FolderID = targetFolder.ID, FolderName = targetFolder.Name });

        // bind
        rptParentFolders.DataSource = parentFolders;
        rptParentFolders.DataBind();

    }

    public static string GetFolderPath(IConciergeAPIService api, string folderID)
    {
        if (api == null) throw new ArgumentNullException("api");
        if (folderID == null) throw new ArgumentNullException("folderID");

        Search sFolders = new Search(msFileFolder.CLASS_NAME);
        sFolders.AddCriteria(Expr.Equals("ID", folderID ));
        sFolders.AddOutputColumn("FolderPath");

        var sr = api.ExecuteSearch(sFolders, 0, 1).ResultValue;
        if (sr.TotalRowCount > 0)
            return (string)sr.Table.Rows[0]["FolderPath"];

        return null;

    }

     
    public static MemberSuiteObject GetFileCabinetContext(IConciergeAPIService api, msFileFolder targetFolder )
    {
         var msoCabinet = api.Get(targetFolder.FileCabinet).ResultValue;

     
        switch (msoCabinet.ClassType)
        {
            case msCommitteeFileCabinet.CLASS_NAME:
                return api.Get(msoCabinet.SafeGetValue<string>(msCommitteeFileCabinet.FIELDS.Committee)).ResultValue;

            case msChapterFileCabinet.CLASS_NAME:
                return api.Get(msoCabinet.SafeGetValue<string>(msChapterFileCabinet.FIELDS.Chapter)).ResultValue;

            case msSectionFileCabinet.CLASS_NAME:
                return api.Get(msoCabinet.SafeGetValue<string>(msSectionFileCabinet.FIELDS.Section)).ResultValue;

            case msOrganizationalLayerFileCabinet.CLASS_NAME:
                return api.Get(msoCabinet.SafeGetValue<string>(msOrganizationalLayerFileCabinet.FIELDS.OrganizationalLayer)).ResultValue; 

            case msAssociationFileCabinet.CLASS_NAME:
                return null;

            default:
                throw new NotSupportedException("Unknown cabinet " + msoCabinet.ClassType);
        }
         
    }

    public static bool CanViewMembershipDocuments(IConciergeAPIService api, string membershipObjectID, string entityID)
    {
        
        // checks to see if you can view section/chapter/org layer docs
        var typeName = api.DetermineObjectType(membershipObjectID).ResultValue ;

        if (typeName == msOrganizationalLayer.CLASS_NAME) // special logic
        {
            string orgLayerID = membershipObjectID;
            var results = api.ExecuteSearch(MembershipLogic.GetSearchForOrganizationalLayerMemberships(), 0, null).ResultValue.Table;

            // search all the columns for the id
            foreach (DataRow dr in results.Rows)
                foreach (DataColumn dc in results.Columns)
                    if (Convert.ToString(dr[dc.ColumnName]) == orgLayerID)
                        return true;    // great! We're a member

            return false;
        }

        // is the person a member of the chapter?
        Search s = new Search( typeName + "Membership");
        s.AddCriteria(Expr.Equals("Membership.Owner", entityID));
        s.AddCriteria(Expr.Equals(typeName, membershipObjectID));
        s.AddCriteria( Expr.Equals("IsCurrent", true ));
        s.AddOutputColumn("Membership");
        s.AddSortColumn("ListIndex");

        if (api.ExecuteSearch(s, 0, 1).ResultValue.TotalRowCount > 0)
            return true;    // yep

        // ok - what about a leader?
        Search sLeaders = new Search( typeName + "Leader" );
        sLeaders.AddCriteria( Expr.Equals( typeName, membershipObjectID ) );
        sLeaders.AddCriteria(Expr.Equals( msMembershipLeader.FIELDS.Individual , entityID ));
        sLeaders.AddOutputColumn(typeName);
        sLeaders.AddSortColumn("ListIndex");

        if (api.ExecuteSearch(sLeaders, 0, 1).ResultValue.TotalRowCount > 0)
            return true;    // they are a leader

        return false;
       
    }

    public static bool HasAdministrativeAccessTo(msFileFolder targetFolder, string entityID)
    {
        // checks to see if the entity has administrative access to the folder
        // ok, first, let's see what kind of file cabinet it is
        using (var api = ConciergeAPIProxyGenerator.GenerateProxy())
        {
            var msoFileCabinet = api.Get(targetFolder.FileCabinet).ResultValue;
             
            switch (msoFileCabinet.ClassType)
            {
                case msCommitteeFileCabinet.CLASS_NAME:
                    return CommitteeLogic.IsAdministrativeMember(api, msoFileCabinet.SafeGetValue<string>(
                        msCommitteeFileCabinet.FIELDS.Committee),
                                                                 ConciergeAPI.CurrentEntity.ID);


                case msSectionFileCabinet.CLASS_NAME:
                case msChapterFileCabinet.CLASS_NAME:
                case msOrganizationalLayerFileCabinet.CLASS_NAME:
                    string typeName = msoFileCabinet.ClassType.Replace("FileCabinet","");
                    Search sLeaders = new Search(typeName + "Leader");
                    sLeaders.AddCriteria(Expr.Equals(typeName , msoFileCabinet[typeName ]));
                    sLeaders.AddCriteria(Expr.Equals(msMembershipLeader.FIELDS.Individual, entityID));
                    sLeaders.AddOutputColumn("ListIndex");
                    sLeaders.AddSortColumn("ListIndex");

                    return api.ExecuteSearch(sLeaders, 0, 1).ResultValue.TotalRowCount > 0;

                
                  
                case msAssociationFileCabinet.CLASS_NAME:
                    return false;   // you never have write access


                default:
                    throw new NotSupportedException("Unkown file cabinet type" + msoFileCabinet.ClassType);
                    

            }


        }
    }

    public static bool CanAccessFile(string fileID)
    {
        // we'll do a search here to avoid downloading the file
        Search s = new Search(msFile.CLASS_NAME);
        s.AddOutputColumn("FileFolder");
        s.AddOutputColumn("FileCabinet");
        s.AddCriteria(Expr.Equals("ID", fileID));

        SearchResult sr;
        using( var api = ConciergeAPIProxyGenerator.GenerateProxy())
        sr = api.ExecuteSearch( s,0,1).ResultValue;

        if (sr.TotalRowCount == 0 ) return false;
        DataRow dr = sr.Table.Rows[0];

        string folderID = Convert.ToString(dr["FileFolder"]);
        string fileCabinet = Convert.ToString(dr["FileCabinet"]);
        
        // now, can I access the folder?
        if ( string.IsNullOrWhiteSpace( fileCabinet ) || string.IsNullOrWhiteSpace( folderID ) )
            return false;

        return CanAccess(fileCabinet, folderID, ConciergeAPI.CurrentEntity.ID);
    }

    /// <summary>
    /// Checks to see if a file in the current session should be downloaded
    /// </summary>
    /// <param name="fileIDToCheck">The context id.</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static bool CheckTemporaryFileAccess(string fileIDToCheck)
    {
        string id = SessionManager.Get<string>("TemporaryFileAccess");
        if (fileIDToCheck == id)
        {
            SetTemporaryFileAccess(null); // clear it out
            return true;

        }

        return false;
    }

    /// <summary>
    /// Temporarily allows for a file in the current session to be downloaded
    /// </summary>
    /// <param name="fileID">The file ID.</param>
    /// <remarks></remarks>
    public static void SetTemporaryFileAccess(string fileID)
    {
        SessionManager.Set( "TemporaryFileAccess", fileID );
    }
}