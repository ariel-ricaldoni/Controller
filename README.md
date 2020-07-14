# Controller

Controller is a console application that allows you to control the mouse and keyboard on a Windows machine through an Xbox 360/One controller. It also allows you to customize the key bindings for all the buttons available on the Xbox 360/One controller.

## Getting Started

Download or clone this repository and load it into either Visual Studio 2019 or Visual Studio Code. Set ControllerCli as the startup project and press F5 to debug. On the first execution a file called config.ini will be created at the same location as the ControllerCli.exe file. This file allows you to customize the key bindings and some other properties. It also contains observations about the use of the application.

## Prerequisites

To build this project you must have .NET Core 3.0 SDK installed on the development machine and either Visual Studio 2019 or Visual Studio Code. The xinput1_4.dll must also be available on the development machine.

## Deployment

Once the project is published you may place the binaries wherever you want. In order to run it on a machine without .Net Framework installed you must publish it with the Deployment Mode as "Self-contained". If you want to deploy the project using custom configurations you must deploy the config.ini file at the same location as the ControllerCli.exe file, the application will read the configurations from that file instead of creating one with default values.

## Built With

* [.NET Core 3.0](https://dotnet.microsoft.com/download/dotnet-core/3.0) - Framework
* [Visual Studio 2019](https://visualstudio.microsoft.com/vs/) - IDE

## Author

* **Ariel Ricaldoni** - [Ariel-Ricaldoni](https://github.com/ariel-ricaldoni)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

* Thank you very much to everybody who helped with feature suggestions and testing. 
