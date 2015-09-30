<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WhatzHappn._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>What'z Happn</title>
    <link href='https://fonts.googleapis.com/css?family=Piedra|Rye|Sarina|Jacques+Francois+Shadow' rel='stylesheet' type='text/css' />
    <link href="../css/WhatzHappn Style.css" rel="stylesheet" type="text/css" />
</head>
<body runat="server" id="WHBody">

    <div class="header">
        <div class="content">
            <h1>What'z Happn</h1>

            <div class="text">
                <p>See whatz happenin in your city! </p>
                <p>Its easy, just click the search button and we will use your current location to pull in the latest tweets, weather and news stories closest to you.</p>
            </div>

            <div id="button">
                <a href="#" class="btn">Search</a>
            </div>

            <div class="map">
                <iframe src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3022.617356072694!2d-73.9878530849208!3d40.74844454332262!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x89c259a9b3117469%3A0xd134e199a405a163!2sEmpire+State+Building!5e0!3m2!1sen!2sus!4v1443542824793" width="500" height="450" /> 
            </div>
        </div>
    </div>

</body>
</html>
