import asyncio
import random
import RPi.GPIO as GPIO
import datetime
import time
import uuid
from azure.iot.device.aio import IoTHubDeviceClient
from azure.iot.device import Message

movement_counter = 0
doorstate = "closed"
roomstate = "vacant"
timeMovementCounterStarted = datetime.datetime.now()
dateLastSeenMovement = datetime.datetime.now()

pin_motion = 5
pin_door = 6

GPIO.setmode(GPIO.BCM)
GPIO.setup(pin_motion,GPIO.IN)
GPIO.setup(pin_door, GPIO.IN)


#def sendTelemetryData():
    
async def checkDoorState(device_client):
    #forever asynchronously check:
    # get the reading of the gpio pin that refers to the reed switch
    # if that reading is different to the current state of the door
    # change the current state of the door and call the doorchangedstate method
    #globals
    global doorstate
    print("door State Running")
    while True:
        pin_door_state = GPIO.input(pin_door)
        temp_door_state = "closed"
        if pin_door_state != 1:
            temp_door_state = "opened"
        
        if temp_door_state != doorstate:
            doorstate = temp_door_state
            await doorChangedState(device_client)
        await asyncio.sleep(0.01)
            
            
        
    
async def isSomeoneInTheRoom(device_client):
      #take readings from the motion sensor every 0.02 seconds to determine movement
        # if movement happens start a movement counter which resets every 30 seconds
        # when the movement counter reaches x mark the room as occupied
        
        #globals
    global roomstate
    global movement_counter
    global timeMovementCounterStarted
    global dateLastSeenMovement
    global doorstate
    print("room checker running")
    while True:
        motion_detected = GPIO.input(pin_motion)
        if(motion_detected == 1):
            dateLastSeenMovement = datetime.datetime.now()
        if roomstate == "vacant":
            if(motion_detected == 1):
                print("motion detected")
                
                if movement_counter == 0:
                    timeMovementCounterStarted = datetime.datetime.now()
                movement_counter += 1
                
                if timeMovementCounterStarted < datetime.datetime.now() - datetime.timedelta(seconds = 30):
                    movement_counter = 0
                    timeMovementCounterStarted = datetime.datetime.now()
                    
                if movement_counter >= 3:
                    await update_room_status(device_client, "occupied")
        await asyncio.sleep(0.8)
        
async def send_telemetry_data(device_client, room_occupancy_status, time_since):
    msg = Message("{\"RoomStatus\" : \"" + room_occupancy_status +"\", \"TimeStampSince\": \""+ str(time_since) + "\"}")
    msg.message_id = uuid.uuid4()
    msg.correlation_id = "correlation-1234"
    await device_client.send_message(msg)
        
async def update_room_status(device_client, room_occupancy_status):
    global roomstate
    roomstate = room_occupancy_status
    print("the roomstate has now been changed to %s" % roomstate)
    await send_telemetry_data(device_client,room_occupancy_status, str(datetime.datetime.now()))
    
    
        
async def doorChangedState(device_client):
    # if doorstate is closed and the state changes to open while the rooms state is vacant
    # then the application should check for movement and mark the room as occupied
    
    #globals
    global doorstate
    global roomstate
    global dateLastSeenMovement
    
    
    
    if doorstate == "closed" and roomstate == "vacant" and await checkWithin5Seconds(dateLastSeenMovement):
        doorstate == "opened"
        await update_room_status(device_client, "occupied")
        #dateLastSeenMovement = datetime.datetime.now() - datetime.timedelta(seconds = 30)
        
    #if doorstate is closed and the state changes to open and the room is occupied
    #then the application should check for movement and mark the room as pending vacant and storing the time of the vacancy
    #however if movement is detected within 30 seconds then the application should mark the room as occupied
    if doorstate == "closed" and roomstate == "occupied" and await checkWithin5Seconds(dateLastSeenMovement):
        doorstate = "opened"
        await update_room_status(device_client ,"vacant")
        #dateLastSeenMovement = datetime.datetime.now() - datetime.timedelta(seconds = 30)
        
    print("doorstate was changed to %s" % doorstate)
        
        
async def checkWithin5Seconds(date_time):
    #check if date_time is > now -5 but < now + 5
    now = datetime.datetime.now()
    x = 5
    if(date_time > (now - datetime.timedelta(seconds=x))):
        return True
    return False

    

async def main():
    
    conn_str = "HostName=tayloriot.azure-devices.net;DeviceId=device-01;SharedAccessKey=WEIxSYOPNl2+zK58JYvuY6SCTkoyh0lMtBPG18SIZCs="
    device_client = IoTHubDeviceClient.create_from_connection_string(conn_str)
    await device_client.connect()

# asynchronous method that runs in an infinite loop asynchronously to check for messages incoming to this device
    async def message_listener(device_client):
        print("message_listener running")
        while True:
            message = await device_client.receive_message()
            print("the data in the message received was")
            print(message.data)
            print("custom properties are")
            print(message.custom_properties)
            await asyncio.sleep(0.01)

    async def twin_patch_listener(device_client):
        while True:
            patch = await device_client.receive_twin_desired_properties_patch()
        print("the data in the desired properties was: {}".format(patch))


    def UpdateTwinReportedProperties(device_client):
        reported_properties = {"temperature":random.randint(320, 800) / 10}
        print("setting property")
        loop = asyncio.get_event_loop()
        loop.run_until_complete(device_client.patch_twin_reported_properties(reported_properties))

    def stdin_listener():
        while True:
            selection = input("Press Q to quit\n type twin to update device twin\n")
            if selection == "Q" or selection == "q":
                print("Quitting")
                break
            elif selection == "twin":
                print("updating twin")
                UpdateTwinReportedProperties(device_client)

    loop = asyncio.get_event_loop()
    loop.create_task(message_listener(device_client))
    loop.create_task(isSomeoneInTheRoom(device_client))
    loop.create_task(checkDoorState(device_client))
    
    user_finished = loop.run_in_executor(None, stdin_listener)

    await user_finished
    print(loop.is_closed())

    await device_client.disconnect()

if __name__ == "__main__":
    loop = asyncio.get_event_loop()
    loop.create_task(main())
    loop.run_forever()
