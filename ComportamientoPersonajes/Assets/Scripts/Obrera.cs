using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Panda;

public class Obrera : HormigaGenerica
{
    #region Variables Obrera
    // Atacar
    // bool hayEnemigosCerca
    public int numeroDeSoldadosCerca = 0;
    public bool reinaEstaCerca = false;

    // Comer
    // float hambre
    // int reina.totalComida

    // Orden de la reina
    // bool meHanMandadoOrden
    // bool hayOrdenDeAtacar;
    // bool hayOrdenCurarHormiga
    // bool hayOrdenBuscarComida
    // bool hayOrdenDeCavar = false;

    // Curar A Una Hormiga
    // HormigaGenerica hormigaACurar
    // int tiempoParaCurar
    // List<HormigaGenerica> hormigasCerca = new List<HormigaGenerica>();

    // Buscar Comida
    // Vector3 siguientePosicionBuscandoComida
    // Comida comida;
    // Room salaDejarComida = null;
    // posDejarComida = Vector3.zero;

    // Cavar
    public int tiempoParaHacerTunel;
    public Vector3 posicionInicialTunel;
    public Vector3 posicionFinalTunel;

    // Comer
    // Comida comidaAComer

    // Explorar
    // Vector3 siguientePosicionExplorar

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Inicialización
        this.zonaDondeEsta = 0;

        // Respecto al hormiguero
        hormigueroDentro = GameObject.FindObjectOfType<Floor>();
        hormigueroFuera = GameObject.FindObjectOfType<Outside>();
        reina = GameObject.FindObjectOfType<Reina>();
        pb = this.gameObject.GetComponent<PandaBehaviour>();
        agente = this.gameObject.GetComponent<NavMeshAgent>();
        reina.totalHormigas++;
        reina.numeroDeObrerasTotal++;
        reina.obrerasDesocupadas.Add(this);

        miSala = reina.meterHormigaEnSala();

        // Prioridades NavMesh
        if (reina.contPrioridadNavMesh > 99)
        {
            reina.contPrioridadNavMesh = 0;
        }
        agente.avoidancePriority = reina.contPrioridadNavMesh;
        reina.contPrioridadNavMesh++;

        // Ataques y Vida
        this.vida = 10;
        this.daño = 2;
        tiempoEntreAtaquesMax = 0.5f;
        this.tiempoEntreAtaques = tiempoEntreAtaquesMax;

        // Hambre
        hambre = 300;
        umbralHambre = 200;
        umbralHambreMaximo = 80;

