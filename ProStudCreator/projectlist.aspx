<%@ Page Title="Projekte" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Projectlist.aspx.cs" Inherits="ProStudCreator.projectlist" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
   <%-- <asp:LoginView runat="server" ViewStateMode="Disabled">
        <LoggedInTemplate>--%>
        <div class="well adminSettings non-selectable">
            <h3>Erhaltene Projekte:</h3>
            <div class="well" style="background-color:#ffffff">
                Keine neuen Projekte
                <asp:GridView ID="CheckProjects" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" Width="1200px" AutoGenerateColumns="False" OnRowCommand="CheckProjectsEvent">
                    <AlternatingRowStyle BackColor="White" />
                    <Columns>
                        <asp:BoundField DataField="advisorName" HeaderText="Betreuer" SortExpression="Advisor" ItemStyle-Width="200px" />
                        <asp:BoundField DataField="advisorEmail" HeaderText="E-Mail" SortExpression="Advisor" ItemStyle-Width="200px" />
                        <asp:BoundField DataField="projectName" HeaderText="Projektname" SortExpression="Advisor" ItemStyle-Width="250px" />
                        <asp:CheckBoxField HeaderText="P5" SortExpression="Advisor" />
                        <asp:CheckBoxField HeaderText="P6" SortExpression="Advisor"/>                   
                        <asp:ImageField ItemStyle-CssClass="img-rounded imageHeight" DataImageUrlField="projectType1" HeaderText="Projekttyp" ItemStyle-Width="20px" />
                        <asp:ImageField ItemStyle-CssClass="img-rounded imageHeight" DataImageUrlField="projectType2" ItemStyle-Width="20px" />
                        <asp:ButtonField CommandName="showProject" ControlStyle-CssClass="btn btn-default btnHeight glyphicon glyphicon-eye-open" />
                        <asp:ButtonField CommandName="editProject" ControlStyle-CssClass="btn btn-default btnHeight glyphicon glyphicon-pencil" />
                        <asp:ButtonField CommandName="deleteProject" ControlStyle-CssClass="btn btn-default btnHeight glyphicon glyphicon-remove" />
                    </Columns>
                    <EditRowStyle BackColor="#2461BF" />
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                    <RowStyle BackColor="#EFF3FB" />
                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                    <SortedAscendingCellStyle BackColor="#F5F7FB" />
                    <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                    <SortedDescendingCellStyle BackColor="#E9EBEF" />
                    <SortedDescendingHeaderStyle BackColor="#4870BE" />
                </asp:GridView>
            </div>
        </div>
        <div class="well usernSettings non-selectable">
            <asp:Button runat="server" ID="newProject" CssClass="btn btn-default buttonFont" Text="Neues Projekt" OnClick="newProject_Click" />
            <asp:Button runat="server" ID="AllProjectsAsPDF" CssClass="btn btn-default buttonFont" Text="Erstelle PDF"/>
            <div class="radioButtonSettings non-selectable">
                <asp:RadioButton ID="rdoAllProjects" runat="server" Text="Alle Projekte" GroupName="SortProjects" Checked="True" />
                <asp:RadioButton ID="rdoMyProjects" runat="server" Text="Meine Projekte" GroupName="SortProjects"/>
                <asp:RadioButton ID="rdoAvailable" runat="server" Text="Veröffentlicht" GroupName="SortProjects"/>
                <asp:RadioButton ID="rdoNotAvailable" runat="server" Text="Nicht Veröffentlicht" GroupName="SortProjects"/>
            </div>
            <div class="well" style="background-color:#ffffff; margin-top: 10px;">
                Keine neuen Projekte

                <br />
                <asp:GridView ID="AllProjects" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" Width="1200px" AutoGenerateColumns="False" OnRowCommand="AllProjectsEvent">
                    <AlternatingRowStyle BackColor="White" />
                    <Columns>
                        <asp:BoundField DataField="id" HeaderText="ID" SortExpression="Advisor" />
                        <asp:BoundField DataField="advisorName" HeaderText="Betreuer" SortExpression="Advisor" ItemStyle-Width="200px" />
                        <asp:BoundField DataField="advisorEmail" HeaderText="E-Mail" SortExpression="Advisor" ItemStyle-Width="200px" />
                        <asp:BoundField DataField="projectName" HeaderText="Projektname" SortExpression="Advisor" ItemStyle-Width="250px" />
                        <asp:BoundField DataField="P5" HeaderText="P5" SortExpression="Advisor" />
                        <asp:BoundField DataField="P6" HeaderText="P6" SortExpression="Advisor" />                      
                        <asp:ImageField ItemStyle-CssClass="img-rounded imageHeight" DataImageUrlField="projectType1" HeaderText="Projekttyp" />
                        <asp:ImageField ItemStyle-CssClass="img-rounded imageHeight" DataImageUrlField="projectType2" />
                        <asp:ButtonField CommandName="showProject" ControlStyle-CssClass="btn btn-default btnHeight glyphicon glyphicon-eye-open" />
                        <asp:ButtonField CommandName="editProject" ControlStyle-CssClass="btn btn-default btnHeight glyphicon glyphicon-pencil" />
                        <asp:ButtonField CommandName="deleteProject" ControlStyle-CssClass="btn btn-default btnHeight glyphicon glyphicon-remove" />
                    </Columns>
                    <EditRowStyle BackColor="#2461BF" />
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                    <RowStyle BackColor="#EFF3FB" />
                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                    <SortedAscendingCellStyle BackColor="#F5F7FB" />
                    <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                    <SortedDescendingCellStyle BackColor="#E9EBEF" />
                    <SortedDescendingHeaderStyle BackColor="#4870BE" />
                </asp:GridView>
            </div>
        </div>
      <%--  </LoggedInTemplate>
    </asp:LoginView>--%>
</asp:Content>