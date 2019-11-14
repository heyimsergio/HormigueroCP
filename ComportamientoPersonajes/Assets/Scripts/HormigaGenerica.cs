using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HormigaGenerica : PersonajeGenerico
{
    protected float hambre = 200;
    public float pesoQuePuedenTransportar;
    protected Vector3 posicionReina;
    public bool estaLuchando = false;

    public bool hayOrdenDeAtacar = false;
    public EnemigoGenerico enemigoAlQueAtacar = null;
    public bool hayOrdenBuscarComida = false;
    public bool hayOrdenCuidarHuevos = false;


    // Start is called before the first frame update
    void Start()
    {
        this.zonaDondeEsta = 0;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void actualizarHambre()
    {
        hambre -= Time.deltaTime;
    }

    public bool quitarVida(int damage)
    {
        Debug.Log("Hormiga perdiendo vida");
        this.vida -= damage;
        if (vida <= 0)
        {
            Destroy(this.gameObject);
            return true;
        }
        return false;
    }

}
