package main

import (
	"bytes"
	"io"
	"log"
	"net"
	"os"
)

const SockAddr = "/var/my.sock"

func echoServer(c net.Conn) {
	var buf bytes.Buffer
	io.Copy(&buf, c)
	// log.Printf("Client connected [%s]", c.RemoteAddr().Network())
	// r := bufio.NewReader(c)
	// _, _, _ = r.ReadLine()
	// log.Printf("Recieved [%s]", line)
	c.Close()
}

func main() {
	if err := os.RemoveAll(SockAddr); err != nil {
		log.Fatal(err)
	}

	l, err := net.ListenUnixgram("unixgram", &net.UnixAddr{SockAddr, "unixgram"})
	if err != nil {
		log.Fatal("listen error:", err)
	}
	defer l.Close()

	log.Printf("Listen [%s]", "unixgram")

	for {
		// Accept new connections, dispatching them to echoServer
		// in a goroutine.
		var buf [1024]byte
		_, err := l.Read(buf[:])
		// conn, err := l.Accept()
		if err != nil {
			log.Fatal("accept error:", err)
		}
		// fmt.Printf("%s\n", string(buf[:n]))
		// go echoServer(conn)
	}
}
