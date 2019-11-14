using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour
{

    #region inspectorVariables
    public int heigth;
    public int width;
    public  GameObject ground;
    public GameObject generalTile;
    public bool createRoom = false;
    #endregion

    GameObject[,] map;

    // Start is called before the first frame update
    void Start()
    {
        int halfHeigth = heigth / 2;
        int halfWidth = width / 2;
        map = new GameObject[heigth,width];
        for (int i = 0; i < heigth; i++)
        {
            for(int z = 0; z < width; z++)
            {

                GameObject TileAux = Instantiate(generalTile);
                TileScript tile = generalTile.GetComponent<TileScript>();

                //tile.initTile(z, i, 0, ground, z - halfWidth, i - halfHeigth);
                map[i, z] = TileAux;

            }
        }
         
    }

    // Update is called once per frame
    void Update()
    {
      

    }



    public void createBackGround()
    {

    }



}
