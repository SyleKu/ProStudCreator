<%@ Page Title="New Project" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AddNewProject.aspx.cs" Inherits="ProStudCreator.AddNewProject" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Stylesheets" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="newProjectDiv" class="well newProjectSettings non-selectable" runat="server">
        <asp:Label runat="server" ID="SiteTitle" Font-Size="24px" Height="50px"></asp:Label>
        <asp:PlaceHolder ID="AdminView" runat="server" Visible="false">
            <asp:Label runat="server" ID="CreatorID" CssClass="pull-right" Font-Size="24px" Height="50px"></asp:Label>
        </asp:PlaceHolder>
        <div class="well contentDesign form-horizontal" style="background-color: #ffffff">
            <div class="form-group" style="margin-top: 15px">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Projektname:"></asp:Label>
                <asp:TextBox runat="server" ID="ProjectName" CssClass="col-sm-9 form-control" MaxLength="100" placeholder="Projektname"></asp:TextBox>
                <asp:RequiredFieldValidator ID="ProjectNameValidator" CssClass="col-sm-9" ForeColor="Red" Display="Dynamic" ControlToValidate="ProjectName" runat="server" ErrorMessage="Der Projektname darf nicht leer sein!"></asp:RequiredFieldValidator>
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
                <asp:RequiredFieldValidator ID="NameBetreuerValidator" CssClass="col-sm-9" ForeColor="Red" Display="Dynamic" ControlToValidate="NameBetreuer1" runat="server" ErrorMessage="Der Name des ersten Betreuers darf nicht leer sein!"></asp:RequiredFieldValidator>
                <asp:RequiredFieldValidator ID="EMail1BetreuerValidator" CssClass="col-sm-9" ForeColor="Red" Display="Dynamic" ControlToValidate="EMail1" runat="server" ErrorMessage="E-Mail des ersten Betreuers darf nicht leer sein!"></asp:RequiredFieldValidator>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3"></asp:Label>
                <asp:TextBox runat="server" ID="NameBetreuer2" CssClass="col-sm-9 form-control" placeholder="Name des zweiten Betreuers"></asp:TextBox>
                <asp:TextBox runat="server" ID="EMail2" CssClass="col-sm-9 form-control" placeholder="E-Mail des zweiten Betreuers" TextMode="Email"></asp:TextBox>
            </div>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Projekttyp:"></asp:Label>
                <asp:ImageButton CssClass="img-rounded" ID="DesignUX" Height="60px" runat="server" ImageUrl="/pictures/projectTypDesignUXUnchecked.png" OnClick="DesignUX_Click" CausesValidation="false" />
                <asp:ImageButton CssClass="img-rounded" ID="HW" Height="60px" runat="server" ImageUrl="/pictures/projectTypHWUnchecked.png" OnClick="HW_Click" CausesValidation="false" />
                <asp:ImageButton CssClass="img-rounded" ID="CGIP" Height="60px" runat="server" ImageUrl="/pictures/projectTypCGIPUnchecked.png" OnClick="CGIP_Click" CausesValidation="false" />
                <asp:ImageButton CssClass="img-rounded" ID="MathAlg" Height="60px" runat="server" ImageUrl="/pictures/projectTypMathAlgUnchecked.png" OnClick="MathAlg_Click" CausesValidation="false" />
                <asp:ImageButton CssClass="img-rounded" ID="AppWeb" Height="60px" runat="server" ImageUrl="/pictures/projectTypAppWebUnchecked.png" OnClick="AppWeb_Click" CausesValidation="false" />
                <asp:ImageButton CssClass="img-rounded" ID="DBBigData" Height="60px" runat="server" ImageUrl="/pictures/projectTypDBBigDataUnchecked.png" OnClick="DBBigData_Click" CausesValidation="false" />
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Priorität 1"></asp:Label>
                <asp:DropDownList runat="server" ID="POneContent" CssClass="form-control col-sm-1" Width="200px">
                    <asp:ListItem>P5 (180h pro Student)</asp:ListItem>
                    <asp:ListItem>P6 (360h pro Student)</asp:ListItem>
                    <asp:ListItem>P5 oder P6</asp:ListItem>
                </asp:DropDownList>
                <asp:DropDownList runat="server" ID="POneTeamSize" CssClass="form-control col-sm-1 teamSizeMarginLeft" Width="200px" OnSelectedIndexChanged="POneTeamSize_SelectedIndexChanged" AutoPostBack="true">
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
                <asp:DropDownList runat="server" ID="PTwoTeamSize" CssClass="form-control col-sm-1 teamSizeMarginLeft" Width="200px" OnSelectedIndexChanged="PTwoTeamSize_SelectedIndexChanged" AutoPostBack="true">
                    <asp:ListItem>------</asp:ListItem>
                    <asp:ListItem>Einzelarbeit</asp:ListItem>
                    <asp:ListItem>2er Team</asp:ListItem>
                    <asp:ListItem>1er oder 2er Team</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Ausgangslage:"></asp:Label>
                <asp:TextBox runat="server" ID="InitialPositionContent" CssClass="col-sm-9 form-control" placeholder="Ausgangslage" TextMode="MultiLine"></asp:TextBox>
                <asp:RequiredFieldValidator ID="InitialPositionContentValidator" CssClass="col-sm-9" ForeColor="Red" Display="Dynamic" ControlToValidate="InitialPositionContent" runat="server" ErrorMessage="Die Ausgangslage darf nicht leer sein!"></asp:RequiredFieldValidator>
            </div>
            <div class="form-group">
                <asp:Label runat="server" ID="AddPictureLabel" CssClass="control-label col-sm-3" Text="Bild hinzufügen:"></asp:Label>
                <asp:FileUpload runat="server" ID="AddPicture" CssClass="control-label col-sm-4" />
                <asp:LinkButton runat="server" ID="DeleteImageButton" OnClick="deleteImage_Click" OnClientClick="return confirm('Wollen Sie wirklich dieses Bild löschen?');" CssClass="btn btn-default btnHeight imageRemoveMargin glyphicon glyphicon-remove" Visible="false"></asp:LinkButton>
            </div>
            <div class="form-group">
                <asp:Label runat="server" ID="ImageLabel" CssClass="control-label col-sm-3" Text=""></asp:Label>
                <asp:Image runat="server" ID="Image1" CssClass="maxImageWidth img-rounded" Visible="false" />
            </div>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Ziel der Arbeit:"></asp:Label>
                <asp:TextBox runat="server" ID="ObjectivContent" CssClass="col-sm-9 form-control" placeholder="Ziel der Arbeit" TextMode="MultiLine" MaxLength="1000"></asp:TextBox>
                <asp:RequiredFieldValidator ID="ObjectivContentValidator" CssClass="col-sm-9" ForeColor="Red" Display="Dynamic" ControlToValidate="ObjectivContent" runat="server" ErrorMessage="Das Feld 'Ziel der Arbeit' darf nicht leer sein!"></asp:RequiredFieldValidator>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Problemstellung:"></asp:Label>
                <asp:TextBox runat="server" ID="ProblemStatementContent" CssClass="col-sm-9 form-control" placeholder="Problemstellung" TextMode="MultiLine" MaxLength="1000"></asp:TextBox>
                <asp:RequiredFieldValidator ID="ProblemStatementContentValidator" CssClass="col-sm-9" ForeColor="Red" Display="Dynamic" ControlToValidate="ProblemStatementContent" runat="server" ErrorMessage="Die Problemstellung darf nicht leer sein!"></asp:RequiredFieldValidator>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Technologien / Fachliche Schwerpunkte / Referenzen:"></asp:Label>
                <asp:TextBox runat="server" ID="ReferencesContent" CssClass="col-sm-9 form-control" placeholder="Technologien / Fachliche Schwerpunkte / Referenzen" TextMode="MultiLine" MaxLength="1000"></asp:TextBox>
                <asp:RequiredFieldValidator ID="ReferencesContentValidator" CssClass="col-sm-9" ForeColor="Red" Display="Dynamic" ControlToValidate="ReferencesContent" runat="server" ErrorMessage="Das Feld 'Technologien / Fachliche Schwerpunkte / Referenzen' darf nicht leer sein!"></asp:RequiredFieldValidator>
            </div>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Bemerkungen (optional):"></asp:Label>
                <asp:TextBox runat="server" ID="RemarksContent" CssClass="form-control col-sm-9" placeholder="Bemerkungen" TextMode="MultiLine" MaxLength="1000"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Reservation (optional):"></asp:Label>
                <asp:TextBox runat="server" ID="ReservationNameOne" CssClass="col-sm-9 form-control" placeholder="Vollständiger Name des Studierender"></asp:TextBox>
                <asp:TextBox runat="server" ID="ReservationNameTwo" CssClass="col-sm-9 form-control contentReservation" placeholder="Vollständiger Name des Studierender" Visible="false"></asp:TextBox>
            </div>
            <!-- CANCELLED PART !!
            <div class="form-group">
                <asp:Label Visible="false" runat="server" CssClass="control-label col-sm-3" Text="Wichtigkeit:"></asp:Label>
                <asp:DropDownList Visible="false" runat="server" ID="ImportanceContent" CssClass="form-control" Width="280px">
                    <asp:ListItem>Normal</asp:ListItem>
                    <asp:ListItem>wichtig aus Sicht Institut oder FHNW</asp:ListItem>
                </asp:DropDownList>
            </div> -->
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Abteilung:"></asp:Label>
                <asp:DropDownList runat="server" ID="Department" CssClass="form-control" Width="80px">
                    <asp:ListItem>i4Ds</asp:ListItem>
                    <asp:ListItem>IMVS</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <asp:Button runat="server" ID="saveNewProject" OnClick="saveProject" CssClass="btn btn-default marginLeftZero" TabIndex="4" Width="113px"></asp:Button>
        <asp:Button runat="server" ID="cancelNewProject" CssClass="btn btn-default marginLeftZero" TabIndex="5" Text="Abbrechen" OnClick="cancelNewProject_Click" CausesValidation="false"></asp:Button>
        <asp:Button runat="server" ID="submitProject" Visible="false" CssClass="btn btn-default marginLeftZero" Text="Projekt einreichen" OnClientClick="return confirm('Wollen Sie wirklich dieses Projekt einreichen?');" OnClick="submitProject_Click"></asp:Button>
        <asp:Button runat="server" ID="publishProject" Visible="false" CssClass="btn btn-default publishProject pull-right" Width="113px" Text="Publish" OnClick="publishProject_Click"></asp:Button>
        <asp:Button runat="server" ID="refuseNewProject" Visible="false" CssClass="btn btn-default refuseProject pull-right" Width="113px" Text="Refuse" OnClick="refuseProject_Click"></asp:Button>
    </div>
</asp:Content>
