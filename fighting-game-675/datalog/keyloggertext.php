<?php
	ini_set("log_errors", 1);
	
	// Not really a secret at this point...
	$secretKey = "TheCakeIsALie";
	
	// Variables passed in to the script
	$time = $_GET['time'];
	$keyPress = $_GET['keyPress'];
	$playerName = $_GET['playerName'];
	$bbstate = $_GET['bbState'];
	
	// Each player gets their own log file
	$logfile = getcwd() . "/logs/" . $playerName . ".log";
	$myfile = fopen($logfile, 'a');
	
	$realHash = md5($time . $keyPress . $playerName . $secretKey);
	if($realHash == $_GET['hash'])
	{
		// Write to file
		file_put_contents($logfile, array($time, $keyPress, $playerName, $bbstate), FILE_APPEND);
		file_put_contents($logfile, "\n", FILE_APPEND);
		echo "Successfully wrote to " . $logfile;
	}
	else
	{
		echo "Invalid attempt to write to " . $logfile;
	}
	
	fclose($myfile);
?>