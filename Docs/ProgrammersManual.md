# Programmer's manual

#### Overview

Project contains 3 libraries : Server.dll, Shared.dll and Components.dll

- **Server.dll** - Contains class Server which runs server and ClientManager which handles clients.
- **Components.dll** - Contains predefined controllers components.
- **Shared.dll** - Contains data structures which are used in Server.dll and Components.dll. 

#### Pipeline

<img src="Images\Pipeline.png" style="zoom:80%;" />

If you are familiar with ASP.NET Core, you know there is a pipeline which contains middleware handling http requests and forwarding them to others. There is **WebSocketMiddleware** which handles web socket's requests in Server.dll. When the middleware accepts a web socket, the web socket is added to **ClientManager** be ClientManager's method. ClientManager is a microservice and is injected into **ICollectionServices** while **Server**'s configuration. ClientManager creates **Player**, which provides API for sending and receiving messages. I will talk about ClientManager and Server classes later in this article. Then the middleware loops in *Process* method until client is connected. When connection is lost, the middleware returns http response into the pipeline.

#### Client management

<img src="Images\Management.png" style="zoom:80%;" />

ClientManager takes care about all connected clients. When a new client is added the manager invoke *ReceiveAsync* is a method of Player and the task is stored into _events. When *ReceiveAsync* is called the manager waits until one task is completed in _tasks.*ReceiveAsync* of ClientManager returns only children of **ControllerEvent** with ControllerEvent itself. Each Player has **MessageManager**. MessageManager takes care about encoding and decoding messages. The manager is state machine and has 3 states: Initial, Configuration, Controllers. Initial is starting state of the manager. The manager waits for configuration message in Configuration mode and waits for controller event message in Controllers mode. Message arrives as sequence of byte. MessageManager recognises three types of messages: Initial, ChangeState, Configuration. Initial message isn't interesting. ChangeState message tells MessageManager to switch it's state. When the manager is in Configuration message, the message is treated as **ConfigurationMessage**. Same thing holds for Controllers state and ControllerEvent. the manager loops receiving messages until first ControllerEvent message occurs.

#### Decoding ControllerEvent

<img src="Images\EventManager.png" style="zoom: 67%;" />

When MessageManager is in Controllers state. The message is handled by **ControllersEventManager**. ControllerEventManager checks first byte of the message and finds a class which represents this message. After that invokes **TryDecode** which converts byte sequence to right .Net class.

#### ConfigurationMessage

<img src="\Images\Assembly.png" style="zoom:67%;" />

It serves to create new table of ControllerEvent class in ControllersEventManager.

#### Server configuration

The server can be parameterized by two classes. **StartUp** is common class in ASP.NET Core. Predefined StartUp adds ClientManager to services and WeSocketMiddleware to pipeline. The second class has to inherits Player. ClientManager is parametrized by this class and a consequence of this is that ClientManager works with the class instead of Player class. So you can configure **Kestrel** server as you want. 

#### Blazor web assembly

<img src="Images\Blazor.png" style="zoom:67%;" />

Blazor application can be fully created as a programmer wants. When the controllers are needed, Components.dll provides Razor components and Shared.dll provides **User** which takes care about a connection and sending or receiving messages. ControllerEvent classes are placed in Shared.dll as well. Because Blazor is standalone application, you have to copy the app with static files to server directory. After that, a middleware, which takes care about providing static files, finds the app and send it to client's browser.  

#### Extensibility

- You can configure the Kestrel server or host a different server by creating new StartUp class.
- You can modify Player class due to an inheritance to make own ClientManager.
- You can make own Blazor application. You just inject User to services and uses Razor components from Components.dll
- You can create own ControllersEvent due to  inheritance, so you can do own controllers.  