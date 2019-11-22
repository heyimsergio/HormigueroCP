﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using UnityEngine.AI;

public class Reina : HormigaGenerica
{

    public enum TipoHormiga {NURSE,OBRERA,SOLDADO,NULL};

    #region atributos propios de la reina

    // NavMesh
    public int contPrioridadNavMesh = 1;

    //Poner huevos
    [Header("Variables Poner Huevos Reina")]
    public int tiempoQueTardaEnPonerHuevo;
    public float tiempoQueLlevaPoniendoHuevo;
    public bool estaPoniendoHuevo = false;
    private bool tienePosicionPonerHuevo = false;
    private TipoHormiga hormigaAponer = TipoHormiga.NULL;
    private Room salaDondePonerHuevo = null;
    private TileScript casillaDondePonerHuevo = null;
    public Vector3 posicionParaColocarHuevo;
    public bool puedePonerHuevo;
    public int tiempoMaximoParaPonerHuevo;
    public float tiempoRestanteHuevo;

    //Cuidar huevos
    [Header("Variables Cuidar Huevos Reina")]
    //public int numeroDeNurses;
    public List<Huevo> huevosQueTienenQueSerCuidados = new List<Huevo>();

    //Atacar
    [Header("Variables Atacar Reina")]
    //public EnemigoGenerico[] enemigosCercanos;
    //public int numEnemigosCerca;

    //Curar
    [Header("Variables Curar Hormiga Reina")]
    //public HormigaGenerica hormigasACurar;
    //public HormigaGenerica hormigasDesocupadas;

    #endregion

    #region logica global hormiguero
    [Header("Variables Lógica Global Hormiguero")]
    public List<Nurse> nursesOcupadas = new List<Nurse>();
    public List<Obrera> obrerasOcupadas = new List<Obrera>();
    //public List<Soldado> soldadosOcupadas = new List<Soldado>();
    public List<Nurse> nursesDesocupadas = new List<Nurse>();
    public List<Obrera> obrerasDesocupadas = new List<Obrera>();
    //public List<Soldado> soldadosDesocupadas  = new List<Soldado>();
    public int capacidadTotalDeHormigas;
    public int capacidadTotalDeComida;
    public int capacidadTotalDeHuevos;
    public int totalHormigas;
    //public int totalComida;                             // Esto para que ??
    //public int totalHuevos;                             // Esto para que ??
    public int numeroDeNursesTotal;
    public int numeroDeObrerasTotal;
    public int numeroDeSoldadosTotal;

    // ?????????????????????????????????????????????????????????
    public int numHormigasCuidandoHuevos;
    public int numHormigasAtacando;
    public int numHormigasBuscandoComida;
    public int numHormigasCavandoTuneles;
    public int numHormigasPatrullando;
    public int numHormigasCurando;
    public int numHormigasAbriendoHormiguero;
    public int numHormigasAbriendoCerrandoHormiguero;
    // ?????????????????????????????????????????????????????????

    public List<EnemigoGenerico> enemigosTotal = new List<EnemigoGenerico>();
    public List<Comida> comidaTotal = new List<Comida>();
    public List<Comida> comidaVista = new List<Comida>();
    public List<Huevo> huevosTotal = new List<Huevo>();
    public List<HormigaGenerica> hormigasHeridas = new List<HormigaGenerica>();

    // ?????????????????????????????????????????????????????????
    public int tiempoQueQuedaParaQueLlueva;
    public int tiempoQueLlueve;
    public int tiempoGlobal;
    // ?????????????????????????????????????????????????????????

    public bool espacioLlenoHormiguero = false;

    //FALTA ESTRUCTURA DE DATOS DEL MAPA
    [Header("Otros")]
    public Floor hormiguero;
    public Outside afueras;
    public List<Room> salasHormigas = new List<Room>();
    public List<Room> salasComida = new List<Room>();
    public List<Room> salasHuevos = new List<Room>();

    public int probComida = 10;
    public int MaxProbComida = 1000;

    float initTime;
    float actualTime;

    //IMPORTANCIA DE OBJETIVOS
    public float importanciaHormigas;
    public float importanciaHuevos;
    public float importanciaComida;

    public float importanciaNurses;
    public float importanciaObreras;
    public float importanciaSoldados;

    //NECESIDADES DEL HORMIGUERO
    // MandarHordenes
    //enum tipoOrden {CAVAR, BUSCAR, ATACAR, CUIDAR, PATRULLAR, NADA}
    public bool hayQueCrearSalasHormigas = false;
    public bool hayQueCrearSalasComida = false;
    public bool hayQueCrearSalasHuevos = false;
    public bool hayQueCrearSalas = false;

