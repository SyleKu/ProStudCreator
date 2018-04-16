<%@ Page Title="IP5/IP6 Projekte" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="True" CodeBehind="Termine.aspx.cs" Inherits="ProStudCreator.Termine" %>

<%--
    
    Projektmodul	Dauer (KW)	    Starttermin	        Abgabetermin	    Aufwand
   ---------------------------------------------------------------------------------
    IP5     	    18 resp. 17     KW  8 resp. KW 38	KW 24 resp. KW  3   180 h
    IP5bb     	    26 resp. 27     KW  8 resp. KW 38	KW 33 resp. KW 12   180 h
    IP6     	    26 resp. 27     KW  8 resp. KW 38	KW 33 resp. KW 12   360 h

    TODO: konkrete termine automatisch aus KW berechnen
    
--%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="well usernSettings">
        <h3>Termine</h3>
        <%--<p>
            <i>Hinweis: Plane bei der Einreichung genügend Zeit für Iterationen ein!</i>
        </p>--%>
        <div class="well" style="background-color: #ffffff">
            <asp:GridView ID="AllEvents" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" AutoGenerateColumns="True" AlternatingRowStyle-BackColor="#e7e7e7" Width="100%" OnDataBound="AllEvents_DataBinding">
                <%--<AlternatingRowStyle BackColor="White" />--%>
            </asp:GridView>
            <hr/>
            <p>
                <b>Titeländerungen von Informatikprojekten sind bis <%: ProStudCreator.Global.AllowTitleChangesBeforeSubmission %> Wochen vor Abgabe möglich!</b>
            </p>
        </div>
    </div>
</asp:Content>