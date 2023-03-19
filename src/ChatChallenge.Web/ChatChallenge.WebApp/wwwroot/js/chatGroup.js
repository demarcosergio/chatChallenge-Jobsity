document.addEventListener('DOMContentLoaded', function () {

    // Start the connection.
    var connection = new signalR.HubConnectionBuilder()
        .withUrl('/chat')
        .build();

    // Create a function that the hub can call to broadcast messages.
    connection.on('broadcastGroupMessage', function (chatMessageObj) {
        // Html encode display name and message.
        var messageDateTime = new Date(chatMessageObj.messageDateTime).toISOString().slice(0, 19).replace(/-/g, "/").replace("T", " ");
        var encodedName = chatMessageObj.userName;
        var encodedMsg = chatMessageObj.messageText;

        // Add the message to the page.
        var liElement = document.createElement('li');
        liElement.innerHTML = '<strong>' + messageDateTime + ' - ' + encodedName + '</strong>:&nbsp;&nbsp;' + encodedMsg;
        document.getElementById('messagesList').appendChild(liElement);
    });
    connection.on('broadcastGroupJoin', function (userNameObj) {
        // Add the message to the page.
        var liElement = document.createElement('li');
        liElement.style.cssText = "color: limegreen";
        liElement.innerHTML = '<p>' + userNameObj + '</p>';
        document.getElementById('usersInGroupList').appendChild(liElement);
    });
    connection.on('loadUsersInGroup', function (users) {
        var userList = document.getElementById('usersInGroupList');
        console.log("loadUsersInGroup");
        console.log(users);

        users.map(function (user) {
            var liElement = document.createElement('li');
            liElement.style.cssText = "color: limegreen";
            liElement.innerHTML = '<p>' + user.userName + '</p>';
            userList.appendChild(liElement);
        });
        //document.getElementById('divLoading').setAttribute("style", "display:none");
    });

    connection.on('loadGroupChatHistory', function (chatHistory) {
        var messageList = document.getElementById('messagesList');
        chatHistory.map(function (chatMessage) {
            var messageDateTime = new Date(chatMessage.messageDateTime).toISOString().slice(0, 19).replace(/-/g, "/").replace("T", " ");
            var encodedName = chatMessage.userName;
            var encodedMsg = chatMessage.messageText;
            var liElement = document.createElement('li');
            liElement.innerHTML = '<strong>' + messageDateTime + ' | ' + encodedName + '</strong>:&nbsp;&nbsp;' + encodedMsg;
            messageList.appendChild(liElement);
        });
        document.getElementById('divLoading').setAttribute("style", "display:none");
    });



    // Transport fallback functionality is now built into start.
    connection.start()
        .then(function () {
            console.log('connection started');
            connection.invoke('retriveGroupChatHistory', roomId.innerHTML);

            connection.invoke("RetriveListOfUsers", userName.innerHTML, roomId.innerHTML).catch(function (err) {
                return console.error(err.toString());

            });connection.invoke("JoinGroup", userName.innerHTML, roomId.innerHTML).catch(function (err) {
                return console.error(err.toString());
            });

            document.getElementById('sendButton').addEventListener('click', function (event) {
                connection.invoke("SendMessageToGroup", userName.innerHTML, roomId.innerHTML, messageInput.value).catch(function (err) {
                    return console.error(err.toString());
                });
                // Call the Send method on the hub.
                //connection.invoke('send', userName.innerHTML, messageInput.value);

                // Clear text box and reset focus for next comment.
                messageInput.value = '';
                messageInput.focus();
                event.preventDefault();
            });
        })
        .catch(error => {
            console.error(error.message);
        });
});