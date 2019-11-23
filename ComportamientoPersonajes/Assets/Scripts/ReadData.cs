using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadData : MonoBehaviour
{

    DataController data;
    float time;
    Vector3 posInicial = new Vector3(85, 0, 25);
    bool creado = false;

    // Start is called before the first frame update
    void Start()
    {
        data = FindObjectOfType<DataController>();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if ( time > 0.1f && !creado)
        {
            //Debug.Log("instanciar");
            for (int i = 0; i < data.numNurse; i++)
            {
                Instantiate(data.nursePrefab, posInicial, Quaternion.identity);
            }
            for (int i = 0; i < data.numObreras; i++)
            {
                Instantiate(data.obreraPrefab, posInicial, Quaternion.identity);
            }
            for (int i = 0; i < data.numSoldados; i++)
            {
                Instantiate(data.soldadoPrefab, posInicial, Quaternion.identity);
            }
            creado = true;
        }
    }
}
