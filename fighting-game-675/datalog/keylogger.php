<?php
	// Configuration
	$hostname = 'localhost';
	$username = getenv('DB_USERNAME');
	$password = getenv('DB_PASS');
	$database = getenv('DB_NAME');
	
	// Not really a secret at this point...
	$secretKey = getenv('SECRET_KEY');
	
	// Connect to database
	try {
		$dbh = new PDO('mysql:host='. $hostname .';dbname='. $database, $username, $password);
		
		// Variables passed in to the script
		$matchId = $_GET['matchId'];
		$time = $_GET['time'];
		$keyPress = $_GET['keyPress'];
		$playerName = $_GET['playerName'];
		
		$realHash = md5($matchId . $time . $keyPress . $playerName . $secretKey);
		if($realHash == $_GET['hash'])
		{
			$stmt = $dbh->prepare("INSERT INTO keypresses VALUES (:name, :value)");
			$stmt->bindParam(':name', $name);
			$stmt->bindParam(':value', $value);
			$sth = $dbh->prepare('INSERT INTO scores VALUES (null, :name, :score)');
			try {
				$sth->execute($_GET);
			}
			catch (Exception $e)
			{
				echo '<h1>An error has ocurred.</h1><pre>', $e->getMessage(), '</pre>';
			}
		}
	} catch(PDOException $e) {
		echo '<h1>An error has ocurred.</h1><pre>', $e->getMessage(), '</pre>';
	}
?>