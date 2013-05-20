<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="UploadResume.aspx.cs" Inherits="careercenter_UploadResume" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    <asp:Literal runat="server" ID="litTitle" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                 <asp:Literal ID="lYourResumeInfo" runat="server">Your Resume Information</asp:Literal></h2>
        </div>
         <asp:Literal ID="PageText" runat="server"/>
        <div class="sectionContent">
            <table>
                <tr>
                    <td class="columnHeader">
                        <asp:Literal ID="lResumeName" runat="server">Resume Name:<span class="requiredField">*</span></asp:Literal>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="tbName" />
                        <asp:RequiredFieldValidator runat="server" ID="rfvName" Display="None" ControlToValidate="tbName"
                            ErrorMessage="Please specify a name for this resume." />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lIsActive" runat="server">Is Active:</asp:Literal>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="cbIsActive" />
                    </td>
                </tr>
                <tr>
                    <td class="columnHeader">
                         <asp:Literal ID="lFile" runat="server">File:</asp:Literal>
                    </td>
                    <td>
                        <asp:UpdatePanel ID="upFileUpload" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:HyperLink runat="server" ID="hlView" Text="(view uploaded file)" />
                                <asp:LinkButton runat="server" ID="lbDifferentFile" Text="(upload a different file)"
                                    OnClick="lbDifferentFile_Click" CausesValidation="false" />
                                <div runat="server" id="divUploadResume">
                                    <asp:FileUpload ID="fuUploadResume" Width="300px" runat="server" /><br />
                                    <asp:Literal ID="lFileLimit" runat="server">
                                    Any attempt to upload a file larger than 4MB will result in a system error!
                                    </asp:Literal>
                                </div>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="lbDifferentFile" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
                                                            <asp:CustomValidator ID="cvUploadResume" runat="server" ControlToValidate="fuUploadResume" ForeColor="Red"
                                        OnServerValidate="fuUploadResume_ServerValidate" ErrorMessage="The file specified is not a valid word document." />
                    </td>
                </tr>
            </table>
            <p>
            </p>
            <div style="text-align: center">
                <asp:Button runat="server" ID="btnSave" Text="Save" OnClick="btnSave_Click" />
                <asp:Button runat="server" ID="btnCancel" Text="Cancel" OnClick="btnCancel_Click" />
            </div>
        </div>
    </div>
   <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
               <ASP:Literal ID="lTasks" runat="server">Tasks</ASP:Literal></h2>
        </div>
        <ul>
            <li><asp:HyperLink ID="hlGoHome" runat="server" NavigateUrl="/">Go Home</asp:HyperLink>
            </li>
        </ul>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
