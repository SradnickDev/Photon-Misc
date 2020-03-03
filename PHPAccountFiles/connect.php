<?php
$host_name = 'host';
$database = 'db';
$user_name = 'user';
$password = 'pw';

$conn = null;
try {
  $conn = new PDO("mysql:host=$host_name; dbname=$database;", $user_name, $password);
} catch (PDOException $e) {
  echo "Fehler!: " . $e->getMessage() . "<br/>";
  die();
}
?>
