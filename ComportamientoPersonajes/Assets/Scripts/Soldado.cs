using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Panda;

public class Soldado : HormigaGenerica
{
    #region Variables Soldado
    // Atacar
    // bool hayEnemigosCerca

    // Comer
    // float hambre
    // int reina.totalComida

    // Orden de la reina
    // bool meHanMandadoOrden
    // bool hayOrdenDeAtacar
    // bool hayOrdenCurarHormiga
    // bool hayOrdenBuscarComida
    public bool hayOrdenDePatrullar;

    // Curar A Una Hormiga
    // HormigaGenerica hormigaACurar
    // int tiempoParaCurar

    // Buscar Comida
    // Vector3 siguientePosicionBuscandoComida
    // Comida comida;
    // Room salaDejarComida = null;
    // posDejarComida = Vector3.zero;

    //Patrullar
    public float tiempoPatrullandoMax;
    public float tiempoActualPatrullando;
    public Vector3 posicionPatrullar;
    public int radio;

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
        reina.numeroDeSoldadosTotal++;
        reina.soldadosDesocupadas.Add(this);

        miSala = reina.MeterHormigaEnSala();

        // Prioridades NavMesh
        if (reina.contPrioridadNavMesh > 99)
        {
            reina.contPrioridadNavMesh = 1;
        }
        agente.avoidancePriority = reina.contPrioridadNavMesh;
        priority = reina.contPrioridadNavMesh;
        reina.contPrioridadNavMesh++;

        // Ataques y Vida
        totalVida = 15;
        this.vida = totalVida;
        this.umbralPuedeCurarse = 10;
        this.umbralNecesitaCurarse = 5;
        this.daño = 3;
        tiempoEntreAtaquesMax = 0.5f;
        this.tiempoEntreAtaques = tiempoEntreAtaquesMax;

        // Hambre
        hambre = 300 + Random.Range(0, 100);
        umbralHambre = 200;
        umbralHambreMaximo = 80;

        // Explorar
        siguientePosicionExplorar = Vector3.zero;

        // Curar
        tiempoParaCurar = 10.0f;

        // Patrullar
        tiempoActualPatrullando = 0;
        tiempoPatrullandoMax = 20.0f;
        posicionPatrullar = Vector3.zero;
        radio = 10;