        // Explorar
        siguientePosicionExplorar = this.transform.position;
    }

    // Update is called once per frame
    /*void Update()
    {
        actualizarHambre();
    }*/

    private void OnTriggerEnter(Collider other)
    {
        // Si encuentras un enemigo y no está en la lista de enemigos
        if (other.tag == "Enemigo")
        {
            numeroDeSoldadosCerca = GameObject.FindGameObjectsWithTag("Soldado").Length;

            EnemigoGenerico aux = other.GetComponent<EnemigoGenerico>();
            aux.hormigasCerca.Add(this);
            if (!enemigosCerca.Contains(aux))
            {
                reina.recibirAlertaEnemigo(aux);
                enemigosCerca.Add(aux);
            }
        }
        else if (other.tag == "Trigo")
        {
            if (comida == null)
            {
                Comida aux = other.GetComponent<Comida>();
                if (!aux.laEstanLLevando && !aux.haSidoCogida)
                {
                    reina.recibirAlertaComida(aux);
                    comida = aux;
                    aux.laEstanLLevando = true;
                }

            }
            else
            {
                reina.recibirAlertaComida(other.GetComponent<Comida>());
            }
        }
        else if (other.tag == "Reina")
        {
            reinaCerca = true;
        }
        else if (other.tag == "Nurse" || other.tag == "Obrera" || other.tag == "Soldado")
        {
            HormigaGenerica aux = other.GetComponent<HormigaGenerica>();
            if (!hormigasCerca.Contains(aux))
            {
                hormigasCerca.Add(aux);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Si un enemigo sale de nuestro collider, y estabamos luchando con el actualizar la lista
        if (other.tag == "Enemigo")
        {
            EnemigoGenerico aux = other.GetComponent<EnemigoGenerico>();
            aux.hormigasCerca.Remove(this);
            if (enemigosCerca.Contains(aux))
            {
                enemigosCerca.Remove(aux);
            }
            // Si no hay más enemigos
            if (enemigosCerca.Count == 0)
            {
                siguientePosicionExplorar = this.transform.position;
            }
        }
        else if (other.tag == "Trigo")
        {
            if (comida == other)
            {
                comida = null;
            }
        }
        else if (other.tag == "Reina")
        {
            reinaCerca = false;
            Debug.Log("Reina cerca");
        }
        else if (other.tag == "Nurse" || other.tag == "Obrera" || other.tag == "Soldado")
        {
            HormigaGenerica aux = other.GetComponent<HormigaGenerica>();
            if (hormigasCerca.Contains(aux))
            {
                hormigasCerca.Remove(aux);
            }
        }
    }

    #region Tareas Obrera

    // HayEnemigosCerca()

    [Task]
    public void HaySoldadosCerca()
    {
        if (numeroDeSoldadosCerca > 0)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    // ReinaEnPeligro()
    // Atacar()
    // Huir()
    // TengoMuchaHambre()
    // TengoHambre()
    // HayComida()
    // Comer()

    [Task]
    public void TengoOrdenDeLaReina()
    {
        if (hayOrdenDeCavar || hayOrdenCurarHormiga || hayOrdenBuscarComida || hayOrdenDeAtacar)
        {
            Task.current.Succeed();
        }
        else
        {
            if (reina.obrerasOcupadas.Contains(this))
            {
                reina.obrerasOcupadas.Remove(this);
                reina.obrerasDesocupadas.Add(this);
            }
            Task.current.Fail();
        }
    }

    // TengoOrdenDeAtacar()
    // TengoOrdenDeCurarHormiga()
    // CurarHormiga()
    // TengoOrdenDeBuscarComida()
    // BuscarComida()

    [Task]
    public void TengoOrdenDeCavar()
    {
        if (hayOrdenDeCavar)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void Cavar()
    {
        Task.current.Fail();
    }

    // HayHormigaQueCurarCerca()

    [Task]
    public void HaySuficienteComida()
    {
        //Debug.Log(reina.umbralComida * reina.totalHormigas);
        if (reina.totalComida < reina.umbralComida * reina.totalHormigas)
        {
            Task.current.Fail();
        }
        else
        {
            Task.current.Succeed();
        }
    }

    [Task]
    public void Explorar()
    {
        if (this.zonaDondeEsta == 1)
        {
            //Debug.Log("Estoy fuera");
            float distanceToTarget = Vector3.Distance(transform.position, siguientePosicionExplorar);
            //Debug.Log(distanceToTarget);
            if (distanceToTarget < 0.2f)
            {
                Vector3 randomDirection;
                NavMeshHit aux;
                do
                {
                    randomDirection = UnityEngine.Random.insideUnitSphere * 10 + this.transform.position;
                } while (!NavMesh.SamplePosition(randomDirection, out aux, 1.0f, NavMesh.AllAreas));
                //saliendo = true;
                agente.SetDestination(aux.position);
                siguientePosicionExplorar = aux.position;
            }
            //
        }
        else
        {
            //Debug.Log("Estoy dentro");
            if (this.zonaDondeEsta == 0)
            {
                //Debug.Log("No estoy saliendo");
                Vector3 randomDirection;
                NavMeshHit aux;
                bool aux2;
                do
                {
                    randomDirection = UnityEngine.Random.insideUnitSphere * 100 + this.transform.position;
                    aux2 = NavMesh.SamplePosition(randomDirection, out aux, 1.0f, NavMesh.AllAreas);
                } while (aux.position.x > (hormigueroDentro.transform.position.x - (hormigueroDentro.width / 2)) || !aux2);
                //Debug.Log("Salir hacia: " + aux.position);
                //saliendo = true;
                agente.SetDestination(aux.position);
                siguientePosicionExplorar = aux.position;
            }
            else if (this.zonaDondeEsta == 3)
            {
                //Debug.Log("Estoy saliendo");
                if ((transform.position.x - (hormigueroFuera.transform.position.x + hormigueroFuera.width / 2) < 2f))
                {
                    //saliendo = false;
                    //estaDentro = false;
                }
            }
        }
        Task.current.Succeed();
    }

    #endregion
}
