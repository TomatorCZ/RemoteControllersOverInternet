<h1 align="center">Remote Controllers Over Internet</h1>

### Description

The software provides you a simple and universal way, how to create remote controllers inside your browsers. It is divided into 2 components. 

First component behaves like a client. Actually, it is Blazor WebAssembly server which handles an user http request. Let me introduce you an example. An user wants to play a game, but doesn’t have a remote controller. The game, which contains the library, gives him an URL where the Blazor WebAssembly application lives and some authentication. The user fills in the authentication and his web browser is transformed into a controller defined by a creator of game.

Second component is a proper server. When the user starts to manipulate with the controller, the first component produces data which are sent over TCP to the second component. All logic of this feature is hidden and the game takes care about the server which gives it data from the user.

### Technologies

- [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor) - It is used on client-side
- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-3.1)

### Future plans

- Support for UDP connection - There is no support for this protocol in Blazor yet.

### How to use the library

See Docs/UsersManual.md

### Status

You should be able to run demo and library.  

There are no releases yet. If you are interest in it take a look at Docs or feel free to contact me (husaktomas98@gmail.com).
