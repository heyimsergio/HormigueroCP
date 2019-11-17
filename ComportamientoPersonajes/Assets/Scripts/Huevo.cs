﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Huevo : MonoBehaviour
{
    public Reina.TipoHormiga miType;
    public float tiempoParaNacer;
    public float tiempoQueAguantaSinCuidar;
    public float maxTimeParaCuidar;
    public bool necesitaCuidados = false;
    public bool puedeSerCuidado = false;
    public HormigaGenerica siendoCuidadoPor = null;
    public int umbralDeAvisoCuidarHuevo;
    public int umbralDePoderseCuidar;
    public Room myRoom;
    private Reina miReina;
    Collider huevoCollider;

    // Start is called before the first frame update
    void Start()
    {
        miReina = GameObject.FindObjectOfType<Reina>();
        miReina.huevosTotal.Add(this);
        tiempoQueAguantaSinCuidar = maxTimeParaCuidar;
        huevoCollider = GetComponent<Collider>();
        huevoCollider.isTrigger = true;
    }

    public void init(Room aux, Reina.TipoHormiga tipo)
    {
        myRoom = aux;
        miType = tipo;
    }

    // Update is called once per frame
    void Update()
    {
        if(tiempoQueAguantaSinCuidar < umbralDePoderseCuidar)
        {
            puedeSerCuidado = true;
        }

        //Avisamos de que hay que cuidar al huevo
        if(tiempoQueAguantaSinCuidar < umbralDeAvisoCuidarHuevo)
        {
            necesitaCuidados = true;
            if (siendoCuidadoPor == null)
            {
                miReina.huevoNecesitaCuidado(this);
            }
        }

        // Huevo muerto
        if(tiempoQueAguantaSinCuidar < 0)
        {
            Debug.Log("Huevo Muerto");
            miReina.huevoMuerto(this);
            myRoom.sacarCosas();
            Destroy(this.gameObject);
        }

        tiempoQueAguantaSinCuidar -= Time.deltaTime;

        // Nace Huevo
        if (tiempoParaNacer < 0)
        {
            myRoom.sacarCosas();
            miReina.NaceHuevo(this);
            Destroy(this.gameObject);
        }
        tiempoParaNacer -= Time.deltaTime;
    }

    /*   public void quitarHuevo()
        {
            myRoom.sacarCosas();
            miReina.NaceHuevo(this);
        }
    */

    public void cuidar()
    {
        if (necesitaCuidados)
        {
            necesitaCuidados = false;
            miReina.huevoCuidado(this);
        }
        puedeSerCuidado = false;
        tiempoQueAguantaSinCuidar = maxTimeParaCuidar;
    }
}