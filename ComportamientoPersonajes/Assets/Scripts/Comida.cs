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

    //Variables de ubicacion de la comida;
    public Room misala;
    public TileScript miTile;

    public HormigaGenerica[] hormigasCogiendoLaComida;

    Reina reina = null;

    

    // Start is called before the first frame update
    void Start()
    {
        reina = GameObject.FindObjectOfType<Reina>();
        miTile = null;
    }

    public void initComida(comidaType tipo)
    {
        switch (tipo)
        {
            case comidaType.Trigo:
                peso = 1;
                tiempoVida = 100;
                hambreQueRestaura = Random.Range(100,200);
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
                reina.ComidaHaMuerto(this);
                Destroy(this.gameObject);
            }
        }
        if (usosDeLaComida <= 0)
        {
            reina.ComidaHaMuerto(this);
            Destroy(this.gameObject);
        }
    }

    public int comer()
    {
        usosDeLaComida--;
        return hambreQueRestaura;
    }

    /// <summary>
    /// Actualiza sala y tile, una vez muerta habrá que liberarlos;
    /// </summary>
    /// <param name="misala"> Sala donde se ubica la comida</param>
    /// <param name="miTile"> Tile donde se ubica la comida</param>
    /*
    public void cogerComida(Room misala, TileScript miTile)
    {
        this.misala = misala;
        this.miTile = miTile;
        haSidoCogida = true;
        this.transform.position = misala.centroDeLaSala;
    }

    */


}
