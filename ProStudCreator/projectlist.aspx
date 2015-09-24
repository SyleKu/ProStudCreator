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
                <asp:ListItem Value="AllPastProjects">Dieses Semester</asp:ListItem>
                <asp:ListItem Value="AllCurrentProjects">Nächstes Semester</asp:ListItem>
                <asp:ListItem Value="AllFutureProjects">Zukünftiges Semester</asp:ListItem>
                <asp:ListItem Value="InProgress" Selected="True">Eigene, nicht eingereicht</asp:ListItem>
                <asp:ListItem Value="Submitted">Eigene, Eingereicht</asp:ListItem>
                <%--<asp:ListItem Value="Published">Eigene, Veröffentlicht</asp:ListItem>--%>
            </asp:RadioButtonList>
        </div>
        <div class="well" style="background-color: #ffffff; margin-top: 10px;">
            <asp:GridView ID="AllProjects" ItemType="ProStudCreator.ProjectSingleElement" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" AutoGenerateColumns="False" OnRowCommand="ProjectRowClick" OnRowDataBound="AllProjects_RowDataBound">
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
        </div>
    </div>
    <div class="well usernSettings non-selectable">
        <h3>Allgemeine Informationen</h3>
        <p>Für das FS16 rechnen wir mit rund 130 Studierenden, die ein Projekt brauchen.</p>
        <ul>
            <li>
                ca. 30 Studierende benötigen ein IP5 
            </li>
            <li>
                ca. 100 Studierende benötigen ein IP6
            </li>
        </ul>
        <p>
            Wir benötigen daher pro Institut für eine gute Auswahl ca. 70 Projekte.<br />
        </p>
        <h3>Termine</h3>
        <p>
            Siehe auch <a href="https://web.fhnw.ch/plattformen/technik-i-ip/fs16">FS16 Semesterplan auf Plone</a>.
        </p>
        <table>
            <tr>
                <th></th>
                <th>HS15</th>
                <th>FS16</th>
            </tr>
            <tr>
                <td>Deadline Projekteinreichung durch Studierende</td>
                <td></td>
                <td>01.11.2015</td>
            </tr>
            <tr>
                <td>Deadline Projekteinreichung</td>
                <td>01.06.2015</td>
                <td>30.11.2015</td>
            </tr>
            <tr>
                <td>Projektvorstellung</td>
                <td>08.06.2015</td>
                <td>08.12.2015</td>
            </tr>
            <tr>
                <td>Projektzuteilung</td>
                <td>07.07.2015</td>
                <td>14.01.2016</td>
            </tr>
            <tr>
                <td>Semesterbeginn</td>
                <td>14.09.2015</td>
                <td>22.02.2016</td>
            </tr>
            <tr>
                <td>Abgabe IP5</td>
                <td>15.01.2016</td>
                <td></td>
            </tr>
            <tr>
                <td>Verteidigung IP6</td>
                <td>vor 10.04.2016</td>
                <td></td>
            </tr>
        </table>
        <h3>Projektbeispiele</h3>
        <ul>
            <li><a href="Content/examples/08.pdf" class="pdf">Erkennung von schweizerdeutschen Fachausdrücken im Beratungsumfeld</a></li>
            <li><a href="Content/examples/11.pdf" class="pdf">FaceSimilarity</a></li>
            <li><a href="Content/examples/56.pdf" class="pdf">Docker IPython Notebook Multi-User-Server</a></li>
        </ul>
        <h3>FAQ</h3>
        <h4>Wer ist die richtige Ansprechperson bei allen Projekt-bezogenen Fragen?</h4>
        <p>Im <a href="https://web.fhnw.ch/plattformen/technik-i-ip/hs15">Handbuch</a> findet Ihr viele Antworten.
            Alle anderen Fragen bitte jeweils an die Projektkoordinatoren an den Instituten schicken: Am IMVS koordiniert <a href="mailto:simon.felix@fhnw.ch">Dominik Gruntz</a> die Projekte, am i4Ds <a href="mailto:simon.felix@fhnw.ch">Simon Felix</a>.
            </p>

        <h4>Wie können Studierende Projekte einreichen/vorschlagen?</h4>
        <p>Hier gelten die folgenden Regeln:</p>
        <ol>
            <li>Nur Berufsbegleitende-Studierende mit gutem Ranking sollen Projekte aus ihrem Unternehmen einreichen. Bei anderen Studierenden geht das nur in Ausnahmefällen (Bitte SGL kontaktieren)</li>
            <li>Studierende sollen ein Motivationsschreiben und einen ersten Projektvorschlag ausarbeiten</li>
            <li>Die SGL überprüft die Eignung und erteilt die Bewilligung</li>
            <li>Die Studierenden schicken den Projektbeschrieb an <a href="mailto:dominik.gruntz@fhnw.ch">Dominik Gruntz</a> (IMVS) und/oder <a href="mailto:simon.felix@fhnw.ch">Simon Felix</a> (i4Ds).</li>
        </ol>
        <p>
            Damit genügend Zeit bleibt müssen diese Projekte früher als reguläre Projekte bei den Instituten eingereicht werden. Im <a href="https://web.fhnw.ch/plattformen/technik-i-ip/hs15">Handbuch</a> gibt es Details dazu.
        </p>


        <h4>Wer darf Projekte einreichen?</h4>
        <p>Jeder. Externe Auftraggeber (ohne KTI-Projekt) müssen die Arbeiten aber bezahlen (CHF 1500 + MWSt). Externe Auftraggeber besprechen die Projekte am besten mit euch direkt oder mit mir. Für Externe gibt es hier weitere Informationen: <a href="http://www.fhnw.ch/technik/dienstleistung/studierendenprojekte">http://www.fhnw.ch/technik/dienstleistung/studierendenprojekte</a>. U.u. macht es Sinn Industriepartner jetzt über diese Möglichkeit zu informieren. Studierende dürfen nur in Ausnahmefällen Projekte selbst einreichen, Details dazu im <a href="https://web.fhnw.ch/plattformen/technik-i-ip/hs15">Handbuch auf Plone</a>.</p>
        <h4>Wer betreut die Durchführung der Projekte?</h4>
        <p>
            Als Betreuer kommen alle Dozierenden an den Instituten (z.B. Manfred Vogel, Christoph Stamm, André Csillaghy, Dominik Gruntz, …) in Frage. Es besteht aber die Möglichkeit in einem 2er-Team (z.B. DozentIn + WiMi) zu arbeiten. Wer also nicht doziert, aber trotzdem ein Projekt betreuen möchte, kann das nach Absprache mit einem Dozierenden trotzdem tun: Die Betreuung kann man aufteilen, die Gesamtverantwortung bleibt beim Dozierenden.
        </p>
        <h4>Wie gross ist der Aufwand für die Betreuung? Was muss ich tun?</h4>
        <p>
            Die Studierenden sollen sich selbst organisieren und Verantwortung übernehmen. Sie sollen selbst mögliche Lösungsansätze erarbeiten, vergleichen, umsetzen und Zeit managen. Ihr sollt nicht am Projekt mitarbeiten, Protokolle schreiben oder ähnliches! Die Aufgabe der Betreuung besteht nur darin die Studierenden in Fachfragen zu unterstützen und Risiken zu minimieren. Speziell zu Beginn der Arbeit eignen sich dazu kurze, wöchentliche Sitzungen sehr gut. Generell sollen die Studierenden aber selbstständig (!) arbeiten.
        </p>
        <h4>Welche Projekte eignen sich als IP5? Welche als IP6 (=Bachelorarbeit)?</h4>
        <p>
            Eine Übersicht über Projekte aus den vergangenen Jahren findet ihr auf <a href="http://www.fhnw.ch/technik/bachelor/informatik/studium/studierendenprojekte">Plone</a> (IP5 = „5. Semester“, IP6 = „Bachelor Thesis“).
        </p>
        <p>
            Als IP5 kommen viele Arbeiten in Frage. Die Studierenden sollen ein klares Ziel haben und der Arbeitsumfang soll angemessen sein. (z.B. Webapplikationen ab einer gewissen Komplexität, Visualisierungen, etwas Bildverarbeitung, …). Üblicherweise geht es darum, bestehende Technologien/Algorithmen/Methoden anzuwenden. In einem IP6 werden die Weichen dann mehr in Richtung Wissenschaft gestellt. Laut den Richtlinien sollen die Studierenden in der Bachelorthesis „bestimmte Aufgabe wissenschaftlich reflektiert, theoretisch und praktisch sowie selbständig lösen“. Eine Bachelorarbeit soll sich also nicht mit Grundlagenforschung befassen, aber neue und wissenschaftlich anspruchsvolle Bereiche enthalten. <i>Geradlinige Arbeiten sind <b>nicht</b> geeignet.</i> Trotzdem muss das Ziel der Arbeit klar definiert sein. Achtet darauf, dass die Aufgabenstellung eine gewisse intellektuelle Flughöhe erreicht, aber gegen oben nicht begrenzt ist. Eine ideale Arbeit enthält einen kleinen, offenen Teil, worin sich die Studierenden austoben können.
        </p>
        <h4>Welche Themen können in einem IP5/IP6 behandelt werden?</h4>
        <p>
            Idealerweise stammen die Aufgabenstellungen aus Industrie- oder Forschungsprojekten der Institute. Meldet euch bei Unsicherheiten.
        </p>
        <h4>Welchen Arbeitsumfang hat ein IP5/IP6?</h4>
        <ul>
            <li>IP5: 1-2 Studierende à 180h</li>
            <li>IP6: 1-2 Studierende à 360h</li>
        </ul>
        <p>
            <i>Hinweis: Einzelprojekte sind aus Skalierungsgründen nur in Ausnahmefällen möglich.</i>
        </p>
        <h4>Wie läuft die Zuteilung der Projekte?</h4>
        <p>
            Eine Auswahl der Projekte werden den Studierenden von Dominik Gruntz und Simon Felix vorgestellt, die restlichen Projektbeschriebe werden als PDF verteilt. Die Studierenden bewerben sich auf die Projekte und werden danach automatisiert zugeteilt. Ihr kriegt also vermutlich nicht eure Wunschkandidaten. Das Reservieren von Arbeiten ist nur in Ausnahmefällen und über die SGL möglich.
        </p>
        <h4>Kann/soll/muss ich Projekte, die letztes Semester nicht gewählt wurden, erneut einreichen?</h4>
        <p>
            Alle Projekte, die letztes Jahr nicht gewählt wurden, werden wir durchgehen und mit euch das weitere Vorgehen klären. Ihr braucht nichts weiter zu unternehmen.
        </p>
    </div>
</asp:Content>
