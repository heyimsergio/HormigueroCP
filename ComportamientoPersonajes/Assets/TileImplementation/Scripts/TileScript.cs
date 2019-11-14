using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{

    public int indexX;
    public int indexY;
    int depth;

    GameObject myObject;

    int posX;
    int posY;

    public enum type { ROOM,GROUND,CORRIDOR}

    public type tileType;


    private void Start()
    {
        
    }

    //Inicializa la casilla:
    //@Params:
    // indexX: indice de casilla en el piso
    // depth: nivel de profundidad
    //posx : posicion x real.
    // posy : posicion y real.
    public void initTile(int indexX, int indexY, int depth, int posX, int posY, GameObject prefab)
    {
        this.indexX = indexX;
        this.indexY = indexY;
        this.depth = depth;
        myObject = Instantiate(prefab,this.GetComponentInParent<Transform>());
        tileType = type.GROUND;
    }

    public void destroyGround()
    {
        Destroy(myObject);

        tileType = type.ROOM;
    }


    //Cambia el tipo de sala
    //@Params:
    // newRype: nuevo tipo de casilla
    // prefab: prefab de la casilla.
    public void changeType(type newType,GameObject prefab, int depth)
    {

        Destroy(myObject);
        myObject = Instantiate(prefab, this.GetComponentInParent<Transform>());
        switch (depth)
        {
            case 0:
                myObject.layer = 14;
                break;
            case 1:
                myObject.layer = 11;
                break;
            case 2:
                myObject.layer = 12;
                break;
            case 3:
                myObject.layer = 13;
                break;
        }
        tileType = newType;
    }


}
