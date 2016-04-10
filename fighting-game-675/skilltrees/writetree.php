<?php
	// Variables passed in to the script
	$json = $_GET['json'];
	$playerName = $_GET['playerName'];
	
	// Go to the /trees folder
	$treefile_path = getcwd() . "/trees";
	if (is_readable($treefile_path) == false)
	{
		mkdir($treefile_path);
	}
	chdir($treefile_path);
	
	// Each player gets their own log file
	$treefile_name = $playerName . "_skilltree.json";
	if (is_readable($treefile_name) == false)
	{
		touch($treefile_name);
		chmod($treefile_name, 0777);
	}
	
	// Write to file
	$treefile = fopen($treefile_name, "a+") or die('Failed to open file');
	
	fwrite($treefile, $json) or die("Failed to write file");
	
	fclose($treefile);
		
	// Confirmation mesaage
	echo $treefile_path . "/" . $treefile_name;
	echo $json;
?>