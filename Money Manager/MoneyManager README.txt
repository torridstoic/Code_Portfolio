Money Manager

What This Is: Money Manager is an application designed to assist with money management, transaction tracking, and budgeting. It was created by me and one partner, from scratch, in a three-month development period for our midpoint project in our degree program.

Development: The backend structure is written in C#, the frontend GUI is a Windows Forms app, and the heavy data focus of the application is a local SQLite database: everything entered will be saved and recovered whenever the app is restarted. We followed a very agile development path, focusing on learning the process as we went, trying to gain some real-world experience through the process. One of our final goals was to port the app from WForms to Android, but we were unable to successfully integrate our existing database, so only a "demo" Android build was completed (in another branch).

How To Use: Simply build and run in VisualStudio. Make sure to unload MoneyManager.Forms. It was replaced by the newer MoneyManager.Forms.v2, which should be set as the startup project.

My Contributions: Each of us collabored on the entire project, though I did focus more on the C# and less on the SQL.