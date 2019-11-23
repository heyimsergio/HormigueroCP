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

    // Start is called before the first frame update
    void Start()
    {
        Color aux = fondoNum.color;
        aux.a = 0.5f;
        fondoNum.color = aux;
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
}
