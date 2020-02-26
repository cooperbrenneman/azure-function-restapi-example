module.exports = async function (context, req) {
    context.log('Starting GetDogById function...');

    if(context.bindings.tableBinding){
        context.log(context.bindings.tableBinding);
        context.res = {
            status: 200,
            body: context.bindings.tableBinding
        }
    } else {
        context.res = {
            status: 404,
            body: "Unable to find specified item with id: " + req.params.id
        }
    }

};