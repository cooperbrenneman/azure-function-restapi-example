# Azure Function REST API Example

Example project for using Azure functions for a REST API for C# and Javascript.

## Overview

In this example, you will be creating a set of Azure Functions

We will have the below routing for our application:

| HTTP Method | Route     | Function   | Description                 |
|-------------|-----------|------------|-----------------------------|
| GET         | /dogs     | GetAllDogs | Get all dogs                |
| GET         | /dogs/:id | GetDogById | Get a dog based on an id    |
| POST        | /dogs     | CreateDog  | Creates a new dog           |
| DELETE      | /dogs/:id | DeleteDog  | Delete a dog based on an id |
| PUT         | /dogs/:id | UpdateDog  | Update a dog based on an id |

There are example dogs in `dogs.json` for reference.

### Notes on Languages

In this tutorial, there are two languages that will be supported:

- C#
- Javascript

In some cases, the steps used to configure these Azure Functions will differ because of the languages. In those cases, there will language specific notes to ensure that the correct steps can be followed.


## Creating Resources

## Creating a Resource Group

It is not necessary, but highly encouraged to create a resource group for all of these items below. You can do this through the Azure portal or the Azure CLI.

### Creating Table Storage Resource

You can go to the Azure portal and create a new Storage account (blob, file, table, queue). The overview can be found [here](https://docs.microsoft.com/en-us/azure/storage/common/storage-account-overview).

## C\#

### Configuring Function App Environment

#### Visual Studio Code

You will need to download and install Visual Studio Code and the Azure Functions Extension for Visual Studio Code. You will be using both of them to create the Azure Function Apps, develop locally, and deploy them to Azure.

#### Creating the Azure Function

Go to the Azure button in Visual Studio Code and locate the "Functions" section. Select "Create New Project...", select the location you want to use for your rest api.

Then you will need to select C#. Then, select the "HttpTrigger", name the project (which will become your namespace), and select a specific namespace, and finally select the "Function" access.

You will need to download the corresponding .NET packages and install them acccordingly. You may also have to restore the packages as well.

#### Installing Additional Packages

In this tutorial, you will also have to install the latest packages depending on the Function version.

For example, if using Functions 2.x and higher and installing on Visual Studio Code you can run the below:

```shell
dotnet add package Microsoft.Azure.WebJobs.Extensions.Storage
```

More information can be found [here](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-table?tabs=csharp).

### Creating a Dog Model Class

A great thing to do is create a Dog.cs file that you will use to declare the Dog Model that the API will be working with. This will greatly simplify validating data from the API in the C# case. Make sure that your namespace is updated to contain the your exact namespace.

```cs
using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureRESTAPIDogExample
{
    public class Dog
    {
        public string Id {get; set;} = Guid.NewGuid().ToString();
        public DateTime CreatedTime {get; set; } = DateTime.UtcNow;
        public string Name { get; set; }
        public string Breed { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
    }

    public class DogCreateModel
    {
        public string Name { get; set; }
        public string Breed { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
    }

        public class DogUpdateModel
    {
        public string Name { get; set; }
        public string Breed { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
    }

        public class DogTableEntity : TableEntity
    {
        public DateTime CreatedTime { get; set; }
        public string Name { get; set; }
        public string Breed { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
    }

    public static class Mappings
    {
        public static DogTableEntity ToTableEntity(this Dog dog)
        {
            return new DogTableEntity()
            {
                PartitionKey = "DOG",
                RowKey = dog.Id,
                CreatedTime = dog.CreatedTime,
                Name = dog.Name,
                Breed = dog.Breed,
                Age = dog.Age,
                Sex = dog.Sex
            };
        }
        public static Dog ToDog(this DogTableEntity dog)
        {
            return new Dog()
            {
                Id = dog.RowKey,
                CreatedTime = dog.CreatedTime,
                Name = dog.Name,
                Breed = dog.Breed,
                Age = dog.Age,
                Sex = dog.Sex
            };
        }
    }

}
```

This file declares the classes that will be used to help serialize and deserialize the dog data. It will also allow us to 

### Creating CreateDog POST Method

Create the azure function below in your cs file (`AzureFunctions.cs` in this example):

```cs
[FunctionName("CreateDog")]
public static async Task<IActionResult> CreateDog(
    [HttpTrigger(AuthorizationLevel.Function, "post", Route = "dogs")] HttpRequest req,
    [Table("dogs", Connection = "AzureWebJobsStorage")] IAsyncCollector<DogTableEntity> dogTable,
    ILogger log)
{
    log.LogInformation("Starting CreateDog function...");
    try
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonConvert.DeserializeObject<DogCreateModel>(requestBody);

        var dog = new Dog()
        {
            Name = data.Name,
            Breed = data.Breed,
            Age = data.Age,
            Sex = data.Sex
        };

        await dogTable.AddAsync(dog.ToTableEntity());

        return new OkObjectResult(dog);
    }
    catch (Exception e)
    {
        return new BadRequestObjectResult(e);
    }
}
```

### Creating GetAllDogs GET Method

Create the azure function below in your cs file (`AzureFunctions.cs` in this example):

```cs
[FunctionName("GetAllDogs")]
public static async Task<IActionResult> GetAllDogs(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "dogs")] HttpRequest req,
    [Table("dogs", Connection = "AzureWebJobsStorage")] CloudTable dogTable,
    ILogger log)
{
    log.LogInformation("Starting GetAllDogs function...");

    var query = new TableQuery<DogTableEntity>();

    var segment = await dogTable.ExecuteQuerySegmentedAsync(query, null);

    return new OkObjectResult(segment.Select(Mappings.ToDog));
}
```

### Creating GetDog GET Method

Create the azure function below in your cs file (`AzureFunctions.cs` in this example):

```cs
[FunctionName("GetDogById")]
public static async Task<IActionResult> GetDogById(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "dogs/{id}")] HttpRequest req,
    [Table("dogs", "DOG", "{id}", Connection = "AzureWebJobsStorage")] DogTableEntity dog,
    ILogger log,
    string id)
{
    log.LogInformation("Starting GetDogById function...");

    if (dog == null)
    {
        log.LogInformation($"Dog {id} not found");
        return new NotFoundResult();
    }

    return new OkObjectResult(dog);
}
```

### Creating DeleteDog DELETE Method

Create the azure function below in your cs file (`AzureFunctions.cs` in this example):

```cs
[FunctionName("DeleteDog")]
public static async Task<IActionResult> DeleteDog(
    [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "dogs/{id}")] HttpRequest req,
    [Table("dogs", Connection = "AzureWebJobsStorage")] CloudTable dogTable,
    ILogger log,
    string id)
{
    log.LogInformation("Starting DeleteDog function...");

    var deleteOperation = TableOperation.Delete(new TableEntity() { PartitionKey = "DOG", RowKey = id, ETag = "*" });
    try
    {
        var deleteResult = await dogTable.ExecuteAsync(deleteOperation);
        return new OkObjectResult(deleteResult);
    }
    catch (StorageException e) when (e.RequestInformation.HttpStatusCode == 404)
    {
        return new NotFoundResult();
    }
}
```

### Creating UpdateDog PUT Method

Create the azure function below in your cs file (`AzureFunctions.cs` in this example):

```cs
[FunctionName("UpdateDog")]
public static async Task<IActionResult> UpdateDog(
    [HttpTrigger(AuthorizationLevel.Function, "put", Route = "dogs/{id}")] HttpRequest req,
    [Table("dogs", Connection = "AzureWebJobsStorage")] CloudTable dogTable,
    ILogger log,
    string id)
{
    log.LogInformation("Starting UpdateDog function...");

    try
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonConvert.DeserializeObject<DogCreateModel>(requestBody);
        var findOperation = TableOperation.Retrieve<DogTableEntity>("DOG", id);
        var findResult = await dogTable.ExecuteAsync(findOperation);
        if (findResult.Result == null)
        {
            return new NotFoundResult();
        }
        var existingDogRow = (DogTableEntity)findResult.Result;

        existingDogRow.Age = data.Age;

        if (!string.IsNullOrEmpty(data.Name))
        {
            existingDogRow.Name = data.Name;
        }

        if (!string.IsNullOrEmpty(data.Breed))
        {
            existingDogRow.Breed = data.Breed;
        }

        if (!string.IsNullOrEmpty(data.Sex))
        {
            existingDogRow.Sex = data.Sex;
        }

        var replaceOperation = TableOperation.Replace(existingDogRow);
        await dogTable.ExecuteAsync(replaceOperation);

        return new OkObjectResult(existingDogRow.ToDog());
    }
    catch (Exception e)
    {
        return new BadRequestObjectResult(e);
    }
}
```

### Adding Connection String

Once created, navigate to the newly created resource.

Go to `Access keys` -> key1 `Connection string`, and copy this value into your `local.settings.json` file:

```json
{
  "Values": {
    "AzureWebJobsStorage": "<Paste-Your-Connection-String-Here>",
}
```

The `Connection` variable in the `Table` attribute will then use the connection string in the `local.settings.json` file, looking for the key `AzureWebJobsStorage`.

## Javascript

### Configuring Function App Environment


#### Creating the Azure Function

Go to the Azure button in Visual Studio Code and locate the "Functions" section. Select "Create New Project...", select the location you want to use for your rest api.

Then you will need to select Javascript. Then, select the "HttpTrigger", name the project (which will become your namespace), and select a specific namespace, and finally select the "Function" access.

You will need to download the corresponding node packages and install them acccordingly.

#### Installing Additional Packages

For the javascript example, you will be using the `uuid` and the `azure-storage` packages. To install these, navigate to the working directory and run the below command:

```shell
npm install uuid azure-storage --save
```

This will install and save the packages in the `package.json` file.

### Creating CreateDog POST Method

Create the azure function below in a directory (in this example it is `CreateDog\index.js`):

```javascript
const { v4: uuidv4 } = require('uuid');

module.exports = async function (context, req) {
    context.log('Starting CreateDog function...');

    if(req.body){

        if(req.body.name && req.body.breed && req.body.age && req.body.sex){
           
            context.bindings.tableBinding = [];

            var dog = {
                Name: req.body.name,
                Breed: req.body.breed,
                Age: req.body.age,
                Sex: req.body.sex,
                CreatedTime: new Date().toISOString(),
                RowKey: uuidv4(),
                PartitionKey: "DOG"
            };
            
            context.bindings.tableBinding.push(dog);

            context.res = {
                status: 200,
                body: dog
            }

        } else {
            context.res = {
                status: 400,
                body: "Please pass a dog with all required properties in the request body: name, breed, age, sex"
            };
        }
    } else {
        context.res = {
            status: 400,
            body: "Please pass a dog in the request body"
        };
    }
};
```

Update the function.json to the below:

```json
{
  "bindings": [
    {
      "authLevel": "function",
      "type": "httpTrigger",
      "direction": "in",
      "name": "req",
      "methods": [
        "post"
      ],
      "route": "dogs"
    },
    {
      "type": "table",
      "direction": "out",
      "name": "tableBinding",
      "tableName": "dogs",
      "connection": "AZURE_STORAGE_CONNECTION_STRING"
    },
    {
      "type": "http",
      "direction": "out",
      "name": "res"
    }
  ]
}
```

### Creating GetAllDogs GET Method

Create the azure function below in a directory (in this example it is `GetAllDogs\index.js`):

```javascript
```

Update the function.json to the below:
```json
{
  "bindings": [
    {
      "authLevel": "function",
      "type": "httpTrigger",
      "direction": "in",
      "name": "req",
      "methods": [
        "get"
      ],
      "route": "dogs"
    },
    {
      "name": "dogEntity",
      "type": "table",
      "tableName": "dogs",
      "connection": "AZURE_STORAGE_CONNECTION_STRING",
      "direction": "in"
    },
    {
      "type": "http",
      "direction": "out",
      "name": "res"
    }
  ]
}
```

### Creating GetDog GET Method

Create the azure function below in a directory (in this example it is `CreateDog\index.js`):


```javascript
```

Update the function.json to the below:
```json
{
  "bindings": [
    {
      "authLevel": "function",
      "type": "httpTrigger",
      "direction": "in",
      "name": "req",
      "methods": [
        "post"
      ],
      "route": "dogs"
    },
    {
      "type": "table",
      "direction": "out",
      "name": "tableBinding",
      "tableName": "dogs",
      "connection": "AZURE_STORAGE_CONNECTION_STRING"
    },
    {
      "type": "http",
      "direction": "out",
      "name": "res"
    }
  ]
}
```

### Creating DeleteDog DELETE Method

TODO

Create the below azure function below:

```javascript
```

Update the function.json to the below:
```json
{
  "bindings": [
    {
      "authLevel": "function",
      "type": "httpTrigger",
      "direction": "in",
      "name": "req",
      "methods": [
        "post"
      ],
      "route": "dogs"
    },
    {
      "type": "table",
      "direction": "out",
      "name": "tableBinding",
      "tableName": "dogs",
      "connection": "AZURE_STORAGE_CONNECTION_STRING"
    },
    {
      "type": "http",
      "direction": "out",
      "name": "res"
    }
  ]
}
```

### Creating UpdateDog PUT Method

TODO

Create the below azure function below:

```javascript
```

Update the function.json to the below:
```json
{
  "bindings": [
    {
      "authLevel": "function",
      "type": "httpTrigger",
      "direction": "in",
      "name": "req",
      "methods": [
        "post"
      ],
      "route": "dogs"
    },
    {
      "type": "table",
      "direction": "out",
      "name": "tableBinding",
      "tableName": "dogs",
      "connection": "AZURE_STORAGE_CONNECTION_STRING"
    },
    {
      "type": "http",
      "direction": "out",
      "name": "res"
    }
  ]
}
```

## Deploying To Azure

TODO

## Supporting Containers

TODO
