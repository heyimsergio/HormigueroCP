using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAlerts : MonoBehaviour
{
    public float tiempoMaxVivo;
    public float tiempoVivo;

    // Start is called before the first frame update
    void Start()
    {
        tiempoMaxVivo = 5.0f;
        tiempoVivo = tiempoMaxVivo;
    }

    // Update is called once per frame
    void Update()
    {
        tiempoVivo -= Time.deltaTime;
        if (tiempoVivo <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
