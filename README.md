# MVGLive v.2.0.0

Small WPF-based application to display a live schedule of public transport in Munich.

![Screenshot v.2.0.0](https://raw.githubusercontent.com/serhuey/MVGLive/master/Images/Screenshot_1.png)

## Some features:

- The settings window opens at startup.

![Settings v.2.0.0](https://raw.githubusercontent.com/serhuey/MVGLive/master/Images/SettingsWindow_0.png)


- The application is capable of displaying one to four schedules simultaneously.
- You can also adjust the colors, size and typeface of the fonts.
- To prevent the settings window from opening, for example, when the application has to start automatically at system startup and all settings have already been made, you need to use the command line option "-ns".

```
MVGLive.exe -ns
```

- The application blocks the screen saver and sleep mode after startup and restores them after its completion.
- All data about the schedule and current arrival of transport are taken from the official site mvg.de.
- In version 2.0, a local database with station identifiers has been added to reduce traffic to the official website.
- Special icons for U-Bahn and S-Bahn marks in destination string instead of the letters "U" and "S".

![Aditional U-S icons v.2.0.0](https://raw.githubusercontent.com/serhuey/MVGLive/master/Images/3d_AdditionalDestinationUSIcons.png)


- There are "First cars/Last cars" icons for branching routes such as S1 direction Flughafen/Freising.

![Aditional First-Last icons v.2.0.0](https://raw.githubusercontent.com/serhuey/MVGLive/master/Images/ForkedLines.png)


- Special small icons for routes that arrive late.

![Aditional delay icons v.2.0.0](https://raw.githubusercontent.com/serhuey/MVGLive/master/Images/DelayIcon.png)


- Some multi-platform stations show special icons with platform number. The shape of platform's icon depends on the route type.

![Multi-platform icons Tram v.2.0.0](https://raw.githubusercontent.com/serhuey/MVGLive/master/Images/HstIcon.png)

![Multi-platform icons S v.2.0.0](https://raw.githubusercontent.com/serhuey/MVGLive/master/Images/GleisIcon.png)


- Added a procedure that eliminates duplication of identical routes in one table. Sometimes mvg.de sends up to four identical routes for one request.
- Cancelled routes are no longer displayed.
- The application correctly handles short network or remote server shutdowns using previously obtained data to display.
- Tram route icons now use the color assigned to them on official maps.
- Added and modified original Express Bus icon for presentation on low screen resolutions displays.

![Express Bus icon S v.2.0.0](https://raw.githubusercontent.com/serhuey/MVGLive/master/Images/ExpressBusIcon.png)


- All icons are stored in Svg format. There is no more quality loss after changing the scale factor. The colour of some icons changes programmatically with the colour of the font to which they relate. This is not a problem for vector icons.

![SVG icons v.2.0.0](https://raw.githubusercontent.com/serhuey/MVGLive/master/Images/6_SVG_Example.png)

- The default PT Sans font is stored as an internal resource and does not require installation on the system.
- All operations to receive data from the server are asynchronous.

### Prerequisites

- .NET Framework 4.6.1

## Built With

* [Microsoft Visual Studio Community 2019, v.16.4.6] (https://visualstudio.microsoft.com/)
* [AutoCompleteTextBox, v.1.1.0] (https://github.com/quicoli/WPF-AutoComplete-TextBox)
* [ColorPickerWPF, v.1.0.9] (https://github.com/drogoganor/ColorPickerWPF)
* [WriteableBitmapEx v.1.5.1] (https://github.com/reneschulte/WriteableBitmapEx)
* [Newtonsoft.Json v.12.0.2] (https://www.nuget.org/packages/Newtonsoft.Json) - Parsing the mvg.de response
* [PT Sans font] (https://fonts.google.com/specimen/PT+Sans)
* [Gudea font] (https://fonts.google.com/specimen/Gudea)
* [Xara Photo & Graphic Designer 16.3.0.57723] (https://maven.apache.org/) - Vector icons drawing

## Authors

* **Sergei Grigorev** 

## Binaries

You can download the zipped install package here: 
(https://github.com/serhuey/MVGLive/blob/master/Deploy/MVGLiveSetup.zip)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details

