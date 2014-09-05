<%@ Page Title="New Project" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AddNewProject.aspx.cs" Inherits="ProStudCreator.addNewProject" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Stylesheets" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
            <div class="well newProjectSettings non-selectable">
                <h3>Neues Projekt erstellen:</h3>
                <div class="well contentDesign form-horizontal" style="background-color:#ffffff">
                    <div class="form-group" style="margin-top: 25px">
                        <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Projektname:"></asp:Label>
                        <asp:TextBox runat="server" ID="ProjectName" CssClass="col-sm-9 form-control" MaxLength="100" placeholder="Projektname"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Betreuer:"></asp:Label>
                        <asp:TextBox runat="server" CssClass="col-sm-9 form-control" placeholder="Name des Betreuers"></asp:TextBox>
                        <asp:TextBox runat="server" CssClass="col-sm-9 form-control" placeholder="E-Mail des Betreuers" TextMode="Email"></asp:TextBox>
                        <asp:Button runat="server" CssClass="btn btn-default" Text="+"/>
                    </div>
                    <hr />
                    <div class="form-group">
                        <!--<asp:UpdatePanel runat="server">
                            <ContentTemplate>-->
                                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Projekttyp:"></asp:Label>
                                <asp:ImageButton CssClass="img-rounded" ID="DesignUX" Height="60px" runat="server" ImageUrl="/pictures/projectTypDesignUXUnchecked.png" OnClick="DesignUX_Click"/>
                                <asp:ImageButton CssClass="img-rounded" ID="HW" Height="60px" runat="server" ImageUrl="/pictures/projectTypHWUnchecked.png" OnClick="HW_Click"/>
                                <asp:ImageButton CssClass="img-rounded" ID="CGIP" Height="60px" runat="server" ImageUrl="/pictures/projectTypCGIPUnchecked.png" OnClick="CGIP_Click"/>
                                <asp:ImageButton CssClass="img-rounded" ID="MathAlg" Height="60px" runat="server" ImageUrl="/pictures/projectTypMathAlgUnchecked.png" OnClick="MathAlg_Click"/>
                                <asp:ImageButton CssClass="img-rounded" ID="AppWeb" Height="60px" runat="server" ImageUrl="/pictures/projectTypAppWebUnchecked.png" OnClick="AppWeb_Click"/>
                                <asp:ImageButton CssClass="img-rounded" ID="DBBigData" Height="60px" runat="server" ImageUrl="/pictures/projectTypDBBigDataUnchecked.png" OnClick="DBBigData_Click"/>
                            <!--</ContentTemplate>
                        </asp:UpdatePanel>-->
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Priorität 1"></asp:Label>
                        <asp:DropDownList runat="server" CssClass="form-control" Width="200px">
                            <asp:ListItem>P5 (180h pro Student)</asp:ListItem>
                            <asp:ListItem>P6 (360h pro Student)</asp:ListItem>
                            <asp:ListItem>P5 oder P6</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="form-group">
                        <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Priorität 2"></asp:Label>
                        <asp:DropDownList runat="server" CssClass="form-control" Width="200px">
                            <asp:ListItem>------</asp:ListItem>
                            <asp:ListItem>P5 (180h pro Student)</asp:ListItem>
                            <asp:ListItem>P6 (360h pro Student)</asp:ListItem>
                            <asp:ListItem>P5 oder P6</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Ausgangslage:"></asp:Label>
                        <asp:TextBox runat="server" CssClass="col-sm-9 form-control" placeholder="Ausgangslage" TextMode="MultiLine"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Bilder hinzufügen:"></asp:Label>
                        <asp:ImageButton ID="AddPicture" Height="100px" runat="server" ImageUrl="/pictures/addPicture.png" OnClick="AddPicture_Click"/>
                        <asp:PlaceHolder ID="PlaceHolder1" runat="server" ></asp:PlaceHolder>
                    </div>
                    <hr />
                    <div class="form-group">
                        <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Ziel der Arbeit:"></asp:Label>
                        <asp:TextBox runat="server" CssClass="col-sm-9 form-control" placeholder="Ziel der Arbeit" TextMode="MultiLine"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Problemstellung:"></asp:Label>
                        <asp:TextBox runat="server" CssClass="col-sm-9 form-control" placeholder="Problemstellung" TextMode="MultiLine"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Technologien / Fachliche Schwerpunkte / Referenzen:"></asp:Label>
                        <asp:TextBox runat="server" CssClass="col-sm-9 form-control" placeholder="Technologien / Fachliche Schwerpunkte / Referenzen" TextMode="MultiLine"></asp:TextBox>
                    </div>
                    <hr />
                    <div class="form-group">
                        <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Bemerkungen:"></asp:Label>
                        <asp:TextBox runat="server" CssClass="form-control col-sm-9" placeholder="Bemerkungen" TextMode="MultiLine"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Wichtigkeit:"></asp:Label>
                        <asp:DropDownList runat="server" CssClass="form-control" Width="280px">
                            <asp:ListItem>Normal</asp:ListItem>
                            <asp:ListItem>wichtig aus Sicht Institut oder FHNW</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <asp:Button runat="server" CssClass="btn btn-default" Text="Speichern" TabIndex="4" Width="113px" ID="saveNewProject"></asp:Button>
                <a class="btn btn-default" href="projectlist.aspx" tabindex="5">Abbrechen</a>
            </div> 
</asp:Content>
