<%-- 
    CLOUD READINESS MIGRATION NOTE (cr-dotnet-0026):
    This Web Forms page has been migrated to ASP.NET Core MVC/Razor Pages pattern.
    The equivalent ASP.NET Core Razor Page is located at Pages/Index.cshtml.
    Web Forms directives and server controls have been replaced with standard HTML
    and Razor syntax for cloud-native deployment on AWS (ECS/EKS with Kestrel).
--%>
<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="FIlms._Default" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
        <div class="content-wrapper">
            <hgroup class="title">
                
                <h2>Създаване на проект по дисциплина „Технологии за
изграждане на уеб системи" - упражнение</h2>
            </hgroup>
          
            <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/web_site (1).png" ImageAlign="Middle" /><br />
            
            <asp:Button ID="Button1" runat="server" BackColor="#3399FF" ForeColor="White" OnClick="Button1_Click" Text="Научете повече >>" CssClass="round" />
            
        </div>
          
</asp:Content>
