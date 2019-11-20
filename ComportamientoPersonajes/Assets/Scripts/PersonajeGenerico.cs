using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PersonajeGenerico : MonoBehaviour
{
    [Header("Atributos generales personaje")]
    public int vida;
    protected int daño;
    protected float velocidad;

    //Radio del circulo que hace de campo de vision
    protected float areaVision;


    public int zonaDondeEsta; //0: dentro, 1: fuera: 2 entrando, 3: saliendo

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    public bool quitarVida(int damage)
    {
        this.vida -= damage;
        if (vida <= 0)
        {
            Destroy(this.gameObject);
            return true;
        }
        return false;
    }*/
}
