# Code Portfolio
## For Sean Davis

Welcome. This repository is intended to be an online code portfolio of some of my past work.

Folders:
* **Template Data Structures**: May 2016. This contains a collection of C++ header files. Each was written as a templated data structure, designed to replace the STL when accomplishing specific tasks.
* **Game Of Life**: April 2016. An early project of mine, this is a Windows Forms version of John Conway's Game Of Life: https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life
* **Money Manager**: June-August 2016. This was designed to assist with transaction tracking, financial help, and budgeting, similar to (but smaller than) Mint. It was my degree's midpoint project, made from scratch with one teammate with Agile-style development. We used Windows Forms with a C# backend, and a local SQLite database. I focused more on the C# and WF, but we each worked on everything. An Android version was added near the end, but it's just a demo, not a finished product.
* **Trading Sidekick GW2**: December 2016. This is an Android app, written in C# with Xamarin. The application takes user input and accesses the API endpoints of Guild Wars 2 (a MMORPG) to retrieve current in-game data, specifically the item trading post.
* **TCP Network Chat**: January 2017. This is a networked (Client-Server) chat application, built on top of an existing "game." The chatting and network messages are written on separate threads from the rest of the app, so they won't interrupt anything else. Up to four instances can be connected in one chatroom. Note: Not everything here is my work. I wrote the files: TCPChatClient(.h&.cpp) and TCPChatServer(.h&.cpp).
* **UDP Network Game**: January 2017. This contains Meatball Tennis, a basic version of pong. The game is networked (Client-Server) using UDP. Two instances can be connected for each game. Note: Again, not everything here is my work. My files are: /Client/Client(.h&.cpp) and /Server/Server(.h&.cpp).
* **Pathfinder AI**: February 2017. This is, as advertised, a pathfinding AI, using an A* algorithm to find a route from point to point. It can be loaded and run with multiple maps, and start/endpoints can be chosen. I wrote the algorithm, which is implemented on an existing pathing application. My files: /PathSearch/PathSearch(.h&.cpp).
* **Victorious Tournaments:** February-June 2017. This is a team project (3 developers) website, located at https://victorioustournaments.com/. The website is a tournament management platform. Users can create, customize, and progress their own tournaments, or view and join existing ones. I engineered the various bracket structures and behavior algorithms, with types including elimination, scoring, and Swiss.
* More to be added over time.