    /*public bool HayQueBuscarComida = false;
    private bool HayQueAtacar = false;
    private bool HayQuePatrullar = false;
    private bool HayQueCuidarHuevos = false;*/

    //para ver si hay que mandar patrullar
    public bool hormigaMuerta = false;

    // para mandar a atacar;
    public float umbralHormigasAtacando;

    // tiempos de cambio de prioridades
    public int TIEMPO1 = 2*60+30; // 2 mins y 30 segs 
    public int TIEMPO2 = 5*60;// 5 mins
    public int TIEMPO3 = 7*60 +30; // 7 mins y 30 segs
    public int TIEMPO4 = 10*60;

    // planificador de necesidad  de crear salas
    public float umbralCapacidadHormigas = 0.2f;
    public float umbralCapacidadHuevos = 0.1f;
    public float umbralCapacidadComida = 0.3f;
    public int tamañoMaxColaConstruccionSalas;
    public int salasEnConstruccion = 0;

    // planificador necesidades
    public float umbralComida;
    #endregion

    #region prefabs
    public GameObject comidaPrefab;
    public GameObject PrefabHuevo;
    public GameObject prefabNurse;
    public GameObject prefabObrera;
    #endregion

    #region Variables Auxiliares Durante el desarrollo
    public bool crearSala = false;
    public bool ponerHuevo = false;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        // Inicializacion
        initTime = Time.time;
        this.zonaDondeEsta = 0;

        // Respecto al hormiguero
        tamañoMaxColaConstruccionSalas = 1;
        agente = this.gameObject.GetComponent<NavMeshAgent>();
        hormiguero = GameObject.FindObjectOfType<Floor>();
        afueras = GameObject.FindObjectOfType<Outside>();
        reina = this;

        // Atributos poner huevos
        tiempoQueLlevaPoniendoHuevo = 0;
        tiempoRestanteHuevo = tiempoMaximoParaPonerHuevo;

        // Prioridades NavMesh
        agente.avoidancePriority = 0;

        // Ataques y Vida
        this.vida = 10;
        this.daño = 2;
        tiempoEntreAtaquesMax = 0.5f;
        this.tiempoEntreAtaques = tiempoEntreAtaquesMax;

        // Hambre
        hambre = 400;
        umbralHambre = 230;
        umbralHambreMaximo = 120;

        // Cuidar huevos
        tiempoCuidandoHuevos = 10.0f;
        TiempoActual = tiempoCuidandoHuevos;

