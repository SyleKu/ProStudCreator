<%@ Page Title="Projekt Information" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProjectInfoPage.aspx.cs" Inherits="ProStudCreator.ProjectInfoPage" %>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="well newProjectSettings non-selectable">
        <asp:Label runat="server" ID="SiteTitle" Font-Size="24px" Height="50px" Text="Projektinformationen"></asp:Label>
        <div class="well contentDesign form-horizontal" style="background-color: #ffffff">
            <div class="form-group" style="text-align: left">
                <asp:Label runat="server" Text="Projektname:" CssClass="control-label col-sm-3"></asp:Label>
                <div class="col-sm-6">
                    <asp:TextBox runat="server" ID="ProjectTitle" CssClass="form-control maxWidth"></asp:TextBox>
                </div>
            </div>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" Text="Studierende:" CssClass="control-label col-sm-3"></asp:Label>
                <asp:Label runat="server" ID="Student1Name" CssClass="col-sm-3 alignbottom"></asp:Label>
                <asp:Label runat="server" ID="Student2Name" CssClass="col-sm-3 alignbottom"></asp:Label>
                <br />
                <br />
                <asp:Label runat="server" Text="Betreuung:" CssClass="control-label col-sm-3"></asp:Label>
                <asp:Label runat="server" ID="Advisor1Name" CssClass="col-sm-3 alignbottom"></asp:Label>
                <asp:Label runat="server" ID="Advisor2Name" CssClass="col-sm-3 alignbottom"></asp:Label>
                <br />
                <br />
                <asp:Label runat="server" Text="Experte:" CssClass="control-label col-sm-3"></asp:Label>
                <asp:Label runat="server" ID="ExpertName" CssClass="col-sm-9 alignbottom"></asp:Label>
                <br />
                <br />
            </div>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" Text="Art des Projektes:" CssClass="control-label col-sm-3"></asp:Label>
                <asp:Label runat="server" ID="lblProjectType" CssClass="col-sm-3 alignbottom"></asp:Label>
                <br />
                <br />
                <asp:Label runat="server" Text="Dauer:" CssClass="control-label col-sm-3"></asp:Label>
                <asp:Label runat="server" ID="lblProjectDuration" CssClass="col-sm-3 alignbottom"></asp:Label>
            </div>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" Text="Abgabe:" CssClass="control-label col-sm-3"></asp:Label>
                <asp:Label runat="server" ID="ProjectDelivery" CssClass="col-sm-3 alignbottom"></asp:Label>
                <br />
                <br />
                <asp:Label runat="server" Text="Schlusspräsentation:" CssClass="control-label col-sm-3" ID="lblProjectEndPresentation"></asp:Label>
                <asp:Label runat="server" ID="ProjectEndPresentation" CssClass="col-sm-9 alignbottom"></asp:Label>
                <br />
                <br />
                <asp:Label runat="server" Text="Ausstellung Bachelorthese:" ID="lblAussstellungBachelorthese" CssClass="control-label col-sm-3"></asp:Label>
                <asp:Label runat="server" ID="ProjectExhibition" CssClass="col-sm-3 alignbottom"></asp:Label>
                <br />
            </div>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" Text="Verrechnungsstatus:" CssClass="control-label col-sm-3"></asp:Label>
                <div class="col-sm-3">
                    <asp:DropDownList runat="server" DataValueField="Id" DataTextField="DisplayName" ID="SemesterDropdown" AutoPostBack="false" CssClass="form-control"></asp:DropDownList>
                </div>
            </div>
            <br />
            <div class="form-group">
                <asp:Label runat="server" Text="Durchführungssprache:" CssClass="control-label col-sm-3"></asp:Label>
                <div class="col-sm-3">
                    <asp:DropDownList runat="server" DataValueField="Id" DataTextField="DisplayName" ID="drpLogLanguage" AutoPostBack="false" CssClass="form-control"><asp:ListItem text="(Bitte Auswählen)" value="0"/><asp:ListItem text="Englisch" value="1"/><asp:Listitem text="Deutsch" value="2"/></asp:DropDownList>
                </div>
            </div>
            <br />
            <div class="form-group">
                <asp:Label runat="server" Text="Note:" CssClass="control-label col-sm-3"></asp:Label>
                <div class="col-sm-3">
                    <asp:TextBox runat="server" TextMode="Number" min="1" max="6" step="0.1" id="nbrGrade" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" ID="lblAddCode" CssClass="control-label col-sm-3" Text="Code hinzufügen:"></asp:Label>
                <div class="col-sm-3">
                    <asp:FileUpload runat="server" ID="fuAddCode" CssClass="control-label" />
                </div>
                <div class="col-sm-1">
                    <asp:LinkButton runat="server" ID="btnDeleteCode" OnClick="btnDeleteCode_OnClick" OnClientClick="return confirm('Wollen Sie den Code wirklich entfernen?');" CssClass="btn btn-default btnHeight imageRemoveMargin glyphicon glyphicon-remove" Visible="false"></asp:LinkButton>
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" ID="lblAddDoc" CssClass="control-label col-sm-3" Text="Dokumentation hinzufügen:"></asp:Label>
                <div class="col-sm-3">
                    <asp:FileUpload runat="server" ID="fuAddDoc" accept=".pdf" CssClass="control-label" />
                </div>
                <div class="col-sm-1">
                    <asp:LinkButton runat="server" ID="btnDeleteDoc" OnClick="btnDeleteDoc_OnClick" OnClientClick="return confirm('Wollen Sie die Dokumentation wirklich entfernen?');" CssClass="btn btn-default btnHeight imageRemoveMargin glyphicon glyphicon-remove" Visible="false"></asp:LinkButton>
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" ID="lblAddPresentation" CssClass="control-label col-sm-3" Text="Präsentation hinzufügen:"></asp:Label>
                <div class="col-sm-3">
                    <asp:FileUpload runat="server" ID="fuAddPresentation" accept=".pptx" CssClass="control-label" />
                </div>
                <div class="col-sm-1">
                    <asp:LinkButton runat="server" ID="btnDeletePresentation" OnClick="btnDeletePresentation_OnClick" OnClientClick="return confirm('Wollen Sie die Präsentation wirklich entfernen?');" CssClass="btn btn-default btnHeight imageRemoveMargin glyphicon glyphicon-remove" Visible="false"></asp:LinkButton>
                </div>
            </div>

        </div>
        <div style="text-align: right;">
            <asp:Button runat="server" ID="BtnSaveChanges" OnClick="BtnSaveChanges_OnClick" CssClass="btn btn-default" Text="Speichern"></asp:Button>
            <asp:Button runat="server" ID="BtnCancel" OnClick="BtnCancel_OnClick" CssClass="btn btn-default" Text="Abbrechen"></asp:Button>
        </div>
    </div>
</asp:Content>



