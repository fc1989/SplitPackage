HOST='192.168.1.24'
PORT=3306
while :
do
    (echo >/dev/tcp/$HOST/$PORT) >/dev/null 2>&1
    result=$?
    if [[ $result -eq 0 ]]; then
        break
    fi
	echo 'sleep 1'
    sleep 1
done
exec "/project/command.sh"
