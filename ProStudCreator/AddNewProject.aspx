<%@ Page Title="Neues Projekt" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AddNewProject.aspx.cs" Inherits="ProStudCreator.AddNewProject" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="refusedReason" class="well newProjectSettings non-selectable" runat="server" visible="false">
        <asp:Label runat="server" ID="refusedReasonTitle" Text="Ablehnungsgrund:" Font-Size="24px" Height="50px"></asp:Label>
        <div class="form-group">
            <asp:Label runat="server" CssClass="control-label" Text="Weshalb wird dieses Projekt abgelehnt? Der Text wird dem Projektersteller via E-Mail zugestellt."></asp:Label>
            <asp:TextBox runat="server" ID="refusedReasonText" CssClass="form-control contentRefuseDesign" TextMode="MultiLine"></asp:TextBox>
        </div>
        <asp:Button runat="server" ID="refuseDefinitiveNewProject" CssClass="btn btn-default refuseProject" Width="125px" Text="Ablehnen" OnClientClick="return confirm('Dieses Projekt wirklich ablehnen?');" OnClick="refuseDefinitiveNewProject_Click"></asp:Button>
        <asp:Button runat="server" ID="cancelRefusion" CssClass="btn btn-default" Text="Abbrechen" OnClick="cancelRefusion_Click" CausesValidation="false"></asp:Button>
    </div>

    <div id="newProjectDiv" class="well newProjectSettings non-selectable" runat="server">
        <asp:Label runat="server" ID="SiteTitle" Font-Size="24px" Height="50px"></asp:Label>
        <asp:PlaceHolder ID="AdminView" runat="server" Visible="false">
            <asp:Label runat="server" ID="CreatorID" CssClass="pull-right" Font-Size="24px" Height="50px"></asp:Label>
        </asp:PlaceHolder>
        <div class="well contentDesign form-horizontal" style="background-color: #ffffff">
            <div class="form-group" style="margin-top: 15px">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Projektname:"></asp:Label>
                <asp:TextBox runat="server" ID="ProjectName" CssClass="col-sm-9 form-control" MaxLength="100" placeholder="Projektname"></asp:TextBox>
                <asp:RequiredFieldValidator ID="ProjectNameValidator" CssClass="col-sm-9" ForeColor="Red" Display="Dynamic" ControlToValidate="ProjectName" runat="server" ErrorMessage="Bitte geben Sie einen Projektnamen an."></asp:RequiredFieldValidator>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Kunde (optional):"></asp:Label>
                <asp:TextBox runat="server" ID="Employer" CssClass="col-sm-9 form-control" placeholder="Name des Kunden"></asp:TextBox>
                <asp:TextBox runat="server" ID="EmployerMail" CssClass="col-sm-9 form-control" placeholder="E-Mail des Kunden" TextMode="Email"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Betreuung:"></asp:Label>
                <asp:TextBox runat="server" ID="NameBetreuer1" CssClass="col-sm-9 form-control" placeholder="Vorname Nachname des offiziellen Betreuers"></asp:TextBox>
                <asp:TextBox runat="server" ID="EMail1" CssClass="col-sm-9 form-control" placeholder="E-Mail des offiziellen Betreuers" TextMode="Email"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3"></asp:Label>
                <asp:TextBox runat="server" ID="NameBetreuer2" CssClass="col-sm-9 form-control" placeholder="Vorname Nachname des Zweitbetreuers"></asp:TextBox>
                <asp:TextBox runat="server" ID="EMail2" CssClass="col-sm-9 form-control" placeholder="E-Mail des Zweitbetreuers" TextMode="Email"></asp:TextBox>
            </div>
            <hr />
            <asp:UpdatePanel runat="server" class="form-group">
                <ContentTemplate>
                    <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Themengebiet:"></asp:Label>
                    <asp:ImageButton CssClass="img-rounded" ID="DesignUX" Height="60px" runat="server" ToolTip="Design, Usability, User Interfaces, ..." ImageUrl="pictures/projectTypDesignUXUnchecked.png" OnClick="DesignUX_Click" CausesValidation="false" />
                    <asp:ImageButton CssClass="img-rounded" ID="HW" Height="60px" runat="server" ToolTip="Hardwarenah, Embedded, Low-level, ..." ImageUrl="pictures/projectTypHWUnchecked.png" OnClick="HW_Click" CausesValidation="false" />
                    <asp:ImageButton CssClass="img-rounded" ID="CGIP" Height="60px" runat="server" ToolTip="Computergrafik, 3D, Bildverarbeitung, ..." ImageUrl="pictures/projectTypCGIPUnchecked.png" OnClick="CGIP_Click" CausesValidation="false" />
                    <asp:ImageButton CssClass="img-rounded" ID="MathAlg" Height="60px" runat="server" ToolTip="Mathematik, Algorithmen, Machine Learning, Data Mining, ..." ImageUrl="pictures/projectTypMathAlgUnchecked.png" OnClick="MathAlg_Click" CausesValidation="false" />
                    <asp:ImageButton CssClass="img-rounded" ID="AppWeb" Height="60px" runat="server" ToolTip="Mobile Apps, Webentwicklung, ..." ImageUrl="pictures/projectTypAppWebUnchecked.png" OnClick="AppWeb_Click" CausesValidation="false" />
                    <asp:ImageButton CssClass="img-rounded" ID="DBBigData" Height="60px" runat="server" ToolTip="Datenbanken, Big Data, Data Spaces, ..." ImageUrl="pictures/projectTypDBBigDataUnchecked.png" OnClick="DBBigData_Click" CausesValidation="false" />
                    <asp:Timer runat="server" Interval="60000" />
                </ContentTemplate>
            </asp:UpdatePanel>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Priorität 1"></asp:Label>
                <asp:DropDownList runat="server" ID="POneType" DataValueField="Id" DataTextField="Description" CssClass="form-control col-sm-1" Width="200px">
                </asp:DropDownList>
                <asp:DropDownList runat="server" ID="POneTeamSize" DataValueField="Id" DataTextField="Description" CssClass="form-control col-sm-1 teamSizeMarginLeft" Width="200px" OnSelectedIndexChanged="POneTeamSize_SelectedIndexChanged" AutoPostBack="true">
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Priorität 2"></asp:Label>
                <asp:DropDownList runat="server" ID="PTwoType" DataValueField="Id" DataTextField="Description" CssClass="form-control col-sm-1" Width="200px">
                </asp:DropDownList>
                <asp:DropDownList runat="server" ID="PTwoTeamSize" DataValueField="Id" DataTextField="Description" CssClass="form-control col-sm-1 teamSizeMarginLeft" Width="200px" OnSelectedIndexChanged="PTwoTeamSize_SelectedIndexChanged" AutoPostBack="true">
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Ausgangslage:"></asp:Label>
                <asp:TextBox runat="server" ID="InitialPositionContent" CssClass="col-sm-9 form-control" TextMode="MultiLine"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:Label runat="server" ID="AddPictureLabel" CssClass="control-label col-sm-3" Text="Bild hinzufügen:"></asp:Label>
                <asp:FileUpload runat="server" ID="AddPicture" accept=".jpeg,.jpg,.png" CssClass="control-label col-sm-4" />
                <asp:LinkButton runat="server" ID="DeleteImageButton" OnClick="deleteImage_Click" OnClientClick="return confirm('Dieses Bild wirklich entfernen?');" CssClass="btn btn-default btnHeight imageRemoveMargin glyphicon glyphicon-remove" Visible="false"></asp:LinkButton>
            </div>
            <div class="form-group">
                <asp:Label runat="server" ID="ImageLabel" CssClass="control-label col-sm-3" Text=""></asp:Label>
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
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Technologien:<br/>Schwerpunkte:<br/>Referenzen:"></asp:Label>
                <asp:TextBox runat="server" ID="ReferencesContent" CssClass="col-sm-9 form-control" placeholder="Technologien/Schwerpunkte/Referenzen" TextMode="MultiLine" MaxLength="1000"></asp:TextBox>
            </div>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Bemerkungen (optional):"></asp:Label>
                <asp:TextBox runat="server" ID="RemarksContent" CssClass="form-control col-sm-9" placeholder="Bemerkungen" TextMode="MultiLine" MaxLength="1000"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Reserviert für (optional):"></asp:Label>
                <asp:TextBox runat="server" ID="ReservationNameOne" CssClass="col-sm-9 form-control" placeholder="(Vorname Nachname)"></asp:TextBox>
                <asp:TextBox runat="server" ID="ReservationNameTwo" CssClass="col-sm-9 form-control contentReservation" placeholder="(Vorname Nachname)" Visible="false"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Institut:"></asp:Label>
                <asp:DropDownList runat="server" DataValueField="Id" DataTextField="DepartmentName" ID="Department" CssClass="form-control" Width="80px"></asp:DropDownList>
            </div>
        </div>
        <div style="text-align:right;">
            <asp:Button runat="server" ID="publishProject" Visible="false" CssClass="btn btn-default publishProject" Width="113px" Text="Veröffentlichen" OnClientClick="return confirm('Wirklich veröffentlichen?');" OnClick="publishProject_Click"></asp:Button>
            <asp:Button runat="server" ID="refuseProject" Visible="false" style="margin-right:32px;" CssClass="btn btn-default refuseProject" Width="113px" Text="Ablehnen" OnClick="refuseProject_Click"></asp:Button>
            <asp:Button runat="server" ID="moveProjectToTheNextSemester" Visible="false" CssClass="btn btn-default redButton" Text="Ins kommende Semester" OnClientClick="return confirm('Wirklich in das kommende Semester verschieben?');" OnClick="moveProjectToTheNextSemester_Click"></asp:Button>
            <asp:Button runat="server" ID="rollbackProject" Visible="false" style="margin-right:32px;" CssClass="btn btn-default rollbackMarginRight redButton" Text="Zurückziehen" OnClientClick="return confirm('Projekt wirklich zurückziehen?');" OnClick="rollbackProject_Click"></asp:Button>
            <asp:Button runat="server" ID="submitProject" Visible="false" style="margin-right:32px;" CssClass="btn btn-default greenButton" Text="Einreichen" OnClientClick="return confirm('Dieses Projekt offiziell einreichen?');" OnClick="submitProject_Click"></asp:Button>
            <asp:Button runat="server" ID="saveProject" OnClick="saveProjectButton" CssClass="btn btn-default" Text="Speichern"></asp:Button>
            <asp:Button runat="server" ID="editProject" Visible="false" OnClick="editProject_Click" CssClass="btn btn-default" Text="Bearbeiten"></asp:Button>
            <asp:Button runat="server" ID="cancelProject" CssClass="btn btn-default" TabIndex="5" Text="Abbrechen" OnClick="cancelNewProject_Click" CausesValidation="false"></asp:Button>
        </div>
    </div>
</asp:Content>
