using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldado : HormigaGenerica
{
    //Atacar
    public bool hayEnemigosCerca;
    EnemigoGenerico[] enemigosCerca;

    //Patrullar
    public int tiempoPatrullando;
    public Vector3 centro;
    public int radio;

    //Curar
    public HormigaGenerica hormigaACurar;
    public int tiempoParaCurar;

    //Ordenes de la reina
    public Reina reina;
    public bool meHanMandadoOrden;
    public enum ordenes { ORDEN1, ORDEN2 };

    //Explorar
    public Vector3 siguientePosicionExplorar;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
