# Constants

A static class containing definitions for moves and BlackBoard states for the game

# PostDataToServer

This is an implementation of a data-logging queue
is 
Each player gets their own queue of WWW objects. As objects are enqueued, they are dequeued to be sent to the server as quickly as possible. This system prevents overlapping write operations from splitting logs. An example of the error that was present before this implementation is shown below:

```
Player1
99
Button1
None
```

and

```
Player1
99
BlackBoard Update
{...}
```

were mixed in the log file as follows...

```
Player1
99
BlackBoard Update
Player1
99
Button1
{...}
None
```

With this script, mixing of logs does not occur, as the coroutine waits for a server response before sending the next block. Presumably, since Player1 logs and Player2 logs are not written to the same location, it is safe to send 2 blocks of data for 2 separate players simultaneously.
