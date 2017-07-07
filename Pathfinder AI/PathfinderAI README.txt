A* Pathfinder AI

What This Is: This is a pathfinder application. Given a map of "weighted" tiles, this will find the preferred route between two points (and visualize it, optionally).

How To Use: The easiest method is just to build & run through Visual Studio. Optionally, it can be run via commandline with arguments passed in. For optimal speed, turn off the #define DEBUG in PathSearch.cpp, build the solution in release mode, and run the generated exe file. This will, however, turn off visualization, so the search progression can't be watched.

My Contribution: The framework was provided. I wrote the search behavior and algorithms.

My Files: My work was all done in /PathSearch/PathSearch.cpp and PathSearch.h.
