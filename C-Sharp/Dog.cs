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