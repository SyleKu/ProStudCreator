<%@ Page Title="Admin Bereich" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminPage.aspx.cs" Inherits="ProStudCreator.AdminPage" %>


<asp:Content ID="AdminPageContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="well newProjectSettings">
        <asp:Label runat="server" Font-Size="24px" Height="50px" Text="Diese Seite ist noch in der Entwicklung!" ForeColor="red"></asp:Label>
        <div class="well contentDesign form-horizontal" style="background-color: #ffffff">
            <asp:GridView ID="GVTasks" ItemType="ProStudCreator.ProjectSingleTask" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" AutoGenerateColumns="False" Width="100%">
                <%--<AlternatingRowStyle BackColor="White" />--%>
                <Columns>
                    <asp:BoundField DataField="project" HeaderText="Projekt" SortExpression="Project" ItemStyle-Width="60%" />
                    <asp:ImageField ControlStyle-CssClass="img-rounded imageHeight" DataImageUrlField="taskOrganiseExpert" HeaderText="Tasks" ItemStyle-Width="20px" />
                    <asp:ImageField ControlStyle-CssClass="img-rounded imageHeight" DataImageUrlField="taskOrganiseDate" ItemStyle-Width="20px" />
                    <asp:ImageField ControlStyle-CssClass="img-rounded imageHeight" DataImageUrlField="taskOrganiseRoom" ItemStyle-Width="20px" />
                    <asp:ImageField ControlStyle-CssClass="img-rounded imageHeight" DataImageUrlField="taskPayExpert" ItemStyle-Width="20px" />
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
    <asp:PlaceHolder ID="AdminView" runat="server">
        <div class="well adminSettings">
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
                        <asp:TemplateField ItemStyle-Wrap="false">
                            <ItemTemplate>
                                <a title="Projekt Informationen" class="btn btn-default btnHeight glyphicon glyphicon-info-sign" href="ProjectInfoPage?id=<%# Item.id %>"></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-Wrap="false">
                            <ItemTemplate>
                                <a title="Projekt bearbeiten" class="btn btn-default btnHeight glyphicon glyphicon-pencil" href="AddNewProject?id=<%# Item.id %>"></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-Wrap="false">
                            <ItemTemplate>
                                <a title="PDF anzeigen" class="btn btn-default btnHeight glyphicon glyph-pdf" href="PDF?dl=false&id=<%# Item.id %>"></a>
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
        </div>
    </asp:PlaceHolder>
    <div class="well newProjectSettings">
        <asp:Label runat="server" Font-Size="24px" Height="50px" Text="Zusätzliche Informationen"></asp:Label>
        <div class="well contentDesign form-horizontal" style="background-color: #ffffff">
            <div class="form-group">
                <asp:Label runat="server" Text="Projektzuteilungssoftware:" CssClass="control-label col-sm-3"></asp:Label>
                <asp:Label runat="server" CssClass="col-sm-3 alignbottom"><a href="https://github.com/i4Ds/ProDist" target="_blank">Git-Repository</a></asp:Label>
            </div>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" Text="Server-Infos:" CssClass="control-label col-sm-3"></asp:Label>
                <asp:Label runat="server" CssClass="col-sm-3 alignbottom">server1085.cs.technik.fhnw.ch</asp:Label>
            </div>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" Text="Ansprechpersonen:" CssClass="control-label col-sm-3"></asp:Label>
                <asp:Label runat="server" CssClass="col-sm-3 alignbottom"><a href="mailto:simon.felix@fhnw.ch" target="_blank">Simon Felix</a></asp:Label>
                <br />
                <br />
                <div class="col-sm-3"></div>
                <asp:Label runat="server" CssClass="col-sm-3 alignbottom"><a href="mailto:flavio.mueller@fhnw.ch" target="_blank">Flavio Müller</a></asp:Label>
                <br />
                <br />
                <div class="col-sm-3"></div>
                <asp:Label runat="server" CssClass="col-sm-3 alignbottom"><a href="mailto:stephen.randles@fhnw.ch" target="_blank">Stephen Randles</a></asp:Label>
            </div>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" Text="Semesterdaten:" CssClass="control-label col-sm-3"></asp:Label>
                <asp:Label runat="server" CssClass="col-sm-3 alignbottom"><a href="http://www.fhnw.ch/technik/bachelor/informationen/termine/studientermine" target="_blank">Akademischer Kalender</a></asp:Label>
                <br />
                <br />
                <div class="col-sm-3"></div>
                <asp:Label runat="server" CssClass="col-sm-3 alignbottom"><a href="http://www.fhnw.ch/technik/bachelor/informationen/termine" target="_blank">Stunden und Semesterpläne</a></asp:Label>
            </div>
        </div>
    </div>
    <div class="well newProjectSettings">
        <asp:Label runat="server" Font-Size="24px" Height="50px" Text="Excel-Export"></asp:Label>
        <div class="well contentDesign form-horizontal" style="background-color: #ffffff">
            <div class="form-group">

                <asp:Label runat="server" Text="Semester:" CssClass="control-label col-sm-3"></asp:Label>
                <div class="col-sm-3">
                    <asp:DropDownList runat="server" DataValueField="Id" DataTextField="Name" ID="SelectedSemester" AutoPostBack="false" CssClass="form-control col-sm-3 alignbottom "></asp:DropDownList>
                </div>
                <br/>
                <hr />
                <asp:Label runat="server" Text="Projekt-Spezifikation:" CssClass="control-label col-sm-3"></asp:Label>
                <asp:RadioButtonList ID="radioProjectStart" runat="server" RepeatDirection="Vertical" AutoPostBack="false" CssClass="col-sm-3, alignbottom" TextAlign="Right">
                    <asp:ListItem Value="StartingProjects">Startende Projekte</asp:ListItem>
                    <asp:ListItem Value="EndingProjects" Selected="True">Endende Projekte</asp:ListItem>
                </asp:RadioButtonList>
            </div>
        </div>
        <div style="text-align: right;">
            <asp:Button runat="server" ID="btnMarketingExport" OnClick="btnMarketingExport_OnClick" CssClass="btn btn-default" Text="Export"></asp:Button>
        </div>
    </div>
</asp:Content>
