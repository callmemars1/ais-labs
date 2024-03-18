import socket
import threading
import argparse
import time
import logging
import signal
import sys

class Server:
    def __init__(self, host = '0.0.0.0', port = 5000, max_threads = 100, log_path = './'):
        self.host = host
        self.port = port
        self.max_threads = max_threads
        self.log_path = log_path
        self.server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.server.bind((self.host, self.port))
        self.server.listen()
        self.semaphore = threading.BoundedSemaphore(self.max_threads)
        logging.basicConfig(filename=f"{self.log_path}/server.log", level=logging.INFO)

        signal.signal(signal.SIGTERM, self.graceful_shutdown)
        signal.signal(signal.SIGHUP, self.reload_configuration)

    def handle_client(self, client_socket):
        with self.semaphore:
            request = client_socket.recv(1024).decode('utf-8')
            log = f"Time: {time.ctime(time.time())} | Request Body: {request}"
            print(log)
            logging.info(log)
            client_socket.close()

    def run(self):
        while True:
            client, _ = self.server.accept()
            client_handler = threading.Thread(target=self.handle_client, args=(client,))
            client_handler.start()

    def graceful_shutdown(self, signum, frame):
        print("Received SIGTERM. Shutting down...")
        self.server.close()
        sys.exit(0)

    def reload_configuration(self, signum, frame):
        print("Received SIGHUP. Shutting down...")
        self.server.close()
        sys.exit(0)

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("-p", "--port", help="define a port number", type=int, default=5000)
    parser.add_argument("-t", "--threads", help="max threads", type=int, default=100)
    parser.add_argument("-lp", "--logpath", help="path to save the logs", default='./')
    args = parser.parse_args()

    server = Server(port=args.port, max_threads=args.threads, log_path=args.logpath)
    server.run()
