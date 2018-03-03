<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="JsonDemo.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <script>
        var obj = {
            "books": [
               { "language": "Java", "edition": "second" },
               { "language": "C++", "lastName": "fifth" },
               { "language": "C", "lastName": "third" }
            ]
        }

         obj.books[0].language;
    </script>
    
</head>
<body>
    <form id="form1" runat="server">
    <div>
       obj
        
    </div>
    </form>
</body>
</html>
