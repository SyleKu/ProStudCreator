<%@ Page Title="Projekt Information" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProjectInfoPage.aspx.cs" Inherits="ProStudCreator.ProjectInfoPage" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="ProjectInofpageContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="well newProjectSettings">
        <asp:Label runat="server" ID="SiteTitle" Font-Size="24px" Height="50px" Text="Projektinformationen"></asp:Label>
        <div class="well contentDesign form-horizontal" style="background-color: #ffffff">
            <div class="form-group" style="text-align: left">
                <asp:Label runat="server" Text="Projektname:" CssClass="control-label col-sm-3"></asp:Label>
                <div class="col-sm-6">
                    <asp:TextBox runat="server" ID="ProjectTitle" CssClass="form-control maxWidth" MaxLength="100"></asp:TextBox>
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
                            <asp:Label runat="server" Text="Kundentyp:" CssClass="control-label col-sm-3"></asp:Label>
                            <div class="col-sm-7 radioButtonSettings">
                                <asp:RadioButtonList runat="server" RepeatDirection="Horizontal" TextAlign="Right" BorderStyle="None" ID="radioClientType" AutoPostBack="true" OnSelectedIndexChanged="radioClientType_SelectedIndexChanged">
                                    <asp:ListItem Text=" Unternehmen" Value="Company" />
                                    <asp:ListItem Text=" Privatperson" Value="PrivatePerson" />
                                </asp:RadioButtonList>
                            </div>
                        </div>
                        <div class="col-sm-1"></div>
                        <hr class="col-sm-10" />
                        <div style="clear: both"></div>
                        <asp:UpdatePanel runat="server" ID="updateClientCompany" UpdateMode="Conditional">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="radioClientType" EventName="SelectedIndexChanged" />
                            </Triggers>
                            <ContentTemplate>
                                <div class="form-group" style="text-align: left" runat="server" id="divClientCompany">
                                    <asp:Label runat="server" Text="Unternehmen*:" CssClass="control-label col-sm-3"></asp:Label>
                                    <div class="col-sm-6">
                                        <asp:TextBox runat="server" ID="txtClientCompany" CssClass="form-control maxWidth" MaxLength="255"></asp:TextBox>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
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
                                <asp:TextBox runat="server" ID="txtClientName" CssClass="form-control maxWidth" MaxLength="100"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group" style="text-align: left">
                            <asp:Label runat="server" Text="E-Mail Adresse*:" CssClass="control-label col-sm-3"></asp:Label>
                            <div class="col-sm-6">
                                <asp:TextBox runat="server" ID="txtClientEmail" CssClass="form-control maxWidth" MaxLength="100"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group" style="text-align: left">
                            <asp:Label runat="server" Text="Abteilung:" CssClass="control-label col-sm-3"></asp:Label>
                            <div class="col-sm-6">
                                <asp:TextBox runat="server" ID="txtClientDepartment" CssClass="form-control maxWidth" Placeholder="Falls vorhanden" MaxLength="50"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group" style="text-align: left">
                            <asp:Label runat="server" Text="Strasse und Nummer*:" CssClass="control-label col-sm-3"></asp:Label>
                            <div class="col-sm-6">
                                <asp:TextBox runat="server" ID="txtClientStreet" CssClass="form-control maxWidth" MaxLength="100"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group" style="text-align: left">
                            <asp:Label runat="server" Text="PLZ*:" CssClass="control-label col-sm-3"></asp:Label>
                            <div class="col-sm-6">
                                <asp:TextBox runat="server" ID="txtClientPLZ" CssClass="form-control maxWidth" MaxLength="10"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group" style="text-align: left">
                            <asp:Label runat="server" Text="Ort*:" CssClass="control-label col-sm-3"></asp:Label>
                            <div class="col-sm-6">
                                <asp:TextBox runat="server" ID="txtClientCity" CssClass="form-control maxWidth" MaxLength="100"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-group" style="text-align: left">
                            <asp:Label runat="server" Text="Referenz des Kunden:" CssClass="control-label col-sm-3"></asp:Label>
                            <div class="col-sm-6">
                                <asp:TextBox runat="server" ID="txtClientReference" CssClass="form-control maxWidth" Placeholder="Falls vorhanden." ToolTip="z.B. Bestellnummer des Auftraggebers." MaxLength="50"></asp:TextBox>
                            </div>
                        </div>
                        <h6>Mit * markierte Felder sind Pflichtfelder.</h6>
                    </asp:PlaceHolder>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div style="clear: both"></div>
        <div style="text-align: right;">
            <asp:Button runat="server" ID="BtnSaveChanges" OnClick="BtnSaveChanges_OnClick" CssClass="btn btn-default" Text="Speichern & Schliessen"></asp:Button>
            <asp:Button runat="server" ID="BtnSaveBetween" OnClick="BtnSaveBetween_OnClick" CssClass="btn btn-default" Text="Zwischenspeichern" />
            <asp:Button runat="server" ID="BtnCancel" OnClick="BtnCancel_OnClick" CssClass="btn btn-default" Text="Abbrechen"></asp:Button>
        </div>
    </div>
    <div class="well newProjectSettings">
        <asp:Label runat="server" ID="lblProjectAttachements" Font-Size="24px" Height="50px" Text="Projekt Artefakte"></asp:Label>
        <button runat="server" class="btn" OnServerClick="downloadFiles_OnClick"><img src="Content/zip.png" style="height: 30px;" alt="download"/>  Download ZIP </button>
        <div class="well contentDesign form-horizontal" style="background-color: #ffffff">
            <asp:UpdatePanel runat="server" ID="updateProjectAttachements" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Label runat="server" Text="Projekt Artefakte (Dokumentation, Präsentation, Code):" CssClass="control-label col-sm-3"></asp:Label>
                    <div class="form-group col-sm-9">
                        <asp:GridView runat="server" Width="100%" ID="gridProjectAttachs" EmptyDataText="             Noch keine Dokumente hochgeladen." ItemType="ProStudCreator.ProjectSingleAttachment" EnableModelValidation="False" ValidateRequestMode="Disabled" OnSelectedIndexChanged="gridProjectAttachs_OnSelectedIndexChanged" CellPadding="4" EnableViewState="False" GridLines="None" AutoGenerateColumns="False" ForeColor="#333333" AllowSorting="False" OnRowCommand="gridProjectAttachs_OnRowCommand" OnRowDataBound="gridProjectAttachs_OnRowDataBound" DataKeyNames="Guid">
                            <Columns>
                                <asp:ImageField ItemStyle-Width="20px" DataImageUrlField="FileType" ControlStyle-Height="30px" />
                                <asp:BoundField DataField="Name" HeaderText="Dokumentname" ItemStyle-Wrap="False" />
                                <asp:BoundField DataField="Size" HeaderText="Dateigrösse" />
                                <asp:TemplateField ItemStyle-Wrap="False">
                                    <ItemTemplate>
                                        <asp:LinkButton runat="server" ID="deleteProjectAttachButton" ToolTip="Datei löschen" CommandName="deleteProjectAttach" OnClientClick="return confirm('Wollen Sie diese Datei wirklich löschen?');" CommandArgument="<%# Item.Guid %>" CssClass="btn btn-default btnHeight glyphicon glyphicon-remove"></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <HeaderStyle BackColor="#a5c7ff" Font-Bold="True" ForeColor="Black" />
                            <RowStyle BackColor="#FFFFFF" ForeColor="0000000"></RowStyle>
                        </asp:GridView>
                    </div>
                    <div style="clear: both"></div>
                    <div runat="server" id="divFileUpload">
                        <hr />
                        <asp:Label runat="server" Text="Upload Projekt Artefakte:" CssClass="control-label col-sm-3"></asp:Label>
                        <div class="form-group">
                            <ajax:AjaxFileUpload runat="server" MaxFileSize="-1" OnUploadComplete="OnUploadComplete" ClearFileListAfterUpload="True" AutoStartUpload="True" ID="fileUpProjectAttach" AllowedFileTypes="7z,aac,avi,bz2,csv,doc,docx,gif,gz,htm,html,jpeg,jpg,md,mp3,mp4,ods,odt,ogg,pdf,png,ppt,pptx,svg,tar,tgz,txt,xls,xlsx,xml,zip" OnClientUploadCompleteAll="doPostBack" MaximumNumberOfFiles="-1"  />
                            <small class="col-sm-offset-4">Dokumente mit gleichem Namen werden überschriben.</small>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <script type="text/javascript">
        function doPostBack() {
            var updatePanel1 = '<%=updateProjectAttachements.ClientID%>';
            __doPostBack(updatePanel1, '');
        }
    </script>
</asp:Content>

