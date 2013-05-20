<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Subscriptions.ascx.cs"
    Inherits="homepagecontrols_Subscriptions" %>
<div class="sectCont" runat="server" id="divSubscriptions">
    <div class="sectHeaderTitle hIconLetterFeather">
        <h2>
            <asp:Literal ID="Widget_MySubscriptions_Title" runat="Server">Subscriptions</asp:Literal>
        </h2>
    </div>
    <p />
    <ul style="margin-left: -20px">
        <asp:HyperLink ID="hlSubscribe" runat="server" NavigateUrl="/subscriptions/Subscribe.aspx"
            Visible="true"><li>Subscribe to a Publication</li></asp:HyperLink>
        <asp:HyperLink ID="hlViewMySubscriptions" runat="server" NavigateUrl="/subscriptions/ViewMySubscriptions.aspx"
            Visible="true"><li>View My Subscriptions</li></asp:HyperLink>
    </ul>
    <%--This is the placeholder for portal form generation. Removing it will render portal forms for this widget inoperable.--%>
    <div id="divForms" runat="server" visible="false"/>    
</div>
