using UnityEngine;

public class FloorGrid : MonoBehaviour{
    public int height, width;
    public int size;
    public FloorTile[,] grid; //[height, width]
    public GameObject tile;

    public FloorGrid(int h, int w, int s) {
        height = h;
        width = w;
        grid = new FloorTile[h, w];
    }

    public void generateEmpty() {
        GameObject parent = new GameObject("Grid");
        grid = new FloorTile[width , height];

        for(int w  = 0; w < width; w++) {
            for(int h = 0; h < height; h++) {
                GameObject obj = Instantiate(tile, new Vector3(w * size , h * size , 0), Quaternion.identity);
                obj.transform.SetParent(parent.transform);
                obj.transform.localScale *= size;

                obj.AddComponent<FloorTile>();
                grid[w, h] = obj.GetComponent<FloorTile>();
                grid[w , h].floorCord = new int[2];
                grid[w , h].floorCord[0] = w;
                grid[w , h].floorCord[1] = h;
                grid[w , h].floorGrid = this;
            }
        }
    }
}
