<%@ Page Title="Projekt Information" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProjectInfoPage.aspx.cs" Inherits="ProStudCreator.ProjectInfoPage" %>

<asp:Content ID="ProjectInofpageContent" ContentPlaceHolderID="MainContent" runat="server">
    <script language="javascript" type="text/javascript">

            function OnClientFileOpen(oExplorer, args) {
                var item = args.get_item();


                // File is a image document, do not open a new window
                args.set_cancel(true);

                // Tell browser to open file directly
                $incomingUrl = "ProjectFilesDownload?fname=" + item._name + "&id=<%: int.Parse(Request.QueryString["id"]) %>";
                document.location = encodeURI($incomingUrl);

            }

            global.OnClientFileOpen = OnClientFileOpen;
    </script>
    <div class="well newProjectSettings">
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
                <div runat="server" id="divExpert">
                    <br />
                    <br />
                    <asp:Label runat="server" Text="Experte:" CssClass="control-label col-sm-3"></asp:Label>
                    <asp:Label runat="server" ID="ExpertName" CssClass="col-sm-9 alignbottom"></asp:Label>
                </div>
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
                <div runat="server" id="divPresentation">
                    <br />
                    <br />
                    <asp:Label runat="server" Text="Schlusspräsentation:" CssClass="control-label col-sm-3" ID="lblProjectEndPresentation"></asp:Label>
                    <asp:Label runat="server" ID="ProjectEndPresentation" CssClass="col-sm-9 alignbottom"></asp:Label>
                </div>
                <div runat="server" id="divBachelor">
                    <br />
                    <br />
                    <asp:Label runat="server" Text="Ausstellung Bachelorthesis:" ID="lblAussstellungBachelorthese" CssClass="control-label col-sm-3"></asp:Label>
                    <asp:Label runat="server" ID="ProjectExhibition" CssClass="col-sm-3 alignbottom"></asp:Label>
                </div>
                <br />
            </div>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" Text="Durchführungssprache:" CssClass="control-label col-sm-3"></asp:Label>
                <div class="col-sm-3">
                    <asp:DropDownList runat="server" DataValueField="Id" DataTextField="DisplayName" ID="drpLogLanguage" AutoPostBack="false" CssClass="form-control">
                        <asp:ListItem Text="(Bitte Auswählen)" Value="0" />
                        <asp:ListItem Text="Englisch" Value="1" />
                        <asp:ListItem Text="Deutsch" Value="2" />
                    </asp:DropDownList>
                </div>
            </div>
            <div runat="server" visible="False" id="divGradeStudent1">
                <br />
                <div class="form-group">
                    <asp:Label runat="server" Text="Note:" CssClass="control-label col-sm-3" ID="lblGradeStudent1"></asp:Label>
                    <div class="col-sm-3">
                        <asp:TextBox runat="server" TextMode="Number" min="1" max="6" step="0.1" ID="nbrGradeStudent1" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div class="form-group" id="divGradeStudent2" runat="server" visible="False">
                <asp:Label runat="server" Text="Note:" ID="lblGradeStudent2" CssClass="control-label col-sm-3"></asp:Label>
                <div class="col-sm-3">
                    <asp:TextBox runat="server" TextMode="Number" min="1" max="6" step="0.1" ID="nbrGradeStudent2" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
            <br />
            <div class="form-group">
                <asp:Label runat="server" Text="Verrechnungsstatus:" CssClass="control-label col-sm-3"></asp:Label>
                <div class="col-sm-3">
                    <asp:DropDownList runat="server" DataValueField="Id" OnSelectedIndexChanged="DrpBillingstatusChanged" DataTextField="DisplayName" ID="drpBillingstatus" AutoPostBack="true" CssClass="form-control"></asp:DropDownList>
                </div>
            </div>
            <asp:UpdatePanel runat="server" ID="BillAddressForm" UpdateMode="Conditional">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="drpBillingstatus" EventName="SelectedIndexChanged" />
                </Triggers>
                <ContentTemplate>
                    <asp:PlaceHolder runat="server" ID="BillAddressPlaceholder">
                        <hr />
                        <h3>Kundeninformationen</h3>
                        <div class="form-group" style="text-align: left">
                            <asp:Label runat="server" Text="Unternehmen*:" CssClass="control-label col-sm-3"></asp:Label>
                            <div class="col-sm-6">
                                <asp:TextBox runat="server" ID="txtClientCompany" CssClass="form-control maxWidth"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group" style="text-align: left">
                            <asp:Label runat="server" Text="Anrede*:" CssClass="control-label col-sm-3"></asp:Label>
                            <div class="col-sm-3">
                                <asp:DropDownList runat="server" DataValueField="Id" DataTextField="DisplayName" ID="drpClientTitle" AutoPostBack="false" CssClass="form-control">
                                    <asp:ListItem Text="Herr" Value="1" />
                                    <asp:ListItem Text="Frau" Value="2" />
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="form-group" style="text-align: left">
                            <asp:Label runat="server" Text="Vor- und Nachname*:" CssClass="control-label col-sm-3"></asp:Label>
                            <div class="col-sm-6">
                                <asp:TextBox runat="server" ID="txtClientName" CssClass="form-control maxWidth"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group" style="text-align: left">
                            <asp:Label runat="server" Text="Abteilung:" CssClass="control-label col-sm-3"></asp:Label>
                            <div class="col-sm-6">
                                <asp:TextBox runat="server" ID="txtClientDepartment" CssClass="form-control maxWidth" Placeholder="Falls vorhanden"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group" style="text-align: left">
                            <asp:Label runat="server" Text="Strasse und Nummer*:" CssClass="control-label col-sm-3"></asp:Label>
                            <div class="col-sm-6">
                                <asp:TextBox runat="server" ID="txtClientStreet" CssClass="form-control maxWidth"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group" style="text-align: left">
                            <asp:Label runat="server" Text="PLZ*:" CssClass="control-label col-sm-3"></asp:Label>
                            <div class="col-sm-6">
                                <asp:TextBox runat="server" ID="txtClientPLZ" CssClass="form-control maxWidth"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group" style="text-align: left">
                            <asp:Label runat="server" Text="Ort*:" CssClass="control-label col-sm-3"></asp:Label>
                            <div class="col-sm-6">
                                <asp:TextBox runat="server" ID="txtClientCity" CssClass="form-control maxWidth"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group" style="text-align: left">
                            <asp:Label runat="server" Text="Referenz des Kunden:" CssClass="control-label col-sm-3"></asp:Label>
                            <div class="col-sm-6">
                                <asp:TextBox runat="server" ID="txtClientReference" CssClass="form-control maxWidth" Placeholder="Falls vorhanden." ToolTip="z.B. Bestellnummer des Auftraggebers."></asp:TextBox>
                            </div>
                        </div>
                        <h6>Mit * markierte Felder sind Pflichtfelder.</h6>
                    </asp:PlaceHolder>
                </ContentTemplate>
            </asp:UpdatePanel>
            <hr />
            <asp:Label runat="server" Text="Projekt Artefakte (Dokumentation, Präsentation, Code):" CssClass="control-label col-sm-3"></asp:Label>
            <div class="form-group" >
                <telerik:RadFileExplorer RenderMode="Auto" runat="server" CssClass="col-sm-3" ID="FileExplorer" Width="650px" Height="350px"
                    AllowPaging="false" Skin="Default" VisibleControls="ListView,ContextMenus,Toolbar" ToolTip="Laden Sie hier Ihre Dokumente zum Projekt hoch." EnableCreateNewFolder="False" Configuration-ViewPaths="~\App_Data\ProjectFiles" Configuration-UploadPaths="~\App_Data\ProjectFiles" Configuration-DeletePaths="~\App_Data\ProjectFiles" EnableTheming="True" Configuration-AllowFileExtensionRename="False" OnItemCommand="FileExplorer_OnItemCommand" OnClientFileOpen="OnClientFileOpen">
                </telerik:RadFileExplorer>
            </div>
        </div>
        <div style="text-align: right;">
            <asp:Button runat="server" ID="BtnSaveChanges" OnClick="BtnSaveChanges_OnClick" CssClass="btn btn-default" Text="Speichern & Schliessen"></asp:Button>
            <asp:Button runat="server" ID="BtnSaveBetween" OnClick="BtnSaveBetween_OnClick" CssClass="btn btn-default" Text="Zwischenspeichern" />
            <asp:Button runat="server" ID="BtnCancel" OnClick="BtnCancel_OnClick" CssClass="btn btn-default" Text="Abbrechen"></asp:Button>
        </div>
    </div>
</asp:Content>



