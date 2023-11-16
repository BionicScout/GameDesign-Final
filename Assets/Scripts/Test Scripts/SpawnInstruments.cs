using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnInstruments : MonoBehaviour
{
    [SerializeField] public GameObject Instru;
    public bool tileHasInstru = false;
    public bool roomHasInstru = false;
    public bool playerHasInstru = false;
    private FloorTile player;
    // Start is called before the first frame update
    void Start()
    {
        TileHasInstru();
    }

    // Update is called once per frame
    void Update()
    {
        //check if the player collided with a instrument
        if (player.transform.position.x == gameObject.transform.position.x &&
            player.transform.position.y == gameObject.transform.position.y)
        {
            playerHasInstru = true;
            Destroy(gameObject);
        }
    }
    void TileHasInstru()
    {
        if (tileHasInstru == false && roomHasInstru == false)
        {
            int instruValue = Random.Range(0, 10);
            if(instruValue <= 5)
            {
                tileHasInstru = false;
                roomHasInstru = false;
            }

            if(instruValue > 5 && roomHasInstru == false) 
            {
                tileHasInstru = true;
                roomHasInstru = true;
                float x = gameObject.transform.position.x;
                float y = gameObject.transform.position.y;
                Instantiate(Instru, new Vector2(x, y), Quaternion.identity);
            }
        }
    }

    private void Instantiate(GameObject instru, Vector2 vector2)
    {
        throw new System.NotImplementedException();
    }
}
