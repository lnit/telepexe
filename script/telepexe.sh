ROOT_DIR=\/\home\/
FILE=$(cd $(dirname "$1") && pwd)/$(basename "$1")
echo $FILE | sed -e "s|$ROOT_DIR||g" | nc 127.0.0.1 6900
