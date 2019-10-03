// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var connection = new signalR.HubConnectionBuilder().withUrl("/roomdetector").build();

connection.on("UpdateData", function (data) {
    console.log(data);
    var name = data.name.toLowerCase().replaceAll(" ", "-");
    $(".js-" + name + " .js-roomstatus").html(data.roomStatus);
    $(".js-" + name + " .js-timestamp").html(data.timeStampSince.toString());

});

connection.start().catch(function (err) {
    return console.error(err.toString());
});

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};