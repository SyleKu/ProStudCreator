<%@ Page Title="New Project" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AddNewProject.aspx.cs" Inherits="ProStudCreator.AddNewProject" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Stylesheets" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">

        $('#InitialPositionContent').keypress(function (event) {

            var keycode = (event.keyCode ? event.keyCode : event.which);

            if (key == 8 || key == 127 || key == 37 || key == 38 || key == 39 || key == 40) {

                return true;

            } else {

                var textbox = document.getElementById("<%=InitialPositionContent.ClientID%>").value;
                if (textbox.trim().length >= 1000) {
                    return false;
                }
                else {
                    return true;
                }
            }
        });

    </script>
    <div class="well newProjectSettings non-selectable">
        <asp:Label runat="server" ID="SiteTitle" Font-Size="24px" Height="50px"></asp:Label>
        <asp:PlaceHolder ID="AdminView" runat="server" Visible="false">
            <asp:Label runat="server" ID="CreatorID" CssClass="pull-right" Font-Size="24px" Height="50px"></asp:Label>
        </asp:PlaceHolder>
        <div class="well contentDesign form-horizontal" style="background-color: #ffffff">
            <div class="form-group" style="margin-top: 15px">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Projektname:"></asp:Label>
                <asp:TextBox runat="server" ID="ProjectName" CssClass="col-sm-9 form-control" MaxLength="100" placeholder="Projektname"></asp:TextBox>
                <asp:CompareValidator runat="server" ControlToValidate="ProjectName" ID="CompareProjectName" ErrorMessage="Projectname already exists!" Visible="false"></asp:CompareValidator>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Auftraggeber:"></asp:Label>
                <asp:TextBox runat="server" ID="Employer" CssClass="col-sm-9 form-control" placeholder="Name des Auftraggebers"></asp:TextBox>
                <asp:TextBox runat="server" ID="EmployerMail" CssClass="col-sm-9 form-control" placeholder="E-Mail des Auftraggebers" TextMode="Email"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="BetreuerIn:"></asp:Label>
                <asp:TextBox runat="server" ID="NameBetreuer1" CssClass="col-sm-9 form-control" placeholder="Name des ersten Betreuers"></asp:TextBox>
                <asp:TextBox runat="server" ID="EMail1" CssClass="col-sm-9 form-control" placeholder="E-Mail des ersten Betreuers" TextMode="Email"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3"></asp:Label>
                <asp:TextBox runat="server" ID="NameBetreuer2" CssClass="col-sm-9 form-control" placeholder="Name des zweiten Betreuers"></asp:TextBox>
                <asp:TextBox runat="server" ID="EMail2" CssClass="col-sm-9 form-control" placeholder="E-Mail des zweiten Betreuers" TextMode="Email"></asp:TextBox>
            </div>
            <hr />
            <div class="form-group">
                <!--<asp:UpdatePanel runat="server">
                            <ContentTemplate>-->
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Projekttyp:"></asp:Label>
                <asp:ImageButton CssClass="img-rounded" ID="DesignUX" Height="60px" runat="server" ImageUrl="/pictures/projectTypDesignUXUnchecked.png" OnClick="DesignUX_Click" />
                <asp:ImageButton CssClass="img-rounded" ID="HW" Height="60px" runat="server" ImageUrl="/pictures/projectTypHWUnchecked.png" OnClick="HW_Click" />
                <asp:ImageButton CssClass="img-rounded" ID="CGIP" Height="60px" runat="server" ImageUrl="/pictures/projectTypCGIPUnchecked.png" OnClick="CGIP_Click" />
                <asp:ImageButton CssClass="img-rounded" ID="MathAlg" Height="60px" runat="server" ImageUrl="/pictures/projectTypMathAlgUnchecked.png" OnClick="MathAlg_Click" />
                <asp:ImageButton CssClass="img-rounded" ID="AppWeb" Height="60px" runat="server" ImageUrl="/pictures/projectTypAppWebUnchecked.png" OnClick="AppWeb_Click" />
                <asp:ImageButton CssClass="img-rounded" ID="DBBigData" Height="60px" runat="server" ImageUrl="/pictures/projectTypDBBigDataUnchecked.png" OnClick="DBBigData_Click" />
                <!--</ContentTemplate>
                        </asp:UpdatePanel>-->
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Priorität 1"></asp:Label>
                <asp:DropDownList runat="server" ID="POneContent" CssClass="form-control col-sm-1" Width="200px">
                    <asp:ListItem>P5 (180h pro Student)</asp:ListItem>
                    <asp:ListItem>P6 (360h pro Student)</asp:ListItem>
                    <asp:ListItem>P5 oder P6</asp:ListItem>
                </asp:DropDownList>
                <asp:DropDownList runat="server" ID="POneTeamSize" CssClass="form-control col-sm-1 teamSizeMarginLeft" Width="200px">
                    <asp:ListItem>Einzelarbeit</asp:ListItem>
                    <asp:ListItem>2er Team</asp:ListItem>
                    <asp:ListItem>1er oder 2er Team</asp:ListItem>
                </asp:DropDownList>
            </div>

            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Priorität 2"></asp:Label>
                <asp:DropDownList runat="server" ID="PTwoContent" CssClass="form-control col-sm-1" Width="200px">
                    <asp:ListItem>------</asp:ListItem>
                    <asp:ListItem>P5 (180h pro Student)</asp:ListItem>
                    <asp:ListItem>P6 (360h pro Student)</asp:ListItem>
                    <asp:ListItem>P5 oder P6</asp:ListItem>
                </asp:DropDownList>
                <asp:DropDownList runat="server" ID="PTwoTeamSize" CssClass="form-control col-sm-1 teamSizeMarginLeft" Width="200px">
                    <asp:ListItem>------</asp:ListItem>
                    <asp:ListItem>Einzelarbeit</asp:ListItem>
                    <asp:ListItem>2er Team</asp:ListItem>
                    <asp:ListItem>1er oder 2er Team</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Ausgangslage:"></asp:Label>
                <asp:TextBox runat="server" ID="InitialPositionContent" CssClass="col-sm-9 form-control" placeholder="Ausgangslage" TextMode="MultiLine"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:Label runat="server" ID="AddPictureLabel" CssClass="control-label col-sm-3" Text="Bild hinzufügen:"></asp:Label>
                <asp:FileUpload runat="server" ID="AddPicture" CssClass="control-label col-sm-4" />
            </div>
            <div class="form-group">
                <asp:Label runat="server" ID="ImageLabel" CssClass="control-label col-sm-3 " Text=""></asp:Label>
                <asp:Image runat="server" ID="Image1" CssClass="maxImageWidth img-rounded" Visible="false" />
            </div>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Ziel der Arbeit:"></asp:Label>
                <asp:TextBox runat="server" ID="ObjectivContent" CssClass="col-sm-9 form-control" placeholder="Ziel der Arbeit" TextMode="MultiLine" MaxLength="1000"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Problemstellung:"></asp:Label>
                <asp:TextBox runat="server" ID="ProblemStatementContent" CssClass="col-sm-9 form-control" placeholder="Problemstellung" TextMode="MultiLine" MaxLength="1000"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Technologien / Fachliche Schwerpunkte / Referenzen:"></asp:Label>
                <asp:TextBox runat="server" ID="ReferencesContent" CssClass="col-sm-9 form-control" placeholder="Technologien / Fachliche Schwerpunkte / Referenzen" TextMode="MultiLine" MaxLength="1000"></asp:TextBox>
            </div>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Bemerkungen:"></asp:Label>
                <asp:TextBox runat="server" ID="RemarksContent" CssClass="form-control col-sm-9" placeholder="Bemerkungen" TextMode="MultiLine" MaxLength="1000"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Wichtigkeit:"></asp:Label>
                <asp:DropDownList runat="server" ID="ImportanceContent" CssClass="form-control" Width="280px">
                    <asp:ListItem>Normal</asp:ListItem>
                    <asp:ListItem>wichtig aus Sicht Institut oder FHNW</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Abteilung:"></asp:Label>
                <asp:DropDownList runat="server" ID="Department" CssClass="form-control" Width="80px">
                    <asp:ListItem>i4Ds</asp:ListItem>
                    <asp:ListItem>IMVS</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Projekt noch in Bearbeitung:"></asp:Label>
                <asp:CheckBox runat="server" ID="InProgressCheckBox" CssClass="checkbox-inline" />
            </div>
        </div>
        <asp:Button runat="server" ID="saveNewProject" OnClick="saveProject" CssClass="btn btn-default marginLeftZero" TabIndex="4" Width="113px"></asp:Button>
        <asp:Button runat="server" ID="cancelNewProject" CssClass="btn btn-default marginLeftZero" TabIndex="5" Text="Abbrechen" OnClick="cancelNewProject_Click" />
        <asp:Button runat="server" ID="publishProject" Visible="false" CssClass="btn btn-default pull-right" Width="113px" Text="Publish" OnClick="publishProject_Click"></asp:Button>
    </div>
</asp:Content>
