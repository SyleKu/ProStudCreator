<%@ Page Title="Projekte" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Projectlist.aspx.cs" Inherits="ProStudCreator.projectlist" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <asp:LoginView runat="server" ViewStateMode="Disabled">
        <LoggedInTemplate>
        <div class="well adminSettings">
            <h3>Erhaltene Projekte:</h3>
            <div class="well" style="background-color:#ffffff">
                Keine neuen Projekte
                <asp:GridView ID="GridView1" runat="server" Width="549px">
                </asp:GridView>
            </div>
        </div>
        <div class="well usernSettings">
            <a class="btn btn-default" style="font-size: 18px" href="addNewProject.aspx">Neues Projekt</a>
            <div class="radioButtonSettings non-selectable">
                <asp:RadioButton ID="rdoAllProjects" runat="server" Text="Alle Projekte" GroupName="SortProjects" Checked="True" />
                <asp:RadioButton ID="rdoMyProjects" runat="server" Text="Meine Projekte" GroupName="SortProjects"/>
                <asp:RadioButton ID="rdoAvailable" runat="server" Text="Veröffentlicht" GroupName="SortProjects"/>
                <asp:RadioButton ID="rdoNotAvailable" runat="server" Text="Nicht Veröffentlicht" GroupName="SortProjects"/>
            </div>
            <div class="well" style="background-color:#ffffff; margin-top: 10px;">
                Keine neuen Projekte
                <asp:GridView ID="GridView2" runat="server" Width="549px">
                </asp:GridView>
            </div>
        </div>
        </LoggedInTemplate>
    </asp:LoginView>
</asp:Content>