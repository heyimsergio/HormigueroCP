using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActualizarDatosCanvas : MonoBehaviour
{
    public Text txtNurses;
    public Text txtObreras;
    public Text txtSoldados;
    public Text txtHuevos;
    public Text txtComida;

    public int numNurses;
    public int numObreras;
    public int numSoldados;
    public int numHuevos;
    public int numComida;

    public Reina reina;
    public Image fondoNum;
    public Image fondoOrdenes;

    /*
    public bool ordenAtacar;
    public bool ordenCavar;
    public bool ordenCuidarHormiga;
    public bool ordenCurar;
    public bool ordenPatrullar;
    public bool ordenBuscarComida;*/

    public Image ordenAtacarImg;
    public Image ordenCavarImg;
    public Image ordenCuidarHormigaImg;
    public Image ordenCurarImg;
    public Image ordenPatrullarImg;
    public Image ordenBuscarComidaImg;


    // Start is called before the first frame update
    void Start()
    {
        Color aux = fondoNum.color;
        aux.a = 0.3f;
        fondoNum.color = aux;
        aux = fondoOrdenes.color;
        aux.a = 0.3f;
        fondoOrdenes.color = aux;

        aux = ordenAtacarImg.color;
        aux.a =3f;
        ordenAtacarImg.color = aux;

        aux = ordenCavarImg.color;
        aux.a = 0.3f;
        ordenCavarImg.color = aux;

        aux = ordenCuidarHormigaImg.color;
        aux.a = 0.3f;
        ordenCuidarHormigaImg.color = aux;

        aux = ordenCurarImg.color;
        aux.a = 0.3f;
        ordenCurarImg.color = aux;

        aux = ordenPatrullarImg.color;
        aux.a = 0.3f;
        ordenPatrullarImg.color = aux;

        aux = ordenBuscarComidaImg.color;
        aux.a = 0.3f;
        ordenBuscarComidaImg.color = aux;
        
    }

    // Update is called once per frame
    void Update()
    {
        numNurses = reina.numeroDeNursesTotal;
        numObreras = reina.numeroDeObrerasTotal;
        numSoldados = reina.numeroDeSoldadosTotal;
        numHuevos = reina.huevosTotal.Count;
        numComida = reina.comidaTotal.Count;

        txtNurses.text = ": " + numNurses;
        txtObreras.text = ": " + numObreras;
        txtSoldados.text = ": " + numSoldados;
        txtHuevos.text = ": " + numHuevos;
        txtComida.text = ": " + numComida;
    }

    public void OrdenAtacar()
    {
        Color aux;
        aux = ordenAtacarImg.color;
        aux.a = 1f;
        ordenAtacarImg.color = aux;

        aux = ordenCavarImg.color;
        aux.a = 0.3f;
        ordenCavarImg.color = aux;

        aux = ordenCuidarHormigaImg.color;
        aux.a = 0.3f;
        ordenCuidarHormigaImg.color = aux;

        aux = ordenCurarImg.color;
        aux.a = 0.3f;
        ordenCurarImg.color = aux;

        aux = ordenPatrullarImg.color;
        aux.a = 0.3f;
        ordenPatrullarImg.color = aux;

        aux = ordenBuscarComidaImg.color;
        aux.a = 0.3f;
        ordenBuscarComidaImg.color = aux;
    }
    public void OrdenCavar()
    {
        Color aux;
        aux = ordenAtacarImg.color;
        aux.a = 0.3f;
        ordenAtacarImg.color = aux;

        aux = ordenCavarImg.color;
        aux.a = 1f;
        ordenCavarImg.color = aux;

        aux = ordenCuidarHormigaImg.color;
        aux.a = 0.3f;
        ordenCuidarHormigaImg.color = aux;

        aux = ordenCurarImg.color;
        aux.a = 0.3f;
        ordenCurarImg.color = aux;

        aux = ordenPatrullarImg.color;
        aux.a = 0.3f;
        ordenPatrullarImg.color = aux;

        aux = ordenBuscarComidaImg.color;
        aux.a = 0.3f;
        ordenBuscarComidaImg.color = aux;
    }
    public void OrdenCuidarHormiga()
    {
        Color aux;
        aux = ordenAtacarImg.color;
        aux.a = 0.3f;
        ordenAtacarImg.color = aux;

        aux = ordenCavarImg.color;
        aux.a = 0.3f;
        ordenCavarImg.color = aux;

        aux = ordenCuidarHormigaImg.color;
        aux.a = 1f;
        ordenCuidarHormigaImg.color = aux;

        aux = ordenCurarImg.color;
        aux.a = 0.3f;
        ordenCurarImg.color = aux;

        aux = ordenPatrullarImg.color;
        aux.a = 0.3f;
        ordenPatrullarImg.color = aux;

        aux = ordenBuscarComidaImg.color;
        aux.a = 0.3f;
        ordenBuscarComidaImg.color = aux;
    }
    public void OrdenCurar()
    {
        Color aux;
        aux = ordenAtacarImg.color;
        aux.a = 0.3f;
        ordenAtacarImg.color = aux;

        aux = ordenCavarImg.color;
        aux.a = 0.3f;
        ordenCavarImg.color = aux;

        aux = ordenCuidarHormigaImg.color;
        aux.a = 0.3f;
        ordenCuidarHormigaImg.color = aux;

        aux = ordenCurarImg.color;
        aux.a = 1f;
        ordenCurarImg.color = aux;

        aux = ordenPatrullarImg.color;
        aux.a = 0.3f;
        ordenPatrullarImg.color = aux;

        aux = ordenBuscarComidaImg.color;
        aux.a = 0.3f;
        ordenBuscarComidaImg.color = aux;
    }
    public void OrdenPatrullar()
    {
        Color aux;
        aux = ordenAtacarImg.color;
        aux.a = 0.3f;
        ordenAtacarImg.color = aux;

        aux = ordenCavarImg.color;
        aux.a = 0.3f;
        ordenCavarImg.color = aux;

        aux = ordenCuidarHormigaImg.color;
        aux.a = 0.3f;
        ordenCuidarHormigaImg.color = aux;

        aux = ordenCurarImg.color;
        aux.a = 0.3f;
        ordenCurarImg.color = aux;

        aux = ordenPatrullarImg.color;
        aux.a = 1f;
        ordenPatrullarImg.color = aux;

        aux = ordenBuscarComidaImg.color;
        aux.a = 0.3f;
        ordenBuscarComidaImg.color = aux;
    }
    public void OrdenBuscarComida()
    {
        Color aux;
        aux = ordenAtacarImg.color;
        aux.a = 0.3f;
        ordenAtacarImg.color = aux;

        aux = ordenCavarImg.color;
        aux.a = 0.3f;
        ordenCavarImg.color = aux;

        aux = ordenCuidarHormigaImg.color;
        aux.a = 0.3f;
        ordenCuidarHormigaImg.color = aux;

        aux = ordenCurarImg.color;
        aux.a = 0.3f;
        ordenCurarImg.color = aux;

        aux = ordenPatrullarImg.color;
        aux.a = 0.3f;
        ordenPatrullarImg.color = aux;

        aux = ordenBuscarComidaImg.color;
        aux.a = 1f;
        ordenBuscarComidaImg.color = aux;
    }
}
