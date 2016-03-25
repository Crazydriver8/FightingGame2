<?php
	// Configuration
	$hostname = 'localhost';
	$username = getenv('DB_USERNAME');
	$password = getenv('DB_PASS');
	$database = getenv('DB_NAME');
	
	// Not really a secret at this point...
	$secretKey = getenv('SECRET_KEY');
	
	// Variables passed in to the script
	$time = $_GET['time'];
	$keyPress = $_GET['keyPress'];
	$playerName = $_GET['playerName'];
	$bbstate = $_GET['bbState']
	
	// Each player gets their own log file
	$logfile = getcwd() . "/logs/" . playerName . "_log.txt";
	
	$realHash = md5($time . $keyPress . $playerName . $secretKey);
	if($realHash == $_GET['hash'])
	{
		// Write to file
		file_put_contents($logfile, array($time, $keyPress, $playerName, $bbstate), FILE_APPEND);
	}
?>