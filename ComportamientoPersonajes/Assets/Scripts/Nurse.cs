using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using UnityEngine.AI;

public class Nurse : HormigaGenerica
{
    #region Variables Nurse
    //Atacar
    // List<EnemigoGenerico> enemigosCerca
    // List<Huevo> huevosCerca = new List<Huevo>();
    // int numeroDeObrerasCerca = 0;
    // int numeroDeSoldadosCerca = 0;
    // bool reinaCerca = false;

    // Comer
    // float hambre
    // int reina.totalComida

    // Orden de la reina
    // bool meHanMandadoOrden
    // bool hayOrdenCuidarHuevos = false;
    // bool hayOrdenCurarHormiga
    // bool hayOrdenBuscarComida

    //Cuidar de huevos
    // float tiempoCuidandoHuevos = 20.0f;
    // Huevo huevoACuidar = null;
    // Vector3 posHuevo = Vector3.zero;
    // float TiempoActual;

    //Curar A Una Hormiga
    // HormigaGenerica hormigaACurar
    // int tiempoParaCurar
    // List<HormigaGenerica> hormigasCerca = new List<HormigaGenerica>();

    // Buscar Comida
    // Vector3 siguientePosicionBuscandoComida
    // Comida comida;
    // Room salaDejarComida = null;
    // posDejarComida = Vector3.zero;

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
        reina.numeroDeNursesTotal++;
        reina.nursesDesocupadas.Add(this);

        miSala = reina.meterHormigaEnSala();

        // Prioridades NavMesh
        if (reina.contPrioridadNavMesh > 99)
        {
            reina.contPrioridadNavMesh = 1;
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

        // Cuidar huevos
        tiempoCuidandoHuevos = 10.0f;
        TiempoActual = tiempoCuidandoHuevos;

        // Explorar
        siguientePosicionExplorar = this.transform.position;
    }

    /*private void Update()
    {
    }*/

    private void OnTriggerEnter(Collider other)
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
        else if (other.tag == "Huevo")
        {
            Huevo aux = other.GetComponent<Huevo>();
            // Actualizas al huevo de las hormigas que tiene cerca
            if (!aux.hormigasCerca.Contains(this))
            {
                aux.hormigasCerca.Add(this);
            }
            // Actualizas la lista de huevos cerca
            if (!huevosCerca.Contains(aux))
            {
                huevosCerca.Add(aux);
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
            if (comida == other)
            {
                comida = null;
            }
        }
        else if (other.tag == "Huevo")
        {
            Huevo aux = other.GetComponent<Huevo>();
            huevosCerca.Remove(aux);
            aux.hormigasCerca.Remove(this);
        }
    }

    #region Tareas Nurse

    // HayEnemigosCerca()

