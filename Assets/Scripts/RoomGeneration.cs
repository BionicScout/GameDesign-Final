using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RoomGeneration : MonoBehaviour {
    int[,] roomLayout;
    public int gridSize;

    struct RoomInfo {
        public int x, y;
        public bool[] directions; //0 Up, 1 Left, 2 Down, 3 Right. //True means room is in that direction
        public int doorsLeft;

        public void set(int xCoord, int yCoord) {
            x = xCoord; 
            y = yCoord;

            directions = new bool[4];
            directions[0] = true;
            directions[1] = true;
            directions[2] = true;
            directions[3] = true;

            doorsLeft = 0;
        }
    }

    public void generate() {
        roomLayout = new int[gridSize , gridSize];
        setToNegative1();

        int middle = Mathf.FloorToInt(gridSize / 2f);
        roomLayout[middle, middle] = 0;

        //Generation
        List<RoomInfo> rooms = new List<RoomInfo>(); //Tuple represents a rooms x, y, and doorWaysLeft

        RoomInfo temp = new RoomInfo();
        temp.set(middle, middle);
        rooms.Add(temp);

        int roomsToGen = 25;

        while(roomsToGen > 0) {
        //Get Room with doors
            Debug.Log("RoomsToGen: " + roomsToGen);
            int i = Random.Range(0, rooms.Count);

            if(rooms[i].doorsLeft == 0)
                continue;

        //Pick Direction of Room
            int direction = Random.Range(0 , 3);
            bool openDirection = rooms[i].directions[direction];

            while(openDirection) {
                direction = (direction+1) % 4;
                openDirection = rooms[i].directions[direction];
            }

        //Place Room
            temp = new RoomInfo();

            if(direction == 0) {
                temp.set(rooms[i].x, rooms[i].y + 1);
            }
            if (direction == 1) {
                temp.set(rooms[i].x - 1, rooms[i].y);
            }
            if(direction == 2) {
                temp.set(rooms[i].x, rooms[i].y - 1);
            }
            if(direction == 3) {
                temp.set(rooms[i].x + 1, rooms[i].y);
            }
            rooms.Add(temp);
            rooms[i].directions[direction] = false;
            roomLayout[temp.x, temp.y] = rooms.Count - 1;

        //Check The new room for near by rooms
            if(roomLayout[temp.x , temp.y + 1] != -1) {
                RoomInfo adjRoom = rooms[roomLayout[temp.x , temp.y + 1]];
                adjRoom.doorsLeft--;
                adjRoom.directions[0] = false;

                temp.doorsLeft--;
            }
            if(roomLayout[temp.x - 1, temp.y] != -1) {
                RoomInfo adjRoom = rooms[roomLayout[temp.x - 1 , temp.y]];
                adjRoom.doorsLeft--;
                adjRoom.directions[1] = false;

                temp.doorsLeft--;
            }
            if(roomLayout[temp.x , temp.y - 1] != -1) {
                RoomInfo adjRoom = rooms[roomLayout[temp.x , temp.y - 1]];
                adjRoom.doorsLeft--;
                adjRoom.directions[2] = false;

                temp.doorsLeft--;
            }
            if(roomLayout[temp.x + 1, temp.y] != -1) {
                RoomInfo adjRoom = rooms[roomLayout[temp.x + 1 , temp.y]];
                adjRoom.doorsLeft--;
                adjRoom.directions[3] = false;

                temp.doorsLeft--;
            }





            roomLayout[temp.x , temp.y] = temp.doorsLeft;
            roomsToGen--;
            Debug.Log("END LOOP -----------------------------");
        }

        
        

        print();
    }

    public void setToNegative1() {
        for(int x = 0; x < gridSize; x++) {
            for(int y = 0; y < gridSize; y++) {
                roomLayout[x , y] = -1;
            }
        }
    }

    public void print() {
        string text = "";

        for(int x = 0; x < gridSize; x++) {
            for(int y = 0; y < gridSize; y++) {
                text += roomLayout[x , y].ToString() + "\t";
            }
            text += "\n";
        }

        StreamWriter writer1 = new StreamWriter("TestRoom.txt" , false);
        writer1.Write("");
        writer1.Close();
        StreamWriter writer = new StreamWriter("TestRoom.txt" , true);
        writer.Write(text);
        writer.Close();
    }
}
