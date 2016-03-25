<?php
	// Not really a secret at this point...
	$secretKey = "TheCakeIsALie";
	
	// Variables passed in to the script
	$time = $_GET['time'];
	$keyPress = $_GET['keyPress'];
	$playerName = $_GET['playerName'];
	$bbState = $_GET['bbState'];
	
	// Go to the /logs folder
	$logfile_path = getcwd() . "/logs";
	if (is_readable($logfile_path) == false)
	{
		mkdir($logfile_path);
	}
	chdir($logfile_path);
	
	// Each player gets their own log file
	$logfile_name = $playerName . ".log";
	if (is_readable($logfile_name) == false)
	{
		touch($logfile_name);
		chmod($logfile_name, 0777);
	}
	
	$realHash = md5($time . $keyPress . $playerName . $secretKey);
	if($realHash == $_GET['hash'])
	{
		// Write to file
		$logfile = fopen($logfile_name, "a+");
		
		fwrite($logfile, $playerName . "\n");
		fwrite($logfile, $time . "\n");
		fwrite($logfile, $keyPress . "\n");
		fwrite($logfile, $bbState . "\n\n");
		
		fclose($logfile);
		
		// Confirmation mesaage
		echo $logfile_path . "/" . $logfile_name;
	}
	else
	{
		echo "Invalid attempt to write to " . $logfile_path . "/" . $logfile_name;
	}
?>