using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Searching.Operations;
using MemberSuite.SDK.Types;

public partial class documents_MoveFolder : PortalPage 
{

    public msFileFolder targetFolder;
    protected FolderInfo foldersAndFiles;
    
    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();
        targetFolder = LoadObjectFromAPI<msFileFolder>(ContextID);
        if (targetFolder == null)
            GoToMissingRecordPage();

        
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        return DocumentsLogic.CanWriteTo(targetFolder, ConciergeAPI.CurrentEntity.ID);
    }

    protected override void InitializePage()
    {
        base.InitializePage();


        using (var api = GetServiceAPIProxy())
        {
            foldersAndFiles = api.DescribeFolder(targetFolder.ID).ResultValue;
            DocumentsLogic.SetupContextAndParentFolders(api, targetFolder, hlFolderContext, foldersAndFiles,
                                                        rptParentFolders, true);

            // get the folder path
            lblSourceFolder.Text = DocumentsLogic.GetFolderPath(api, targetFolder.ID);
        }

        

        Search sFolders = new Search( msFileFolder.CLASS_NAME );
        sFolders.AddCriteria(Expr.Equals("FileCabinet", targetFolder.FileCabinet));
        sFolders.AddOutputColumn("FolderPath");
        sFolders.AddSortColumn("FolderLevel");
        sFolders.AddSortColumn("Name");

        ddlDestination.DataSource = APIExtensions.GetSearchResult(sFolders, 0, null).Table;
        ddlDestination.DataTextField = "FolderPath";
        ddlDestination.DataValueField = "ID";
        ddlDestination.DataBind();

        ddlDestination.Items.Insert(0, new ListItem("--- Select a Destination Folder ---", ""));

        
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoTo("BrowseFileFolder.aspx?contextID=" + targetFolder.ID);
    }

    protected void btnMove_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        string destinationFolderID = ddlDestination.SelectedValue;
        if ( targetFolder.ID == destinationFolderID )
            throw new ConciergeClientException(MemberSuite.SDK.Concierge.ConciergeErrorCode.IllegalParameter,
                                                  "You cannot move a folder onto itself.");

        if (destinationFolderID == targetFolder.ParentFolder )
            throw new ConciergeClientException(MemberSuite.SDK.Concierge.ConciergeErrorCode.IllegalParameter,
                                               "The source folder already resides in the specified folder. Please choose another destination folder.");

        targetFolder.ParentFolder = destinationFolderID;
        SaveObject(targetFolder);

        GoTo("BrowseFileFolder.aspx?contextID=" + targetFolder.ID, string.Format("Folder '{0}' has been moved successfully.", targetFolder.Name));

    }
}