#   Binds REP socket to tcp://*:5555

import time
import zmq
import json
import paho.mqtt.client as mqtt

# MQTT broker info
broker_address= "localhost"
port = 1883
user = ""
password = ""

class Irida():
    def __init__(self, socket):

        self.socket = socket
        self.connected = False # check connection with broker

        self.service_data = {
                    "Hello" : "Irida"
                    }

        self.topics =   {
                    "result": "irida/test/data",
                }

        ##  --- MQTT client initialization --- ##
        self.client = mqtt.Client()                             # create a new instance
        self.client.username_pw_set(user, password=password)    #set username and password
        self.client.on_connect = self.on_connect                #attach function to callback
        self.client.on_message = self.on_message
        self.client.on_subscribe = self.on_subscribe
        self.client.on_publish = self.on_publish
        self.client.connect(broker_address, port=port)          #connect to broker
        self.client.loop_start()                                #start the loop

    def on_connect(self, client, userdata, flags, rc):
        if rc==0:
            client.connected_flag=True #set flag
            print("connected OK Returned code =", rc)
        else:
            print("Bad connection Returned code= ", rc)

    def on_subscribe(self, client, userdata, mid, granted_qos):
        pass

    def on_message(self, client, userdata, msg):
        # print(msg.topic)
        if msg.topic == "results":
            self.majority_votes = json.loads(msg.payload.decode("utf-8"))

        elif msg.topic == "answers":
            self.answers = json.loads(msg.payload.decode("utf-8"))
        else:
            pass

    def on_publish(self, client, userdata, result):             #create function for callback
        pass

    def disconnect(self):
        self.client.disconnect()
        self.client.loop_stop()

    def processing(self):
        print('Connection success. Waiting for message.')
        self.client.subscribe([("results", 0), ("answers", 0)])
        while True:
            # Wait for Unity to request update
            message = self.socket.recv()
            print("Received request: {}".format(message))

            # Based on the request, publish the appropriate topic
            # ret = self.client.publish("answer")
            ret = self.client.publish("result")
            time.sleep(0.5) # In future, replace this with checking if message list is empty

            #  Send reply back to client
            self.socket.send(json.dumps(self.majority_votes).encode('utf-8'))

def main():
    # Establish ZMQ connection with Unity client
    context = zmq.Context()
    socket = context.socket(zmq.REP)
    socket.bind("tcp://*:5555")

    irida = Irida(socket)
    try:
        irida.processing()
    except KeyboardInterrupt:
        irida.disconnect()

if __name__=="__main__":
    main()
