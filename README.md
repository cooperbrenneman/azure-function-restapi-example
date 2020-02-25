# Azure Function REST API Example

Example project for using Azure functions for a REST API for C# and Javascript.

## Overview

In this example, you will be creating a set of Azure Functions

### Notes on Languages

In this tutorial, there are two languages that will be supported:

- C#
- Javascript

In some cases, the steps used to configure these Azure Functions will differ because of the languages. In those cases, there will language specific notes to ensure that the correct steps can be followed.

## Configuring Function App Environment

### Visual Studio Code

You will need to download and install Visual Studio Code and the Azure Functions Extension for Visual Studio Code. You will be using both of them to create the Azure Function Apps, develop locally, and deploy them to Azure.

### Creating the Azure Function

Go to the Azure button in Visual Studio Code and locate the "Functions" section. Select "Create New Project...", select the location you want to use for your rest api.

Then you will need to select your language (either C# or Javascript). Then, select the "HttpTrigger", name the project (which will become your namespace), and select a specific namespace, and finally select the "Function" access.

Note: If you selected C#, you will need to download the corresponding .NET packages and install them acccordingly. You may also have to restore the packages as well.
Note: If you selected Node.js, you will need to download the corresponding Node.js packages and install them accordingly.

### Installing Additional Packages

In this tutorial, you will also have to install the latest packages depending on the Function version.

For example, if using Functions 2.x and higher and installing on Visual Studio Code you can run the below:

```shell
dotnet add package Microsoft.Azure.WebJobs.Extensions.Storage
```

More information can be found [here](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-table?tabs=csharp).

## Routing Overview

We will have the below routing for our application:

| HTTP Method | Route     | Function   | Description                 |
|-------------|-----------|------------|-----------------------------|
| GET         | /dogs     | GetAllDogs | Get all dogs                |
| GET         | /dogs/:id | GetDogById | Get a dog based on an id    |
| POST        | /dogs     | CreateDog  | Creates a new dog           |
| DELETE      | /dogs/:id | DeleteDog  | Delete a dog based on an id |
| PUT         | /dogs/:id | UpdateDog  | Update a dog based on an id |

## Creating a Dog Model Class

If you are using C#, a great thing to do is create a Dog.cs file that you will use to declare the Dog Model that the API will be working with. This will greatly simplify validating data from the API in the C# case. Make sure that your namespace is updated to contain the your exact namespace.

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

TODO: Description of Dog.cs

## Creating CreateDog POST Method

TODO

Create the below azure function below:

```cs
```

## Creating GetAllDogs GET Method

TODO

Create the below azure function below:

```cs
```

## Creating GetDog GET Method

TODO

Create the below azure function below:

```cs
```

## Creating DeleteDog DELETE Method

TODO

Create the below azure function below:

```cs
```

## Creating UpdateDog PUT Method

TODO

Create the below azure function below:

```cs
```

## Creating Table Storage

### Creating Table Storage Resource

You can go to the Azure portal and create a new Storage account (blob, file, table, queue). The overview can be found [here](https://docs.microsoft.com/en-us/azure/storage/common/storage-account-overview).

### Adding Connection String

Once created, navigate to the newly created resource.

Go to `Access keys` -> key1 `Connection string`, and copy this value into your `local.settings.json` file:

```json
{
  "Values": {
    "AzureWebJobsStorage": "<Paste-Your-Connection-String-Here>",
}
```

## Deploying To Azure

TODO

## Supporting Containers

TODO

## Notes on Node.js

TODO
