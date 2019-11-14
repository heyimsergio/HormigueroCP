using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comida : MonoBehaviour
{

    public enum comidaType { Trigo}

    comidaType myType;

    public int peso;
    public double tiempoVida;
    public int hambreQueRestaura;
    public int usosDeLaComida;
    public bool haSidoCogida = false;
    public bool laEstanLLevando = false;

    public Room misala;

    public HormigaGenerica[] hormigasCogiendoLaComida;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void initComida(comidaType tipo)
    {
        switch (tipo)
        {
            case comidaType.Trigo:
                peso = 1;
                tiempoVida = 100;
                hambreQueRestaura = Random.Range(40,70);
                usosDeLaComida = 1;

                break;

        }
    }


    // Update is called once per frame
    void Update()
    {

        if (!haSidoCogida)
        {
            tiempoVida -= Time.deltaTime;
            if (tiempoVida <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public int comer()
    {
        usosDeLaComida--;
        return hambreQueRestaura;
    }

    // habria que hacer que la comida fuera con la navMesh a la sala y la hormiga detras
   public void cogerComida(Room misala)
    {
        this.misala = misala;
        haSidoCogida = true;
        this.transform.position = misala.centroDeLaSala;
    }




}
