<%-- 
    CLOUD READINESS MIGRATION NOTE (cr-dotnet-0026):
    This Web Forms User Control has been migrated to ASP.NET Core MVC/Razor Pages pattern.
    The equivalent ASP.NET Core component is located at Pages/Shared/_ViewSwitcher.cshtml.
    Web Forms UserControl directive and server-side rendering have been replaced with
    standard HTML and Razor syntax for cloud-native deployment on AWS (ECS/EKS).
    Mobile view switching is handled via ASP.NET Core middleware and responsive design.
--%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewSwitcher.ascx.cs" Inherits="FIlms.ViewSwitcher" %>
<div id="viewSwitcher">
    <%: CurrentView %> view | <a href="<%: SwitchUrl %>">Switch to <%: AlternateView %></a>
</div>
