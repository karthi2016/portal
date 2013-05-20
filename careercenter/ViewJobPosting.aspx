<%@ Page Language="C#" MasterPageFile="~/App_Master/GeneralPage.master" AutoEventWireup="true"
    CodeFile="ViewJobPosting.aspx.cs" Inherits="careercenter_ViewJobPosting" %>

<%@ Register Assembly="MemberSuite.SDK.Web" Namespace="MemberSuite.SDK.Web.Controls"
    TagPrefix="cc1" %>
<%@ Register Src="../controls/CustomFieldSet.ascx" TagName="CustomFieldSet" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TopMenu" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PageTitle" runat="Server">
    View Job Posting
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TopRightContent" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PageContent" runat="Server">
    <div class="section" style="margin-top: 10px">
        <div class="sectionHeaderTitle">
            <h2>
                <%= targetJobPosting.Name %></h2>
        </div>
        <asp:Literal ID="PageText" runat="server"></asp:Literal>
        <div class="sectionContent">
            <div align="center"> <B><%= targetJobPosting.CompanyName %></B></div>
            <%= targetJobPosting.Body %>
            <p>
            </p>
            <table>
                
                <tr id="trDivision" runat="server">
                    <td class="columnHeader" style="width: 150px">
                        <asp:Literal ID="lDivision" runat="server">Division:</asp:Literal>
                    </td>
                    <td>
                        <%= targetJobPosting.Division %>
                    </td>
                </tr>
                <tr id="trCode" runat="server">
                    <td class="columnHeader">
                        <asp:Literal ID="lReferenceCode" runat="server">Internal Reference Code:</asp:Literal>
                    </td>
                    <td>
                        <%= targetJobPosting.InternalReferenceCode %>
                    </td>
                </tr>
                <tr id="trLocation" runat="server">
                    <td class="columnHeader">
                       <asp:Literal ID="lLocation" runat="server">Location:</asp:Literal>
                    </td>
                    <td>
                        <%= targetJobPostingLocation.Name %>
                    </td>
                </tr>
                 <tr id="trCategories" runat="server">
                    <td class="columnHeader">
                        <asp:Literal ID="lCategories" runat="server">Categories:</asp:Literal>
                    </td>
                    <td>
                        <asp:Label ID="lblCategories" runat="server" />
                    </td>
                </tr>
            </table>
            <uc1:CustomFieldSet ID="CustomFieldSet1" runat="server" EditMode="false" />
        </div>
    </div>
    <div class="section" style="margin-top: 10px" runat="server" id="divTasks">
        <div class="sectionHeaderTitle">
            <h2>
                <asp:Literal ID="lTasks" runat="server">Tasks</asp:Literal></h2>
        </div>
        <asp:UpdatePanel ID="upApply" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <ul>
                    <li runat="server" id="liApply">
                        <asp:Label runat="server" ID="lblApply">Apply to this Posting (disabled until you <a href="ManageResumes.aspx">upload a resume)</a>
                        </asp:Label>  
                        <asp:LinkButton
                            runat="server" ID="lbApply" Visible="false" Text="Apply To Posting" OnClick="lbApply_Click" />
                            </li>
                </ul>
              <asp:Panel ID="pnlSelectResume" runat="server" Visible="false">
              <b>Select the Resume to Send:</b> <asp:DropDownList ID="ddlSelectResume" runat="server" /> 
                  <asp:Button ID="btnApply" runat="server" Text="Send this Resume" 
                      onclick="btnApply_Click" />
              </asp:Panel>
              
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="lbApply" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
        <ul runat="server">
            <li><a href="SearchJobPostings_Results.aspx"><asp:Literal ID="lBackToResults" runat="server">Back to Search Results</asp:Literal></a></li>
            <li><a href="/"><asp:Literal ID="lGoHome" runat="server">Go Home</asp:Literal></a></li>
        </ul>
    </div>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="FooterContent" runat="Server">
</asp:Content>
