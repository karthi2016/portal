<%@ Page Language="C#" AutoEventWireup="true" CodeFile="KeepAlive.aspx.cs" Inherits="KeepAlive" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="/images/portal.css" />
    <script type="text/javascript" src="/js/portal.js"></script>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table cellpadding="5">
            <tr>
                <td align="center">
                    <span class="redHighlight">Warning!</span>
                </td>
            </tr>
            <tr>
                <td align="center">
                    For your protection, you will be logged out automatically in <span id="secondsLeft"
                        class="columnHeader"></span> seconds.
                </td>
            </tr>
            <tr>
                <td align="center">
                    <b>If you would like to extend your session for an additional
                        <%=Session.Timeout %>
                        minutes please click the button below.</b>
                </td>
            </tr>
            <tr>
                <td align="center">
                    <asp:Button runat="server" ID="btnContinue" Text="I'm still here" OnClick="btnContinue_Click" />
                </td>
            </tr>
        </table>
    </div>
    </form>
    <script language="javascript" type="text/javascript">
        var dtNow = new Date();
        dtNow.setMinutes(dtNow.getMinutes() + <%=Session.Timeout %>);
        
        TimeoutTick(dtNow, <%=(ConciergeAPI.CurrentUser != null).ToString().ToLower() %>);
    </script>
</body>
</html>
