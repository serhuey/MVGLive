# MVGLive
Small WPF-based application to display time-tables for public transportation of Munich.

This is an example of using MVGTimeTable user control. 
The main application displays three controls with station's name and a clock. 
The example of usage:

    MVGLive.exe Hauptbahnhof Karlsplatz Odeonsplatz

Default stations are "Hirschgarten", "Briefzentrum", "Steubenplatz".
Each MVGTimeTable control refresh its data with time interval defined in code. For my point of view, 15 seconds interval is a good compromise.
