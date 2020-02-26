using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;
using Microsoft.WindowsAzure.Storage;

namespace AzureRESTAPIDogExample
{
    public static class DogAPI
    {
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
    }
}
