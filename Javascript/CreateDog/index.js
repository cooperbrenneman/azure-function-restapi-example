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