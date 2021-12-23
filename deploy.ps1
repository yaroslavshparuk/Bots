$address=$args[0]
$user=$args[1]
$password=$args[2]
pscp -pw $password -r src\Bot\bin\publish\ $user@$($address):/home/$user/Bot