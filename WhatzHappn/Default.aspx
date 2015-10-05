<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WhatzHappn._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>What'z Happn</title>
    <link href='https://fonts.googleapis.com/css?family=Piedra|Rye|Sarina|Jacques+Francois+Shadow' rel='stylesheet' type='text/css' />
    <link href='https://fonts.googleapis.com/css?family=Londrina+Shadow' rel='stylesheet' type='text/css'/>
    <link href="css/WhatzHappn Style.css" rel="stylesheet" type="text/css" />

</head>
<body runat="server" id="WHBody">

    <div class="header">

        <img alt="logo" class="whlogo fadeInLeft" src="images/wh_logo.png" />


        <h1 class="fadeInDown">What'z Happn</h1>
              
    <div class="textContainer">
       
            <div class="text animated fadeInLeft">
             <p>See whatz happenin in your city! </p>
            <p>Its easy, just click the search button and we will use your current location to pull in the latest tweets, weather and news stories closest to you.</p>
        <p> If you don't see your current location or want to search a different location, either type in the location you want to search or just move the icon on the map.</p>
        </div>
        </div>
    
        <img alt="line" id="line" src="images/Line.jpg" />

        <div class="map animated fadeInRight">
            <iframe id="googlemap" src="https://www.google.com/maps/embed?q=-37.866963,144.980615" width="500" height="450" runat="server"></iframe>
        </div>
       </div>
    
</body>
</html>
