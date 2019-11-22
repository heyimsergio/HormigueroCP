﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneradorEnemigos : MonoBehaviour
{

    public GameObject enemigo;
    public float tiempoParaSpawnear;
    public float tiempoMaximo;
    public float tiempoMinimo;

    public Vector3 pos1;
    public Vector3 pos2;
    public Vector3 pos3;
    public Vector3 pos4;

    // Start is called before the first frame update
    void Start()
    {
        tiempoMaximo = 20f;
        tiempoMinimo = 10f;
        tiempoParaSpawnear = tiempoMinimo;
        pos1 = new Vector3(0.5f, 0, 0.5f);
        pos2 = new Vector3(1f, 0, 49f);
        pos3 = new Vector3(49f, 0, 1f);
        pos4 = new Vector3(49f, 0, 49f);
    }

    // Update is called once per frame
    void Update()
    {
        tiempoParaSpawnear -= Time.deltaTime;
        if (tiempoParaSpawnear <= 0)
        {
            SpawnEnemigo();
            tiempoParaSpawnear = Random.Range(tiempoMinimo, tiempoMaximo);
        }
    }

    public void SpawnEnemigo()
    {
        int pos = Mathf.RoundToInt(Random.Range(1, 4));
        switch (pos)
        {
            case 1:
                //Debug.Log("spawn en 1");
                Instantiate(enemigo, pos1, Quaternion.identity);
                break;
            case 2:
                //Debug.Log("spawn en 2");
                Instantiate(enemigo, pos2, Quaternion.identity);
                break;
            case 3:
                //Debug.Log("spawn en 3");
                Instantiate(enemigo, pos3, Quaternion.identity);
                break;
            case 4:
                //Debug.Log("spawn en 4");
                Instantiate(enemigo, pos4, Quaternion.identity);
                break;
            default:
                break;
        }

    }
}