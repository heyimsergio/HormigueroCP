using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using UnityEngine.AI;

public class HormigaGenerica : PersonajeGenerico
{
    // Atributos de las hormigas generales
    protected float hambre = 200;
    public float pesoQuePuedenTransportar;
    protected Vector3 posicionReina;

    //Agente Navmesh
    public NavMeshAgent agente;
    public PandaBehaviour pb;
    public Floor hormigueroDentro; //Saber donde empieza el suelo para no meterte dentro del hormiguero cuando exploras
    public Outside hormigueroFuera;
    //bool estaDentro = true; //True: está dentro, false: esta fuera
    // bool saliendo = false;

    // Atacar
    public bool hayEnemigosCerca = false;
    public List<EnemigoGenerico> enemigosCerca = new List<EnemigoGenerico>();

    //Explorar
    public Vector3 siguientePosicionExplorar;

    // Comer
    public Comida comidaAComer;

    // Reina
    public Reina reina;

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

    // Tareas Panda
    [Task]
    public void HayEnemigosCerca()
    {
        if (hayEnemigosCerca)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void Atacar()
    {
        EnemigoGenerico enemigo = enemigosCerca[0];
        if (enemigo != null)
        {
            //Debug.Log("Hay enemigo");
            agente.SetDestination(enemigo.transform.position);
            float distanceToTarget = Vector3.Distance(transform.position, enemigo.transform.position);
            //Debug.Log(distanceToTarget);
            if (distanceToTarget < 1.2f)
            {
                //Debug.Log("Quitar vida");
                float random = Random.Range(0, 10);
                if (random < 9f)
                {
                    enemigo.quitarVida(this.daño);
                }
                else
                {
                    Debug.Log("Ataque fallido");
                }
            }
            else
            {
                //Debug.Log("no estoy a rango para pegarle");
            }
        }
        else
        {
            //Debug.Log("No hay enemigo");
            enemigosCerca.RemoveAt(0);
            if (enemigosCerca.Count == 0)
            {
                hayEnemigosCerca = false;
                siguientePosicionExplorar = this.transform.position;
            }
        }
        Task.current.Succeed();

    }

    [Task]
    public void TengoHambre()
    {
        //Debug.Log("Hola");
        //Debug.Log(this.hambre + " : hambre");
        if (this.hambre <= 75)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void TengoMuchaHambre()
    {
        if (this.hambre <= 30)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void HayComida()
    {
        if (reina.totalComida <= 0)
        {
            Task.current.Fail();
        }
        else
        {
            Task.current.Succeed();
        }
    }

    [Task]
    public void ReinaEnPeligro()
    {
        if (reina.hayEnemigosCerca == true)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void Comer()
    {
        Debug.Log("comer");
        if (comidaAComer == null)
        {
            Debug.Log("Tengo comida");
            comidaAComer = reina.pedirComida();
            if (comidaAComer != null)
            {
                agente.SetDestination(comidaAComer.transform.position);
            }
            else
            {
                Task.current.Fail();
            }
        }
        else
        {
            if (Vector3.Distance(this.transform.position, comidaAComer.transform.position) < 0.2f)
            {
                Debug.Log("He llegado a la comida");
                hambre += comidaAComer.comer();
                if (comidaAComer.usosDeLaComida == 0)
                {
                    reina.sacarComidaSala(comidaAComer.misala, comidaAComer);
                    Destroy(comidaAComer.gameObject);
                    comidaAComer = null;
                    Debug.Log("Comida destruida");
                    Task.current.Succeed();
                }

            }
        }

    }

    [Task]
    public void Huir()
    {
        Task.current.Succeed();
        Debug.Log("Huir");
    }

}
