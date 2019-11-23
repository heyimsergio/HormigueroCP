using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using UnityEngine.AI;

public class Nurse : HormigaGenerica
{
    #region Variables Nurse
    // Atacar
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

    // Cuidar de huevos
    // float tiempoCuidandoHuevos = 20.0f;
    // Huevo huevoACuidar = null;
    // Vector3 posHuevo = Vector3.zero;
    // float TiempoActual;

    // Curar A Una Hormiga
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

        // Curar Hormigas
        tiempoParaCurar = 10.0f;

        // Explorar
        siguientePosicionExplorar = Vector3.zero;

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
                reina.RecibirAlertaEnemigo(aux);
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
            if (!aux.haSidoCogida && !aux.laEstanLLevando && aux.hormigaQueLlevaLaComida == null)
            {
                reina.RecibirAlertaComida(aux);
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
            Comida aux = other.gameObject.GetComponent<Comida>();
            aux.hormigasCerca.Remove(this);
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
            return;
        }
        else
        {
            /*if (reina.nursesOcupadas.Contains(this))
            {
                reina.nursesOcupadas.Remove(this);
                reina.nursesDesocupadas.Add(this);
            }*/
            Task.current.Fail();
            return;
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
                //Debug.Log("Se asigna la posicion del huevo a curar");
                TiempoActual = tiempoCuidandoHuevos;
                posHuevo = huevoACuidar.transform.position;
                agente.SetDestination(huevoACuidar.transform.position);
                Task.current.Succeed();
                return;
            }
            // Cuando la distancia al huevo sea pequeña
            if (Vector3.Distance(this.transform.position, posHuevo) < 0.2)
            {
                TiempoActual -= Time.deltaTime;
                // Si ha pasado el tiempo de cuidar
                if (TiempoActual <= 0)
                {
                    huevoACuidar.Cuidar();
                    //Debug.Log("Huevo Cuidado");
                    // Reseteas todos los valores
                    TiempoActual = tiempoCuidandoHuevos;
                    posHuevo = Vector3.zero;
                    huevoACuidar.siendoCuidadoPor = null;
                    huevoACuidar = null;
                    // Si se trataba de una orden de cuidar huevos
                    if (hayOrdenCuidarHuevos == true)
                    {
                        hayOrdenCuidarHuevos = false;
                        SacarDeOcupadas();
                    }
                }
            }
            else
            {
                agente.SetDestination(huevoACuidar.transform.position);
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
                SacarDeOcupadas();
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
                        //Debug.Log("Hay Huevo Cerca que puede ser o necesita cuidados");
                        return;
                    }
                }
            }
            else
            {
                Task.current.Succeed();
                return;
            }
        }

        // Si no encuentra huevo, o tiene orden
        Task.current.Fail();
    }

    // HayHormigaQueCurarCerca()

    [Task]
    public void Explorar()
    {
        if (bocadillos.hormigaSeleccionada != null && bocadillos.hormigaSeleccionada == this)
        {
            bocadillos.Explorar();
        }
        if (zonaDondeEsta == 1)
        {
            siguientePosicionExplorar = Vector3.zero;
            Vector3 randomDirection;
            NavMeshHit aux;
            bool aux2;
            do
            {
                randomDirection = UnityEngine.Random.insideUnitSphere * 10 + reina.hormiguero.centro;
                aux2 = NavMesh.SamplePosition(randomDirection, out aux, 1.0f, NavMesh.AllAreas);
            } while (!aux2);
            siguientePosicionExplorar = new Vector3(aux.position.x, 0, aux.position.z);
            //Debug.Log("Posicion a la que va: " + siguientePosicionExplorar);
            agente.SetDestination(siguientePosicionExplorar);
        }
        else
        {
            if(siguientePosicionExplorar == Vector3.zero)
            {
                Vector3 randomDirection;
                NavMeshHit aux;
                bool aux2;
                do
                {
                    randomDirection = UnityEngine.Random.insideUnitSphere * (reina.hormiguero.heigth/2-5) + reina.hormiguero.centro;
                    aux2 = NavMesh.SamplePosition(randomDirection, out aux, 4.0f, NavMesh.AllAreas);
                } while (!aux2);
                siguientePosicionExplorar = new Vector3(aux.position.x,0,aux.position.z);
                //Debug.Log("Posicion a la que va: " + siguientePosicionExplorar);
                agente.SetDestination(siguientePosicionExplorar);
            }
            else if(Vector3.Distance(this.transform.position,siguientePosicionExplorar)< 0.5f)
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
