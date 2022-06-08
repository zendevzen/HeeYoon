import socket

HOST='127.0.0.1'
PORT=8001
BUFFER_SIZE = 1024
isconnect = False

def connect_socket():
    global isconnect
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    server_socket.bind((HOST, PORT))
    server_socket.listen()

    global client_soket

    client_soket, addr = server_socket.accept()
    print('connected by', addr)

    isconnect = True

def send_socket(str):
    global isconnect
    try:
        if isconnect == False:
            connect_socket()
        client_soket.sendall(str.encode())
        
    except:
        print("센드소켓 except")
        isconnect = False
        client_soket.close()
        connect_socket()


