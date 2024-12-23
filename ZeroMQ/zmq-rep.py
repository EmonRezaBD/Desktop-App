import zmq

context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://127.0.0.1:5555")

message = socket.recv_json()
print("Received request: %s" % message)

socket.close()
context.term()