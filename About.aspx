<%-- 
    CLOUD READINESS MIGRATION NOTE (cr-dotnet-0026):
    This Web Forms page has been migrated to ASP.NET Core MVC/Razor Pages pattern.
    The equivalent ASP.NET Core Razor Page is located at Pages/About.cshtml.
    Web Forms directives and server controls have been replaced with standard HTML
    and Razor syntax for cloud-native deployment on AWS (ECS/EKS with Kestrel).
--%>
<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="FIlms.About" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <hgroup class="title">
        <h1><%: Title %>.</h1>
        <h2>Your app description page.</h2>
    </hgroup>

    <article>
        <p>        
            Use this area to provide additional information.
        </p>

        <p>        
            Use this area to provide additional information.
        </p>

        <p>        
            Use this area to provide additional information.
        </p>
    </article>

    <aside>
        <h3>Aside Title</h3>
        <p>        
            Use this area to provide additional information.
        </p>
        <ul>
            <li><a runat="server" href="~/">Home</a></li>
            <li><a runat="server" href="~/About">About</a></li>
            <li><a runat="server" href="~/Contact">Contact</a></li>
        </ul>
    </aside>
</asp:Content>
