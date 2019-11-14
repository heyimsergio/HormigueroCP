using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Floor : MonoBehaviour
{
    public NavMeshSurface surface;
    //medidas del suelo
    public float width;
    public float heigth;
    public int depth;
    // medidas de la sala
    public int minRoom;
    public int maxRoom;
    //medidas pasillos
    public int minCorridor;
    public int maxCorridor;

    //prefabs
    public GameObject[] prefabGroundList = new GameObject[5];
    public GameObject[] prefabRoomList = new GameObject[7];
    public GameObject prefabCorridor;
    public GameObject prefabTile;
    public GameObject prefabAgujero;

    public bool createRoomBool;

    // lo que miden las casillas
    float prefWidth = 1;

    GameObject[,] level;
    // Start is called before the first frame update

    int go = 0;

    ArrayList levelRooms = new ArrayList();
    ArrayList roomsAvailable = new ArrayList();

    int posX;
    int posZ;

    public enum CorridorDir {LEFT, RIGTH, TOP, BOTTOM };


    int numSalas = 0;

    void Start()
    {

        posX = +(int)this.transform.position.x;
        posZ = +(int)this.transform.position.z;
        level = new GameObject[(int)width, (int)heigth];

        for(int x = 0 ; x<width; x++)
        {
            for (int y = 0; y <heigth; y++)
            {
                int random = Random.Range(0, 100);

                GameObject tileObject = Instantiate(prefabTile,GetComponentInParent<Transform>());
                tileObject.transform.position = new Vector3(x + posX, this.transform.position.y, y + posZ);                
                TileScript tile = tileObject.GetComponent<TileScript>();
                int idx = (int) Random.Range(0.0f, 4.0f);
                    tile.initTile(x, y, depth, x, y, prefabGroundList[idx]);
                level[x, y] = tileObject;
            }
        }

        Room aux = createRoom((int) width/2, (int) heigth/2,Room.roomType.EMPTY);

        aux.addAgujero(level[(int)width / 2, (int)heigth / 2], prefabAgujero, depth);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(numSalas < 6)
        {
            createCorridor(roomType);
            Debug.Log("La sala: " + numSalas + " es del tipo: " + roomType);
            numSalas++;
        }
        */

    }

    //Busca la direccion libre en unsa sala.
    // @Params 
        // room: sala;
    public CorridorDir generateDir (Room room)
    {
        int num = Random.Range(0, 4);
        switch (num)
        {
            case 0:
                if (!room.left)
                {
                    room.left = true;
                    return CorridorDir.LEFT;
                }else if (!room.rigth)
                {
                    room.rigth = true;
                    return CorridorDir.RIGTH;
                }
                else if (!room.top)
                {
                    room.top = true;
                    return CorridorDir.TOP;
                } else {
                    room.bottom = true;
                    roomsAvailable.Remove(room);
                    return CorridorDir.BOTTOM;

                }
                break;
            case 1:
                if (!room.bottom)
                {
                    room.bottom = true;
                    return CorridorDir.BOTTOM;
                }
                else if (!room.left)
                {
                    room.left = true;
                    return CorridorDir.LEFT;
                }
                else if (!room.rigth)
                {
                    room.rigth = true;
                    return CorridorDir.RIGTH;
                }
                else if (!room.top)
                {
                    room.top = true;
                    roomsAvailable.Remove(room);
                    return CorridorDir.TOP;
                }
                break;
            case 2:
                if (!room.top)
                {
                    room.top = true;
                    return CorridorDir.TOP;
                }
                else if (!room.bottom)
                {
                    room.bottom = true;
                    return CorridorDir.BOTTOM;
                }
                else if (!room.left)
                {
                    room.left = true;
                    return CorridorDir.LEFT;
                }
                else if (!room.rigth)
                {
                    room.rigth = true;
                    roomsAvailable.Remove(room);
                    return CorridorDir.RIGTH;
                }
                break;
            case 3:
                if (!room.rigth)
                {
                    room.rigth = true;
                    return CorridorDir.RIGTH;
                }
                else if (!room.top)
                {
                    room.top = true;
                    return CorridorDir.TOP;
                }
                else if (!room.bottom)
                {
                    room.bottom = true;
                    return CorridorDir.BOTTOM;
                }
                else if (!room.left)
                {
                    room.left = true;
                    roomsAvailable.Remove(room);
                    return CorridorDir.LEFT;
                }
                
                break;

        }
        return CorridorDir.RIGTH;
    }


    //Orden de crear nueva sala:
    //Primero decide de que sala y en que direccion crearlo.
    // segundo lo crea
    //Comprueba que la sala  se puede crear con elcheckRoomSpace
    // si la pued ecrear llama a acreate Room y la crea
    public Room createCorridor(Room.roomType roomType)
    {
        if(roomsAvailable.Count == 0)
        {
            return null;
        }
        int roomIndex = Random.Range(0, roomsAvailable.Count-1);
        Room room = (Room)roomsAvailable[roomIndex];
        CorridorDir dir = generateDir(room);
        Room roomAux = null;

        int longui = Random.Range(minCorridor, maxCorridor);
        int x;
        int y;
        GameObject tile;
        TileScript myTile;
        switch (dir)
        {
            case CorridorDir.BOTTOM:
                 y = 0;
                 x = (int)(room.width/2);
                 tile = room.tiles[x, y];
                 myTile = tile.GetComponent<TileScript>();
                x = myTile.indexX;
                y = myTile.indexY-1;
                for(int i = y; i> y -longui; i--)
                {
                    if(i>= 0 && i < heigth)
                    {
                         tile = level[x, i];
                         myTile = tile.GetComponent<TileScript>();
                        if(myTile.tileType == TileScript.type.GROUND)
                        {
                            if(x > 0 && x < width - 1)
                            {
                                if (level[x + 1, y].GetComponent<TileScript>().tileType != TileScript.type.CORRIDOR && level[x - 1, y].GetComponent<TileScript>().tileType != TileScript.type.CORRIDOR)
                                {
                                    myTile.changeType(TileScript.type.CORRIDOR, prefabCorridor,depth);
                                }
                            } else
                            {
                                myTile.changeType(TileScript.type.CORRIDOR, prefabCorridor,depth);
                            }
  
                            
                        } else
                        {
                            return createCorridor(roomType);
                            
                        }
                         
                    } else
                    {
                        return createCorridor(roomType);
                        
                    }
                }


                roomAux = createRoom(x, y - longui, roomType);
                if (roomAux != null)
                {
                    roomAux.top = true;
                }

                break;
            case CorridorDir.TOP:
                 y = (int)room.heigth-1;
                 x = (int)(room.width / 2);
                 tile = room.tiles[x, y];
                 myTile = tile.GetComponent<TileScript>();
                x = myTile.indexX;
                y = myTile.indexY+1;
                for (int i = y; i < y + longui; i++)
                {
                    if (i >= 0 && i < heigth)
                    {
                        tile = level[x, i];
                        myTile = tile.GetComponent<TileScript>();
                        if (myTile.tileType == TileScript.type.GROUND)
                        {
                            if (x > 0 && x < width - 1)
                            {
                                if (level[x + 1, y].GetComponent<TileScript>().tileType != TileScript.type.CORRIDOR && level[x - 1, y].GetComponent<TileScript>().tileType != TileScript.type.CORRIDOR)
                                {
                                    myTile.changeType(TileScript.type.CORRIDOR, prefabCorridor,depth);
                                }
                            }
                            else
                            {
                                myTile.changeType(TileScript.type.CORRIDOR, prefabCorridor,depth);
                            }

                        }
                        else
                        {
                            return createCorridor(roomType);
                        }
                    }
                    else
                    {
                        return createCorridor(roomType);
                    }
                }

                 roomAux = createRoom(x, y + longui, roomType);
                if (roomAux != null)
                {
                    roomAux.bottom = true;
                }

                break;
            case CorridorDir.RIGTH:
                y = (int)(room.heigth /2);
                x = (int) room.width -1;
                tile = room.tiles[x, y];               
                myTile = tile.GetComponent<TileScript>();
                x = myTile.indexX+1;
                y = myTile.indexY;
                for (int i = x; i < x + longui; i++)
                {
                    if (i >= 0 && i < width)
                    {
                        tile = level[i, y];
                        tile.transform.Rotate(0.0f, 90.0f, 0.0f);
                        myTile = tile.GetComponent<TileScript>();
                        if (myTile.tileType == TileScript.type.GROUND)
                        {
                            if (y > 0 && y < heigth - 1)
                            {
                                if (level[x , y+1].GetComponent<TileScript>().tileType != TileScript.type.CORRIDOR && level[x , y-1].GetComponent<TileScript>().tileType != TileScript.type.CORRIDOR)
                                {
                                    myTile.changeType(TileScript.type.CORRIDOR, prefabCorridor,depth);
                                }
                            }
                            else
                            {
                                myTile.changeType(TileScript.type.CORRIDOR, prefabCorridor,depth);
                            }

                        }
                        else
                        {
                            return createCorridor(roomType);
                        }
                    }
                    else
                    {
                        return createCorridor(roomType);
                    }
                }

                roomAux = createRoom(x + longui, y, roomType);
                if (roomAux != null)
                {
                    roomAux.left = true; 
                }
                
                break;
            case CorridorDir.LEFT:
                y = (int) (room.heigth/2);
                x = 0;
                tile = room.tiles[x, y];
                myTile = tile.GetComponent<TileScript>();
                x = myTile.indexX-1;
                y = myTile.indexY;
                for (int i = x; i > x - longui; i--)
                {
                    if (i >= 0 && i < width)
                    {
                        tile = level[i, y];
                        tile.transform.Rotate(0.0f, 90.0f, 0.0f);
                        myTile = tile.GetComponent<TileScript>();
                        if (myTile.tileType == TileScript.type.GROUND)
                        {
                            if (y > 0 && y < heigth - 1)
                            {
                                if (level[x, y + 1].GetComponent<TileScript>().tileType != TileScript.type.CORRIDOR && level[x, y - 1].GetComponent<TileScript>().tileType != TileScript.type.CORRIDOR)
                                {
                                    myTile.changeType(TileScript.type.CORRIDOR, prefabCorridor,depth);
                                }
                            }
                            else
                            {
                                myTile.changeType(TileScript.type.CORRIDOR, prefabCorridor,depth);
                            }

                        }
                        else
                        {
                            return createCorridor(roomType);
                        }
                    }
                    else
                    {
                        return createCorridor(roomType);
                    }
                }
                roomAux = createRoom(x - longui, y,roomType);
                if(roomAux != null)
                {
                    roomAux.rigth = true;
                }
                
                break;

        }
        surface.BuildNavMesh();
        return roomAux;
    }
    
    //comprueba que la sala sea válida y despues la crea
    // @Params :
        //x: indice de la casilla x donde queremos que cree la sala
        //y: indice de la casilla y donde queremos que cree la sala
    public  Room createRoom(int x, int y, Room.roomType roomType)
    {

        int roomWidth = Random.Range(minRoom, maxRoom);
        int roomHeigth = roomWidth;
        Room roomAux = null;

        x = (int)(x - (roomWidth / 2));
        y = (int)(y - (roomHeigth / 2));


        if ((x + roomWidth) < width && (y + roomHeigth) < heigth && x > 0 && y > 0)
        {

            if (!checkRoomSpace(x, y, roomWidth, roomHeigth))
            {
                return null;
            }

            roomAux = new Room(roomWidth, roomHeigth, prefabRoomList, roomType);
            for (int i = x; i < x + roomWidth; i++)
            {
                for (int j = y; j < y + roomHeigth; j++)
                {

                    roomAux.addTile(level[i, j],depth);
                }
            }
            levelRooms.Add(roomAux);
            roomsAvailable.Add(roomAux);
           
            surface.BuildNavMesh();
            return roomAux;
        }
        else
        {
            return createCorridor(roomType);
        }
        
    }

    //q Comprueba si la sala tiene espacio en el interior del plano. Y que deja al menos 1 casilla de distancia con otras salas
    //@Params:
    //x: posicion x donde creamos la sala
    //y: posicion y donde creamos la sala
    // roomWidth : anchura
    // roomHeigth: altura
    public bool checkRoomSpace(int x, int y, int roomWidth, int roomHeigth)
    {
        //Comprobamos que todas las casillas de las salas esten libres;
        for (int a = x; a < x + roomWidth; a++)
        {
            for (int b = y; b < y + roomHeigth; b++)
            {

                if (a == x && a > 0)
                {
                    // da out of bounds exception en el array
                    if (level[a - 1, b].GetComponent<TileScript>().tileType == TileScript.type.ROOM)
                    {
                        return false;
                    }
                    else if (level[a-1, b].GetComponent<TileScript>().tileType == TileScript.type.CORRIDOR)
                    {
                    }
                }

                if (a == x + roomWidth-1 && a != width-1)
                {

                    if (level[a + 1, b].GetComponent<TileScript>().tileType == TileScript.type.ROOM)
                    {
                        return false;
                    }
                    else if (level[a + 1, b].GetComponent<TileScript>().tileType == TileScript.type.CORRIDOR)
                    {
                    }
                }

                if (b == y && b >0)
                {
                    if (level[a, b-1].GetComponent<TileScript>().tileType == TileScript.type.ROOM)
                    {
                        return false;
                    }
                    else if (level[a, b-1].GetComponent<TileScript>().tileType == TileScript.type.CORRIDOR)
                    {
                    }
                }

                if (b == y + roomHeigth-1 && b != heigth-1)
                {

                    if (level[a, b+1].GetComponent<TileScript>().tileType == TileScript.type.ROOM)
                    {
                        return false;
                    }
                    else if (level[a , b+1].GetComponent<TileScript>().tileType == TileScript.type.CORRIDOR)
                    {
                    }
                }




                if (level[a, b].GetComponent<TileScript>().tileType == TileScript.type.ROOM)
                {
                    return false;
                } else if(level[a, b].GetComponent<TileScript>().tileType == TileScript.type.CORRIDOR)
                {
                }
            }
        }
        return true;
    }
}
