Network Chat App

What This Is: This is a networked (Client-Server) chat application, built on top of an existing "game." The chatting and network messages are written on separate threads from the rest of the app, so they won't interrupt anything else.

How To Use: Multiple instances must be run. In the first instance, select "HOST:" this will be the Client-Server. In the others (up to 4 total), select "JOIN:" clients. Each instance has a small textbox at the very bottom: anything typed in here and sent will be visible to all other connected clients.

My contribution: The background application was already built. My job was to code the server and clients, using Sockets to send and receive data from each other. The messages were parsed/decoded and handled, though my only interaction with the UI was sending it data.

My files: /GNWLabSuite/TCPChatClient.cpp and /GNWLabSuite/TCPChatServer.cpp (with edits to their associated .h files).