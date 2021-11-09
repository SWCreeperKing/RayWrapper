the logger system is one of the most important systems in RayWrapper

if the game were to crash, the logger will auto save the log in the crash logs folder it auto creates when a crash occurs

there is an ingame console command that will allow the user to print the logger out to a file, it is `printstat`

the logger logs stuff 
- from raylib (albeit its a bit odd (numbers are crazy on it))
- from the user (as in from a console command)
- from the console

the logger is a static class, so it is very easy to use

`Logger.Log(logLevel, message);`

loglevel is an enum in the logger class and message is a string