using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room 
{
    public bool left = false;
    public bool rigth = false;
    public bool top = false;
    public bool bottom = false;
    public bool full = false;

    public int width;
    public int heigth;

    int x = 0;
    int y = 0;

    public int capacidadTotalRoom;
    public int llenadoActual = 0;
    public bool isFull = false;

    public Vector3 centroDeLaSala;

    public GameObject[,] tiles;

    public enum roomType { STORAGE, EGGROOM, EMPTY, LIVEROOM};
    roomType myType;

    public GameObject[] roomPrefabList;

    public Room(int width, int heigth, GameObject[] roomPrefabList, roomType type)
    {
        this.width = width;
        this.heigth = heigth;
        this.roomPrefabList = roomPrefabList;
        tiles = new GameObject[width, heigth];
        myType = type;
        capacidadTotalRoom = (int)(width * heigth) / 2;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // primeor recorro en las y y luego en las x.
    // La sala se guarda de la esquina de abajo izq a la arriba der
    public void addTile(GameObject tile, int depth)
    {
        if(x == (int)(width/2) && y == (int)(heigth / 2))
        {
            centroDeLaSala = tile.transform.position;
        }
        tiles[x, y] = tile;
        int idx = (int)Random.Range(0.0f, 6.0f);
            tile.GetComponent<TileScript>().changeType(TileScript.type.ROOM, roomPrefabList[idx], depth);
        y++;
        if(y >= heigth)
        {
            y = 0;
            x++;
        }
    }

    public void addAgujero(GameObject tile, GameObject agujeroPrefab, int depth)
    {

            tile.GetComponent<TileScript>().changeType(TileScript.type.ROOM, agujeroPrefab, depth);
        
    }

    public Vector3 getCenter()
    {
        return centroDeLaSala;
    }

    public void meterCosas()
    {
        llenadoActual++;
        if (llenadoActual >= capacidadTotalRoom)
        {
            isFull = true;
        }
    }


    public void sacarCosas()
    {
        llenadoActual--;
        if (isFull)
        {
            isFull = false;
        }
    }

    public Vector3 getRandomPosition()
    {
       return (centroDeLaSala + new Vector3(Random.Range(-width / 2, width / 2), 0, Random.Range(-heigth / 2, heigth / 2)));
    }


}
