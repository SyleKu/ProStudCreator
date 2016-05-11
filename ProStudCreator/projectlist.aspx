<%@ Page Title="IP5/IP6 Projekte" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Projectlist.aspx.cs" Inherits="ProStudCreator.Projectlist" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:PlaceHolder ID="AdminView" runat="server" Visible="false">
        <div class="well adminSettings non-selectable">
            <h3>Projekte zur Freigabe</h3>
            <div class="well" style="background-color: #ffffff">
                <asp:GridView ID="CheckProjects" ItemType="ProStudCreator.ProjectSingleElement" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" AutoGenerateColumns="False" OnRowCommand="ProjectRowClick">
                    <%--<AlternatingRowStyle BackColor="White" />--%>
                    <Columns>
                        <asp:BoundField DataField="advisorName" HeaderText="Betreuer" SortExpression="Advisor" ItemStyle-Width="200px" HtmlEncode="false" ItemStyle-Wrap="false" />
                        <asp:BoundField DataField="Institute" HeaderText="Institut" SortExpression="Institute" />
                        <asp:BoundField DataField="projectName" HeaderText="Projektname" SortExpression="projectName" ItemStyle-Width="100%" />
                        <asp:CheckBoxField HeaderText="P5" DataField="p5" SortExpression="P5" />
                        <asp:CheckBoxField HeaderText="P6" DataField="p6" SortExpression="P6" />
                        <asp:ImageField ControlStyle-CssClass="img-rounded imageHeight" DataImageUrlField="projectType1" HeaderText="Themen" ItemStyle-Width="20px" />
                        <asp:ImageField ControlStyle-CssClass="img-rounded imageHeight" DataImageUrlField="projectType2" ItemStyle-Width="20px" />
                        <asp:TemplateField ItemStyle-CssClass="nowrap">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" CommandName="editProject" ToolTip="Projekt bearbeiten" CommandArgument="<%# Item.id %>" CssClass="btn btn-default btnHeight glyphicon glyphicon-pencil"></asp:LinkButton><asp:LinkButton runat="server" CommandName="SinglePDF" ToolTip="PDF erzeugen" CommandArgument="<%# Item.id %>" CssClass="btn btn-default btnHeight glyph-pdf"></asp:LinkButton><asp:LinkButton runat="server" CommandName="deleteProject" ToolTip="Projekt löschen" OnClientClick="return confirm('Wollen Sie dieses Projekt wirklich löschen?');" CommandArgument="<%# Item.id %>" CssClass="btn btn-default btnHeight glyphicon glyphicon-remove"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
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
    </asp:PlaceHolder>
    <div class="well usernSettings non-selectable">
        <div class="radioButtonSettings non-selectable">
            <asp:RadioButtonList ID="ListFilter" RepeatDirection="Horizontal" runat="server" AutoPostBack="true">
                <asp:ListItem Value="AllPastProjects">Vergangene Semester</asp:ListItem>
                <asp:ListItem Value="AllCurrentProjects">Laufendes Semester</asp:ListItem>
                <asp:ListItem Value="AllFutureProjects">N&auml;chstes Semester</asp:ListItem>
                <asp:ListItem Value="InProgress" Selected="True">Eigene, nicht eingereicht</asp:ListItem>
                <asp:ListItem Value="Submitted">Eigene, eingereicht</asp:ListItem>
                <%--<asp:ListItem Value="Published">Eigene, veröffentlicht</asp:ListItem>--%>
            </asp:RadioButtonList>
        </div>
        <div class="well" style="background-color: #ffffff; margin-top: 10px;">
            <asp:GridView ID="AllProjects" ItemType="ProStudCreator.ProjectSingleElement" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" AutoGenerateColumns="False" OnRowCommand="ProjectRowClick" OnRowDataBound="AllProjects_RowDataBound" AllowSorting="True" OnSorting="AllProjects_Sorting">
                <%--<AlternatingRowStyle BackColor="White" />--%>
                <Columns>
                    <asp:BoundField DataField="advisorName" HeaderText="Betreuer" SortExpression="Advisor" HtmlEncode="false" ItemStyle-Wrap="false" />
                    <asp:BoundField DataField="Institute" HeaderText="Institut" SortExpression="Institute" />
                    <asp:BoundField DataField="projectName" HeaderText="Projektname" SortExpression="projectName" ItemStyle-Width="100%" />
                    <asp:CheckBoxField HeaderText="P5" DataField="p5" SortExpression="P5" />
                    <asp:CheckBoxField HeaderText="P6" DataField="p6" SortExpression="P6" />
                    <asp:ImageField ControlStyle-CssClass="img-rounded imageHeight" DataImageUrlField="projectType1" HeaderText="Themen" ItemStyle-Width="20px" />
                    <asp:ImageField ControlStyle-CssClass="img-rounded imageHeight" DataImageUrlField="projectType2" ItemStyle-Width="20px" />
                    <asp:TemplateField ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="editProjectButton" ToolTip="Projekt bearbeiten" CommandName="editProject" CommandArgument="<%# Item.id %>" CssClass="btn btn-default btnHeight glyphicon glyphicon-pencil"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="SinglePDFButton" ToolTip="PDF erzeugen" CommandName="SinglePDF" CommandArgument="<%# Item.id %>" CssClass="btn btn-default btnHeight glyph-pdf"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="deleteProjectButton" ToolTip="Projekt löschen" CommandName="deleteProject" OnClientClick="return confirm('Wollen Sie dieses Projekt wirklich löschen?');" CommandArgument="<%# Item.id %>" CssClass="btn btn-default btnHeight glyphicon glyphicon-remove"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
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
        <div style="font-size:70%">
            <asp:Button runat="server" Enabled="false" CssClass="btn btn-default btnHeight" BackColor="#CEECF5" /> <%--
            <asp:Button runat="server" Enabled="false" CssClass="btn btn-default btnHeight"/>--%> = In Bearbeitung&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button runat="server" Enabled="false" CssClass="btn btn-default btnHeight" BackColor="#A9F5A9" /> = Veröffentlicht&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button runat="server" Enabled="false" CssClass="btn btn-default btnHeight" BackColor="#F5A9A9" /> = Abgelehnt&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button runat="server" Enabled="false" CssClass="btn btn-default btnHeight" BackColor="#F3F781" /> = PDF >1 Seite&nbsp;&nbsp;&nbsp;&nbsp;
        </div>
        <div style="margin-top:16px;">
            <asp:Button runat="server" ID="newProject" CssClass="btn btn-default buttonFont" Text="Neues Projekt" OnClick="newProject_Click" />
            <asp:Button runat="server" ID="AllProjectsAsPDF" CssClass="btn btn-default buttonFont pdf" Text="Projekte als PDF" OnClick="AllProjectsAsPDF_Click" />
            <asp:Button runat="server" ID="AllProjectsAsExcel" CssClass="btn btn-default buttonFont" Text="Projektliste in Excel" OnClick="AllProjectsAsExcel_Click" />
        </div>
    </div>
</asp:Content>
