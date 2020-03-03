<?php

$secure = new secure();

class secure
{
    public function get_salt()
    {
        return "d41d8cd98f00b2";
    }

    public function random_string()
    {
        $characters = '0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ';
        $retVal = "";
        for ($i = 0; $i < 10; $i++) {
            $idx = rand(0, strlen($characters) - 1);
            $retVal .= $characters[$idx];
        }
        return $retVal;
    }
}

?>