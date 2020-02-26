var azure = require('azure-storage');

module.exports = async function (context, req) {
    context.log('Starting DeleteDog function...');
    
    if(req.params.id){

        var tableService = azure.createTableService();

        var entGen = azure.TableUtilities.entityGenerator;

        var entity = {
            PartitionKey: entGen.String("DOG"),
            RowKey: entGen.String(req.params.id)
        }

        tableService.deleteEntity("dogs", entity, (err, result, response) => 
        {
            if(!err){
                context.res = {
                    status: 200,
                    body: "Dog deleted with id: " + req.params.id
                };
            } else {
                context.res = {
                    status: 502,
                    body: "Unable to delete the entry"
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