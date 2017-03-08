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
        <div class="well" style="background-color: #ffffff">
            <asp:GridView ID="AllEvents" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" AutoGenerateColumns="True" AlternatingRowStyle-BackColor="#e7e7e7" Width="100%">
                <%--<AlternatingRowStyle BackColor="White" />--%>
            </asp:GridView>
            <hr />
            <p>Titeländerungen von Informatikprojekten sind bis 4 Wochen vor Abgabe möglich!</p>
        </div>
    </div>
</asp:Content>
