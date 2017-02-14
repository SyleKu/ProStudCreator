<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadEngine.aspx.cs" Inherits="ProStudCreator.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Upload Engine</title>
</head>
<body>
    <form id="Form" runat="server" enctype="multipart/form-data">
        <asp:ScriptManager runat="server" ID="ScriptManager"></asp:ScriptManager>
        <script type="text/javascript">
            function pageLoad(sender, args) {
                window.parent.register($get('<%= this.Form.ClientID%>'),
                   $get('<% = this.fileUpload.ClientID%>'));
            }

            function pageLoad() {
                $addHandler($get('upload'), 'click', onUploadClick);
            }
        </script>
        <asp:FileUpload runat="server" ID="fileUpload" />
    </form>
</body>
</html>