        if (!bocadillosFound)
        {
            bocadillos = FindObjectOfType<BocadillosControlador>();
            if (bocadillos != null)
            {
                bocadillosFound = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!agente.isOnOffMeshLink)
        {
            // Si encuentras un enemigo y no está en la lista de enemigos
            if (other.tag == "Enemigo")
            {
                EnemigoGenerico aux = other.GetComponent<EnemigoGenerico>();
                // Actualizas al enemigo de que hay hormiga cerca
                if (!aux.hormigasCerca.Contains(this))
                {
                    aux.hormigasCerca.Add(this);
                }
                // Actualizas a la hormiga y avisas a la reina de este enemigo
                if (!enemigosCerca.Contains(aux))
                {
                    if (aux.hormigasAtacandole.Count < 2)
                    {
                        if (aux.hormigasAtacandole.Count == 0)
                        {
                            reina.RecibirAlertaEnemigo(aux);
                        }
                        else if (aux.hormigasAtacandole[0] == this)
                        {
                            reina.RecibirAlertaEnemigo(aux);
                        }
                    }
                    enemigosCerca.Add(aux);
                }
            }
            else if (other.tag == "Trigo")
            {
                Comida aux = other.gameObject.GetComponent<Comida>();
                if (!aux.hormigasCerca.Contains(this))
                {
                    aux.hormigasCerca.Add(this);
                }
                if (!comidaQueHayCerca.Contains(aux) && !aux.haSidoCogida && !aux.laEstanLLevando && aux.hormigaQueLlevaLaComida == null)
                {
                    reina.RecibirAlertaComida(aux);
                    comidaQueHayCerca.Add(aux);
                }
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
            enemigosCerca.Remove(aux);
        }
        else if (other.tag == "Trigo")
        {
            Comida aux = other.gameObject.GetComponent<Comida>();
            comidaQueHayCerca.Remove(aux);
            aux.hormigasCerca.Remove(this);
        }
        else if (other.tag == "Reina")
        {
            reinaCerca = false;
            Debug.Log("Reina cerca");
        }
    }

    #region Tareas Soldado

    // HayEnemigosCerca()
    // Atacar()
    // TengoMuchaHambre()
    // TengoHambre()
    // HayComida()
    // Comer()

    [Task]
    public void TengoOrdenDeLaReina()
    {
        if (hayOrdenDePatrullar || hayOrdenCurarHormiga || hayOrdenBuscarComida || hayOrdenDeAtacar)
        {
            Task.current.Succeed();
        }
        else
        {
            SacarDeOcupadas();
            Task.current.Fail();
        }
    }

    // TengoOrdenDeAtacar()
    // TengoOrdenDeCurarHormiga()
    // CurarHormiga()
    // TengoOrdenDeBuscarComida()
    // BuscarComida()

    [Task]
    public void TengoOrdenDePatrullar()
    {
        if (hayOrdenDePatrullar)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    /// <summary>
    /// Hay que setearle la posicion a patrullar cuando se le mande la orden
    /// </summary>
    [Task]
    public void Patrullar()
    {
        if (bocadillos.hormigaSeleccionada != null && bocadillos.hormigaSeleccionada == this)
        {
            bocadillos.Patrullar();
        }
        // si esta dentro
        if (zonaDondeEsta == 0)
        {
            // sale
            siguientePosicionExplorar = Vector3.zero;
            Vector3 randomDirection;
            NavMeshHit aux;
            bool aux2;
            do
            {
                randomDirection = UnityEngine.Random.insideUnitSphere * radio + posicionPatrullar;
                aux2 = NavMesh.SamplePosition(randomDirection, out aux, 1.0f, NavMesh.AllAreas);
            } while (!aux2);
            siguientePosicionExplorar = new Vector3(aux.position.x, 0, aux.position.z);
            //Debug.Log("Posicion a la que va: " + siguientePosicionExplorar);
            agente.SetDestination(siguientePosicionExplorar);
            Task.current.Succeed();
        }
        else
        {
            if (tiempoActualPatrullando < tiempoPatrullandoMax)
            {
                tiempoPatrullandoMax += Time.deltaTime;
                if (siguientePosicionExplorar == Vector3.zero)
                {
                    Vector3 randomDirection;
                    NavMeshHit aux;
                    bool aux2;
                    do
                    {
                        randomDirection = UnityEngine.Random.insideUnitSphere * (radio) + posicionPatrullar;
                        aux2 = NavMesh.SamplePosition(randomDirection, out aux, 4.0f, NavMesh.AllAreas);
                    } while (!aux2);
                    siguientePosicionExplorar = new Vector3(aux.position.x, 0, aux.position.z);
                    //Debug.Log("Posicion a la que va: " + siguientePosicionExplorar);
                    agente.SetDestination(siguientePosicionExplorar);
                }
                else if (Vector3.Distance(this.transform.position, siguientePosicionExplorar) < 0.5f)
                {
                    siguientePosicionExplorar = Vector3.zero;
                }
                else
                {
                    agente.SetDestination(siguientePosicionExplorar);
                }
                Task.current.Succeed();
            }
            else
            {
                Debug.Log("Acabe de patrullar");
                hayOrdenDePatrullar = false;
                tiempoActualPatrullando = 0;
                Task.current.Fail();
            }
        }
    }


// HayHormigaQueCurarCerca()

[Task]
public void HaceMuchoQueHabiaEnemigos()
{
    Task.current.Fail();
}

[Task]
public void Explorar()
{
    if (bocadillos.hormigaSeleccionada != null && bocadillos.hormigaSeleccionada == this)
    {
        if (!this.agente.isOnOffMeshLink)
        {
            bocadillos.Explorar();
        }
        else
        {
            bocadillos.Nada();
        }
    }
    // si esta dentro
    if (zonaDondeEsta == 0)
    {
        // sale
        siguientePosicionExplorar = Vector3.zero;
        Vector3 randomDirection;
        NavMeshHit aux;
        bool aux2;
        do
        {
            randomDirection = UnityEngine.Random.insideUnitSphere * 10 + reina.afueras.centro;
            aux2 = NavMesh.SamplePosition(randomDirection, out aux, 1.0f, NavMesh.AllAreas);
        } while (!aux2);
        siguientePosicionExplorar = new Vector3(aux.position.x, 0, aux.position.z);
        //Debug.Log("Posicion a la que va: " + siguientePosicionExplorar);
        agente.SetDestination(siguientePosicionExplorar);

    }
    else
    {
        if (siguientePosicionExplorar == Vector3.zero)
        {
            Vector3 randomDirection;
            NavMeshHit aux;
            bool aux2;
            do
            {
                randomDirection = UnityEngine.Random.insideUnitSphere * (10) + this.transform.position;
                aux2 = NavMesh.SamplePosition(randomDirection, out aux, 4.0f, NavMesh.AllAreas);
            } while (!aux2);
            siguientePosicionExplorar = new Vector3(aux.position.x, 0, aux.position.z);
            //Debug.Log("Posicion a la que va: " + siguientePosicionExplorar);
            agente.SetDestination(siguientePosicionExplorar);
        }
        else if (Vector3.Distance(this.transform.position, siguientePosicionExplorar) < 0.5f)
        {
            siguientePosicionExplorar = Vector3.zero;
        }
        else
        {
            agente.SetDestination(siguientePosicionExplorar);
        }

    }
    Task.current.Succeed();
}

    #endregion
}
