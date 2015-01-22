<%@ Page Title="New Project" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AddNewProject.aspx.cs" Inherits="ProStudCreator.AddNewProject" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Stylesheets" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="refusedReason" class="well newProjectSettings non-selectable" runat="server" visible="false">
        <asp:Label runat="server" ID="refusedReasonTitle" Text="Refuse Reason:" Font-Size="24px" Height="50px"></asp:Label>
        <div class="form-group">
            <asp:Label runat="server" CssClass="control-label" Text="Please tell the reason why you want to reject this project. This text will be sent via e-mail to the user."></asp:Label>
            <asp:TextBox runat="server" ID="refusedReasonText" CssClass="form-control contentRefuseDesign" TextMode="MultiLine"></asp:TextBox>
        </div>
        <asp:Button runat="server" ID="refuseDefinitiveNewProject" CssClass="btn btn-default refuseProject" Width="125px" Text="Refuse Definitive" OnClientClick="return confirm('Do you really want to refuse this project?');" OnClick="refuseDefinitiveNewProject_Click"></asp:Button>
        <asp:Button runat="server" ID="cancelRefusion" CssClass="btn btn-default" Text="Cancel" OnClick="cancelRefusion_Click" CausesValidation="false"></asp:Button>
    </div>

    <div id="newProjectDiv" class="well newProjectSettings non-selectable" runat="server">
        <asp:Label runat="server" ID="SiteTitle" Font-Size="24px" Height="50px"></asp:Label>
        <asp:PlaceHolder ID="AdminView" runat="server" Visible="false">
            <asp:Label runat="server" ID="CreatorID" CssClass="pull-right" Font-Size="24px" Height="50px"></asp:Label>
        </asp:PlaceHolder>
        <div class="well contentDesign form-horizontal" style="background-color: #ffffff">
            <div class="form-group" style="margin-top: 15px">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Projectname:"></asp:Label>
                <asp:TextBox runat="server" ID="ProjectName" CssClass="col-sm-9 form-control" MaxLength="100" placeholder="Projectname"></asp:TextBox>
                <asp:RequiredFieldValidator ID="ProjectNameValidator" CssClass="col-sm-9" ForeColor="Red" Display="Dynamic" ControlToValidate="ProjectName" runat="server" ErrorMessage="The project name cannot be blank!"></asp:RequiredFieldValidator>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Employer:"></asp:Label>
                <asp:TextBox runat="server" ID="Employer" CssClass="col-sm-9 form-control" placeholder="Name of the Employer"></asp:TextBox>
                <asp:TextBox runat="server" ID="EmployerMail" CssClass="col-sm-9 form-control" placeholder="E-Mail of the Employer" TextMode="Email"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Advisor:"></asp:Label>
                <asp:TextBox runat="server" ID="NameBetreuer1" CssClass="col-sm-9 form-control" placeholder="Name of the first advisor"></asp:TextBox>
                <asp:TextBox runat="server" ID="EMail1" CssClass="col-sm-9 form-control" placeholder="E-Mail of the first advisor" TextMode="Email"></asp:TextBox>
                <asp:RequiredFieldValidator ID="NameBetreuerValidator" CssClass="col-sm-9" ForeColor="Red" Display="Dynamic" ControlToValidate="NameBetreuer1" runat="server" ErrorMessage="The Name of the first advisor cannot be blank!"></asp:RequiredFieldValidator>
                <asp:RequiredFieldValidator ID="EMail1BetreuerValidator" CssClass="col-sm-9" ForeColor="Red" Display="Dynamic" ControlToValidate="EMail1" runat="server" ErrorMessage="The E-Mail of the first advisor cannot be blank!"></asp:RequiredFieldValidator>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3"></asp:Label>
                <asp:TextBox runat="server" ID="NameBetreuer2" CssClass="col-sm-9 form-control" placeholder="Name of the second advisor"></asp:TextBox>
                <asp:TextBox runat="server" ID="EMail2" CssClass="col-sm-9 form-control" placeholder="E-Mail of the second advisor" TextMode="Email"></asp:TextBox>
            </div>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Projecttype:"></asp:Label>
                <asp:ImageButton CssClass="img-rounded" ID="DesignUX" Height="60px" runat="server" ImageUrl="/pictures/projectTypDesignUXUnchecked.png" OnClick="DesignUX_Click" CausesValidation="false" />
                <asp:ImageButton CssClass="img-rounded" ID="HW" Height="60px" runat="server" ImageUrl="/pictures/projectTypHWUnchecked.png" OnClick="HW_Click" CausesValidation="false" />
                <asp:ImageButton CssClass="img-rounded" ID="CGIP" Height="60px" runat="server" ImageUrl="/pictures/projectTypCGIPUnchecked.png" OnClick="CGIP_Click" CausesValidation="false" />
                <asp:ImageButton CssClass="img-rounded" ID="MathAlg" Height="60px" runat="server" ImageUrl="/pictures/projectTypMathAlgUnchecked.png" OnClick="MathAlg_Click" CausesValidation="false" />
                <asp:ImageButton CssClass="img-rounded" ID="AppWeb" Height="60px" runat="server" ImageUrl="/pictures/projectTypAppWebUnchecked.png" OnClick="AppWeb_Click" CausesValidation="false" />
                <asp:ImageButton CssClass="img-rounded" ID="DBBigData" Height="60px" runat="server" ImageUrl="/pictures/projectTypDBBigDataUnchecked.png" OnClick="DBBigData_Click" CausesValidation="false" />
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Priority 1"></asp:Label>
                <asp:DropDownList runat="server" ID="POneContent" CssClass="form-control col-sm-1" Width="200px">
                </asp:DropDownList>
                <asp:DropDownList runat="server" ID="POneTeamSize" CssClass="form-control col-sm-1 teamSizeMarginLeft" Width="200px" OnSelectedIndexChanged="POneTeamSize_SelectedIndexChanged" AutoPostBack="true">
                </asp:DropDownList>
            </div>

            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Priority 2"></asp:Label>
                <asp:DropDownList runat="server" ID="PTwoContent" CssClass="form-control col-sm-1" Width="200px">
                </asp:DropDownList>
                <asp:DropDownList runat="server" ID="PTwoTeamSize" CssClass="form-control col-sm-1 teamSizeMarginLeft" Width="200px" OnSelectedIndexChanged="PTwoTeamSize_SelectedIndexChanged" AutoPostBack="true">
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Initial position:"></asp:Label>
                <asp:TextBox runat="server" ID="InitialPositionContent" CssClass="col-sm-9 form-control" TextMode="MultiLine"></asp:TextBox>
                <asp:RequiredFieldValidator ID="InitialPositionContentValidator" CssClass="col-sm-9" ForeColor="Red" Display="Dynamic" ControlToValidate="InitialPositionContent" runat="server" ErrorMessage="The Initial position cannot be blank!"></asp:RequiredFieldValidator>
            </div>
            <div class="form-group">
                <asp:Label runat="server" ID="AddPictureLabel" CssClass="control-label col-sm-3" Text="Add image:"></asp:Label>
                <asp:FileUpload runat="server" ID="AddPicture" CssClass="control-label col-sm-4" />
                <asp:LinkButton runat="server" ID="DeleteImageButton" OnClick="deleteImage_Click" OnClientClick="return confirm('Do you really want to delete this image?');" CssClass="btn btn-default btnHeight imageRemoveMargin glyphicon glyphicon-remove" Visible="false"></asp:LinkButton>
            </div>
            <div class="form-group">
                <asp:Label runat="server" ID="ImageLabel" CssClass="control-label col-sm-3" Text=""></asp:Label>
                <asp:Image runat="server" ID="Image1" CssClass="maxImageWidth img-rounded" Visible="false" />
            </div>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Objective:"></asp:Label>
                <asp:TextBox runat="server" ID="ObjectivContent" CssClass="col-sm-9 form-control" placeholder="Objective" TextMode="MultiLine" MaxLength="1000"></asp:TextBox>
                <asp:RequiredFieldValidator ID="ObjectivContentValidator" CssClass="col-sm-9" ForeColor="Red" Display="Dynamic" ControlToValidate="ObjectivContent" runat="server" ErrorMessage="The field 'Objective' cannot be blank!"></asp:RequiredFieldValidator>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Problem statement:"></asp:Label>
                <asp:TextBox runat="server" ID="ProblemStatementContent" CssClass="col-sm-9 form-control" placeholder="ProblemStatement" TextMode="MultiLine" MaxLength="1000"></asp:TextBox>
                <asp:RequiredFieldValidator ID="ProblemStatementContentValidator" CssClass="col-sm-9" ForeColor="Red" Display="Dynamic" ControlToValidate="ProblemStatementContent" runat="server" ErrorMessage="The Problemstatement cannot be blank!"></asp:RequiredFieldValidator>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Technologies / main focus / references:"></asp:Label>
                <asp:TextBox runat="server" ID="ReferencesContent" CssClass="col-sm-9 form-control" placeholder="Technologies / main focus / references" TextMode="MultiLine" MaxLength="1000"></asp:TextBox>
                <asp:RequiredFieldValidator ID="ReferencesContentValidator" CssClass="col-sm-9" ForeColor="Red" Display="Dynamic" ControlToValidate="ReferencesContent" runat="server" ErrorMessage="The field 'Technologies / main focus / references' cannot be blank!"></asp:RequiredFieldValidator>
            </div>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Remarks (optional):"></asp:Label>
                <asp:TextBox runat="server" ID="RemarksContent" CssClass="form-control col-sm-9" placeholder="Remarks" TextMode="MultiLine" MaxLength="1000"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Reservation (optional):"></asp:Label>
                <asp:TextBox runat="server" ID="ReservationNameOne" CssClass="col-sm-9 form-control" placeholder="Full name of the student"></asp:TextBox>
                <asp:TextBox runat="server" ID="ReservationNameTwo" CssClass="col-sm-9 form-control contentReservation" placeholder="Full name of the student" Visible="false"></asp:TextBox>
            </div>
            <!-- CANCELLED PART !!
            <div class="form-group">
                <asp:Label Visible="false" runat="server" CssClass="control-label col-sm-3" Text="Wichtigkeit:"></asp:Label>
                <asp:DropDownList Visible="false" runat="server" ID="ImportanceContent" CssClass="form-control" Width="280px">
                    <asp:ListItem>Normal</asp:ListItem>
                    <asp:ListItem>wichtig aus Sicht Institut oder FHNW</asp:ListItem>
                </asp:DropDownList>
            </div> -->
            <div class="form-group">
                <asp:Label runat="server" CssClass="control-label col-sm-3" Text="Department:"></asp:Label>
                <asp:DropDownList runat="server" ID="Department" CssClass="form-control" Width="80px">
                </asp:DropDownList>
            </div>
        </div>
        <asp:Button runat="server" ID="saveNewProject" OnClick="saveProject" CssClass="btn btn-default marginLeftZero" TabIndex="4" Width="113px"></asp:Button>
        <asp:Button runat="server" ID="cancelNewProject" CssClass="btn btn-default marginLeftZero" TabIndex="5" Text="Cancel" OnClick="cancelNewProject_Click" CausesValidation="false"></asp:Button>
        <asp:Button runat="server" ID="submitProject" Visible="false" CssClass="btn btn-default marginLeftZero" Text="Submit project" OnClientClick="return confirm('Do your really want to submit this project?');" OnClick="submitProject_Click"></asp:Button>
        <asp:Button runat="server" ID="publishProject" Visible="false" CssClass="btn btn-default publishProject pull-right" Width="113px" Text="Publish" OnClientClick="return confirm('Do your really want to publish this project?');" OnClick="publishProject_Click"></asp:Button>
        <asp:Button runat="server" ID="refuseNewProject" Visible="false" CssClass="btn btn-default refuseProject pull-right" Width="113px" Text="Refuse" OnClick="refuseProject_Click"></asp:Button>
        <asp:Button runat="server" ID="rollbackProject" Visible="false" CssClass="btn btn-default pull-right" Width="113px" Text="Rollback" OnClientClick="return confirm('Do your really want to rollback this project?');" OnClick="rollbackProject_Click"></asp:Button>
    </div>
</asp:Content>
