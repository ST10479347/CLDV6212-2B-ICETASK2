# CLDV6212-2B-ICETASK2

An Azure Functions (HTTP Trigger, isolated worker, .NET 8) project with three
functions that create and retrieve user records in Azure Table Storage.


AzureFunctionApp IceTask2/
├── Functions/
│   ├── CreateUserFunction.cs
│   ├── GetUserFunction.cs
│   └── GetAllUsersFunction.cs
|   └── Fuction
├── Models/
│   ├── UserEntity.cs   (Azure Table Storage entity)
│   └── UserDto.cs      (JSON request/response shape)
├── Program.cs           (registers the TableClient)
├── host.json
├── local.settings.json  (local dev only — not published)
└── AzureFunctionApp Icetask2.csproj
