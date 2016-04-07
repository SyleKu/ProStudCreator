<%@ Page Title="Projekt bearbeiten" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AddNewProject.aspx.cs" Inherits="ProStudCreator.AddNewProject" %>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        var hasUnsavedChanges = false;

        /**
        * Asks user to confirm an action, which then saves the project.
        * Supresses the "Unsaved changes" dialog.
        **/
        function confirmSaving(message) {
            var ok = confirm(message);
            if (ok) {
                hasUnsavedChanges = false;
            }
            return ok;
        }

        function isContentStud(currentObject) {
            var txtBoxValue = currentObject.value;
            var term = "@students.fhnw.ch";
            var index = txtBoxValue.indexOf(term);
            if (index == -1) {
                alert("Geben Sie eine E-Mail Adresse an, welche mit @students.fhnw.ch endet");
                currentObject.style.borderColor = 'red';
            }
            else {
                currentObject.style.borderColor = 'green';
            }

        }

        // Attach event handlers once page is loaded
        $(document).ready(function () {
            $(":input").change(function () {
                hasUnsavedChanges = true;
            });
            $("#projectTypes :input").click(function () {
                hasUnsavedChanges = true;
            });

            // Textarea max. length checks
            $("textarea[maxlength]").after(function () {
                return '<span class="text-muted text-right"><span id="' + $(this).attr('id') + '_len">' + $(this).val().length + "</span> / " + $(this).attr('maxlength') + "</span>";
            });
            $("textarea[maxlength]").on("change paste keyup", function () {

                var currLen = $(this).val().length;
                var maxLen = $(this).attr('maxlength');

                var lengthDisp = $("#" + $(this).attr('id') + "_len");
                if (currLen >= maxLen) {
                    lengthDisp.parent().addClass("alert-warning");
                } else {
                    lengthDisp.parent().removeClass("alert-warning");
                }

                lengthDisp.text(currLen);

                // textarea maxlength is supported by all modern browsers. For archaic browsers, uncomment:
                //if (currLen > maxLen) {
                //    $(this).val( $(this).val().substring(0, maxLen) );
                //}                
            });

        });
        $(document).ready(function () {
            var count = 0; // 
            var focusCount = 0;

            $('input[type=textbox]').focus(function () {

                focusCount = $(this).val().length;
                count = 0;
                // get total text length
                $('input[type=textbox]').each(function () {
                    count += $(this).val().length;
                });
                // remove text length of focused input from total count
                count -= focusCount;
            });

            $('input[type=textbox]').keyup(function () {
                focusCount = $(this).val().length;
                $('#charNum').text(2600 - (count + focusCount));
            });
        });

        $(window).on('beforeunload', function () {
            if (hasUnsavedChanges) {
                return "Änderungen wurden noch nicht gespeichert. Seite wirklich verlassen?";
            }
        });
    </script>
    <script runat="server" type="text/c#"> // script for updatepanel
        protected void Pdfupdatetimer_Tick(object sender, EventArgs e)
        {
            Pdfupdatelabel.Text = "Panel refreshed at: " +
    DateTime.Now.ToLongTimeString();
        }
    </script>
    <div id="refusedReason" class="well newProjectSettings non-selectable" runat="server" visible="false">
        <asp:Label runat="server" ID="refusedReasonTitle" Text="Ablehnungsgrund:" Font-Size="24px" Height="50px"></asp:Label>
        <div class="form-group">
            <asp:Label runat="server" CssClass="control-label" Text="Weshalb wird dieses Projekt abgelehnt? Der Text wird dem Projektersteller via E-Mail zugestellt."></asp:Label>
            <asp:TextBox runat="server" ID="refusedReasonText" CssClass="form-control contentRefuseDesign" TextMode="MultiLine"></asp:TextBox>
        </div>
        <asp:Button runat="server" ID="refuseDefinitiveNewProject" CssClass="btn btn-default refuseProject" Width="125px" Text="Ablehnen" OnClientClick="return confirmSaving('Dieses Projekt wirklich ablehnen?');" OnClick="refuseDefinitiveNewProject_Click"></asp:Button>
        <asp:Button runat="server" ID="cancelRefusion" CssClass="btn btn-default" Text="Abbrechen" OnClick="cancelRefusion_Click" CausesValidation="false"></asp:Button>
    </div>
    <div class="well newProjectSettings non-selectable">
        <div id="fixedupdatepanel" style="display:none">
            <asp:UpdatePanel ID="PdfupdatePanel" runat="server">
                <ContentTemplate>
                    <asp:Label ID="Pdfupdatelabel" runat="server" Text="Panel nor Refreshed yet"></asp:Label>
                    <asp:Timer ID="Pdfupdatetimer" runat="server" Interval="3000" OnTick="Pdfupdatetimer_Tick">
                    </asp:Timer>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <asp:Label runat="server" ID="SiteTitle" Font-Size="24px" Height="50px"></asp:Label>
        <asp:PlaceHolder ID="AdminView" runat="server" Visible="false">
            <asp:Label runat="server" ID="CreatorID" CssClass="pull-right" Font-Size="24px" Height="50px"></asp:Label>
        </asp:PlaceHolder>
        <div class="well contentDesign form-horizontal" style="background-color: #ffffff">
            <div class="form-group" style="margin-top: 15px">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Projektname:"></asp:Label>
                <div class="col-sm-9">
                    <asp:TextBox runat="server" ID="ProjectName" CssClass="form-control" MaxLength="80" placeholder="Projektname"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="ProjectNameValidator" ForeColor="Red" Display="Dynamic" ControlToValidate="ProjectName" runat="server" ErrorMessage="Bitte geben Sie einen Projektnamen an."></asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Kunde (optional):"></asp:Label>
                <div class="col-sm-3">
                    <asp:TextBox runat="server" ID="Employer" CssClass="form-control" placeholder="Unternehmen"></asp:TextBox>
                </div>
                <div class="col-sm-3">
                    <asp:TextBox runat="server" ID="EmployerPerson" CssClass="form-control" placeholder="Vorname Nachname der Kontaktperson"></asp:TextBox>
                    <small>Für interne, administrtative Zwecke (wird nicht Veröffentlicht)</small>
                </div>
                <div class="col-sm-3">
                    <asp:TextBox runat="server" ID="EmployerMail" CssClass="form-control" placeholder="E-Mail des Kunden" TextMode="Email"></asp:TextBox>
                    <small>Für interne, administrtative Zwecke (wird nicht Veröffentlicht)</small>
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Betreuung:"></asp:Label>
                <div class="col-sm-3">
                    <asp:TextBox runat="server" ID="NameBetreuer1" CssClass="form-control" placeholder="Vorname Nachname des offiziellen Betreuers"></asp:TextBox>
                </div>
                <div class="col-sm-3">
                    <asp:TextBox runat="server" ID="EMail1" CssClass="form-control" placeholder="E-Mail des offiziellen Betreuers" TextMode="Email"></asp:TextBox>
                </div>
                <div class="col-sm-3"></div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3"></asp:Label>
                <div class="col-sm-3">
                    <asp:TextBox runat="server" ID="NameBetreuer2" CssClass="form-control" placeholder="Vorname Nachname des Zweitbetreuers"></asp:TextBox>
                </div>
                <div class="col-sm-3">
                    <asp:TextBox runat="server" ID="EMail2" CssClass="form-control" placeholder="E-Mail des Zweitbetreuers" TextMode="Email"></asp:TextBox>
                </div>
                <div class="col-sm-3"></div>
            </div>
            <hr />
            <asp:UpdatePanel runat="server" class="form-group">
                <ContentTemplate>
                    <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Themengebiet:"></asp:Label>
                    <div id="projectTypes" class="col-sm-9">
                        <asp:ImageButton CssClass="img-rounded" ID="DesignUX" Height="60px" runat="server" ToolTip="Design, Usability, User Interfaces, ..." ImageUrl="pictures/projectTypDesignUXUnchecked.png" OnClick="DesignUX_Click" CausesValidation="false" />
                        <asp:ImageButton CssClass="img-rounded" ID="HW" Height="60px" runat="server" ToolTip="Hardwarenah, Embedded, Low-level, ..." ImageUrl="pictures/projectTypHWUnchecked.png" OnClick="HW_Click" CausesValidation="false" />
                        <asp:ImageButton CssClass="img-rounded" ID="CGIP" Height="60px" runat="server" ToolTip="Computergrafik, 3D, Bildverarbeitung, ..." ImageUrl="pictures/projectTypCGIPUnchecked.png" OnClick="CGIP_Click" CausesValidation="false" />
                        <asp:ImageButton CssClass="img-rounded" ID="MathAlg" Height="60px" runat="server" ToolTip="Mathematik, Algorithmen, Machine Learning, Data Mining, ..." ImageUrl="pictures/projectTypMathAlgUnchecked.png" OnClick="MathAlg_Click" CausesValidation="false" />
                        <asp:ImageButton CssClass="img-rounded" ID="AppWeb" Height="60px" runat="server" ToolTip="Mobile Apps, Webentwicklung, ..." ImageUrl="pictures/projectTypAppWebUnchecked.png" OnClick="AppWeb_Click" CausesValidation="false" />
                        <asp:ImageButton CssClass="img-rounded" ID="DBBigData" Height="60px" runat="server" ToolTip="Datenbanken, Big Data, Data Spaces, ..." ImageUrl="pictures/projectTypDBBigDataUnchecked.png" OnClick="DBBigData_Click" CausesValidation="false" />
                    </div>
                    <asp:Timer runat="server" Interval="60000" />
                </ContentTemplate>
            </asp:UpdatePanel>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Sprachen:"></asp:Label>
                <div class="col-sm-6">
                    <asp:CheckBox ID="LanguageGerman" CssClass="checkbox" Text="Deutsch" Checked="true" runat="server" CausesValidation="true" />
                    <asp:CheckBox ID="LanguageEnglish" CssClass="checkbox" Text="Englisch" runat="server" CausesValidation="true" />
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Dauer:"></asp:Label>
                <div class="col-sm-6">
                    <asp:CheckBox ID="DurationOneSemester" CssClass="checkbox" Text="Projekt muss in 1 Semester durchgeführt werden." runat="server" />
                    <p class="text-muted">(Dies schliesst berufsbegleitende Studierenden aus)</p>
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Priorität 1:"></asp:Label>
                <div class="col-sm-3">
                    <asp:DropDownList runat="server" ID="POneType" DataValueField="Id" DataTextField="Description" CssClass="form-control" Width="200px" />
                </div>
                <div class="col-sm-3">
                    <asp:DropDownList runat="server" ID="POneTeamSize" DataValueField="Id" DataTextField="Description" CssClass="form-control" Width="200px" OnSelectedIndexChanged="TeamSize_SelectedIndexChanged" AutoPostBack="true" />
                </div>
                <div class="col-sm-3"></div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Priorität 2:"></asp:Label>
                <div class="col-sm-3">
                    <asp:DropDownList runat="server" ID="PTwoType" DataValueField="Id" DataTextField="Description" CssClass="form-control" Width="200px" />
                </div>
                <div class="col-sm-3">
                    <asp:DropDownList runat="server" ID="PTwoTeamSize" DataValueField="Id" DataTextField="Description" CssClass="form-control" Width="200px" OnSelectedIndexChanged="TeamSize_SelectedIndexChanged" AutoPostBack="true" />
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Ausgangslage:"></asp:Label>
                <div class="col-sm-9">
                    <asp:TextBox runat="server" ID="InitialPositionContent" CssClass="form-control" TextMode="MultiLine" MaxLength="1000"></asp:TextBox>
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" ID="AddPictureLabel" CssClass="control-label col-sm-3" Text="Bild hinzufügen:"></asp:Label>
                <div class="col-sm-3">
                    <asp:FileUpload runat="server" ID="AddPicture" accept=".jpeg,.jpg,.png" CssClass="control-label" /><small>(max. 1MB)</small>
                </div>
                <div class="col-sm-1">
                    <asp:LinkButton runat="server" ID="DeleteImageButton" OnClick="deleteImage_Click" OnClientClick="return confirm('Dieses Bild wirklich entfernen?');" CssClass="btn btn-default btnHeight imageRemoveMargin glyphicon glyphicon-remove" Visible="false"></asp:LinkButton>
                </div>
                <div class="col-sm-2"></div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" ID="ImageLabel" CssClass="control-label col-sm-3" Text=""></asp:Label>
                <div class="col-sm-9">
                    <asp:Image runat="server" ID="Image1" CssClass="maxImageWidth img-rounded" Visible="false" EnableViewState="False" />
                </div>
            </div>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Ziel der Arbeit:"></asp:Label>
                <div class="col-sm-9">
                    <asp:TextBox runat="server" ID="ObjectivContent" CssClass="form-control" placeholder="Ziel der Arbeit" TextMode="MultiLine" MaxLength="1000"></asp:TextBox>
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Problemstellung:"></asp:Label>
                <div class="col-sm-9">
                    <asp:TextBox runat="server" ID="ProblemStatementContent" CssClass="form-control" placeholder="Problemstellung" TextMode="MultiLine" MaxLength="1000"></asp:TextBox>
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Technologien:<br/>Schwerpunkte:<br/>Referenzen:"></asp:Label>
                <div class="col-sm-9">
                    <asp:TextBox runat="server" ID="ReferencesContent" CssClass="form-control" placeholder="Technologien/Schwerpunkte/Referenzen" TextMode="MultiLine" MaxLength="1000"></asp:TextBox>
                </div>
            </div>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Bemerkungen (optional):"></asp:Label>
                <div class="col-sm-9">
                    <asp:TextBox runat="server" ID="RemarksContent" CssClass="form-control" placeholder="Bemerkungen" TextMode="MultiLine" MaxLength="1000"></asp:TextBox>
                </div>
            </div>
            <asp:UpdatePanel runat="server">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="POneTeamSize" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="PTwoTeamSize" EventName="SelectedIndexChanged" />
                </Triggers>
                <ContentTemplate>
                    <div class="form-group">
                        <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Reserviert für (optional):"></asp:Label>
                        <div class="col-sm-3">
                            <asp:TextBox runat="server" ID="Reservation1Name" CssClass="col-sm-9 form-control" placeholder="(Vorname Nachname)"></asp:TextBox>
                        </div>
                        <div class="col-sm-3">
                            <asp:TextBox runat="server" onchange="isContentStud(this)" ID="Reservation1Mail" CssClass="col-sm-9 form-control" placeholder="(E-Mail)" TextMode="Email"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label runat="server" CssClass="control-label col-sm-3"></asp:Label>
                        <div class="col-sm-3">
                            <asp:TextBox runat="server" ID="Reservation2Name" CssClass="col-sm-9 form-control contentReservation" placeholder="(Vorname Nachname)"></asp:TextBox>
                        </div>
                        <div class="col-sm-3">
                            <asp:TextBox runat="server" onchange="isContentStud(this)" ID="Reservation2Mail" CssClass="col-sm-9 form-control" placeholder="(E-Mail)" TextMode="Email"></asp:TextBox>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Institut:"></asp:Label>
                <div class="col-sm-3">
                    <asp:DropDownList runat="server" DataValueField="Id" DataTextField="DepartmentName" ID="Department" CssClass="form-control"></asp:DropDownList>
                </div>
            </div>

            <div class="form-group">
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="true" ShowSummary="true" />
            </div>
        </div>
        <div style="text-align: right;">
            <asp:Button runat="server" ID="publishProject" Visible="false" CssClass="btn btn-default publishProject" Width="113px" Text="Veröffentlichen" OnClick="publishProject_Click" OnClientClick="return confirmSaving('Projekt wirklich veröffentlichen?');"></asp:Button>
            <asp:Button runat="server" ID="refuseProject" Visible="false" Style="margin-right: 32px;" CssClass="btn btn-default refuseProject" Width="113px" Text="Ablehnen" OnClick="refuseProject_Click" OnClientClick="return confirmSaving('Projekt wirklich ablehnen?');"></asp:Button>
            <asp:Button runat="server" ID="rollbackProject" Visible="false" Style="margin-right: 32px;" CssClass="btn btn-default rollbackMarginRight redButton" Text="Zurückziehen" OnClick="rollbackProject_Click" OnClientClick="return confirmSaving('Projekt wirklich zurückziehen?');"></asp:Button>
            <asp:Button runat="server" ID="submitProject" Visible="false" Style="margin-right: 32px;" CssClass="btn btn-default greenButton" Text="Einreichen" OnClick="submitProject_Click" OnClientClick="return confirmSaving('Dieses Projekt einreichen?');"></asp:Button>
            <asp:Button runat="server" ID="saveCloseProject" OnClick="saveCloseProjectButton" CssClass="btn btn-default" Text="Speichern & Schliessen" OnClientClick="hasUnsavedChanges = false;"></asp:Button>
            <asp:Button runat="server" ID="saveProject" OnClick="saveProjectButton" CssClass="btn btn-default" Text="Zwischenspeichern" OnClientClick="hasUnsavedChanges = false;"></asp:Button>

            <asp:Button runat="server" ID="cancelProject" CssClass="btn btn-default" TabIndex="5" Text="Abbrechen" OnClick="cancelNewProject_Click" CausesValidation="false"></asp:Button>
        </div>
    </div>
</asp:Content>
