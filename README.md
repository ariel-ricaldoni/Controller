# Controller

Controller is a console application that allows you to control mouse and keyboard inputs on a Windows machine through a Xbox gamepad. It also allows you to customize the key bindings for all the buttons available on the Xbox gamepad.

## Getting Started

Download or clone this repository and load it into either Visual Studio 2019 or Visual Studio Code. Be sure to set ControllerCli as the startup project. On the first execution a file called config.ini will be created at the same location of the ControllerCli.exe file. This file allows you to customize the key bindings and other properties. It also contains instructions about the usage of the application.

## Prerequisites

To build this project you must have .NET Core 3.1 SDK installed on the development machine and either Visual Studio 2019 or Visual Studio Code. The Xbox gamepad driver xinput1_4.dll must also be available on the development machine.

## Deployment

Once the project is published you may place the binaries wherever you want. In order to run it on a machine without .Net Framework installed you must publish it with the Deployment Mode as "Self-contained". If you want to deploy the project using custom configurations you must deploy the config.ini file at the same location as the ControllerCli.exe file, the application will read the configurations from that file instead of creating one with default values.

## Observation

If you do not want to build the application yourself, you may try it by download the Controller.zip file. It contains the latest self contained build.

## Known Issues

* Xbox Series gamepads won't display the battery percentage on wireless mode.

## Change Log

* [v1.0.0.7] - 2023/02/18
	* Code refactoring
	* Keyboard macros implemented 
	* Minor changes to the "Complete" View option
	* Simple View option renamed to Basic
	* Newtonsoft.Json version upgraded to 13.0.2
	* Readme and Configuration revision

* [v1.0.0.6] - 2021/09/12
	* Code refactoring to make it easier for unit tests (unit tests to come on future releases)
	* View configuration bug fix
	* Console window bug fix
	* Upgraded the project to NetCore 3.1 following NetCore 3.0 deprecation

* [v1.0.0.5] - 2020/11/28
	* Code refactoring and bug fixes
	* Multiple key bindigns implemented
	* Directional keys and WASD support implemented for the analog sticks

* [v1.0.0.4] - 2020/07/14
	* Code refactoring and bug fixes
	* Application icon created
	* Project published to Github

* [v1.0.0.3] - 2020/04/30
	* Synchronizer bug fix
	* Right analog button bug fix

* [v1.0.0.2] - 2020/04/17
	* Code refactoring
	* Virtual keyboard macro implemented

* [v1.0.0.1] - 2020/04/13
	* Key bindings implemented
	* Input lock implemented

* [v1.0.0.0] - 2020/04/12
	* Mouse movement and scroll implemented
	* Keyboard key actions implemented
	* Controller input and battery state reading implement

## Built With

* [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1) - Framework
* [Visual Studio 2019](https://visualstudio.microsoft.com/vs/) - IDE

## Author

* **Ariel Ricaldoni** - [Ariel-Ricaldoni](https://github.com/ariel-ricaldoni)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

* Thank you very much to everybody who helped with feature suggestions and testing. 
