## Project structure

If you open the Solution explorer, you can see several projects 

The solution contains:

- **Client** - Web Assembly downloaded into your browser. This project takes care about how the web application looks like and behaves.
- **Components** - Library provides razor components which are used by **Client** (Buttons etc..).
- **Server** - Starts a Http server which routes to **Client** and provides a communication between user's browser and **Server**. 
- **Shared** - Other classes are used **Client**, **Components** and **Server**.

![SolutionStructure](SolutionStructure.png)

#### Frameworks

| Project    | Target Framework |
| ---------- | ---------------- |
| Client     | netstandard2.1   |
| Components | netstandard2.0   |
| Server     | netcoreapp3.1    |
| Shared     | netcoreapp3.1    |

#### Client

// TODO

#### Components

// TODO

#### Server

// TODO

#### Shared

// TODO