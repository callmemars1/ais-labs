import socket
import argparse

def send_file_to_server(file_path, server_host = '0.0.0.0', server_port = 5000):
    client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    client.connect((server_host, server_port))

    with open(file_path, 'r') as file:
        data = file.read()

    client.send(data.encode())
    client.close()

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("-f", "--filepath", help="path of the file to send", required=True)
    parser.add_argument("-a", "--address", help="server address", default='0.0.0.0')
    parser.add_argument("-p", "--port", help="server port", type=int, default=5000)
    args = parser.parse_args()

    send_file_to_server(args.filepath, args.address, args.port)
