Victorious Tournaments

What This Is: Victorious was developed by a team of three students, including me, over a period of five months. This is a tournament management web application. We designed it specifically for tournament administrators, to assist with their jobs, but it can be used by a very wide-ranging audience. To simplify the scope as much as possible: the application assists in the creation, modification, and customization of tournaments. As the tournament begins, the admin can enter match results, which will automatically update the rest of the tournament as necessary. It also currently includes basic "participant/player" functionality, allowing players to register & check-in to tournaments.

Development: The website used a variety of languages and tools, including C#/.NET, Java, ASP, jQuery, LESS/CSS, and JSON. The database was developed with Entity Framework. The algorithms for the bracket structures and behavior were written in C#. Unit testing was also done in C#, with Moq.

How To Use: Our latest release is publicly available at https://victorioustournaments.com/.

My Contributions: As a team, we separated our responsibilities during this project. One person concentrated on the web development, one person on database development, and I engineered and tested the bracket algorithms (/Tournament.Structure/ and /Tournament.Structure.Tests/).