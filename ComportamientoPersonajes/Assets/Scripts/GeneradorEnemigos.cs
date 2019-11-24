using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneradorEnemigos : MonoBehaviour
{

    int numeroDeGusanosPorEscarabajo;
    int numeroActual;
    public GameObject enemigo;
    public GameObject escarabajo;
    public float tiempoParaSpawnear;
    public float tiempoMaximo;
    public float tiempoMinimo;

    float initTiempoMaximo;
    float initTiempoMinimo;
    int minimoTiempo = 5;

    float tiempoActual = 0;


    public Vector3 pos1;
    public Vector3 pos2;
    public Vector3 pos3;
    public Vector3 pos4;

    // Start is called before the first frame update
    void Start()
    {
        numeroActual = 0;
        DataController config = FindObjectOfType<DataController>();
        if (config.facil)
        {
            numeroDeGusanosPorEscarabajo = 100;
            tiempoMaximo = 50f;
            tiempoMinimo = 25f;
        } else if (config.medio)
        {
            numeroDeGusanosPorEscarabajo = 20;
            tiempoMaximo = 35f;
            tiempoMinimo = 15f;
        } else
        {
            numeroDeGusanosPorEscarabajo = 12;
            tiempoMaximo = 25f;
            tiempoMinimo = 10f;
        }

        initTiempoMaximo = tiempoMaximo;
        initTiempoMinimo = tiempoMinimo;

        tiempoParaSpawnear = tiempoMinimo;
        pos1 = new Vector3(0.5f, 0, 0.5f);
        pos2 = new Vector3(1f, 0, 49f);
        pos3 = new Vector3(49f, 0, 1f);
        pos4 = new Vector3(49f, 0, 49f);
    }

    // Update is called once per frame
    void Update()
    {
        tiempoActual += Time.deltaTime;
        tiempoMaximo = initTiempoMaximo - tiempoActual / 90;
        tiempoMinimo = initTiempoMinimo - tiempoActual / 90;

        if(tiempoMinimo < minimoTiempo)
        {
            tiempoMinimo = minimoTiempo;
        }
        if(tiempoMaximo < minimoTiempo)
        {
            tiempoMaximo = minimoTiempo;
        }

        tiempoParaSpawnear -= Time.deltaTime;
        if (tiempoParaSpawnear <= 0)
        {
            SpawnEnemigo();
            tiempoParaSpawnear = Random.Range(tiempoMinimo, tiempoMaximo);
           
        }
    }

    public void SpawnEnemigo()
    {
        numeroActual++;
        if (numeroActual == numeroDeGusanosPorEscarabajo)
        {
            SpawnEscarabajo();
            numeroActual = 0;
        }
        int pos = Mathf.RoundToInt(Random.Range(1, 5));
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

        public void SpawnEscarabajo()
        {
            int pos = Mathf.RoundToInt(Random.Range(1, 5));
            switch (pos)
            {
                case 1:
                    //Debug.Log("spawn en 1");
                    Instantiate(escarabajo, pos1, Quaternion.identity);
                    break;
                case 2:
                    //Debug.Log("spawn en 2");
                    Instantiate(escarabajo, pos2, Quaternion.identity);
                    break;
                case 3:
                    //Debug.Log("spawn en 3");
                    Instantiate(escarabajo, pos3, Quaternion.identity);
                    break;
                case 4:
                    //Debug.Log("spawn en 4");
                    Instantiate(escarabajo, pos4, Quaternion.identity);
                    break;
                default:
                    break;
            }

        }
}
