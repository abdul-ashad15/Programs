<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AngularDemo.aspx.cs" Inherits="AngularDemoProj.AngularDemo" %>

<!DOCTYPE html>

<html ng-app ="myModule">
<head runat="server">
    <title></title>
    <script src="angular.min.js"></script>
    <script src="Script.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div ng-controller="myController">
       <%-- <asp:TextBox ID="txt" runat="server" ng-model="message"></asp:TextBox>
        {{message}}--%>

       <%--<table>
            <thead>
                <th>FirstName</th>
                <th>LastName</th>
                <th>Gender</th>
            </thead>
            <tbody>
                <tr ng-repeat="employee in employees">
                    <td>{{employee.firstName}}</td>
                   <td>{{employee.lastName}}</td>
                   <td>{{employee.gender}}</td>
                 </tr>
            </tbody>
        </table>--%>

       <%-- <ul>
            <li ng-repeat="country in countries">
                {{country.name}}
                <ul>
                    <li ng-repeat="city in country.cities">
                        {{city.name}}
                    </li>
                </ul>
            </li>
        </ul>--%>

       <%-- <table>
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Likes</th>
                    <th>DisLikes</th>
                    <th>Like/DisLike</th>
                </tr>
            </thead>
            <tbody>
                <tr ng-repeat="technology in technologies">
                    <td>{{technology.name}}</td>
                    <td>{{technology.likes}}</td>
                    <td>{{technology.dislikes}}</td>
                    <td>
                        <asp:Button runat="server" ID="btnLikes" text="Likes" ng-click="incrementLikes(technology)" />
                        <asp:Button runat="server" ID="btnDisLikes" text="Dislikes" ng-click="incrementDislikes(technology)" />
                    </td>
                </tr>
            </tbody>
        </table>--%>

        <table>
            {{error}}
             <thead>
                <tr>
                    <th>StudentId</th>
                    <th>StudentName</th>
                </tr>
            </thead>
            <tbody>
               <tr ng-repeat="student in Students">
                                <td> <span>{{student.StudentId}}</span></td>
                                <td> <span>{{student.StudentName}}</span></td>
                            </tr>
            </tbody>
        </table>
    </div>
    </form>
</body>
</html>
