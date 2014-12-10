<%@ Page Language="C#" AutoEventWireup="true" CodeFile="JumpToExternal.aspx.cs" Inherits="JumpToExternal" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Redirecting to External Page</title>
</head>
<body>
    <div id="loadingFrame" style="text-align: center;"><br/>Redirecting<br/><br/><img src="/Images/loading1.gif"/></div>
    <form name="LoginForm" method="post" id="LoginForm" action="JumpToExternal.aspx">
        <input type="hidden" name="Token" id="Token" />
        <input type="hidden" name="ReturnUrl" id="ReturnUrl" />
        <input type="hidden" name="NextUrl" id="NextUrl" />
    </form>
    <script type="text/javascript" language="javascript">
        var hfReturnUrl = document.getElementById("ReturnUrl");
        var hfNextUrl = document.getElementById("NextUrl");
        var hfToken = document.getElementById("Token");
        var loginForm = document.getElementById("LoginForm");

        hfReturnUrl.value = '<asp:Literal runat=server ID="litReturnUrl" />';
        hfToken.value = '<asp:Literal runat=server ID="litToken" />';
        hfNextUrl.value = '<asp:Literal runat=server ID="litNextUrl" />';

        loginForm.action = '<asp:Literal runat=server ID="litAction" />';
        loginForm.submit();
    </script>
</body>
</html>