"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/salesHub").build();

connection.start().then(function () {
    var productId = document.getElementById("objectId").getAttribute("value");
    connection.invoke("ListenProduct", productId).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
}).catch(function (err) {
    return console.error(err.toString());
});