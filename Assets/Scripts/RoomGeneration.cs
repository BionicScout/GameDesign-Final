using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class RoomGeneration : MonoBehaviour {
    int[,] roomLayout;
    public int gridSize;
    public int howManyRooms;
    public int floorSize;

    int[,] baseDirections = { {-1, 0} , {0, -1}, {1, 0}, {0, 1} }; //[0 Up, 1 Left, 2 Down, 3 Right   ,   0 x, 1 y]

    public GameObject roomTilePrefab;
    public GameObject floorTilePrefab;
    public FloorGrid grid;
    public Material doorMat;

    public struct RoomInfo {
        public int x, y;
        public bool[] directions; //0 Up, 1 Left, 2 Down, 3 Right. //True means room is in that direction
        public int doorsLeft;
        public int previousRoom; //The id of the room used to generate this one. This determines door ways
        public int previousRoomDir; //Base Directions
        public bool[] doorDirections; //True when door is in direction

        public void set(int xCoord, int yCoord) {
            x = xCoord; 
            y = yCoord;

            directions = new bool[4];
            directions[0] = true;
            directions[1] = true;
            directions[2] = true;
            directions[3] = true;

            doorsLeft = 4;

            previousRoom = -1;
            previousRoomDir = -1;

            doorDirections = new bool[4];
            doorDirections[0] = false;
            doorDirections[1] = false;
            doorDirections[2] = false;
            doorDirections[3] = false;
        }

        public string print() {
            string text = "";
            text += "X: " + x + "\t";
            text += "Y: " + y + "\n";

            text += "Up: " + directions[0] + "\t";
            text += "Left: " + directions[1] + "\t";
            text += "Down: " + directions[2] + "\t";
            text += "Right: " + directions[3] + "\n";

            text += "Doors Left: " + doorsLeft + "\n";

            return text;
        }
    }

    public void generate() {
        //Set Grid values to -1. This represents no room there
        roomLayout = new int[gridSize , gridSize];
        setToNegative1();

        //Create List of Rooms that are generated and create a room in the middle of the map
        List<RoomInfo> rooms = new List<RoomInfo>();

        int middle = Mathf.FloorToInt(gridSize / 2f);
        RoomInfo middleRoom = new RoomInfo();
        middleRoom.set(middle, middle);
        roomLayout[middle , middle] = 0;
        rooms.Add(middleRoom);

        //Generate Rooms
        int roomsToGen = howManyRooms;

        while(roomsToGen > 0) {
        //Get Room with doors
            int roomIndex = Random.Range(0, rooms.Count);

            if(rooms[roomIndex].doorsLeft == 0)
                continue;

            //Pick Direction of Room
            int newRoomDirection = pickDirection(rooms[roomIndex]);
            rooms[roomIndex].doorDirections[newRoomDirection] = true;

            //Place Room
            rooms = createRoom(rooms, roomIndex, newRoomDirection);

            roomsToGen--;
        }

        generatetiles(rooms);
    }

    public int pickDirection(RoomInfo room) {
        int direction = Random.Range(0 , 4) % 4;
        bool openDirection = room.directions[direction];

        while(!openDirection) {
            direction = (direction + 1) % 4;
            openDirection = room.directions[direction];
        }

        return direction;
    }

    public List<RoomInfo> createRoom(List<RoomInfo> rooms, int baseRoomIndex, int newRoomDir) {
        //Create the new room
        RoomInfo newRoom = new RoomInfo();

        newRoom.set(rooms[baseRoomIndex].x + baseDirections[newRoomDir , 0] , rooms[baseRoomIndex].y + baseDirections[newRoomDir , 1]);
        newRoom.previousRoom = baseRoomIndex;
        newRoom.previousRoomDir = (newRoomDir + 2) % 4;
        newRoom.doorDirections[(newRoomDir + 2) % 4] = true;
        rooms.Add(newRoom);
        roomLayout[newRoom.x , newRoom.y] = rooms.Count - 1;

        //Check The new room for near by rooms
        for(int i = 0; i < baseDirections.GetLength(0); i++) {
            int adjRoom_X = newRoom.x + baseDirections[i , 0];
            int adjRoom_Y = newRoom.y + baseDirections[i , 1];

            //Check if adjRoom direction goes of the grid.
            if(adjRoom_X < 0 || adjRoom_Y < 0 || adjRoom_X >= gridSize || adjRoom_Y >= gridSize)
                continue;

            //Get Room Id and continue loop if a room does not exsit
            int roomId = roomLayout[adjRoom_X , adjRoom_Y];

            if(roomId == -1)
                continue;

            //In adjacent room, update to show a new room next to it
            RoomInfo adjRoom = rooms[roomId];
            adjRoom.doorsLeft = adjRoom.doorsLeft - 1;
            adjRoom.directions[(i + 2) % 4] = false;
            rooms[roomId] = adjRoom;

            //In New Room, update info in adjacent room direction
            newRoom.doorsLeft--;
            newRoom.directions[i] = false;
        }

        rooms[rooms.Count - 1] = newRoom;

        return rooms;
    }

    public void generatetiles(List<RoomInfo> rooms) {
        GameObject parent = new GameObject("Room Map");
        grid.size = floorSize;
        grid.grid = new FloorTile[grid.width , grid.height];

        for(int i = 0; i < rooms.Count; i++) {
            RoomInfo room = rooms[i];

            float middle = Mathf.FloorToInt(gridSize / 2f);
            GameObject obj = Instantiate(roomTilePrefab , new Vector3((room.x - middle) * grid.size , (room.y - middle) * grid.size , -1) , Quaternion.identity);
            obj.transform.SetParent(parent.transform);
            obj.transform.localScale *= grid.size;

            obj.AddComponent<FloorGrid>();
            FloorGrid floor = obj.GetComponent<FloorGrid>();
            floor.width = floorSize;
            floor.height = floorSize;
            floor.size = 1;
            floor.tile = floorTilePrefab;
            floor.generateEmpty(obj);

            for(int dirIndex = 0; dirIndex < baseDirections.GetLength(0); dirIndex++) {
                if(room.doorDirections[dirIndex]) {
                    Renderer doorEdge = obj.transform.GetChild(1).GetChild(dirIndex).gameObject.GetComponent<Renderer>();
                    doorEdge.material = doorMat;
                }
            }



        }
    }

    public void setToNegative1() {
        for(int x = 0; x < gridSize; x++) {
            for(int y = 0; y < gridSize; y++) {
                roomLayout[x , y] = -1;
            }
        }
    }

    public void print(string fileName, List<RoomInfo> rooms) {
        string text = "";

        for(int x = 0; x < gridSize; x++) {
            for(int y = 0; y < gridSize; y++) {
                text += roomLayout[x , y].ToString() + "\t";
            }
            text += "\n";
        }

        text += "\n\n\n";

        for(int x = 0; x < gridSize; x++) {
            for(int y = 0; y < gridSize; y++) {
                if(roomLayout[x, y] != -1)
                    text += rooms[roomLayout[x , y]].doorsLeft.ToString() + "\t";
                else
                    text += "-1" + "\t";
            }
            text += "\n";
        }

        text += "\n\n\n";

        for(int i = 0; i < rooms.Count; i++) {
            text += "Room " + i + ":\n" + rooms[i].print() + "\n";
        }

        StreamWriter writer1 = new StreamWriter("TestRoom" + fileName + ".txt" , false);
        writer1.Write("");
        writer1.Close();
        StreamWriter writer = new StreamWriter("TestRoom" + fileName + ".txt" , true);
        writer.Write(text);
        writer.Close();
    }
}
