using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Outside : MonoBehaviour
{
    // Start is called before the first frame update
    public NavMeshSurface surface;
    //medidas del suelo
    public float width;
    public float heigth;
    public int depth;

    //prefabs
    public GameObject[] prefabOutList = new GameObject[5];
    public GameObject prefabTile;
    public GameObject prefabAgujero;

    // lo que miden las casillas
    float prefWidth = 1;

    GameObject[,] level;


    public Vector3 centro;


    // Start is called before the first frame update

    int go = 0;


    int posX;
    int posZ;

    void Start()
        {

            posX = +(int)this.transform.position.x;
            posZ = +(int)this.transform.position.z;
            level = new GameObject[(int)width, (int)heigth];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < heigth; y++)
                {
                    int random = Random.Range(0, 100);
                
                    GameObject tileObject = Instantiate(prefabTile, GetComponentInParent<Transform>());
                    tileObject.transform.position = new Vector3(x + posX, this.transform.position.y, y + posZ);
                    TileScript tile = tileObject.GetComponent<TileScript>();
                if (x == (int)width / 2 && y == (int)heigth / 2)
                {

                    tile.initTile(x, y, depth, x, y, prefabAgujero);
                    centro = tile.transform.position;
                } else
                {
                    int idx = (int)Random.Range(0.0f, 5.0f);
                    tile.initTile(x, y, depth, x, y, prefabOutList[idx]);
                }
                
                    level[x, y] = tileObject;
                }
            }
        surface.BuildNavMesh();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
