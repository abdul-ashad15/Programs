<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GithubCall.aspx.cs" Inherits="AngularDemoProj.GithubCall" %>

<!DOCTYPE html>

<html ng-app ="GithubModule">
<head runat="server">
    <title></title>
     <script src="angular.min.js"></script>
    <script src="Script.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div ng-controller="GithubController">
        
        <div>
            {{error}}
        </div>
    <div>
        Name : {{user.name}}
    </div>
    <div>
        Location : {{user.location}}
    </div>
    </div>
    </form>
</body>
</html>
