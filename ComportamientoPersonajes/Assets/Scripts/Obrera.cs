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
    [Header("CAVAR")]
    public int tiempoParaHacerTunel;
    public float tiempoQueLlevaHaciendoElTunel;
    public Vector3 posicionInicialTunel;
    public Vector3 posicionFinalTunel;
    public Vector3 posCavar;

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

        // Explorar
        siguientePosicionExplorar = Vector3.zero;

        // cavar
        posCavar = Vector3.zero;

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
            if (!comidaQueHayCerca.Contains(aux) && !aux.haSidoCogida && !aux.laEstanLLevando && aux.hormigaQueLlevaLaComida == null)
            {
                reina.RecibirAlertaComida(aux);
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
            /*if (reina.obrerasOcupadas.Contains(this))
            {
                reina.obrerasOcupadas.Remove(this);
                reina.obrerasDesocupadas.Add(this);
            }*/
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
        if (bocadillos.hormigaSeleccionada != null && bocadillos.hormigaSeleccionada == this)
        {
            bocadillos.Cavar();
        }
        //esta fuera
        if (zonaDondeEsta != 0)
        {
            Vector3 randomDirection;
            NavMeshHit aux;
            bool aux2;
            do
            {
                randomDirection = UnityEngine.Random.insideUnitSphere * 10 + reina.hormiguero.centro;
                aux2 = NavMesh.SamplePosition(randomDirection, out aux, 1.0f, NavMesh.AllAreas);
            } while (!aux2);
            posCavar = new Vector3(aux.position.x, 0, aux.position.z);
            //Debug.Log("Posicion a la que va: " + siguientePosicionExplorar);
            agente.SetDestination(posCavar);
        } else
        {
            posCavar = Vector3.zero;
            if (tiempoQueLlevaHaciendoElTunel < tiempoParaHacerTunel)
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
                int capacidadRestanteComida = reina.capacidadTotalDeComida - reina.comidaTotal.Count;
                int capacidadRestanteHuevos = reina.capacidadTotalDeHuevos - reina.huevosTotal.Count;


                if (reina.hayQueCrearSalasComida && reina.hayQueCrearSalasHuevos && reina.hayQueCrearSalasHormigas)
                {
                    min = Reina.CompareLess3(capacidadRestanteHormigas, capacidadRestanteComida, capacidadRestanteHuevos, reina.importanciaHormigas, reina.importanciaComida, reina.importanciaHuevos);
                }
                else if (reina.hayQueCrearSalasHormigas && reina.hayQueCrearSalasHuevos)
                {
                    min = Reina.CompareLess3(capacidadRestanteHormigas, capacidadRestanteComida, capacidadRestanteHuevos, reina.importanciaHormigas, reina.importanciaComida, 0);
                }
                else if (reina.hayQueCrearSalasHormigas && reina.hayQueCrearSalasComida)
                {
                    min = Reina.CompareLess3(capacidadRestanteHormigas, capacidadRestanteComida, capacidadRestanteHuevos, reina.importanciaHormigas, 0, reina.importanciaHuevos);
                }
                else if (reina.hayQueCrearSalasComida && reina.hayQueCrearSalasHuevos)
                {
                    min = Reina.CompareLess3(capacidadRestanteHormigas, capacidadRestanteComida, capacidadRestanteHuevos, 0, reina.importanciaComida, reina.importanciaHuevos);
                }
                else if (reina.hayQueCrearSalasHormigas)
                {
                    min = 0;
                }
                else if (reina.hayQueCrearSalasComida)
                {
                    min = 1;
                }
                else if (reina.hayQueCrearSalasHuevos)
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
                            reina.hayQueCrearSalasHormigas = false;
                            //Debug.Log("Sala de Hormigas creada, la capacidad ahora es: " + capacidadTotalDeHormigas);
                            tiempoQueLlevaHaciendoElTunel = 0;
                            hayOrdenDeCavar = false;
                            SacarDeOcupadas();
                            reina.numHormigasCavandoTuneles--;
                            Task.current.Succeed();
                        }
                        else
                        {
                            reina.espacioLlenoHormiguero = true;
                            tiempoQueLlevaHaciendoElTunel = 0;
                            hayOrdenDeCavar = false;
                            SacarDeOcupadas();
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
                            reina.hayQueCrearSalasComida = false;
                            //Debug.Log("Sala de Comida creada, la capacidad ahora es: " + capacidadTotalDeComida);
                            tiempoQueLlevaHaciendoElTunel = 0;
                            hayOrdenDeCavar = false;
                            SacarDeOcupadas();
                            reina.numHormigasCavandoTuneles--;
                            Task.current.Succeed();
                        }
                        else
                        {
                            reina.espacioLlenoHormiguero = true;
                            tiempoQueLlevaHaciendoElTunel = 0;
                            hayOrdenDeCavar = false;
                            SacarDeOcupadas();
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
                            reina.hayQueCrearSalasHuevos = false;
                            //Debug.Log("Sala de Huevos creada, la capacidad ahora es: " + capacidadTotalDeHuevos);
                            tiempoQueLlevaHaciendoElTunel = 0;
                            hayOrdenDeCavar = false;
                            SacarDeOcupadas();
                            reina.numHormigasCavandoTuneles--;
                            Task.current.Succeed();
                        }
                        else
                        {
                            reina.espacioLlenoHormiguero = true;
                            tiempoQueLlevaHaciendoElTunel = 0;
                            hayOrdenDeCavar = false;
                            SacarDeOcupadas();
                            reina.numHormigasCavandoTuneles--;
                            Task.current.Fail();
                        }
                        break;
                }
            }
        }
    }

    // HayHormigaQueCurarCerca()

    [Task]
    public void HayHuecoParaDejarComida()
    {
        if (this.comida != null)
        {
            Task.current.Succeed();
            return;
        }
        if (reina.comidaTotal.Count >= reina.capacidadTotalDeComida)
        {
            if (comida != null)
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
        if (this.comida != null)
        {
            Task.current.Fail();
            return;
        }
        if (reina.comidaTotal.Count < reina.umbralComida * reina.totalHormigas)
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
                        //Debug.Log("Elijo una comida que este cerca");
                        // Compruebo primeramente que tenga sala libre y la asigno
                        salaDejarComida = reina.MeterComidaEnSala();
                        if (salaDejarComida != null)
                        {
                            // Si la reina lo tiene en su lista de comida vista, lo borro
                            reina.comidaVista.Remove(comidaAux);
                            casillaDejarComida = salaDejarComida.getFreeTile();
                            comidaAux.CogerComida(salaDejarComida, casillaDejarComida);
                            comida = comidaAux;
                            comida.hormigaQueLlevaLaComida = this;
                            posComida = Vector3.zero;
                            posDejarComida = Vector3.zero;
                            Task.current.Succeed();
                            return;
                        }
                        else
                        {
                            // No hay sala para dejar comida, por lo que no la cojo
                            //Debug.Log("No hay sala libre");
                            Task.current.Fail();
                            return;
                        }
                    }
                    else
                    {
                        //Debug.Log("Todas las comidas tienen hormida o las han cogido");
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
        if (bocadillos.hormigaSeleccionada != null && bocadillos.hormigaSeleccionada == this)
        {
            bocadillos.Explorar();
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
            } else
            {
                agente.SetDestination(siguientePosicionExplorar);
            }

        }
        Task.current.Succeed();
    }

 
    #endregion
}