var azure = require('azure-storage');

module.exports = async function (context, req) {
    context.log('Starting UpdateDog function...');
    
    if(req.params.id){

        var tableService = azure.createTableService();

        tableService.retrieveEntity("dogs", "DOG", req.params.id, (err, result, response) => {
            if(!err){
                entity = result;

                if(req.body){
                    if(req.body.name){
                        entity.Name = req.body.name;
                    }
                    if(req.body.age){
                        entity.Age = req.body.age;
                    }
                    if(req.body.sex){
                        entity.Sex = req.body.sex;
                    }
                    if(req.body.breed){
                        entity.Breed = req.body.breed;
                    }
    
                    tableService.insertOrMergeEntity("dogs", entity, (err, result, response) => 
                    {
                        if(!err){
                            context.res = {
                                status: 200,
                                body: "Updated the entry with id " + req.params.id
                            };
                        } else {
                            context.res = {
                                status: 502,
                                body: "Unable to update the entry"
                            };
                        }
                    });
                } else {
                    context.res = {
                        status: 400,
                        body: "Must supply at least one value to update in the body of the request"
                    };
                }
            } else {
                context.res = {
                    status: 502,
                    body: "Unable to update the entry"
                };
            }
        });
    } else {
        context.res = {
            status: 400,
            body: "Please pass an id in the url"
        };
    }
};