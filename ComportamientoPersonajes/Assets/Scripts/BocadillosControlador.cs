using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BocadillosControlador : MonoBehaviour
{

    public HormigaGenerica hormigaSeleccionada;
    public SpriteRenderer bocadilloExplorar;
    public SpriteRenderer bocadilloBuscarComida;
    public SpriteRenderer bocadilloCurar;
    public SpriteRenderer bocadilloPatrullar;
    public SpriteRenderer bocadilloCavar;
    public SpriteRenderer bocadilloComer;
    public SpriteRenderer bocadilloCuidarHuevo;
    public SpriteRenderer bocadilloAtacar;

    bool explorar = false;
    bool buscarComida = false;
    bool curar = false;
    bool patrullar = false;
    bool cavar = false;
    bool comer = false;
    bool cuidarHuevos = false;
    bool atacar = false;

    Vector3 offset = new Vector3(5f, 0, 5f);


    // Start is called before the first frame update
    void Start()
    {
        bocadilloExplorar.gameObject.SetActive(false);
        bocadilloBuscarComida.gameObject.SetActive(false);
        bocadilloCurar.gameObject.SetActive(false);
        bocadilloPatrullar.gameObject.SetActive(false);
        bocadilloCavar.gameObject.SetActive(false);
        bocadilloComer.gameObject.SetActive(false);
        bocadilloCuidarHuevo.gameObject.SetActive(false);
        bocadilloAtacar.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (hormigaSeleccionada != null)
        {
            if (explorar == true)
            {
                bocadilloExplorar.gameObject.SetActive(true);
                bocadilloBuscarComida.gameObject.SetActive(false);
                bocadilloCurar.gameObject.SetActive(false);
                bocadilloPatrullar.gameObject.SetActive(false);
                bocadilloCavar.gameObject.SetActive(false);
                bocadilloComer.gameObject.SetActive(false);
                bocadilloCuidarHuevo.gameObject.SetActive(false);
                bocadilloAtacar.gameObject.SetActive(false);

                bocadilloExplorar.transform.position = hormigaSeleccionada.transform.position + offset;
            }
            else if (buscarComida == true)
            {
                bocadilloExplorar.gameObject.SetActive(false);
                bocadilloBuscarComida.gameObject.SetActive(true);
                bocadilloCurar.gameObject.SetActive(false);
                bocadilloPatrullar.gameObject.SetActive(false);
                bocadilloCavar.gameObject.SetActive(false);
                bocadilloComer.gameObject.SetActive(false);
                bocadilloCuidarHuevo.gameObject.SetActive(false);
                bocadilloAtacar.gameObject.SetActive(false);

                bocadilloBuscarComida.transform.position = hormigaSeleccionada.transform.position + offset;
            }
            else if (curar == true)
            {
                bocadilloExplorar.gameObject.SetActive(false);
                bocadilloBuscarComida.gameObject.SetActive(false);
                bocadilloCurar.gameObject.SetActive(true);
                bocadilloPatrullar.gameObject.SetActive(false);
                bocadilloCavar.gameObject.SetActive(false);
                bocadilloComer.gameObject.SetActive(false);
                bocadilloCuidarHuevo.gameObject.SetActive(false);
                bocadilloAtacar.gameObject.SetActive(false);

                bocadilloCurar.transform.position = hormigaSeleccionada.transform.position + offset;
            }
            else if (patrullar == true)
            {
                bocadilloExplorar.gameObject.SetActive(false);
                bocadilloBuscarComida.gameObject.SetActive(false);
                bocadilloCurar.gameObject.SetActive(false);
                bocadilloPatrullar.gameObject.SetActive(true);
                bocadilloCavar.gameObject.SetActive(false);
                bocadilloComer.gameObject.SetActive(false);
                bocadilloCuidarHuevo.gameObject.SetActive(false);
                bocadilloAtacar.gameObject.SetActive(false);

                bocadilloPatrullar.transform.position = hormigaSeleccionada.transform.position + offset;
            }
            else if (cavar == true)
            {
                bocadilloExplorar.gameObject.SetActive(false);
                bocadilloBuscarComida.gameObject.SetActive(false);
                bocadilloCurar.gameObject.SetActive(false);
                bocadilloPatrullar.gameObject.SetActive(false);
                bocadilloCavar.gameObject.SetActive(true);
                bocadilloComer.gameObject.SetActive(false);
                bocadilloCuidarHuevo.gameObject.SetActive(false);
                bocadilloAtacar.gameObject.SetActive(false);

                bocadilloCavar.transform.position = hormigaSeleccionada.transform.position + offset;
            }
            else if (comer == true)
            {
                bocadilloExplorar.gameObject.SetActive(false);
                bocadilloBuscarComida.gameObject.SetActive(false);
                bocadilloCurar.gameObject.SetActive(false);
                bocadilloPatrullar.gameObject.SetActive(false);
                bocadilloCavar.gameObject.SetActive(false);
                bocadilloComer.gameObject.SetActive(true);
                bocadilloCuidarHuevo.gameObject.SetActive(false);
                bocadilloAtacar.gameObject.SetActive(false);

                bocadilloComer.transform.position = hormigaSeleccionada.transform.position + offset;
            }
            else if (cuidarHuevos == true)
            {
                bocadilloExplorar.gameObject.SetActive(false);
                bocadilloBuscarComida.gameObject.SetActive(false);
                bocadilloCurar.gameObject.SetActive(false);
                bocadilloPatrullar.gameObject.SetActive(false);
                bocadilloCavar.gameObject.SetActive(false);
                bocadilloComer.gameObject.SetActive(false);
                bocadilloCuidarHuevo.gameObject.SetActive(true);
                bocadilloAtacar.gameObject.SetActive(false);

                bocadilloCuidarHuevo.transform.position = hormigaSeleccionada.transform.position + offset;
            }
            else if (atacar == true)
            {
                bocadilloExplorar.gameObject.SetActive(false);
                bocadilloBuscarComida.gameObject.SetActive(false);
                bocadilloCurar.gameObject.SetActive(false);
                bocadilloPatrullar.gameObject.SetActive(false);
                bocadilloCavar.gameObject.SetActive(false);
                bocadilloComer.gameObject.SetActive(false);
                bocadilloCuidarHuevo.gameObject.SetActive(false);
                bocadilloAtacar.gameObject.SetActive(true);

                bocadilloAtacar.transform.position = hormigaSeleccionada.transform.position + offset;
            }
            else
            {
                bocadilloExplorar.gameObject.SetActive(false);
                bocadilloBuscarComida.gameObject.SetActive(false);
                bocadilloCurar.gameObject.SetActive(false);
                bocadilloPatrullar.gameObject.SetActive(false);
                bocadilloCavar.gameObject.SetActive(false);
                bocadilloComer.gameObject.SetActive(false);
                bocadilloCuidarHuevo.gameObject.SetActive(false);
                bocadilloAtacar.gameObject.SetActive(false);

                bocadilloExplorar.transform.position = Vector3.zero;
            }
        }
    }

    void Explorar()
    {
        explorar = true;
        buscarComida = false;
        curar = false;
        patrullar = false;
        cavar = false;
        comer = false;
        cuidarHuevos = false;
        atacar = false;
    }
    void BuscarComida()
    {
        explorar = false;
        buscarComida = true;
        curar = false;
        patrullar = false;
        cavar = false;
        comer = false;
        cuidarHuevos = false;
        atacar = false;
    }
    void Curar()
    {
        explorar = false;
        buscarComida = false;
        curar = true;
        patrullar = false;
        cavar = false;
        comer = false;
        cuidarHuevos = false;
        atacar = false;
    }
    void Patrullar()
    {
        explorar = false;
        buscarComida = false;
        curar = false;
        patrullar = true;
        cavar = false;
        comer = false;
        cuidarHuevos = false;
        atacar = false;
    }
    void Cavar()
    {
        explorar = false;
        buscarComida = false;
        curar = false;
        patrullar = false;
        cavar = true;
        comer = false;
        cuidarHuevos = false;
        atacar = false;
    }
    void Comer()
    {
        explorar = false;
        buscarComida = false;
        curar = false;
        patrullar = false;
        cavar = false;
        comer = true;
        cuidarHuevos = false;
        atacar = false;
    }
    void CuidarHuevos()
    {
        explorar = false;
        buscarComida = false;
        curar = false;
        patrullar = false;
        cavar = false;
        comer = false;
        cuidarHuevos = true;
        atacar = false;
    }
    void Atacar()
    {
        explorar = false;
        buscarComida = false;
        curar = false;
        patrullar = false;
        cavar = false;
        comer = false;
        cuidarHuevos = false;
        atacar = true;
    }
}
