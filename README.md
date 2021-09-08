# Group expenses

## Introduction
The app is created with the aim to solve common problem when travelling in group and that is to calculate the expenses among its members. 

## Architecture

Application consists of 2 parts: backend build on ASP.NET core and frontend build on Blazor WebAssembly.

### Backend
The backend is build as API controllers. There are 3 contoller/main routes (auth, group, user)
Custom exception middleware is used, which will handle thrown exceptions. Also HttpStatusException is used to singalize 
status code of the response. PostgreSQL is used as the database, however any EF Core supported database can be used after slight modification.
JWT token authorization is used and authorized routes are stateless, consuming only the data from request (including the token) and no inner state.


### Frontend
Frontend is build as Blazor WebAssembly.
It highly uses component structure, with the aim to minimize code duplication.
Custom auth route handler has been build, which allows pages to be for authorized users only.
This is achieved by using `@attribute [Authorize]`. Futher improvement is possible by utilizing user roles.
API services are used, which serve as both data stores and data getters. They also have OnChange event subscriber,
which enables changes subscription. Also *provider* alternatives are provided, which exposes data as cascading paramether
for seamless data pass with automatic OnChange update.


## Installation

### Backend

Firstly, BE needs `appsettings.json` file to set variables. Expected shape of the file is 
```{json}
{
    "ConnectionStrings": {
        "PostgreSql": "connection_string_here"
    },
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft": "Warning",
            "Microsoft.Hosting.Lifetime": "Information"
        }
    },
    "AllowedHosts": "*",
    "JWT": {
        "Secret": "your_secret_here"
    }
}

```
Then Nuget package restore is needed. Then it can be run/build/published normally as any other .NET application.

### Frontend

Only requirement for running frontend is doing Nuget package restore. However using `dotnet publish --configuration Release` for build
is recommended, because of IL trimming. Content is placed under `wwwroot` folder and is suitable for static file hosting with single page 
application settings. Also by default files are produced as both compressed and uncompressed, potentionally saving network traffic when used. 

## Usage

Basic usage of the application can be divided into several parts. 

First part consist of basic user managment. User can log in, register and logout.

Second part is the managing of user relationship with other users. User can send, reject and accept friend request. This module is prerequisite for the third module.

Third part is the group managment. User can create group and add his friends into the group.
User can also add, modify and remove payment groups. Payment group is a named group, which represents payment from one user to eventually multiple other users.
Finally, user can calculate group settlement, which consists of payments, which when taken place, every person in the group will be even with other persons.

![Group example](assets/example.png)


