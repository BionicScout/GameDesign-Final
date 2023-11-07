using UnityEngine;

public class MyGrid {
    public int height, width;
    public int size;
    public GameObject[,] grid; //[height, width]

    public MyGrid(int h, int w, int s) {
        height = h;
        width = w;
        grid = new GameObject[h, w];
    }

    public void generateEmpty() {
        for(int h  = 0; h < height; h++) {
            for(int w = 0; w < width; w++) {
                GameObject obj = new GameObject();

                obj.transform.position = new Vector3(h * size, w * size, 0);
            }
        }
    }
}
