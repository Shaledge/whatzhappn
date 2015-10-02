<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WhatzHappn._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>What'z Happn</title>
    <link href='https://fonts.googleapis.com/css?family=Piedra|Rye|Sarina|Jacques+Francois+Shadow' rel='stylesheet' type='text/css' />
    <link href="css/WhatzHappn Style.css" rel="stylesheet" type="text/css" />
</head>
<body runat="server" id="WHBody">

    <div class="header">
        <h1>What'z Happn</h1>

        <div class="text">
            <p>See whatz happenin in your city! </p>
            <p>Its easy, just click the search button and we will use your current location to pull in the latest tweets, weather and news stories closest to you.</p>
        </div>

        <div id="button">
            
        </div>

        <div class="map">
            <!-- iframe caues issue-->
        </div>
    </div>

</body>
</html>
