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
    </div>
</asp:Content>
