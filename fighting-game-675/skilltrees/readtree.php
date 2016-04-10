<?php
	// Variables passed in to the script
	$playerName = $_GET['playerName'];
	
	// Go to the /trees folder
	$treefile_path = getcwd() . "/trees";
	if (is_readable($treefile_path) == false)
	{
		echo "No tree available";
	}
	else
	{
		chdir($treefile_path);
		
		// Each player gets their own log file
		$treefile_name = $playerName . "_skilltree.json";
		if (is_readable($treefile_name) == false)
		{
			echo "No tree available";
		}
		else
		{
			// Read from file and output it to page
			$treefile = fopen($treefile_name, "r");
			echo fread($treefile, filesize($treefile_name));
			
			fclose($treefile);
		}
	}
?>