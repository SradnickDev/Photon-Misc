<?php

try {

    include "connect.php";
    include "secure.php";

    $username = ($_POST['usernamePost']);
    $password = ($_POST['passwordPost']);

    $options = ['cost' => 12,];
    $pepper = $secure->random_string();
    $salt = $secure->get_salt();


    $passwordNew = password_hash($salt.$password.$pepper, PASSWORD_BCRYPT, $options);

    $stmt = $conn->prepare('SELECT * FROM accounts WHERE name=?');
    $stmt->bindParam(1, $username);
    $stmt->execute();
    $row = $stmt->fetch(PDO::FETCH_ASSOC);

    // If Account (DON'T EXIST) { CREATE IT! }.

    if (!$row) {
        if ($stmt = $conn->prepare("INSERT INTO accounts (name,password,pepper) VALUES (?,?,?)")) {
            $stmt->bindValue(1, $username);
            $stmt->bindValue(2, $passwordNew);
            $stmt->bindValue(3, $pepper);
            $stmt->execute();
        }
        echo "01";
    } else {
        echo "00";
    }
} catch (PDOException $e) {
    echo $sql . "</br>" . $e->getMessage();
}
$conn = null;
 