host="your_host";
user="your_user";
password='your_password';

echo "Deploy start"

echo "Start copy files"
sshpass -p $password scp -r ../src/Bot/bin/publish/ $user@$host:/home/sport/Bot
echo "Files have copied"

echo "Start Bot reboot"
sshpass -p $password ssh -t $user@$host << EOF
cd /home/sport/Bot/publish

tmux has-session -t 'Bot' 2>/dev/null
if [ $? -eq 0 ]; then
	tmux kill-ses -t 'Bot'
fi
tmux new -d -s 'Bot' 'chmod 777 ./Bot; ./Bot'

bash -l
EOF
echo "Bot was rebooted"

echo "Deploy finish"