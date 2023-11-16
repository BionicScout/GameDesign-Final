using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnInstruments : MonoBehaviour
{
    [SerializeField] public GameObject Instru;
    public bool tileHasInstru = false;
    public bool roomHasInstru = false;
    // Start is called before the first frame update
    void Start()
    {
        TileHasInstru();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void TileHasInstru()
    {
        if (tileHasInstru == false && roomHasInstru == false)
        {
            int instruValue = Random.Range(0, 2);
            if(instruValue == 0)
            {
                tileHasInstru = false;
                roomHasInstru = false;
            }

            if(instruValue > 0) 
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
