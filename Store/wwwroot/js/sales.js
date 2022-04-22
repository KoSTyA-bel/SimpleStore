﻿"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/salesHub").build();

connection.on("ReceiveMessage", function (id) {
    var li = document.createElement("li")
    document.getElementById("fromHub").appendChild(li)
    li.textContent = id;
});

connection.on("startSales", function (product) {
    var productId = document.getElementById("objectId").getAttribute("value");
    if (productId == product.id) {
        var button = document.getElementById("buy")
        button.hidden = false
    }
});

connection.logging = true;

connection.start().then(function () {
    var productId = document.getElementById("objectId").getAttribute("value");
    connection.invoke("ListenProduct", productId).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
}).catch(function (err) {
    return console.error(err.toString());
});