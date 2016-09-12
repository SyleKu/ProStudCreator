<%@ Page Title="IP5/IP6 Projekte" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="True" CodeBehind="Termine.aspx.cs" Inherits="ProStudCreator.Termine" %>
<%--
    
    Projektmodul	Dauer (KW)	Starttermin	        Abgabetermin	    Aufwand
    IP5vz/tz	    18 resp. 17 KW 38 resp. KW  8	KW 3 resp.KW 24     180 h
    IP6vz/tz, VarA	26 resp. 29 KW  8 resp. KW 38	KW 33 resp. KW 12	360 h
    IP6vz/tz, VarB	43 resp. 48	KW  8 resp. KW 38 	KW 51 resp. KW 33	360 h
				
    IP5bb           41          KW 8                KW 48	            180 h
    IP6bb, VarA     26          KW 8                KW 33	            360 h
    IP6bb, VarB     44          KW 8                KW 51 	            360 h

    TODO: konkrete termine automatisch aus KW berechnen
    
    --%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="well usernSettings">
        <h3>Termine</h3>
        <%--<p>
            <i>Hinweis: Plane bei der Einreichung genügend Zeit für Iterationen ein!</i>
        </p>--%>
        <table class="table termine">
            <tr><!-- http://www.fhnw.ch/technik/bachelor/informationen/termine/studientermine -->
                <th></th>
                <th>FS16<br />22.02.2016 - 18.06.2016</th>
                <th class="success">HS16<br />19.09.2016 - 21.01.2017</th>
                <th class="warning">FS17<br />20.02.2017 - 17.06.2017</th>
                <th>HS17<br />18.09.2017 - 20.01.2017</th>
            </tr>
            <tr>
                <td>Projekteinreichung&nbsp;</td>
                <td>29.11.2015</td>
                <td class="success">30.05.2016</td>
                <td class="warning">22.11.2016</td>
                <td>23.05.2017</td>
            </tr>
            <%--<tr>
                <td>Projektvorstellung</td>
                <td>Anfang Juli/Dez</td>
            </tr>--%>
            <tr>
                <td>Projektzuteilung</td>
                <td>14.01.2016</td>
                <td class="success">Anfang 07.2016</td>
                <td class="warning">Mitte 01.2017</td>
                <td>Anfang 07.2017</td>
            </tr>
            <tr><!-- \\fsemu18.edu.ds.fhnw.ch\e_18_data11$\E1811_Info\E1811_Info_I\Projektschiene -->
                <td>Abgabe IP5 Voll-/Teilzeit</td>
                <td>24.06.2016</td>
                <td class="success">20.01.2017</td>
                <td class="warning">Mitte 06.2017</td>
                <td>Mitte 01.2018</td>
            </tr>
            <tr>
                <td>Abgabe IP5 Berufsbegleitend</td>
                <td>09.12.2016</td>
                <td class="success">16.06.2017</td>
                <td class="warning">Mitte 12.2017</td>
                <td>Mitte 06.2018</td>
            </tr>
            <tr>
                <td>Abgabe IP6 (normal)</td>
                <td>19.08.2016</td>
                <td class="success">17.03.2017</td>
                <td class="warning">Mitte 08.2017</td>
                <td>Mitte 03.2018</td>
            </tr>
            <tr>
                <td>Abgabe IP6 (Variante 2 Sem.)</td>
                <td>?</td>
                <td class="success">18.08.2017</td>
                <td class="warning">?</td>
                <td>Mitte 08.2018</td>
            </tr>
            <tr>
                <td>Verteidigung IP6</td>
                <td>05.09.2016<br />16.09.2016</td>
                <td class="success">03.04.2017<br />13.04.2017</td>
                <td class="warning">Mitte 09.2017</td>
                <td>Mitte 04.2018</td>
            </tr>
            <tr><!-- http://www.fhnw.ch/technik/bachelor/informationen/termine/studientermine -->
                <td>Ausstellung Bachelorthesen</td>
                <td>19.08.2016</td>
                <td class="success">keine</td>
                <td class="warning">18.08.2017</td>
                <td>keine</td>
            </tr>
        </table>
    </div>
</asp:Content>
