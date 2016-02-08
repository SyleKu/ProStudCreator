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
                <asp:ListItem Value="Submitted">Eigene, Eingereicht</asp:ListItem>
                <%--<asp:ListItem Value="Published">Eigene, Veröffentlicht</asp:ListItem>--%>
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
        </div>
    </div>
    <div class="well usernSettings non-selectable">
        <h3>Termine</h3>
        <p>
            <i>Hinweis: Plane bei der Einreichung genügend Zeit für Iterationen ein!</i>
        </p>
        <table class="termine">
            <tr>
                <th></th>
                <th>HS15<br />14.09.2015 - 16.01.2016</th>
                <th>FS16<br />22.02.2016 - 18.06.2016</th>
                <th>HS16<br />19.09.2016 - 21.01.2017</th>
            </tr>
            <tr>
                <td>Projekteinreichung&nbsp;</td>
                <td>01.06.2015</td>
                <td>29.11.2015</td>
                <td>30.05.2016</td>
            </tr>
            <!--<tr>
                <td>Projektvorstellung</td>
                <td>08.06.2015</td>
                <td>08.12.2015</td>
                <td>Anfang 06.2016</td>
            </tr>-->
            <tr>
                <td>Projektzuteilung</td>
                <td>07.07.2015</td>
                <td>14.01.2016</td>
                <td>Anfang 07.2016</td>
            </tr>
            <tr>
                <td>Abgabe IP5 Voll-/Teilzeit</td>
                <td>15.01.2016</td>
                <td>24.06.2016</td>
                <td>Mitte 01.2016</td>
            </tr>
            <tr>
                <td>Abgabe IP5 Berufsbegleitend</td>
                <td>18.06.2016</td>
                <td>09.12.2016</td>
                <td>?</td>
            </tr>
            <tr>
                <td>Abgabe IP6 (normal)</td>
                <td>24.03.2016</td>
                <td>19.08.2016</td>
                <td>?</td>
            </tr>
            <tr>
                <td>Abgabe IP6 (Variante b)</td>
                <td>20.08.2016</td>
                <td>?</td>
                <td>?</td>
            </tr>
            <tr>
                <td>Verteidigung IP6</td>
                <td>11.04.2016<br />15.04.2016</td>
                <td>05.09.2016 - 16.09.2016</td>
                <td>?</td>
            </tr>
            <tr>
                <td>Ausstellung Bachelorthesen</td>
                <td></td>
                <td>19.08.2016</td>
                <td>?</td>
            </tr>
        </table>
        <h3>Ablauf eines Projektes</h3>
        <ol>
            <li>Den <a href="Content/Bewertungsbogen_P5_P9.xlsx" class="xls">Bewertungsbogen</a> herunterlagen</li>
            <li>Die Gewichtung passend festlegen (im Bereich 0 bis 2). Die Gewichtung muss bis vor der
                Projektvereinbarung mit den Studierenden besprochen und fixiert sein.
            </li>
            <li>Die Studierenden verfassen in den ersten Wochen eine Projektvereinbarung. Eine Vorlage dafür
                existiert nicht. Die Projektvereinbarung umfasst etwa 1-2 Seiten und enthält folgende Punkte:
                <ul>
                    <li>Wiedergabe der Ausgangslage und Aufgabenstellung in eigenen Worten</li>
                    <li>Vorgehen mit grobem Zeitplan und Meilensteinen</li>
                    <li>Projektrisiken, falls nötig</li>
                </ul>
            </li>
            <li>Die Studierenden arbeiten am Projekt. Wenn Du möchtest, gibst Du folgende Hilfsmittel ab:
                <ul>
                    <li><a href="Content/Vorlage_Bericht_2016.dot" class="doc">Offizielle Vorlage für Berichte</a></li>
                    <li><a href="Content/Leitfaden_Berichte_4.0.pdf" class="pdf">Offizieller Leitfaden für Berichte</a></li>
                    <li><a href="Content/P5_P6_Guide_20151117.pdf" class="pdf">Guide von Marco Soldati für IP5 und IP6</a></li>
                </ul>
            </li>
            <li>Die Studierenden stellen Ihr IP6 an der Ausstellung mit einem <a href="Content/Poster_TemplateI_150923.pptx" class="ppt">Poster</a> vor. Poster
                können vom Empfang in A0-Grösse gedruckt werden.
            </li>
            <li>Fülle nach der Projektabgabe den Bewertungsbogen aus.</li>
            <li>Die Studierenden erstellen das Websummary. Markus Oehninger schickt dazu ein Mail.</li>
            <li>Schlusspräsentation mit 20-40 Minuten Präsentation, und 20-40 Minuten Fragen von Betreuern, Experten und interessierten Gästen. Bei IP5 ist die Schlusspräsentation optional.</li>
            <li>Notenbesprechung mit den Experten, ggf. Anpassung der Bewertung.</li>
            <li>Die Note den Studierenden kommunizieren. Der Bogen wird den Studierenden <b>NICHT</b> abgegeben. Pro Student kann eine individuelle
                Note festgelegt werden.
            </li>
        </ol>
        <p>
            <i>Hinweis: Falls sich eine ungenügende Note abzeichnet, sind für den Rekursfall einige Regeln zu beachten. Melde Dich bei
                uns. Je früher, desto besser.
            </i>
        </p>
        <h3>FAQ</h3>
        <h4>Welche Projekte eignen sich als IP5? Welche als IP6 (Bachelorthesis)?</h4>
        <p>
            Idealerweise stammen die Themen aus Industrie- oder Forschungsprojekten der Institute. Für externe Projekte gilt diese Einschränkung nicht.
            Projekte sollten einen Programmier-Teil (>50%) und einen Forschungs-Teil (>0%) enthalten.
            </p>
        <p>Als IP5 kommen viele Arbeiten in Frage: z.B. Webapplikationen ab einer gewissen Komplexität, Visualisierungen, Bildverarbeitung, …. Die Studierenden sollen ein klares Ziel haben und der Arbeitsumfang soll angemessen sein. Üblicherweise geht es darum, bestehende Technologien/Algorithmen/Methoden anzuwenden und sich auf das IP6 vorzubereiten. Im IP6 werden die Weichen mehr Richtung Wissenschaft gestellt. Laut Richtlinien sollen die Studierenden in der Bachelorthesis eine „bestimmte Aufgabe wissenschaftlich reflektiert, theoretisch und praktisch sowie selbständig lösen“.
            </p>
        <p>Geradlinige Arbeiten sind <b>NICHT</b> geeignet. Es darf nicht von vornherein klar sein, wie man ein Aufgabe löst. Das Ziel der Arbeit soll hingegen klar definiert sein. Achte darauf, dass die Aufgabenstellung eine gewisse intellektuelle Flughöhe erreicht, aber gegen oben nicht begrenzt ist. Ideale Arbeiten enthalten einen offenen Teil, wo sich die Studierenden austoben können.
            </p>
        <h4>Was gehört in einen Projektbeschrieb?</h4>
        <p>
            Die Studierenden entscheiden sich anhand des Beschriebs für bzw. gegen ein Projekt. Das Review wird ebenfalls anhand des
            Beschriebes gemacht. Daraus ergeben sich folgende Anforderungen:
        </p>
        <ul>
            <li>Es muss klar sein, wo die Herausforderung der Arbeit liegt. Was muss "erforscht" werden?</li>
            <li>Wo liegen die Schwerpunkte der Arbeit?</li>
            <li>Welche Teile bestehen bereits, welche fertigen Libraries dürfen/müssen eingesetzt werden und was muss selbst gemacht
                werden?
            </li>
            <li>Der benötigte Arbeitsaufwand muss abschätzbar sein. Auch bei offenen Forschungsthemen muss ersichtlich sein, was erwartet wird.
                Die Formulierung muss detailliert genug sein, dass man dafür eine Offerte erstellen könnte.
            </li>
        </ul>
        <h4>Wer ist meine Ansprechperson bei Fragen?</h4>
        <ul>
            <li><a href="mailto:dominik.gruntz@fhnw.ch">Dominik Gruntz</a> für Projekte am IMVS</li>
            <li><a href="mailto:simon.felix@fhnw.ch">Simon Felix</a> für Projekte am i4Ds</li>
        </ul>
        <h4>Wie können externe Auftraggeber Projekte einreichen?</h4>
        <p>
            Externe Auftraggeber schicken <a href="Content/Externe_Projekteingabe.pdf" class="pdf">dieses Formular</a> an Markus Oehninger. Informiert Interessenten doch bitte, dass einfache Programmierarbeiten nicht an die Studierenden ausgelagert werden können. Die könnten aber als IP1-4 interessant sein! Für externe Projekte gelten dieselben Anforderungen wie für interne Projekte.
        </p>
        <h4>Können Studierende eigene Projekte einreichen?</h4>
        <p>
            Studierende können ihren Arbeitgeber ermutigen, ein externes Projekt einzureichen. Sie können aber nicht selbst Auftraggeber sein.
        </p>
        <h4>Wer darf Projekte betreuen?</h4>
        <p>
            Als Betreuer kommen alle Dozierenden am i4Ds (z.B. Manfred, André, Doris, Sarah, Ruedi, Stefan, Simon, …) in Frage. Es besteht aber häufig auch die Möglichkeit in einem Zweier-Team (z.B. Dozierender + WiMi) zu arbeiten. Wer also kein Dozierender ist, aber trotzdem ein Projekt betreuen möchte, kann das im Normalfall nach Absprache machen. Die Betreuung kann man aufteilen, die Gesamtverantwortung bleibt beim Dozierenden.
        </p>
        <h4>Wie gross ist der Aufwand für die Betreuung? Was muss ich tun?</h4>
        <p>
            Die Studierenden sollen sich selbst organisieren und Verantwortung übernehmen. Sie sollen selbst mögliche Lösungsansätze erarbeiten, vergleichen, umsetzen und Zeit managen. Du sollt nicht am Projekt mitarbeiten, Protokolle schreiben oder ähnliches! Du sollst die Studierenden nur in Fachfragen unterstützen und Risiken minimieren. Speziell zu Beginn der Arbeit eignen sich dazu kurze, wöchentliche Sitzungen sehr gut. Die Studierenden müssen aber selbstständig arbeiten.
        </p>
        <h4>Kann ich Einzelprojekte einreichen?</h4>
        <p>
            Einzelprojekte sind aus Skalierungsgründen nur in Ausnahmefällen möglich. Der Betreuungsaufwand für viele Einzelprojekte ist zu gross.
        </p>
        <h4>Wie läuft die Zuteilung der Projekte?</h4>
        <p>
            Einige Projekte stellen Dominik Gruntz und Simon Felix den Studierenden vor. Die restlichen Projektbeschriebe werden als PDF abgegeben. Studierende bewerben sich dann auf die Projekte und werden automatisiert zugeteilt. Du kriegst vermutlich nicht Deine Wunschkandidaten.
            Das Reservieren von Arbeiten ist nur in Ausnahmefällen möglich (Folgeprojekte oder Studierende, die an unseren Instituten arbeiten).
        </p>
        <p>
            Sobald die Zuteilung der Projekte erfolgt ist, erhältst Du eine Rückmeldung, welche Projekte von Dir (nicht) gewählt wurden.
        </p>
        <h4>Wie kann Projekte verlängern?</h4>
        <p>
            IP5 können als IP6 verlängert werden. Wie bei allen anderen IP6 musst Du dazu rechtzeitig einen Projektbeschrieb einreichen. Für 
            Folgeprojekte gelten die allgemeinen Anforderungen ebenfalls.
        </p>
        <h4>Muss ich nicht gewählte Projekte erneut einreichen?</h4>
        <p>
            Nicht gewählte Projekte werden auf der Plattform wieder aufgelistet. Falls ein Projekt noch aktuell ist, musst Du es erneut einreichen.
        </p>
        <h4>Ich habe die Deadline verpasst. Kann ich mein Projekt noch nachreichen?</h4>
        <p>
            Weil dann nicht mehr genügend Zeit für Reviews bleibt und die Organisation ein fragiles Gebilde aus etlichen Systemen und Personen
            ist, geht das nicht.
        </p>
        <h4>Wer organisiert Experten für meine Projekte?</h4>
        <p>
            Wir organisieren die Experten für Dich, etwa zur Halbzeit des Projekts.
        </p>
        <h4>Wo sollen die Studierenden Ihren Code ablegen? Woher kriegen meine Studierenden eine VM?</h4>
        <p>
            Ihr könnt für eure Studierenden über <a href="http://web.fhnw.ch/plattformen/cs-support/bestellformulare">http://web.fhnw.ch/plattformen/cs-support/bestellformulare</a>
            diverse Ressourcen bestellen. Es stehen Git- und SVN-Repositories zur Verfügung. Selbst können die Studierenden keine Ressourcen beantragen.
        </p>
    </div>
</asp:Content>
