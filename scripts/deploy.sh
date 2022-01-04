host="your_host";
user="your_user";
password='your_password';

echo "Deploy start"

echo "Start kill Bot tmux session"
sshpass -p $password ssh -t $user@$host << EOF
cd ~/Bot/publish
tmux has-session -t 'Bot' 2>/dev/null
if [ $? -eq 0 ]; then
	tmux kill-ses -t 'Bot'
fi
EOF
echo "Bot tmux session was killed"

echo "Start copy files"
sshpass -p $password scp -r ../src/Bot/bin/publish/ $user@$host:~/Bot
echo "Files have copied"

echo "Start new tmux Bot session"
sshpass -p $password ssh -t $user@$host << EOF
cd ~/Bot/publish
tmux new -d -s 'Bot' 'chmod 777 ./Bot; ./Bot'
EOF
echo "New tmux Bot session was started"

echo "Deploy finish"