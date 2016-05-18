This contains all of the PHP files that handle requests made from Unity to a remote server.

# DATALOG

This directory contains PHP scripts that store BlackBoard states and inputs that a player with name <playername> makes in a file called <playername>.log. The format is standardized and set in Unity, and it is easily parseable in Python using a generator to separate out blocks and `json` to parse the BlackBoard state.

# SKILLTREES

This directory reads and stores skill trees that the players have created. It also sends skill trees to the game upon request.