        // Explorar
        siguientePosicionExplorar = Vector3.zero;


    }

    // Update is called once per frame
    void Update()
    {
        // Actualizar hambre
        ActualizarHambre();

        // Actualizar si tiene vida suficiente o no
        ActualizarSiPuedeSerCurada();

        // Actualización del hormiguero y creación de salas
        actualTime = Time.time - initTime;

        SimulateWorld();
        ActualizarPercepcionesHormiguero();
        ActualizarVariablesReina();
    }

    private void SimulateWorld()
    {
        CrearComida();
    }

    private void ActualizarPercepcionesHormiguero()
    {
        ActualizarPrioridades();
        //checkearNecesidadComida();
    }

    private void ActualizarVariablesReina()
    {
        tiempoRestanteHuevo -= Time.deltaTime;
        if (tiempoRestanteHuevo < 0 && !ponerHuevo)
        {
            ponerHuevo = true;
        }
    }

    /// <summary>
    /// Prioridades de la reina al poner un huevo
    /// </summary>
    #region Funciones actualizar prioridades Reina

    public void ActualizarPrioridades()
    {  

        //Prioridades tipo de hormiga a crear
        if (actualTime < TIEMPO1) // prioriza nurse
        {
            importanciaNurses = 3;
            importanciaObreras = 2;
            importanciaSoldados = 0.1f;

        } else if(actualTime >= TIEMPO1 && actualTime < TIEMPO2) // prioriza obreras
        {
            importanciaNurses = 3;
            importanciaObreras = 1;
            importanciaSoldados = 1f;

        } else if(actualTime >= TIEMPO2 && actualTime < TIEMPO3) // prioriza soldado
        {
            importanciaNurses = 1;
            importanciaObreras = 1.5f;
            importanciaSoldados = 2f;

        } else // todo por igual
        {
            importanciaNurses = 1;
            importanciaObreras = 1.2f;
            importanciaSoldados = 1.2f;

        }
    }

    /*public void checkearNecesidadComida()
    {
        if(totalHormigas * umbralComida  > comidaTotal.Count)
        {
            if(comidaTotal.Count < capacidadTotalDeComida)
            {
                HayQueBuscarComida = true;
            }
            else
            {
                HayQueBuscarComida = false;
            }
        }
    }*/

    #endregion

    // Alertas que recibe la reina
    public void RecibirAlertaComida(Comida comida)
    {
        if (!comidaVista.Contains(comida) && !comida.laEstanLLevando && !comida.haSidoCogida)
        {
            comidaVista.Add(comida);
        }
    }

    public void RecibirAlertaEnemigo(EnemigoGenerico enemigo)
    {
        if (!enemigosTotal.Contains(enemigo))
        {
            enemigosTotal.Add(enemigo);
        }
    }

    // Tareas de la reina
    [Task]
    public void HaySoldadosLibres()
    {
        /*if(soldadosDesocupadas.Count > 0)
        {
            Task.current.Succeed();
        } else
        {
            Task.current.Fail();
        }*/
        Task.current.Fail();
    }

    [Task]
    public void HayObrerasLibres()
    {
       if(obrerasDesocupadas.Count > 0)
        {
            Task.current.Succeed();
            return;
        } else
        {
            Task.current.Fail();
            return;
        }
    }

    [Task]
    public void HayNursesLibres()
    {
        if(nursesDesocupadas.Count > 0)
        {
            Task.current.Succeed();
        } else
        {
            Task.current.Fail();
        }
    }

    #region Ordenes Atacar
    [Task]
    public void SiendoAtacados()
    {
        /*if (HayQueAtacar)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }*/
        Task.current.Fail();
    }

    [Task]
    public void SuficientesLuchando()
    {
        if (totalHormigas * umbralHormigasAtacando > numHormigasAtacando)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void OrdenAtacarSoldado()
    {
        /*Soldado aux = soldadosDesocupadas[0];
        if (aux != null)
        {
            aux.hayOrdenDeAtacar = true;
            aux.enemigoAlQueAtacar = enemigosTotales[0];
            soldadosDesocupadas.Remove(aux);
            soldadosOcupadas.Add(aux);
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }*/
        Task.current.Fail();
    }

    [Task]
    public void OrdenAtacarObrera()
    {
        /*Obrera aux = obrerasDesocupadas[0];
        if (aux != null)
        {
            aux.hayOrdenDeAtacar = true;
            aux.enemigoAlQueAtacar = enemigosTotales[0];
            obrerasDesocupadas.Remove(aux);
            obrerasOcupadas.Add(aux);
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }*/
        Task.current.Fail();
    }

    #endregion

    #region Ordenes Llover
    [Task]
    public void VaALlover()
    {
        Task.current.Fail();
    }

    [Task]
    public void HormigueroEstaTapado()
    {
        Task.current.Fail();
    }

    [Task]
    public void SuficientesObrerasTapando()
    {
        Task.current.Fail();
    }

    [Task]
    public void OrdenTaparObreras()
    {
        Task.current.Fail();
    }

    [Task]
    public void HaDejadoDeLlover()
    {
        Task.current.Fail();
    }

    [Task]
    public void OrdenDestaparObreras()
    {
        Task.current.Fail();
    }

    #endregion

    #region Ordenes Buscar Comida

    [Task]
    public void HayComidaSuficiente()
    {
        if (totalHormigas * umbralComida > comidaTotal.Count)
        {
            if (comidaTotal.Count < capacidadTotalDeComida)
            {
                Task.current.Fail();
            }
            else
            {
                Task.current.Succeed();
            }
        }
        Task.current.Succeed();
    }

    [Task]
    public void HayHormigasBuscandoComida()
    {
        if (numHormigasBuscandoComida == 0)
        {
            Task.current.Succeed();
        }
        Task.current.Fail();
    }

    [Task]
    public void OrdenBuscarComidaObreras()
    {
        Task.current.Fail();
    }

    [Task]
    public void OrdenBuscarComidaSoldados()
    {
        Task.current.Fail();
    }

    [Task]
    public void OrdenBuscarComidaNurses()
    {
        Task.current.Fail();
    }

    #endregion

    #region Ordenes Cavar
    [Task]
    public void HayEspacio()
    {
        if (espacioLlenoHormiguero)
        {
            Task.current.Succeed();
            return;
        }
        Task.current.Fail();
    }

    [Task]
    public void HayObrerasCavando()
    {
        if(numHormigasCavandoTuneles > 0)
        {
            Task.current.Succeed();
            return;
        }
        Task.current.Fail();
    }

    [Task]
    public void OrdenCavarObreras()
    {
        Obrera aux = obrerasDesocupadas[0];
        if(aux != null)
        {
            aux.hayOrdenDeCavar = true;
            obrerasOcupadas.Add(aux);
            obrerasDesocupadas.Remove(aux);
            numHormigasCavandoTuneles++;
            Task.current.Succeed();
            return;
        }
        Task.current.Fail();
    }

    [Task]
    public void NecesitoCrearSala()
    {
        // comprobar si hay que crear Sala de Hormigas, se hace comprobando que haya capacidad para un 20% mas de hormigas
        if (totalHormigas * umbralCapacidadHormigas >= capacidadTotalDeHormigas - totalHormigas)
        {
            hayQueCrearSalasHormigas = true;
            Task.current.Succeed();
            return;
        }
        if (huevosTotal.Count * umbralCapacidadHuevos >= capacidadTotalDeHuevos - huevosTotal.Count)
        {
            hayQueCrearSalasHuevos = true;
            Task.current.Succeed();
            return;
        }
        if (comidaTotal.Count * umbralCapacidadComida >= capacidadTotalDeComida - comidaTotal.Count)
        {
            hayQueCrearSalasComida = true;
            Task.current.Succeed();
            return;
        }

        if (hayQueCrearSalasHormigas || hayQueCrearSalasComida || hayQueCrearSalasHuevos)
        {
            hayQueCrearSalas = true;
            Task.current.Succeed();
            return;
        }
        else
        {
            hayQueCrearSalas = false;
            Task.current.Fail();
            return;
        }


    }


    #endregion

    #region Ordenes Cuidar Huevos

    [Task]
    public void HayHuevosQueCuidar()
    {
        if (huevosQueTienenQueSerCuidados.Count > 0)
        {
            //Debug.Log("Reina: Hay huevos que necesitan cuidados");
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }

    }

    [Task]
    public void OrdenCuidarNurses()
    {
        Nurse aux = nursesDesocupadas[0];
        if (aux != null)
        {
            //Debug.Log("Tengo nurse que mandar a cuidar");
            // hay que asignar el huevo a cuidar, creo que debe ser distinto al huevo que se detcta solo porque si no podria sobreescribirlo, y cambiar la funcion cuidar huevo si me mandan usando ese huevo
            aux.hayOrdenCuidarHuevos = true;
            nursesOcupadas.Add(aux);
            nursesDesocupadas.Remove(aux);
            aux.huevoACuidar = huevosQueTienenQueSerCuidados[0];
            aux.huevoACuidar.siendoCuidadoPor = aux;
            huevosQueTienenQueSerCuidados.RemoveAt(0);
            Task.current.Succeed();
        }
    }

    #endregion

    #region Ordenes Curar Hormiga

    [Task]
    public void HayHormigasHeridas()
    {
        if (hormigasHeridas.Count > 0)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void OrdenCurarSoldado()
    {
        /*Soldado aux = soldadosDesocupadas[0];
        if (aux != null)
        {
            Debug.Log("Tengo nurse que mandar a cuidar");
            // hay que asignar el huevo a cuidar, creo que debe ser distinto al huevo que se detcta solo porque si no podria sobreescribirlo, y cambiar la funcion cuidar huevo si me mandan usando ese huevo
            aux.hayOrdenCuidarHuevos = true;
            nursesOcupadas.Add(aux);
            nursesDesocupadas.Remove(aux);
            aux.huevoACuidar = huevosQueTienenQueSerCuidados[0];
            aux.huevoACuidar.siendoCuidadoPor = aux;
            huevosQueTienenQueSerCuidados.RemoveAt(0);
            Task.current.Succeed();
        }*/
    }

    [Task]
    public void OrdenCurarObreras()
    {
        Obrera aux = obrerasDesocupadas[0];
        HormigaGenerica aux2 = hormigasHeridas[0];
        if (aux == aux2)
        {
            if (obrerasDesocupadas.Count > 1)
            {
                aux = obrerasDesocupadas[1];
            }
            else if (hormigasHeridas.Count > 1)
            {
                aux2 = hormigasHeridas[1];
            }
            else
            {
                // La hormiga herida y la obrera son la misma y no hay más
                Task.current.Fail();
                return;
            }
        }
        if (aux != null && aux != aux2)
        {
            Debug.Log("Tengo obrera que mandar a curar");
            aux.hayOrdenCurarHormiga = true;
            obrerasOcupadas.Add(aux);
            obrerasDesocupadas.Remove(aux);
            aux.hormigaACurar = aux2;
            aux2.siendoCuradaPor = this;
            hormigasHeridas.Remove(aux2);
            Task.current.Succeed();
            return;
        }
    }

    [Task]
    public void OrdenCurarNurses()
    {
        Nurse aux = nursesDesocupadas[0];
        HormigaGenerica aux2 = hormigasHeridas[0];
        if (aux == aux2)
        {
            if (nursesDesocupadas.Count > 1)
            {
                aux = nursesDesocupadas[1];
            }
            else if (hormigasHeridas.Count > 1)
            {
                aux2 = hormigasHeridas[1];
            }
            else
            {
                // La hormiga herida y la nurse son la misma y no hay más
                Task.current.Fail();
                return;
            }
        }
        if (aux != null && aux != aux2)
        {
            Debug.Log("Tengo obrera que mandar a curar");
            aux.hayOrdenCurarHormiga = true;
            nursesOcupadas.Add(aux);
            nursesDesocupadas.Remove(aux);
            aux.hormigaACurar = aux2;
            aux2.siendoCuradaPor = this;
            hormigasHeridas.Remove(aux2);
            Task.current.Succeed();
            return;
        }
    }

    #endregion

    #region Ordenes Patrullar

    [Task]
    public void HaHabidoUnAtaqueReciente()
    {
        Task.current.Fail();
    }

    [Task]
    public void OrdenPatrullarSoldado()
    {
        Task.current.Fail();
    }

    #endregion

    #region Tareas De La Reina

    // HayEnemigosCerca()
    // Atacar()
    // TengoMuchaHambre()
    // TengoHambre()
    // HayComida()
    // Comer()
    // HayHormigaQueCurarCerca()
    // CurarHormiga()

    // HayHuevosCercaQueNecesitanCuidados() --> está en la nurse actualmente solo
    [Task]
    public void HayHuevosCercaQueNecesitanCuidados()
    {
        Task.current.Fail();
    }

    // CuidarHuevos() --> está en la nurse actualmente solo
    [Task]
    public void CuidarHuevos()
    {
        Task.current.Fail();
    }

    [Task]
    public void PuedoPonerHuevos()
    {
        Task.current.Succeed();
    }

    [Task]
    public void PonerHuevos()
    {        
        if (ponerHuevo)
        {
            if (hormigaAponer == TipoHormiga.NULL)
            {
                int min = CompareLess3(numeroDeNursesTotal, numeroDeObrerasTotal, numeroDeSoldadosTotal, importanciaNurses, importanciaObreras, importanciaSoldados);
                switch (min)
                {
                    case 0:
                        hormigaAponer = TipoHormiga.NURSE;
                        break;
                    case 1:
                        hormigaAponer = TipoHormiga.OBRERA;
                        break;
                    case 2:
                        hormigaAponer = TipoHormiga.SOLDADO;
                        break;
                }
            }

            if (salaDondePonerHuevo == null)
            {
                Room Aux = GetSalaLibreHuevos();
                if (Aux != null)
                {
                    salaDondePonerHuevo = Aux;

                }
                else
                {
                    //Debug.Log("No hay sala de huevos");
                    tienePosicionPonerHuevo = false;
                    hormigaAponer = TipoHormiga.NULL;
                    salaDondePonerHuevo = null;
                    casillaDondePonerHuevo = null;
                    ponerHuevo = false;
                    Task.current.Fail();
                    return;
                }
            }

            if (!tienePosicionPonerHuevo)
            {
                casillaDondePonerHuevo = salaDondePonerHuevo.getFreeTile();
                posicionParaColocarHuevo = casillaDondePonerHuevo.transform.position;
                agente.SetDestination(posicionParaColocarHuevo);
                tienePosicionPonerHuevo = true;
            }
            float auxDistance = Vector3.Distance(this.transform.position, posicionParaColocarHuevo);
            if (auxDistance < 0.1 && tienePosicionPonerHuevo)
            {
                estaPoniendoHuevo = true;
                if (tiempoQueLlevaPoniendoHuevo < tiempoQueTardaEnPonerHuevo)
                {
                    tiempoQueLlevaPoniendoHuevo += Time.deltaTime;
                }
                else
                {
                    GameObject huevoAux = Instantiate(PrefabHuevo, posicionParaColocarHuevo, Quaternion.identity);
                    Huevo huevoScript = huevoAux.GetComponent<Huevo>();
                    huevoScript.Init(salaDondePonerHuevo, hormigaAponer,casillaDondePonerHuevo);
                    MeterHuevosEnSala(salaDondePonerHuevo);
                    huevosTotal.Add(huevoScript);
                    huevoScript.miType = hormigaAponer;
                    ponerHuevo = false;
                    tienePosicionPonerHuevo = false;
                    hormigaAponer = TipoHormiga.NULL;
                    salaDondePonerHuevo = null;
                    casillaDondePonerHuevo = null;
                    ponerHuevo = false;
                    tiempoRestanteHuevo = tiempoMaximoParaPonerHuevo;
                    estaPoniendoHuevo = false;
                    tiempoQueLlevaPoniendoHuevo = 0;
                    Task.current.Succeed();
                }
            }
        }
        else
        {
            Task.current.Fail();
        }

    }

    [Task]
    public void Esperar()
    {

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
            if (siguientePosicionExplorar == Vector3.zero)
            {
                Vector3 randomDirection;
                NavMeshHit aux;
                bool aux2;
                do
                {
                    randomDirection = UnityEngine.Random.insideUnitSphere * (reina.hormiguero.heigth / 2 - 15) + reina.hormiguero.centro;
                    aux2 = NavMesh.SamplePosition(randomDirection, out aux, 4.0f, NavMesh.AllAreas);
                } while (!aux2);
                siguientePosicionExplorar = new Vector3(aux.position.x, 0, aux.position.z);
                //Debug.Log("Posicion a la que va: " + siguientePosicionExplorar);
                agente.SetDestination(siguientePosicionExplorar);
                Task.current.Succeed();
                return;
            }
            else if (Vector3.Distance(this.transform.position, siguientePosicionExplorar) < 0.5f)
            {
                siguientePosicionExplorar = Vector3.zero;
                Task.current.Succeed();
                return;
            } else
            {
                agente.SetDestination(siguientePosicionExplorar);
                Task.current.Succeed();
                return;
            }
        }
        Task.current.Succeed();
    }


    #endregion

    // Otros ???

    public static int CompareLess3(int num1, int num2, int num3, float importancia1, float importancia2 , float importancia3)
    {
        if(importancia1 <= 0)
        {
            num1 = 100000000;
        }

        if (importancia2 <= 0)
        {
            num2 = 100000000;
        }

        if (importancia3 <= 0)
        {
            num3 = 100000000;
        }

        if(importancia1 != 0)
        {
            num1 = (int)(num1 / importancia1);
        }

        if(importancia2 != 0)
        {
            num2 = (int)(num2 / importancia2);
        }

        if(importancia3 != 0)
        {
            num3 = (int)(num3 / importancia3);
        }


        int min = num1;
        int finalNum = 0;

        if(min> num2)
        {
            min = num2;
            finalNum = 1;
        }

        if(min > num3)
        {
            min = num3;
            finalNum = 2;
        }


        return finalNum;
    }

    // Métodos para el tratamiento de la comida
    public void ComidaGuardada(Comida comida, Room sala, TileScript tile)
    {
        if (!comidaTotal.Contains(comida))
        {
            //Debug.Log("Meter cosas de comida guardada");
            comidaTotal.Add(comida);
            comida.misala = sala;
            comida.miTile = tile;
            //sala.meterCosas();
        }
        if (comidaVista.Contains(comida))
        {
            comidaVista.Remove(comida);
        }
    }

    // Métodos para el tratamiento de los ataques
    public void HormigaAtacando()
    {
        numHormigasAtacando++;

    }

    public void HormigaDejaDeAtacar()
    {
        numHormigasAtacando--;
    }


    // Métodos para el tratamiento de creaciones
    public void NaceHormiga(Huevo huevo)
    {
        switch (huevo.miType)
        {
            case TipoHormiga.NURSE:
                //numeroDeNursesTotal++;
                //totalHormigas++;
                Debug.Log("Nurse nace");
                Vector3 posNurse = huevo.transform.position;
                GameObject aux = Instantiate(prefabNurse, posNurse, Quaternion.identity);
                //aux.transform.position = huevo.transform.position;
                //nursesDesocupadas.Add(aux.GetComponent<Nurse>());
                //Se instanciaria el objeto de hormiga
                break;
            case TipoHormiga.OBRERA:
                //numeroDeObrerasTotal++;
                //totalHormigas++;
                Debug.Log("Obrera nace");
                Vector3 posObrera = huevo.transform.position;
                GameObject aux2 = Instantiate(prefabObrera, posObrera, Quaternion.identity);
                // se instanciaria
                break;
            case TipoHormiga.SOLDADO:
                //Debug.Log("Soldado nace");
                //numeroDeSoldadosTotal++;
                //totalHormigas++;
                break;
        }
    }

    public void CrearComida()
    {
        int saleComida = Random.RandomRange(0, MaxProbComida + 1);
        if (saleComida < probComida)
        {
            Vector3 centro = afueras.centro;
            float posX = Random.RandomRange(-(afueras.width / 2), (afueras.width / 2));
            float posZ = Random.RandomRange(-(afueras.heigth / 2), (afueras.heigth / 2));

            centro.x += posX;
            centro.z += posZ;

            GameObject aux = Instantiate(comidaPrefab, centro, Quaternion.identity);
            aux.GetComponent<Comida>().InitComida(Comida.comidaType.Trigo);
        }
    }

        // Tarea Poner Huevo de Reina

    // Devuelve la sala porque sera necesario asignarsela a las homirmigas comidas y huevos para manejarlas
    #region Meter Cosas en salas
    public Room MeterComidaEnSala()
    {
        Room sala = GetSalaLibreComida();
        if (sala != null)
        {
            Debug.Log(" Se mete comida desde  meter Comida en sala");
            sala.meterCosas();
        }
        return sala;
    }

    public Room MeterHormigaEnSala()
    {
        Room sala = GetSalaLibreHormigas();
        if (sala != null)
        {
            sala.meterCosas();
        }
        return sala;
    }

    public Room MeterHuevosEnSala(Room sala)
    {
        if (sala != null)
        {
            sala.meterCosas();
            //totalHuevos++;
        }
        return sala;
    }
    #endregion

    #region Manejar cosas Muertas
    // Métodos para el tratamiento de muertos
    public void HormigaHaMuerto(HormigaGenerica hormiga)
    {
        SacarHormigaSala(hormiga.miSala);
        Debug.Log("Hormiga A Muerto");
        // Actualizamos a todos los enemigos que tenga
        foreach (EnemigoGenerico enem in hormiga.enemigosCerca)
        {
            enem.hormigasCerca.Remove(this);
        }
        // Actualizamos a todos los huevos que tenga
        foreach (Huevo huevo in hormiga.huevosCerca)
        {
            huevo.hormigasCerca.Remove(this);
        }
        // Actualizamos a todos las comidas que tenga
        foreach (Comida comida in hormiga.comidaQueHayCerca)
        {
            comida.hormigasCerca.Remove(this);
        }

        // Eliminar a la hormiga de todas las listas
        if (hormigasHeridas.Contains(hormiga))
        {
            hormigasHeridas.Remove(hormiga);
        }

        // Depende de la hormiga que sea, se saca de una lista u otra
        if (hormiga.GetType().Equals("Nurse"))
        {
            Nurse hormigaNurse = (Nurse)hormiga;
            if (nursesDesocupadas.Contains(hormigaNurse))
            {
                nursesDesocupadas.Remove(hormigaNurse);
            }
            else if (nursesOcupadas.Contains(hormigaNurse))
            {
                nursesOcupadas.Remove(hormigaNurse);
            }
        }
        else if (hormiga.GetType().Equals("Obrera"))
        {
            Obrera hormigaObrera = (Obrera)hormiga;
            if (obrerasDesocupadas.Contains(hormigaObrera))
            {
                obrerasDesocupadas.Remove(hormigaObrera);
            }
            else if (obrerasOcupadas.Contains(hormigaObrera))
            {
                obrerasOcupadas.Remove(hormigaObrera);
            }
        }
        else if (hormiga.GetType().Equals("Soldado"))
        {
            /*Soldado hormigaSoldado = (Soldado)hormiga;
            if (soldadosDesocupadas.Contains(hormigaSoldado))
            {
                soldadosDesocupadas.Remove(hormigaSoldado);
            }
            else if (soldadosOcupadas.Contains(hormigaSoldado))
            {
                soldadosOcupadas.Remove(hormigaSoldado);
            }*/
        }
        else
        {
            // Si es la reina, sería fin de la simulación
        }

        // Si la hormiga muere mientras cura a otra
        if (hormiga.hormigaACurar != null)
        {
            // Si la hormiga que estaba siendo curada necesita ser curada y no está en hormigasHeridas, se añade
            if (hormiga.hormigaACurar.necesitaSerCurada && !reina.hormigasHeridas.Contains(hormiga.hormigaACurar))
            {
                reina.hormigasHeridas.Add(hormiga.hormigaACurar);
            }
            hormiga.hormigaACurar.siendoCuradaPor = null;
            hormiga.hormigaACurar = null;
        }
        // Si la hormiga muere mientras cuida un huevo
        if (hormiga.huevoACuidar != null)
        {
            if (hormiga.huevoACuidar.necesitaCuidados == true && !reina.huevosQueTienenQueSerCuidados.Contains(hormiga.huevoACuidar))
            {
                reina.huevosQueTienenQueSerCuidados.Add(hormiga.huevoACuidar);
            }
            hormiga.huevoACuidar.siendoCuidadoPor = null;
            hormiga.huevoACuidar = null;
        }
        // Si la hormiga está siendo curada por alguien y muere
        if (hormiga.siendoCuradaPor != null)
        {
            hormiga.siendoCuradaPor.hormigaACurar = null;
            hormiga.siendoCuradaPor = null;
        }
        // Si la hormiga muere mientras tiene una comida asignada
        if (hormiga.comida != null)
        {
            hormiga.comida.laEstanLLevando = false;
            hormiga.comida.hormigaQueLlevaLaComida = null;
            hormiga.comida.transform.SetParent(null);
            SacarComidaSala(hormiga.salaDejarComida, hormiga.comida, hormiga.casillaDejarComida);
            if (reina.comidaVista.Contains(hormiga.comida))
            {
                reina.comidaVista.Add(hormiga.comida);
            }
        }
        // Si la hormiga muere mientras va a comer
        if (hormiga.comidaAComer != null)
        {
            if (!comidaTotal.Contains(hormiga.comidaAComer))
            {
                comidaTotal.Add(hormiga.comidaAComer);
            }
        }
        // Si la hormiga está siendo curada por alguien
        hormiga.siendoCuradaPor = null;

        hormiga = null;
    }

    public void EnemigoHaMuerto(EnemigoGenerico enemigo)
    {
        // Avisamos a la hormiga de que el enemigo ha muerto
        foreach (HormigaGenerica h in hormigasCerca)
        {
            h.enemigosCerca.Remove(enemigo);
            if (h.enemigoAlQueAtacar == this)
            {
                h.enemigoAlQueAtacar = null;
                h.hayOrdenDeAtacar = false;
            }
        }
        // Eliminamos al enemigo de la lista de enemigosTotales de la reina
        if (enemigosTotal.Contains(enemigo))
        {
            enemigosTotal.Remove(enemigo);
        }
        enemigo = null;
    }

    public void ComidaHaMuerto(Comida comidaMuerta)
    {
        // Sacamos la comida de la sala si muere cuando está siendo llevada
        if (comidaMuerta.hormigaQueLlevaLaComida != null || comidaMuerta.haSidoCogida)
        {
            SacarComidaSala(comidaMuerta.misala, comidaMuerta, comidaMuerta.miTile);
        }

        if (comidaMuerta.hormigaQueLlevaLaComida != null)
        {
            comidaMuerta.hormigaQueLlevaLaComida.comida = null;
            comidaMuerta.hormigaQueLlevaLaComida.salaDejarComida = null;
            comidaMuerta.hormigaQueLlevaLaComida.casillaDejarComida = null;
            comidaMuerta.hormigaQueLlevaLaComida.posComida = Vector3.zero;
            comidaMuerta.hormigaQueLlevaLaComida.posDejarComida = Vector3.zero;
            comidaMuerta.hormigaQueLlevaLaComida = null;
        }

        // Avisamos a las hormigas cercanas de que la comida ha muerto
        foreach (HormigaGenerica h in comidaMuerta.hormigasCerca)
        {
            h.comidaQueHayCerca.Remove(comida);
        }

        // Si la reina tenia esa comida como vista en su lista, se elimina
        comidaVista.Remove(comidaMuerta);

        comidaMuerta = null;
    }

    public void HuevoHaMuerto(Huevo miHuevo)
    {
        // Sacamos el huevo de la sala
        SacarHuevosSala(miHuevo.myRoom, miHuevo);
        // Si el huevo estaba siendo curado, avisamos a la hormiga que lo curaba
        if (miHuevo.siendoCuidadoPor != null)
        {
            miHuevo.siendoCuidadoPor.huevoACuidar = null;
        }
        // Actualizamos a todas las hormigas cercanas a este huevo
        foreach (HormigaGenerica h in miHuevo.hormigasCerca)
        {
            h.huevosCerca.Remove(miHuevo);
        }
        // Si el huevo estaba en la lista de NECESITA ser cuidado, se elimina
        huevosQueTienenQueSerCuidados.Remove(miHuevo);

    }

    #endregion

    #region Metodos para sacar cosas de las salas
    // necesitamos la sala dode se guardan las cosas ???

    public void SacarHormigaSala(Room sala)
    {
        if (sala != null)
        {
            sala.sacarCosas(null);
        }
        totalHormigas--;
    }

    public void SacarComidaSala(Room sala, Comida comida, TileScript miTile)
    {
        if (comidaTotal.Remove(comida))
        {
            Debug.Log("Comida eliminada de comidaTotal");
        }
        sala.sacarCosas(miTile);
    }

    public void SacarHuevosSala(Room sala, Huevo huevo)
    {
        huevosTotal.Remove(huevo);
        sala.sacarCosas(huevo.myTile);
        //totalHuevos--;
    }

    #endregion

    #region Gets de salas libres
    public Room GetSalaLibreHormigas()
    {
        foreach (Room aux in salasHormigas)
        {
            if (!aux.isFull)
            {
                return aux;
            }
        }
        return null;
    }

    public Room GetSalaLibreComida()
    {
        foreach (Room aux in salasComida)
        {
            Debug.Log("Numero de comida: " + aux.llenadoActual);
            Debug.Log("Capacidad de Comida: " + aux.capacidadTotalRoom);
            if (!aux.isFull)
            {
                return aux;
            }
        }
        return null;
    }

    public Room GetSalaLibreHuevos()
    {
        foreach (Room aux in salasHuevos)
        {
            if (!aux.isFull)
            {
                return aux;
            }
        }
        return null;
    }
    #endregion

}
