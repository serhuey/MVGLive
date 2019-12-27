# MVGLive
Small WPF-based application created to display time-tables for public transportation of Munich.

This application is an example of using MVGTimeTable user control. 
It displays three controls with station's name and a simple digital clock in the right upper corner. 
The example of usage with a command string:

    MVGLive.exe Hauptbahnhof Karlsplatz Marienplatz

Default stations are "Hirschgarten", "Briefzentrum", "Steubenplatz".
Each MVGTimeTable control refresh its data with time interval defined in code. I think, 15 seconds interval is a good compromise.

![Screenshot v.1.1.0](https://raw.githubusercontent.com/serhuey/MVGLive/master/ScreenShotV1.1.png)
