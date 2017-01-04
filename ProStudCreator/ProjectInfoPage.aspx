<%@ Page Title="Projekt Information" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProjectInfoPage.aspx.cs" Inherits="ProStudCreator.ProjectInfoPage" %>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="well newProjectSettings non-selectable">
        <asp:Label runat="server" ID="SiteTitle" Font-Size="24px" Height="50px" ForeColor="#ff0000" Text="Diese Funktion ist noch in Entwicklung!"></asp:Label> 
        <div class="well contentDesign form-horizontal" style="background-color: #ffffff">
            <div class="form-group" style="text-align: left">
                <asp:Label runat="server" Text="Projektname:" CssClass="control-label col-sm-3"></asp:Label>
                <div class="col-sm-3">
                    <asp:TextBox runat="server" ID="ProjectTitle" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-sm-3">
                    <asp:Button runat="server" ID="btnProjectTitle" OnClick="ProjectTitleChanged" CssClass="btn btn-default greenButton" Text="Titel ändern"></asp:Button>
                </div>
            </div>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" Text="Studierende:" CssClass="control-label col-sm-3"></asp:Label>
                <asp:Label runat="server" ID="Student1Name" CssClass="col-sm-3 alignbottom" ></asp:Label>
                <asp:Label runat="server" ID="Student2Name" CssClass="col-sm-3 alignbottom"></asp:Label>
                <br />
                <br />
                <asp:Label runat="server" Text="Betreuung:" CssClass="control-label col-sm-3"></asp:Label>
                <asp:Label runat="server" ID="Advisor1Name" CssClass="col-sm-3 alignbottom"></asp:Label>
                <asp:Label runat="server" ID="Advisor2Name" CssClass="col-sm-3 alignbottom"></asp:Label>
                <br />
                <br />
                <asp:Label runat="server" Text="Experte:" CssClass="control-label col-sm-3"></asp:Label>
                <asp:Label runat="server" ID="ExpertName" CssClass="col-sm-3 alignbottom"></asp:Label>
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
                <asp:Label runat="server" ID="ProjectEndPresentation" CssClass="col-sm-3 alignbottom"></asp:Label>
                <br />
                <br />
                <asp:Label runat="server" Text="Ausstellung Bachelorthese:" CssClass="control-label col-sm-3"></asp:Label>
                <asp:Label runat="server" ID="ProjectExhibition" CssClass="col-sm-3 alignbottom"></asp:Label>
                <br />
            </div>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" Text="Kostenstatus:" CssClass="control-label col-sm-3"></asp:Label>
                <div class="col-sm-3">
                    <asp:DropDownList runat="server" DataValueField="Id" DataTextField="DisplayName" ID="SemesterDropdown" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="SemesterDropdown_OnSelectedIndexChanged"></asp:DropDownList>
                </div>
            </div>

        </div>
    </div>
</asp:Content>