    [Task]
    public void HayHuevosCerca()
    {
        if (huevosCerca.Count != 0)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void HayObrerasOSoldadosCerca()
    {
        if (obrerasCerca || soldadosCerca)
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
        if (hayOrdenCuidarHuevos || hayOrdenCurarHormiga || hayOrdenBuscarComida)
        {
            Task.current.Succeed();
        }
        else
        {
            if (reina.nursesOcupadas.Contains(this))
            {
                reina.nursesOcupadas.Remove(this);
                reina.nursesDesocupadas.Add(this);
            }
            Task.current.Fail();
        }
    }

    [Task]
    public void TengoOrdenDeCuidarHuevos()
    {
        if (hayOrdenCuidarHuevos)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void CuidarHuevos()
    {
        if (huevoACuidar != null)
        {
            // Si es la primera vez, no tengo asignada la posicion del huevo
            if (posHuevo == Vector3.zero)
            {
                Debug.Log("Se asigna la posicion del huevo a curar");
                posHuevo = huevoACuidar.transform.position;
                agente.SetDestination(huevoACuidar.transform.position);
                Task.current.Succeed();
                return;
            }
            // Cuando la distancia al huevo sea pequeña
            if (Vector3.Distance(this.transform.position, posHuevo) < 0.2)
            {
                TiempoActual -= Time.deltaTime;
                // Si ha pasado el tiempo de cura
                if (TiempoActual <= 0)
                {
                    huevoACuidar.Cuidar();
                    Debug.Log("Huevo Cuidado");
                    // Reseteas todos los valores
                    TiempoActual = tiempoCuidandoHuevos;
                    posHuevo = Vector3.zero;
                    huevoACuidar.siendoCuidadoPor = null;
                    huevoACuidar = null;
                    // Si se trataba de una orden de cuidar huevos
                    if (hayOrdenCuidarHuevos == true)
                    {
                        hayOrdenCuidarHuevos = false;
                    }
                }
            }
            Task.current.Succeed();
            return;
        }
        // Si el huevo ha muerto o nacido, se devuelve Fail para que siga haciendo cosas del BT
        else
        {
            TiempoActual = tiempoCuidandoHuevos;
            huevoACuidar = null;
            posHuevo = Vector3.zero;
            if (hayOrdenCuidarHuevos == true)
            {
                hayOrdenCuidarHuevos = false;
            }
            Task.current.Fail();
            return;
        }
    }

    // TengoOrdenDeCurarHormiga()
    // CurarHormiga()
    // TengoOrdenDeBuscarComida()
    // BuscarComida()

    [Task]
    public void HayHuevosCercaQueNecesitanCuidados()
    {
        if (hayOrdenCuidarHuevos == false)
        {
            // Si no tengo ningun huevo asignado, miro los que hay alrededor
            if (huevoACuidar == null)
            {
                // Recorremos la lista de huevos cercanos
                foreach (Huevo h in huevosCerca)
                {
                    // Si algun huevo PUEDE ser cuidado y no tiene a nadie asignado, se lo asigno e indico al huevo quien lo cuida
                    if (h.siendoCuidadoPor == null && h.puedeSerCuidado)
                    {
                        huevoACuidar = h;
                        h.siendoCuidadoPor = this;
                        // Si la reina lo tiene en su lista de huevos que necesitan cuidados, lo borro
                        reina.huevosQueTienenQueSerCuidados.Remove(huevoACuidar);
                        Task.current.Succeed();
                        Debug.Log("Hay Huevo Cerca que puede ser o necesita cuidados");
                        return;
                    }
                }
            }
        }

        // Si no encuentra huevo, o tiene orden
        Task.current.Fail();
    }

    // HayHormigaQueCurarCerca()

    [Task]
    public void Explorar()
    {
        if (this.zonaDondeEsta == 1)
        {
            //Esta fuera, tiene que entrar
            //Debug.Log("Estoy fuera ais que tengo que entrar");
            Vector3 randomDirection;
            NavMeshHit aux;
            bool aux2;
            do
            {
                randomDirection = UnityEngine.Random.insideUnitSphere * 100 + this.transform.position;
                aux2 = NavMesh.SamplePosition(randomDirection, out aux, 1.0f, NavMesh.AllAreas);
            } while (aux.position.x < (hormigueroDentro.transform.position.x - (hormigueroDentro.width / 2)) || !aux2);
            //Debug.Log("Salir hacia: " + aux.position);
            //saliendo = true;
            agente.SetDestination(aux.position);
            siguientePosicionExplorar = aux.position;
            //
        }
        else
        {
            //
            if (this.zonaDondeEsta == 0)
            {
                //Debug.Log("Estoy dentro, asi que exploro " + siguientePosicionExplorar + " " + transform.position);
                float distanceToTarget = Vector3.Distance(this.transform.position, siguientePosicionExplorar);
                //Debug.Log(distanceToTarget);
                if (distanceToTarget < 1.8f)
                {
                    //Debug.Log("Esta cerca");
                    Vector3 randomDirection;
                    NavMeshHit aux;
                    bool aux2;
                    do
                    {
                        randomDirection = UnityEngine.Random.insideUnitSphere * 10 + this.transform.position;
                        aux2 = NavMesh.SamplePosition(randomDirection, out aux, 1.0f, NavMesh.AllAreas);
                    } while (!aux2 || (aux.position.x < (hormigueroDentro.transform.position.x - (hormigueroDentro.width / 2))));
                    //saliendo = true;
                    agente.SetDestination(aux.position);
                    siguientePosicionExplorar = aux.position;
                }
                else
                {
                    //Debug.Log("Esta lejos, voy hacia el");
                    agente.SetDestination(siguientePosicionExplorar);

                }
            }
        }
        Task.current.Succeed();
    }

    #endregion
}
