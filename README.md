# OwnRadio.Client.UWP
 Universal Windows Platform OwnRadio Client. Simple streaming audio player.

## Getting started

UWP project requires certificate .pfx. If you don't have a certificate of your own to use follow this steps:

* Open OwnRadio.Client.UWP.csproj
* In Package.appxmanifest select Packaging tab
* Choose Certificate... -> Configure Certificate... -> Create test certificate...

System will create cert _TemporaryKey.pfx.

Check Build Configuration to make sure it compiles on Local Machine.

## Requirements

* Visual Studio 2015
* Microsoft .NET Core 1.0.1
* Windows 10 (10.0; Build 10586)