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
    public SpriteRenderer bocadilloPonerHuevos;
    public SpriteRenderer bocadilloEstarHerido;

    bool explorar = false;
    bool buscarComida = false;
    bool curar = false;
    bool patrullar = false;
    bool cavar = false;
    bool comer = false;
    bool cuidarHuevos = false;
    bool atacar = false;
    bool ponerHuevos = false;
    bool estarHerido = false;

    Vector3 offset = new Vector3(1.5f, 1f, 1.5f);


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
        bocadilloPonerHuevos.gameObject.SetActive(false);
        bocadilloEstarHerido.gameObject.SetActive(false);
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
                bocadilloPonerHuevos.gameObject.SetActive(false);
                bocadilloEstarHerido.gameObject.SetActive(false);

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
                bocadilloPonerHuevos.gameObject.SetActive(false);
                bocadilloEstarHerido.gameObject.SetActive(false);

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
                bocadilloPonerHuevos.gameObject.SetActive(false);
                bocadilloEstarHerido.gameObject.SetActive(false);

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
                bocadilloPonerHuevos.gameObject.SetActive(false);
                bocadilloEstarHerido.gameObject.SetActive(false);

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
                bocadilloPonerHuevos.gameObject.SetActive(false);
                bocadilloEstarHerido.gameObject.SetActive(false);

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
                bocadilloPonerHuevos.gameObject.SetActive(false);
                bocadilloEstarHerido.gameObject.SetActive(false);

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
                bocadilloPonerHuevos.gameObject.SetActive(false);
                bocadilloEstarHerido.gameObject.SetActive(false);

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
                bocadilloPonerHuevos.gameObject.SetActive(false);
                bocadilloEstarHerido.gameObject.SetActive(false);

                bocadilloAtacar.transform.position = hormigaSeleccionada.transform.position + offset;
            }
            else if (ponerHuevos == true)
            {
                bocadilloExplorar.gameObject.SetActive(false);
                bocadilloBuscarComida.gameObject.SetActive(false);
                bocadilloCurar.gameObject.SetActive(false);
                bocadilloPatrullar.gameObject.SetActive(false);
                bocadilloCavar.gameObject.SetActive(false);
                bocadilloComer.gameObject.SetActive(false);
                bocadilloCuidarHuevo.gameObject.SetActive(false);
                bocadilloAtacar.gameObject.SetActive(false);
                bocadilloPonerHuevos.gameObject.SetActive(true);
                bocadilloEstarHerido.gameObject.SetActive(false);

                bocadilloPonerHuevos.transform.position = hormigaSeleccionada.transform.position + offset;
            }
            else if (estarHerido == true)
            {
                bocadilloExplorar.gameObject.SetActive(false);
                bocadilloBuscarComida.gameObject.SetActive(false);
                bocadilloCurar.gameObject.SetActive(false);
                bocadilloPatrullar.gameObject.SetActive(false);
                bocadilloCavar.gameObject.SetActive(false);
                bocadilloComer.gameObject.SetActive(false);
                bocadilloCuidarHuevo.gameObject.SetActive(false);
                bocadilloAtacar.gameObject.SetActive(false);
                bocadilloPonerHuevos.gameObject.SetActive(false);
                bocadilloEstarHerido.gameObject.SetActive(true);

                bocadilloEstarHerido.transform.position = hormigaSeleccionada.transform.position + offset;
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
                bocadilloPonerHuevos.gameObject.SetActive(false);
                bocadilloEstarHerido.gameObject.SetActive(false);

                bocadilloExplorar.transform.position = Vector3.zero;
            }
        } else
        {
            bocadilloExplorar.gameObject.SetActive(false);
            bocadilloBuscarComida.gameObject.SetActive(false);
            bocadilloCurar.gameObject.SetActive(false);
            bocadilloPatrullar.gameObject.SetActive(false);
            bocadilloCavar.gameObject.SetActive(false);
            bocadilloComer.gameObject.SetActive(false);
            bocadilloCuidarHuevo.gameObject.SetActive(false);
            bocadilloAtacar.gameObject.SetActive(false);
            bocadilloPonerHuevos.gameObject.SetActive(false);
            bocadilloEstarHerido.gameObject.SetActive(false);

            bocadilloExplorar.transform.position = Vector3.zero;
        }
    }

    public void Explorar()
    {
        explorar = true;
        buscarComida = false;
        curar = false;
        patrullar = false;
        cavar = false;
        comer = false;
        cuidarHuevos = false;
        atacar = false;
        ponerHuevos = false;
        estarHerido = false;
    }
    public void BuscarComida()
    {
        explorar = false;
        buscarComida = true;
        curar = false;
        patrullar = false;
        cavar = false;
        comer = false;
        cuidarHuevos = false;
        atacar = false;
        ponerHuevos = false;
        estarHerido = false;
    }
    public void Curar()
    {
        explorar = false;
        buscarComida = false;
        curar = true;
        patrullar = false;
        cavar = false;
        comer = false;
        cuidarHuevos = false;
        atacar = false;
        ponerHuevos = false;
        estarHerido = false;
    }
    public void Patrullar()
    {
        explorar = false;
        buscarComida = false;
        curar = false;
        patrullar = true;
        cavar = false;
        comer = false;
        cuidarHuevos = false;
        atacar = false;
        ponerHuevos = false;
        estarHerido = false;
    }
    public void Cavar()
    {
        explorar = false;
        buscarComida = false;
        curar = false;
        patrullar = false;
        cavar = true;
        comer = false;
        cuidarHuevos = false;
        atacar = false;
        ponerHuevos = false;
        estarHerido = false;
    }
    public void Comer()
    {
        explorar = false;
        buscarComida = false;
        curar = false;
        patrullar = false;
        cavar = false;
        comer = true;
        cuidarHuevos = false;
        atacar = false;
        ponerHuevos = false;
        estarHerido = false;
    }
    public void CuidarHuevos()
    {
        explorar = false;
        buscarComida = false;
        curar = false;
        patrullar = false;
        cavar = false;
        comer = false;
        cuidarHuevos = true;
        atacar = false;
        ponerHuevos = false;
        estarHerido = false;
    }
    public void Atacar()
    {
        explorar = false;
        buscarComida = false;
        curar = false;
        patrullar = false;
        cavar = false;
        comer = false;
        cuidarHuevos = false;
        atacar = true;
        ponerHuevos = false;
        estarHerido = false;
    }

    public void PonerHuevos()
    {
        explorar = false;
        buscarComida = false;
        curar = false;
        patrullar = false;
        cavar = false;
        comer = false;
        cuidarHuevos = false;
        atacar = false;
        ponerHuevos = true;
        estarHerido = false;
    }

    public void EstaHerido()
    {
        explorar = false;
        buscarComida = false;
        curar = false;
        patrullar = false;
        cavar = false;
        comer = false;
        cuidarHuevos = false;
        atacar = false;
        ponerHuevos = false;
        estarHerido = true;
    }

    public void Nada()
    {
        explorar = false;
        buscarComida = false;
        curar = false;
        patrullar = false;
        cavar = false;
        comer = false;
        cuidarHuevos = false;
        atacar = false;
        ponerHuevos = false;
        estarHerido = false;
    }

    
}
