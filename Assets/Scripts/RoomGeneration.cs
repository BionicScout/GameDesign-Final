using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomGeneration : MonoBehaviour {
    public int[,] roomLayout;
    public int gridSize;
    public int howManyRooms;

    int[,] baseDirections = { {-1, 0} , {0, -1}, {1, 0}, {0, 1} }; //[0 Up, 1 Left, 2 Down, 3 Right   ,   0 x, 1 y]

    public struct RoomInfo {
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

            doorsLeft = 4;
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
        roomLayout = new int[gridSize , gridSize];
        setToNegative1();

        int middle = Mathf.FloorToInt(gridSize / 2f);

        //Generation
        List<RoomInfo> rooms = new List<RoomInfo>(); //Tuple represents a rooms x, y, and doorWaysLeft

        RoomInfo temp = new RoomInfo();
        temp.set(middle, middle);
        roomLayout[middle , middle] = 0;
        rooms.Add(temp);

        int roomsToGen = howManyRooms;

        int fileText = 0;
        print(fileText.ToString(), rooms);



        while(roomsToGen > 0) {
        //Get Room with doors
            Debug.Log("RoomsToGen: " + roomsToGen);
            int roomIndex = Random.Range(0, rooms.Count);

            //Debug.Log("Room Index: " + roomIndex);

            if(rooms[roomIndex].doorsLeft == 0) {
                Debug.Log("Max Doors!");
                Debug.Log("END LOOP -----------------------------");
                fileText++;
                print(fileText.ToString(), rooms);
                continue;
            }


            //Pick Direction of Room
            int direction = Random.Range(0 , 4) % 4;
            bool openDirection = rooms[roomIndex].directions[direction];

            while(!openDirection) {
                direction = (direction + 1) % 4;
                openDirection = rooms[roomIndex].directions[direction];
            }

            Debug.Log("Direction: " + direction);
            Debug.Log("Open: " + openDirection);


            //Place Room
            temp = new RoomInfo();

            temp.set(rooms[roomIndex].x + baseDirections[direction , 0] , rooms[roomIndex].y + baseDirections[direction , 1]);

            rooms.Add(temp);
            //rooms[roomIndex].directions[direction] = false;
            roomLayout[temp.x , temp.y] = rooms.Count - 1;

            //fileText++;
            //print(fileText.ToString() , rooms);

            //Check The new room for near by rooms
            for(int i = 0; i < baseDirections.GetLength(0); i++) {
                //Debug.Log((temp.x + baseDirections[i , 0]) + ", " + (temp.y + baseDirections[i , 1]));
                int roomId = roomLayout[temp.x + baseDirections[i , 0] , temp.y + baseDirections[i , 1]];


                if(roomId == -1)
                    continue;

                //Debug.Log("Did not continue");

                if(rooms[roomId].doorsLeft > 0) {
                    RoomInfo adjRoom = rooms[roomId];
                    adjRoom.doorsLeft = adjRoom.doorsLeft - 1;                   ;
                    adjRoom.directions[(i+2)%4] = false;
                    rooms[roomId] = adjRoom;

                    temp.doorsLeft--;
                    temp.directions[i] = false;
                }
            }

            rooms[rooms.Count - 1] = temp;



            //Debug.Log(temp.x + " " + temp.y);
            //roomLayout[temp.x , temp.y] = rooms.Count - 1;
            roomsToGen--;
            Debug.Log("END LOOP -----------------------------");

            fileText++;
            print(fileText.ToString() , rooms);
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