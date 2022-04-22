"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/salesHub").build();

connection.on("startSales", function (id) {
    var productId = document.getElementById("objectId").getAttribute("value");
    if (productId == id) {
        var button = document.getElementById("buy")
        button.hidden = false
    }
});

connection.on("productDataChanged", function (product) {
    var productId = document.getElementById("objectId").getAttribute("value");
    if (productId == product.id) {
        document.getElementById("count").innerHTML = 'Количество: ' + product.count.toString();

        if (product.count <= 0) {
            document.getElementById("buy").hidden = true;
        }
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

document.getElementById('buy').addEventListener('click', function (e) {
    var productId = document.getElementById("objectId").getAttribute("value");
    connection.invoke("BuyProduct", productId).catch(function (err) {
        return console.error(err.toString())
    });
    event.preventDefault();
})