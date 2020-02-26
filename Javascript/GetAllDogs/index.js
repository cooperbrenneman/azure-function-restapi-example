module.exports = async function (context, req) {
    context.log('JavaScript HTTP trigger function processed a request.');

    var dogs = context.bindings.dogEntity;

    if (dogs) {
        context.res = {
            body: dogs
        };
    }
    else {
        context.res = {
            body: "Unable to find any dogs"
        };
    }
};