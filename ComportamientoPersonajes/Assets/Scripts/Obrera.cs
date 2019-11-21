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
    // int numeroDeSoldadosCerca = 0;

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
    [Header ("CAVAR")]
    public int tiempoParaHacerTunel;
    public float tiempoQueLlevaHaciendoElTunel;
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
        tiempoQueLlevaHaciendoElTunel = 0;
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
            Comida aux = other.gameObject.GetComponent<Comida>();
            if (!aux.hormigasCerca.Contains(this))
            {
                aux.hormigasCerca.Add(this);
            }
            if (!comidaQueHayCerca.Contains(aux) && !aux.haSidoCogida && aux.hormigaQueLlevaLaComida == null)
            {
                reina.recibirAlertaComida(aux);
                comidaQueHayCerca.Add(aux);
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

    #region Tareas Obrera

    // HayEnemigosCerca()

    [Task]
    public void HaySoldadosCerca()
    {
        if (soldadosCerca)
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
        if(tiempoQueLlevaHaciendoElTunel < tiempoParaHacerTunel)
        {
            tiempoQueLlevaHaciendoElTunel += Time.deltaTime;
            agente.SetDestination(reina.hormiguero.gameObject.transform.position + new Vector3(Random.Range(0, reina.hormiguero.width), 0, Random.Range(0, reina.hormiguero.heigth)));
            Task.current.Succeed();
            return;
        }
        else
        {
            int min = -1;


            int capacidadRestanteHormigas = reina.capacidadTotalDeHormigas - reina.totalHormigas;
            int capacidadRestanteComida = reina.capacidadTotalDeComida - reina.ComidaTotal.Count;
            int capacidadRestanteHuevos = reina.capacidadTotalDeHuevos - reina.totalHuevos;


            if (reina.HayQueCrearSalasComida && reina.HayQueCrearSalasHuevos && reina.HayQueCrearSalasHormigas)
            {
                min = Reina.CompareLess3(capacidadRestanteHormigas, capacidadRestanteComida, capacidadRestanteHuevos, reina.importanciaHormigas, reina.importanciaComida, reina.importanciaHuevos);
            }
            else if (reina.HayQueCrearSalasHormigas && reina.HayQueCrearSalasHuevos)
            {
                min = Reina.CompareLess3(capacidadRestanteHormigas, capacidadRestanteComida, capacidadRestanteHuevos, reina.importanciaHormigas, reina.importanciaComida, 0);
            }
            else if (reina.HayQueCrearSalasHormigas && reina.HayQueCrearSalasComida)
            {
                min = Reina.CompareLess3(capacidadRestanteHormigas, capacidadRestanteComida, capacidadRestanteHuevos, reina.importanciaHormigas, 0, reina.importanciaHuevos);
            }
            else if (reina.HayQueCrearSalasComida && reina.HayQueCrearSalasHuevos)
            {
                min = Reina.CompareLess3(capacidadRestanteHormigas, capacidadRestanteComida, capacidadRestanteHuevos, 0, reina.importanciaComida, reina.importanciaHuevos);
            }
            else if (reina.HayQueCrearSalasHormigas)
            {
                min = 0;
            }
            else if (reina.HayQueCrearSalasComida)
            {
                min = 1;
            }
            else if (reina.HayQueCrearSalasHuevos)
            {
                min = 2;
            }

            Room aux;
            switch (min)
            {
                // no es necesario crear ninguna Sala;
                case -1:
                    Task.current.Fail();
                    break;
                case 0:
                    aux = reina.hormiguero.createCorridor(Room.roomType.LIVEROOM);
                    if (aux != null)
                    {
                        reina.capacidadTotalDeHormigas += aux.capacidadTotalRoom;
                        reina.salasHormigas.Add(aux);
                        reina.HayQueCrearSalasHormigas = false;
                        //Debug.Log("Sala de Hormigas creada, la capacidad ahora es: " + capacidadTotalDeHormigas);
                        tiempoQueLlevaHaciendoElTunel = 0;
                        hayOrdenDeCavar = false;
                        reina.numHormigasCavandoTuneles--;
                        Task.current.Succeed();
                    }
                    else
                    {
                        reina.espacioLlenoHormiguero = true;
                        tiempoQueLlevaHaciendoElTunel = 0;
                        hayOrdenDeCavar = false;
                        reina.numHormigasCavandoTuneles--;
                        Task.current.Fail();
                    }
                    break;
                case 1:

                    aux = reina.hormiguero.createCorridor(Room.roomType.STORAGE);
                    if (aux != null)
                    {
                        reina.salasComida.Add(aux);
                        reina.capacidadTotalDeComida += aux.capacidadTotalRoom;
                        reina.HayQueCrearSalasComida = false;
                        //Debug.Log("Sala de Comida creada, la capacidad ahora es: " + capacidadTotalDeComida);
                        tiempoQueLlevaHaciendoElTunel = 0;
                        hayOrdenDeCavar = false;
                        reina.numHormigasCavandoTuneles--;
                        Task.current.Succeed();
                    }
                    else
                    {
                        reina.espacioLlenoHormiguero = true;
                        tiempoQueLlevaHaciendoElTunel = 0;
                        hayOrdenDeCavar = false;
                        reina.numHormigasCavandoTuneles--;
                        Task.current.Fail();
                    }
                    break;
                case 2:

                    aux = reina.hormiguero.createCorridor(Room.roomType.STORAGE);

                    if (aux != null)
                    {
                        reina.salasHuevos.Add(aux);
                        reina.capacidadTotalDeHuevos += aux.capacidadTotalRoom;
                        reina.HayQueCrearSalasHuevos = false;
                        //Debug.Log("Sala de Huevos creada, la capacidad ahora es: " + capacidadTotalDeHuevos);
                        tiempoQueLlevaHaciendoElTunel = 0;
                        hayOrdenDeCavar = false;
                        reina.numHormigasCavandoTuneles--;
                        Task.current.Succeed();
                    }
                    else
                    {
                        reina.espacioLlenoHormiguero = true;
                        tiempoQueLlevaHaciendoElTunel = 0;
                        hayOrdenDeCavar = false;
                        reina.numHormigasCavandoTuneles--;
                        Task.current.Fail();
                    }
                    break;
            }
        }

        

    }

    // HayHormigaQueCurarCerca()
    
    [Task]
    public void HayHuecoParaDejarComida()
    {
        if (reina.ComidaTotal.Count >= reina.capacidadTotalDeComida)
        {
            if(comida != null)
            {
                comida.transform.SetParent(null);
            }
            posDejarComida = Vector3.zero;
            salaDejarComida = null;
            casillaDejarComida = null;
            posComida = Vector3.zero;
            comida = null;
            Task.current.Fail();
        }
        else
        {
            Task.current.Succeed();
        }
    }
    
    [Task]
    public void HaySuficienteComida()
    {
        //Debug.Log(reina.umbralComida * reina.totalHormigas);
        if (reina.ComidaTotal.Count < reina.umbralComida * reina.totalHormigas)
        {
            Task.current.Fail();
        }
        else
        {
            if (comida != null)
            {
                comida.transform.SetParent(null);
                comida = null;
                posDejarComida = Vector3.zero;
                salaDejarComida = null;
                casillaDejarComida = null;
                posComida = Vector3.zero;
            }
            Task.current.Succeed();
        }
    }

    [Task]
    public void HayComidaParaLlevar()
    {
        if (!hayOrdenBuscarComida)
        {
            // Si no tengo ninguna comida asignada, miro las que hay alrededor
            if (comida == null)
            {
                // Recorremos la lista de comida cerca
                foreach (Comida comidaAux in comidaQueHayCerca)
                {
                    if (comidaAux.hormigaQueLlevaLaComida == null && !comidaAux.haSidoCogida)
                    {
                        // Compruebo primeramente que tenga sala libre y la asigno
                        salaDejarComida = reina.meterComidaEnSala();
                        if (salaDejarComida != null)
                        {
                            casillaDejarComida = salaDejarComida.getFreeTile();
                            comidaAux.cogerComida(salaDejarComida, casillaDejarComida);
                            comida = comidaAux;
                            comida.hormigaQueLlevaLaComida = this;
                            posComida = Vector3.zero;
                            posDejarComida = Vector3.zero;
                            // Si la reina lo tiene en su lista de comida vista, lo borro
                            reina.ComidaVista.Remove(comidaAux);
                            Task.current.Succeed();
                            return;
                        }
                        else
                        {
                            // No hay sala para dejar comida, por lo que no la cojo
                            Task.current.Fail();
                            return;
                        }
                    }
                }
            }
            else
            {
                // Si ya tiene una comida asignada
                Task.current.Succeed();
                return;
            }
        }
        // Si no encuentra comida, o tiene orden
        Task.current.Fail();
        return;
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