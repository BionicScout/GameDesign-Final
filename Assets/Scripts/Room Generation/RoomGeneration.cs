using JetBrains.Annotations;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
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
    //public FloorGrid grid;
    public Material doorMat;

    RoomManager roomManager;

    public struct RoomInfo {
        public int x, y;
        public bool[] directions; //0 Up, 1 Left, 2 Down, 3 Right. //True means room is in that direction
        public int doorsLeft;
        public int previousRoom; //The id of the room used to generate this one. This determines door ways
        public bool[] doorDirections; //True when door is in direction
        public bool roomHasInstru;
        public bool roomHasHealPotion;
        public bool roomHasCrankPotion;
        public bool roomHasTeleport;

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

            doorDirections = new bool[4];
            doorDirections[0] = false;
            doorDirections[1] = false;
            doorDirections[2] = false;
            doorDirections[3] = false;

            roomHasInstru = false;
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
        


        while (roomsToGen > 0) {
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

        rooms = reduceGrid(rooms);
        generatetiles(rooms);

        //generates one instrument in a room
        int instruNeedSpawn = 10;
        for(int i = 0; i < instruNeedSpawn; i++)
        {
            int roomRan = Random.Range(0, rooms.Count);
            FloorTile Tile = roomManager.roomList[roomRan].GetComponent<Room>().floor.GetRandTile();
            Tile.transform.GetChild(2).gameObject.SetActive(true);
            Tile.HasInstru = true;
        }
        //spawns an enimies
        int enimiesNeedSpawn = 15;
        for (int i = 0; i < enimiesNeedSpawn; i++)
        {
            int roomRan = Random.Range(0, rooms.Count);
            FloorTile Tile = roomManager.roomList[roomRan].GetComponent<Room>().floor.GetRandTile();
            Tile.transform.GetChild(3).gameObject.SetActive(true);
            Tile.HasEnemy = true;
            FindObjectOfType<PlayerMovement>().enemyTiles.Add(Tile);
        }
        //spawns health potions
        int healPotionNeedSpawn = 5;
        for (int i = 0; i < healPotionNeedSpawn; i++)
        {
            int roomRan = Random.Range(0, rooms.Count);
            FloorTile Tile = roomManager.roomList[roomRan].GetComponent<Room>().floor.GetRandTile();
            Tile.transform.GetChild(6).gameObject.SetActive(true);
            Tile.hasHealPotion= true;
        }
        //spawns Crank reduce potions
        int crankPotionNeedSpawn = 5;
        for (int i = 0; i < crankPotionNeedSpawn; i++)
        {
            int roomRan = Random.Range(0, rooms.Count);
            FloorTile Tile = roomManager.roomList[roomRan].GetComponent<Room>().floor.GetRandTile();
            Tile.transform.GetChild(7).gameObject.SetActive(true);
            Tile.hasCrankPotion = true;
        }
        //spawns teleport 
        int teleportNeedSpawn = 3;
        for (int i = 0; i < teleportNeedSpawn; i++)
        {
            int roomRan = Random.Range(0, rooms.Count);
            FloorTile Tile = roomManager.roomList[roomRan].GetComponent<Room>().floor.GetRandTile();
            Tile.transform.GetChild(5).gameObject.SetActive(true);
            Tile.hasTeleport = true;
        }







        roomManager.hide(true);

    }

    public void setToNegative1() {
        for(int x = 0; x < gridSize; x++) {
            for(int y = 0; y < gridSize; y++) {
                roomLayout[x , y] = -1;
            }
        }
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

    public List<RoomInfo> reduceGrid(List<RoomInfo> rooms) {
        //
        int minWidth = 0, maxWidth = gridSize;
        int minHeight = 0, maxHeight = gridSize;

        //Find min width
        for(int x = 0; x < gridSize; x++) {
            bool trigger = false;

            for(int y = 0; y < gridSize; y++) {
                if(roomLayout[x, y] != -1) {
                    trigger = true;
                    break;
                }
            }

            if(trigger) {
                minWidth = x;
                break;
            }
        }

        //Find max width
        for(int x = gridSize - 1; gridSize > 0; x--) {
            bool trigger = false;

            for(int y = 0; y < gridSize; y++) {
                if(roomLayout[x , y] != -1) {
                    trigger = true;
                    break;
                }
            }

            if(trigger) {
                maxWidth = x + 1;
                break;
            }
        }

        //Find min height
        for(int y = 0; y < gridSize; y++) {
            bool trigger = false;

            for(int x = 0; x < gridSize; x++) {
                if(roomLayout[x , y] != -1) {
                    trigger = true;
                    break;
                }
            }

            if(trigger) {
                minHeight = y;
                break;
            }
        }

        //Find max width
        for(int y = gridSize - 1; gridSize > 0; y--) {
            bool trigger = false;

            for(int x = 0; x < gridSize; x++) {
                if(roomLayout[x , y] != -1) {
                    trigger = true;
                    break;
                }
            }

            if(trigger) {
                maxHeight = y + 1;
                break;
            }
        }

        //Reduce
        int[,] reduced = new int[maxWidth - minWidth, maxHeight - minHeight];

        for(int x = 0; x < reduced.GetLength(0); x++) {
            for(int y = 0; y < reduced.GetLength(1); y++) {
                //Debug.Log(x + " " + y + "\n" + (x + minWidth) + " " + (y + minHeight));
                reduced[x, y] = roomLayout[x + minWidth, y + minHeight];

                int roomId = reduced[x , y];

                if(roomId != -1) {
                    RoomInfo room = rooms[roomId];
                    room.x = x;
                    room.y = y;
                    rooms[roomId] = room;
                }
            }
        }

        roomLayout = reduced;
        return rooms;
    }

    public void generatetiles(List<RoomInfo> rooms) {
        GameObject parent = new GameObject("Room Manager");
        parent.transform.AddComponent<RoomManager>();
        roomManager = parent.GetComponent<RoomManager>();


        //Define Grid to generate rooms
        Room[,] roomGrid = new Room[roomLayout.GetLength(0) , roomLayout.GetLength(1)];

        for(int i = 0; i < rooms.Count; i++) {
            RoomInfo roomInfo = rooms[i];

            //Gerate object of Room, set the parent, and add Room Component
            float middle = Mathf.FloorToInt(gridSize / 2f);
            GameObject obj = Instantiate(roomTilePrefab , new Vector3((roomInfo.x - middle) * floorSize, 
                (roomInfo.y - middle) * floorSize , -1) , Quaternion.identity);
            obj.transform.localScale *= floorSize;

            obj.transform.SetParent(parent.transform);

            obj.transform.AddComponent<Room>();
            Room room = obj.transform.GetComponent<Room>();

            //Generate Floor
            obj.AddComponent<FloorGrid>();

            FloorGrid floor = obj.GetComponent<FloorGrid>();
            floor.set(floorSize , floorSize , 1);
            floor.tile = floorTilePrefab;

            floor.generateEmpty(obj);

            room.floor = floor;

            //Define Doors
            for(int dirIndex = 0; dirIndex < baseDirections.GetLength(0); dirIndex++) {
                //Debug.Log(dirIndex + " " + roomInfo.doorDirections[dirIndex] + "++++++++++++++++++++++++++++++++=");
                if(roomInfo.doorDirections[dirIndex]) {
                    int adjRoomIndex = roomLayout[roomInfo.x + baseDirections[dirIndex , 0] , roomInfo.y + baseDirections[dirIndex , 1]];

                    if(adjRoomIndex < i && adjRoomIndex != -1) {
                        floor.addDoor((dirIndex + 1) % 4 , doorMat , roomManager.roomList[adjRoomIndex].GetComponent<Room>().floor);
                    }
                    else {
                        floor.addDoor((dirIndex + 1) % 4 , doorMat);
                    }

                }
            }

            //Add Player to first room
            if(i == 0) {
                floor.addPlayer();
                floor.addOswald();
            }

            roomManager.roomList.Add(room.gameObject);
        }
    }

    public void print(string fileName, List<RoomInfo> rooms) {
        string text = "";

        for(int x = 0; x < roomLayout.GetLength(0); x++) {
            for(int y = 0; y < roomLayout.GetLength(1); y++) {
                text += roomLayout[x , y].ToString() + "\t";
            }
            text += "\n";
        }

        text += "\n\n\n";

        for(int x = 0; x < roomLayout.GetLength(0); x++) {
            for(int y = 0; y < roomLayout.GetLength(1); y++) {
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
