<%@ Page Title="IP5/IP6 Projekte" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="True" CodeBehind="Ablauf.aspx.cs" Inherits="ProStudCreator.Ablauf" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="well usernSettings">
        <h3>Durchführung eines Projektes</h3>
        <ol>
            <li>Den <a href="Content/Bewertungsbogen_P5_P9.xlsx" class="xls">Bewertungsbogen</a> herunterladen</li>
            <li>Die Gewichtung der Einzelpunkte passend festlegen. Die Gewichtung der
                Zwischennoten (z.B. "FACHLICHES") nicht verändern. Die Gewichtung muss bis vor der
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
            <li>Die Studierenden arbeiten am Projekt. Bitte kommuniziere, wie Du informiert werden möchtest (z.B. jede Woche ein kurzes Treffen).
                Du gibst den Studierenden folgende Hilfsmittel ab:
                <ul>
                    <li>Ein Hinweis, dass auf diesem <a href="\\fsemu18.edu.ds.fhnw.ch\e_18_data11$\E1811_Info\E1811_Info_I\Projektschiene">Netzwerkshare (\\fsemu18.edu.ds.fhnw.ch\e_18_data11$\E1811_Info\E1811_Info_I\Projektschiene)</a> Unterlagen zu finden sind.</li>
                    <%--<li>Anleitung zum obligatorischen Websummary für <a href="Content/Webauftritt_IP5_2016HS.pdf" class="pdf">IP5</a> oder <a href="Content/Webauftritt_IP6_2016HS.pdf" class="pdf">IP6</a></li>--%>
                    <li>Optional: <a href="Content/P5_P6_Guide_20160906.pdf" class="pdf">Guide von Marco Soldati für IP5 und IP6</a></li>
                    <li>Optional: <a href="Content/Leitfaden_Berichte_4.01.pdf" class="pdf">Offizieller Leitfaden für Berichte</a></li>
                    <li>Optional: <a href="Content/Vorlage_Bericht_2016.dot" class="doc">Offizielle Vorlage für Berichte</a></li>
                </ul>
            </li>
            <li>Nur für FS-Projekte: Die Studierenden stellen Ihr IP6 an der Ausstellung mit einem <a href="Content/Poster_TemplateI_150923.pptx" class="ppt">Poster</a> vor. Poster
                können vom Empfang in A0-Grösse gedruckt werden.</li>
            <li><strong>NEU: KEIN WEBSUMMARY MEHR!</strong> <s>Die Studierenden <a href="Content/Webauftritt_2015HS.pdf" class="pdf">erstellen das Websummary</a>.</s></li>
            <li>Fülle nach der Projektabgabe den Bewertungsbogen aus.</li>
            <li>Schlusspräsentation mit 20-40 Minuten Präsentation, und 20-40 Minuten Fragen von Betreuern, Experten und interessierten Gästen. Beim IP5 ist die Schlusspräsentation nicht zwingend, wird aber empfohlen.</li>
            <li>Notenbesprechung mit den Experten, ggf. Anpassung der Bewertung. Pro Student/-in soll eine
                <i>individuelle</i> Note festgelegt werden.</li>
            <li>Die Note den Studierenden und der Administration kommunizieren, per Mail. Im Idealfall machst Du noch eine Schlusssitzung.
                Der Bewertungsbogen wird den Studierenden <b>NICHT</b> abgegeben. Noten >5.8 müssen vorgängig an die SGL gemeldet werden.</li>
        </ol>
        <p>
            <i>Hinweis: Falls sich eine ungenügende Note abzeichnet, sind für den Rekursfall einige Regeln zu beachten. Melde Dich bei
                uns. Je früher, desto besser.
            </i>
        </p>
    </div>

    <div class="well usernSettings non-selectable">
        <h3>Einreichen eines externen Projekts</h3>
        <img src="Content/Prozess.PNG" />
        <ol>
            <li>Ein externer Auftraggeber (nie ein Student selbst) schickt <a class="pdf" href="Content/Externe_Projekteingabe.pdf">dieses Formular</a> an Markus Oehninger.</li>
            <li>Markus verteilt das Projekt ans i4Ds und IMVS.</li>
            <li>Dominik Gruntz (IMVS) oder Simon Felix (i4Ds) verteilen das Projekt intern an passende Betreuer.</li>
            <li>Die Betreuer erarbeiten einen Projektbeschrieb, zusammen mit dem externen Auftraggeber.</li>
            <li>Der fertige Projektbeschrieb wird über diese Plattform eingereicht und dann geprüft.</li>
        </ol>
        <h3>Einreichen eines eigenen Projekts</h3>
        <ol>
            <li>Du erarbeitest einen Projektbeschrieb.</li>
            <li>Du reichst den Projektbeschrieb auf dieser Plattform ein.</li>
            <li>Der Projektbeschrieb wird geprüft.</li>
        </ol>
        <p>
            Für alle Projekte gelten dieselben Anforderungen (siehe <a href="FAQ.aspx">FAQ</a>).
        </p>
    </div>
</asp:Content>
