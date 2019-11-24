using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Reina : HormigaGenerica
{

    public enum TipoHormiga { NURSE, OBRERA, SOLDADO, NULL };

    #region atributos propios de la reina

    //Actualizar canvas ordenes
    public ActualizarDatosCanvas ordenesCanvas;

    // NavMesh
    public int contPrioridadNavMesh = 1;

    //Poner huevos
    [Header("Variables Poner Huevos Reina")]
    public int tiempoQueTardaEnPonerHuevos;
    public bool estaPoniendoHuevo = false;
    private bool tienePosicionPonerHuevo = false;
    private TipoHormiga hormigaAponer = TipoHormiga.NULL;
    private Room salaDondePonerHuevo = null;
    private TileScript casillaDondePonerHuevo = null;
    public Vector3 posicionParaColocarHuevo;
    // Controlan las tandas de huevos;
    public bool puedePonerHuevo;
    public float tiempoRestanteHuevos;
    public int huevosAPonerEnEstaTanda;
    public int huevosQueLlevaPuestosEnEstaTanda;
    public int numeroMinimoHuevosPorTanda;
    public int numeroMaximoHuevosPorTanda;
    public int numeroDeNurseEnEstaTanda;
    public int numeroDeObrerasEnEstaTanda;
    public int numeroDeSoldadoEnEstaTanda;


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
    public List<Soldado> soldadosOcupadas = new List<Soldado>();
    public List<Nurse> nursesDesocupadas = new List<Nurse>();
    public List<Obrera> obrerasDesocupadas = new List<Obrera>();
    public List<Soldado> soldadosDesocupadas  = new List<Soldado>();
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
    public int MaxProbComida;

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
    // MandarOrdenes
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
    public Vector3 posHormigaMuerta = Vector3.zero;

    // para mandar a atacar;
    public float umbralHormigasAtacando;

    // tiempos de cambio de prioridades
    public int TIEMPO1 = 2 * 60 + 30; // 2 mins y 30 segs 
    public int TIEMPO2 = 5 * 60;// 5 mins
    public int TIEMPO3 = 7 * 60 + 30; // 7 mins y 30 segs
    public int TIEMPO4 = 10 * 60;

    public int saleComida;
    public bool par;

    // planificador de necesidad  de crear salas
    public float umbralCapacidadHormigas = 0.2f;
    public float umbralCapacidadHuevos = 0.1f;
    public float umbralCapacidadComida = 0.3f;
    public int tamañoMaxColaConstruccionSalas;
    public int salasEnConstruccion = 0;

    // planificador necesidades
    public float umbralComida;
    public float umbralNecesidadCogerComida;
    #endregion

    #region prefabs
    public GameObject comidaPrefab;
    public GameObject PrefabHuevo;
    public GameObject prefabNurse;
    public GameObject prefabObrera;
    public GameObject prefabSoldado;
    #endregion

    #region Variables Auxiliares Durante el desarrollo
    public bool crearSala = false;
    public bool ponerHuevo = false;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        DataController config = FindObjectOfType<DataController>();
        if (config.facil)
        {
            MaxProbComida = 1500;
        } else if (config.medio)
        {
            MaxProbComida = 3000;
        } else {
            MaxProbComida = 6000;
        }
        par = false;
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
        huevosAPonerEnEstaTanda = 0;
        huevosQueLlevaPuestosEnEstaTanda = 0;
        huevosAPonerEnEstaTanda = Random.Range(numeroMinimoHuevosPorTanda, numeroMaximoHuevosPorTanda);

        // Prioridades NavMesh
        agente.avoidancePriority = 0;
        priority = 0;
        contPrioridadNavMesh = 1;

        // Ataques y Vida
        totalVida = 50;
        this.vida = totalVida;
        this.daño = 3;
        tiempoEntreAtaquesMax = 0.5f;
        this.tiempoEntreAtaques = tiempoEntreAtaquesMax;
        umbralPuedeCurarse = 20;
        umbralNecesitaCurarse = 10;

        // Hambre
        hambre = 400;
        umbralHambre = 230;
        umbralHambreMaximo = 120;

        // Cuidar huevos
        tiempoCuidandoHuevos = 10.0f;
        TiempoActual = tiempoCuidandoHuevos;

        // Explorar
        siguientePosicionExplorar = Vector3.zero;

        // Curar Hormiga
        tiempoParaCurar = 10.0f;

        // Patrullar
        tiempoDePatrulloMax = 20.0f;
        tiempoDePatrullo = tiempoDePatrulloMax;

        //TiemposEvolucionJuego
        TIEMPO1 = 80;
        TIEMPO2 = 200;
        TIEMPO3 = 250;
        TIEMPO4 = 300;

        //umbrales comida
         umbralComida = 2.3f;
         umbralNecesidadCogerComida = 1f;

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
        tiempoRestanteHuevos -= Time.deltaTime;
        if (tiempoRestanteHuevos < 0 && !ponerHuevo)
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
        }
        else if (actualTime >= TIEMPO1 && actualTime < TIEMPO2) // prioriza obreras
        {
            importanciaNurses = 3;
            importanciaObreras = 1;
            importanciaSoldados = 1f;

        }
        else if (actualTime >= TIEMPO2 && actualTime < TIEMPO3) // prioriza soldado
        {
            importanciaNurses = 1;
            importanciaObreras = 1.5f;
            importanciaSoldados = 2f;

        }
        else // todo por igual
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
        if(soldadosDesocupadas.Count > 0)
        {
            Task.current.Succeed();
        } 
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void HayObrerasLibres()
    {
        if (obrerasDesocupadas.Count > 0)
        {
            Task.current.Succeed();
            return;
        }
        else
        {
            Task.current.Fail();
            return;
        }
    }

    [Task]
    public void HayNursesLibres()
    {
        if (nursesDesocupadas.Count > 0)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    #region Ordenes Atacar
    [Task]
    public void SiendoAtacados()
    {
        if (enemigosTotal.Count > 0)
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
        ordenesCanvas.OrdenAtacar();
        
        Soldado aux = soldadosDesocupadas[Random.Range(0, soldadosDesocupadas.Count)];
        if (aux != null)
        {
            EnemigoGenerico enem = enemigosTotal[Random.Range(0, enemigosTotal.Count)];
            // Si no estoy ya atacando a ese enemigo, se me puede mandar la orden
            if (enem != aux.enemigoAlQueAtacar)
            {
                aux.hayOrdenDeAtacar = true;
                aux.enemigoAlQueAtacarPorOrden = enem;
                aux.SacarDeDesocupadas();
                if (!aux.enemigoAlQueAtacarPorOrden.hormigasAtacandole.Contains(aux))
                {
                    aux.enemigoAlQueAtacarPorOrden.hormigasAtacandole.Add(aux);
                }
                enemigosTotal.Remove(aux.enemigoAlQueAtacarPorOrden);
                Task.current.Succeed();
                //Debug.Log("REINA: Mando soldado a atacar");
            }
            else
            {
                Task.current.Fail();
            }
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void OrdenAtacarObrera()
    {
        ordenesCanvas.OrdenAtacar();

        Obrera aux = obrerasDesocupadas[Random.Range(0, obrerasDesocupadas.Count)];
        if (aux != null)
        {
            EnemigoGenerico enem = enemigosTotal[Random.Range(0, enemigosTotal.Count)];
            // Si no estoy ya atacando a ese enemigo, se me puede mandar la orden
            if (enem != aux.enemigoAlQueAtacar)
            {
                aux.hayOrdenDeAtacar = true;
                aux.enemigoAlQueAtacarPorOrden = enem;
                aux.SacarDeDesocupadas();
                if (!aux.enemigoAlQueAtacarPorOrden.hormigasAtacandole.Contains(aux))
                {
                    aux.enemigoAlQueAtacarPorOrden.hormigasAtacandole.Add(aux);
                }
                enemigosTotal.Remove(aux.enemigoAlQueAtacarPorOrden);
                Task.current.Succeed();
                //Debug.Log("REINA: Mando obrera a atacar");
            }
            else
            {
                Task.current.Fail();
            }
        }
        else
        {
            Task.current.Fail();
        }
    }

    #endregion

    #region Ordenes Buscar Comida

    [Task]
    public void HayEspacioParaDejarComida()
    {
        if (comidaTotal.Count < capacidadTotalDeComida)
        {
            Task.current.Succeed();
            return;
        }
        else
        {
            Task.current.Fail();
            return;
        }
    }

    [Task]
    public void HayComidaSuficiente()
    {
        if (totalHormigas * umbralNecesidadCogerComida < comidaTotal.Count)
        {
            Task.current.Succeed();
            return;
        }
        Task.current.Fail();
        return;
    }

    [Task]
    public void HayHormigasBuscandoComida()
    {

        Task.current.Fail();
        return;
        
    }

    [Task]
    public void HayComidaVistaDisponible()
    {
        if (comidaVista.Count > 0)
        {
            Task.current.Succeed();
            return;
        }
        Task.current.Fail();
        return;
    }

    [Task]
    public void OrdenBuscarComidaObreras()
    {
        Obrera aux = obrerasDesocupadas[0];
        if (aux != null)
        {
            // Si la hormiga ya tiene una hormiga curando
            DesasignarHormigaACurar(aux);
            // Si la hormiga ya tiene una comida asignada
            DesasignarComidaACoger(aux);
            aux.salaDejarComida = reina.MeterComidaEnSala();
            if (aux.salaDejarComida != null)
            {
                // Asignamos el huevo a curar a la hormiga
                aux.hayOrdenBuscarComida = true;
                obrerasOcupadas.Add(aux);
                obrerasDesocupadas.Remove(aux);
                // Actualizamos a la obrera
                aux.casillaDejarComida = aux.salaDejarComida.getFreeTile();
                aux.comida = comidaVista[0];
                aux.comida.CogerComida(aux.salaDejarComida, aux.casillaDejarComida);
                aux.comida.hormigaQueLlevaLaComida = aux;
                aux.posComida = Vector3.zero;
                aux.posDejarComida = Vector3.zero;
                // Si la reina lo tiene en su lista de comida vista, lo borro
                reina.comidaVista.Remove(aux.comida);
                // Actualizo la cantidad de hormigas buscando comida que hay
                numHormigasBuscandoComida++;
                //Debug.Log("REINA: Mando obrera a por comida");
                ordenesCanvas.OrdenBuscarComida();
                Task.current.Succeed();
                return;
            }
        }
        Task.current.Fail();
        return;
    }

    [Task]
    public void OrdenBuscarComidaSoldados()
    {
        Soldado aux = soldadosDesocupadas[0];
        if (aux != null)
        {
            // Si la hormiga ya tiene una hormiga curando
            DesasignarHormigaACurar(aux);
            // Si la hormiga ya tiene una comida asignada
            DesasignarComidaACoger(aux);
            aux.salaDejarComida = reina.MeterComidaEnSala();
            if (aux.salaDejarComida != null)
            {
                // Asignamos el huevo a curar a la hormiga
                aux.hayOrdenBuscarComida = true;
                soldadosOcupadas.Add(aux);
                soldadosDesocupadas.Remove(aux);
                // Actualizamos a la soldado
                aux.casillaDejarComida = aux.salaDejarComida.getFreeTile();
                aux.comida = comidaVista[0];
                aux.comida.CogerComida(aux.salaDejarComida, aux.casillaDejarComida);
                aux.comida.hormigaQueLlevaLaComida = aux;
                aux.posComida = Vector3.zero;
                aux.posDejarComida = Vector3.zero;
                // Si la reina lo tiene en su lista de comida vista, lo borro
                reina.comidaVista.Remove(aux.comida);
                // Actualizo la cantidad de hormigas buscando comida que hay
                numHormigasBuscandoComida++;
                //Debug.Log("REINA: Mando soldado a por comida");
                ordenesCanvas.OrdenBuscarComida();
                Task.current.Succeed();
                return;
            }
        }
        Task.current.Fail();
        return;
    }

    [Task]
    public void OrdenBuscarComidaNurses()
    {
        Nurse aux = nursesDesocupadas[0];
        if (aux != null)
        {
            // Si la hormiga ya estaba cuidando un huevo
            DesasignarHuevoACurar(aux);
            // Si la hormiga ya tiene una hormiga curando
            DesasignarHormigaACurar(aux);
            // Si la hormiga ya tiene una comida asignada
            DesasignarComidaACoger(aux);

            aux.salaDejarComida = reina.MeterComidaEnSala();
            if (aux.salaDejarComida != null)
            {
                // Asignamos la comida a coger por la nurse
                aux.hayOrdenBuscarComida = true;
                nursesOcupadas.Add(aux);
                nursesDesocupadas.Remove(aux);
                // Actualizamos a la nurse
                aux.casillaDejarComida = aux.salaDejarComida.getFreeTile();
                aux.comida = comidaVista[0];
                aux.comida.CogerComida(aux.salaDejarComida, aux.casillaDejarComida);
                aux.comida.hormigaQueLlevaLaComida = aux;
                aux.posComida = Vector3.zero;
                aux.posDejarComida = Vector3.zero;
                // Si la reina lo tiene en su lista de comida vista, lo borro
                reina.comidaVista.Remove(aux.comida);
                // Actualizo el numero de hormigas buscando comida
                numHormigasBuscandoComida++;
                //Debug.Log("REINA: Mando nurse a por comida");
                ordenesCanvas.OrdenBuscarComida();
                Task.current.Succeed();
                return;
            }
        }
        Task.current.Fail();
        return;
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
        if (numHormigasCavandoTuneles > 0)
        {
            Task.current.Succeed();
            return;
        }
        Task.current.Fail();
    }

    [Task]
    public void OrdenCavarObreras()
    {
        ordenesCanvas.OrdenCavar();
        Obrera aux = obrerasDesocupadas[0];
        if (aux != null)
        {
            // Si la hormiga ya tiene una hormiga curando
            DesasignarHormigaACurar(aux);
            // Si la hormiga ya tiene una comida asignada
            DesasignarComidaACoger(aux);
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
            // Si la hormiga ya estaba cuidando un huevo
            DesasignarHuevoACurar(aux);
            // Si la hormiga ya tiene una hormiga curando
            DesasignarHormigaACurar(aux);
            // Si la hormiga ya tiene una comida asignada
            DesasignarComidaACoger(aux);
            // Asignamos el huevo a curar a la hormiga
            aux.hayOrdenCuidarHuevos = true;
            aux.SacarDeDesocupadas();
            aux.huevoACuidar = huevosQueTienenQueSerCuidados[0];
            aux.huevoACuidar.siendoCuidadoPor = aux;
            huevosQueTienenQueSerCuidados.Remove(aux.huevoACuidar);
            ordenesCanvas.OrdenCuidarHuevo();
            Task.current.Succeed();
            return;
        }
        Task.current.Fail();
        return;
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
        Soldado aux = soldadosDesocupadas[0];
        HormigaGenerica aux2 = hormigasHeridas[0];
        if (aux == aux2)
        {
            if (soldadosDesocupadas.Count > 1)
            {
                aux = soldadosDesocupadas[1];
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
            // Si la hormiga ya tiene una hormiga curando
            DesasignarHormigaACurar(aux);
            // Si la hormiga ya tiene una comida asignada
            DesasignarComidaACoger(aux);
            aux.hayOrdenCurarHormiga = true;
            aux.SacarDeDesocupadas();
            aux.hormigaACurar = aux2;
            aux2.siendoCuradaPor = aux;
            aux.posHerida = Vector3.zero;
            hormigasHeridas.Remove(aux2);
            Task.current.Succeed();
            ordenesCanvas.OrdenCurarHormiga();
            //Debug.Log("REINA: Mando soldado a curar");
            return;
        }
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
            // Si la hormiga ya tiene una hormiga curando
            DesasignarHormigaACurar(aux);
            // Si la hormiga ya tiene una comida asignada
            DesasignarComidaACoger(aux);
            aux.hayOrdenCurarHormiga = true;
            aux.SacarDeDesocupadas();
            aux.hormigaACurar = aux2;
            aux2.siendoCuradaPor = aux;
            aux.posHerida = Vector3.zero;
            hormigasHeridas.Remove(aux2);
            //Debug.Log("REINA: Mando obrera a curar");
            ordenesCanvas.OrdenCurarHormiga();
            Task.current.Succeed();
            return;
        }
    }

    [Task]
    public void OrdenCurarNurses()
    {
        Nurse aux = nursesDesocupadas[0];
        HormigaGenerica aux2 = hormigasHeridas[0];
        // Comprobación para que no se cure a si misma
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
            // Si la hormiga ya estaba cuidando un huevo
            DesasignarHuevoACurar(aux);
            // Si la hormiga ya tiene una hormiga curando
            DesasignarHormigaACurar(aux);
            // Si la hormiga ya tiene una comida asignada
            DesasignarComidaACoger(aux);
            // Asignamos la hormiga a cuidar
            aux.hayOrdenCurarHormiga = true;
            aux.SacarDeDesocupadas();
            aux.hormigaACurar = aux2;
            aux2.siendoCuradaPor = aux;
            aux.posHerida = Vector3.zero;
            hormigasHeridas.Remove(aux2);
            //Debug.Log("REINA: Mando nurse a curar");
            ordenesCanvas.OrdenCurarHormiga();
            Task.current.Succeed();
            return;
        }

        Task.current.Fail();
        return;
    }

    #endregion

    #region Ordenes Patrullar

    [Task]
    public void HaHabidoUnAtaqueReciente()
    {
        if (posHormigaMuerta != Vector3.zero && tiempoDePatrullo > 0)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void OrdenPatrullarSoldado()
    {
        Soldado aux = soldadosDesocupadas[0];
        if (aux != null)
        {
            // Si la hormiga ya tiene una hormiga curando
            DesasignarHormigaACurar(aux);
            // Si la hormiga ya tiene una comida asignada
            DesasignarComidaACoger(aux);

            aux.hayOrdenDePatrullar = true;
            aux.SacarDeDesocupadas();
            aux.posicionPatrullar = posHormigaMuerta;
            tiempoDePatrullo = 0;
            Task.current.Succeed();
            //Debug.Log("Reina: Mando soldado a patrullar");
            ordenesCanvas.OrdenPatrullar();
            return;
        }
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
        // Si no tengo ningun huevo asignado, miro los que hay alrededor
        if (huevoACuidar == null)
        {
            // Recorremos la lista de huevos cercanos
            foreach (Huevo h in huevosCerca)
            {
                // Si algun huevo PUEDE ser cuidado y no tiene a nadie asignado, se lo asigno e indico al huevo quien lo cuida
                if (h.siendoCuidadoPor == null && h.necesitaCuidados)
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
        // Si no encuentra ningun huevo
        Task.current.Fail();
        return;
    }

    [Task]
    public void HayNursesEnElHormiguero()
    {
        if (numeroDeNursesTotal > 0)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void HayHuevosQueNecesitanCuidados()
    {
        // Si no tengo ningun huevo asignado, miro los que hay alrededor
        if (huevoACuidar == null)
        {
            // Recorremos la lista de huevos cercanos
            foreach (Huevo h in huevosQueTienenQueSerCuidados)
            {
                // Si algun huevo PUEDE ser cuidado y no tiene a nadie asignado, se lo asigno e indico al huevo quien lo cuida
                if (h.siendoCuidadoPor == null && h.necesitaCuidados)
                {
                    huevoACuidar = h;
                    h.siendoCuidadoPor = this;
                    // Si la reina lo tiene en su lista de huevos que necesitan cuidados, lo borro
                    reina.huevosQueTienenQueSerCuidados.Remove(huevoACuidar);
                    Task.current.Succeed();
                    return;
                }
            }
        }
        else
        {
            Task.current.Succeed();
            return;
        }
        // Si no encuentra ningun huevo
        Task.current.Fail();
        return;
    }


    // CuidarHuevos() --> está en la nurse actualmente solo
    [Task]
    public void CuidarHuevos()
    {
        if (bocadillos.hormigaSeleccionada != null && bocadillos.hormigaSeleccionada == this)
        {
            if (!this.agente.isOnOffMeshLink)
            {
                bocadillos.CuidarHuevos();
            }
            else
            {
                bocadillos.Nada();
            }
        }

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
            if (Vector3.Distance(this.transform.position, huevoACuidar.transform.position) < 0.2f)
            {
                TiempoActual -= Time.deltaTime;
                // Si ha pasado el tiempo de cuidar
                if (TiempoActual <= 0)
                {
                    huevoACuidar.Cuidar();
                    // Debug.Log("Huevo Cuidado");
                    // Reseteas todos los valores
                    TiempoActual = tiempoCuidandoHuevos;
                    posHuevo = Vector3.zero;
                    huevoACuidar.siendoCuidadoPor = null;
                    huevoACuidar = null;
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
            Task.current.Fail();
            return;
        }
    }

    [Task]
    public void PuedoPonerHuevos()
    {
        if (!ponerHuevo)
        {
            tiempoRestanteHuevos -= Time.deltaTime;
            if (tiempoRestanteHuevos < 0)
            {
                ponerHuevo = true;
                Task.current.Succeed();
                return;
            }
            Task.current.Fail();
            return;
        }
        else
        {
            if (huevosQueLlevaPuestosEnEstaTanda == huevosAPonerEnEstaTanda)
            {
                ponerHuevo = false;
                huevosAPonerEnEstaTanda = Random.Range(numeroMinimoHuevosPorTanda, numeroMaximoHuevosPorTanda);
                tiempoRestanteHuevos = tiempoQueTardaEnPonerHuevos;
                huevosQueLlevaPuestosEnEstaTanda = 0;
                numeroDeNurseEnEstaTanda = 0;
                numeroDeObrerasEnEstaTanda = 0;
                numeroDeSoldadoEnEstaTanda = 0;
                Task.current.Fail();
                return;
            }
            Task.current.Succeed();
            return;
        }
    }

    [Task]
    public void PonerHuevos()
    {
        if (bocadillos.hormigaSeleccionada != null && bocadillos.hormigaSeleccionada == this)
        {
            if (!this.agente.isOnOffMeshLink)
            {
                bocadillos.PonerHuevos();
            }
            else
            {
                bocadillos.Nada();
            }
        }
        if (ponerHuevo)
        {
            //Debug.Log(hormigaAponer + " : es la hormiga elegida");
            if (hormigaAponer == TipoHormiga.NULL)
            {
                
                int min = CompareLess3(numeroDeNursesTotal + numeroDeNurseEnEstaTanda, numeroDeObrerasTotal+numeroDeObrerasEnEstaTanda, numeroDeSoldadosTotal+numeroDeSoldadoEnEstaTanda, importanciaNurses, importanciaObreras, importanciaSoldados);
                switch (min)
                {
                    case 0:
                        //Debug.Log("Hormiga Nurse");
                        hormigaAponer = TipoHormiga.NURSE;
                        numeroDeNurseEnEstaTanda++;
                        break;
                    case 1:
                        //Debug.Log("Hormiga Obrera");
                        hormigaAponer = TipoHormiga.OBRERA;
                        numeroDeObrerasEnEstaTanda++;
                        break;
                    case 2:
                        //Debug.Log("Hormiga Soldado");
                        hormigaAponer = TipoHormiga.SOLDADO;
                        numeroDeSoldadoEnEstaTanda++;
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
                GameObject huevoAux = Instantiate(PrefabHuevo, posicionParaColocarHuevo, Quaternion.identity);
                Huevo huevoScript = huevoAux.GetComponent<Huevo>();
                huevoScript.Init(salaDondePonerHuevo, hormigaAponer, casillaDondePonerHuevo);
                MeterHuevosEnSala(salaDondePonerHuevo);
                huevosTotal.Add(huevoScript);
                huevoScript.miType = hormigaAponer;
                ponerHuevo = false;
                tienePosicionPonerHuevo = false;
                hormigaAponer = TipoHormiga.NULL;
                salaDondePonerHuevo = null;
                casillaDondePonerHuevo = null;
                estaPoniendoHuevo = false;
                huevosQueLlevaPuestosEnEstaTanda++;
                Task.current.Succeed();
                return;

            }
            else
            {
                agente.SetDestination(posicionParaColocarHuevo);
                Task.current.Succeed();
                return;
            }
        }
        else
        {
            Task.current.Fail();
            return;
        }

    }

    [Task]
    public void Esperar()
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
        if (zonaDondeEsta == 1)
        {
            //Debug.Log("Esta fuera");
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
            }
            else
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

    public static int CompareLess3(int num1, int num2, int num3, float importancia1, float importancia2, float importancia3)
    {
        if (importancia1 <= 0)
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

        if (importancia1 != 0)
        {
            num1 = (int)(num1 / importancia1);
        }

        if (importancia2 != 0)
        {
            num2 = (int)(num2 / importancia2);
        }

        if (importancia3 != 0)
        {
            num3 = (int)(num3 / importancia3);
        }


        int min = num1;
        int finalNum = 0;

        if (min > num2)
        {
            min = num2;
            finalNum = 1;
        }

        if (min > num3)
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
        comidaVista.Remove(comida);

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

    // Métodos para des-asignar cuando se manda una orden
    public void DesasignarHuevoACurar(HormigaGenerica hormiga)
    {
        if (hormiga.huevoACuidar != null)
        {
            if (hormiga.huevoACuidar.necesitaCuidados == true && !reina.huevosQueTienenQueSerCuidados.Contains(hormiga.huevoACuidar))
            {
                reina.huevosQueTienenQueSerCuidados.Add(hormiga.huevoACuidar);
            }
            hormiga.huevoACuidar.siendoCuidadoPor = null;
            hormiga.huevoACuidar = null;
        }
    }

    public void DesasignarHormigaACurar(HormigaGenerica hormiga)
    {
        if (hormiga.hormigaACurar != null)
        {
            if (hormiga.hormigaACurar.necesitaSerCurada && !reina.hormigasHeridas.Contains(hormiga.hormigaACurar))
            {
                reina.hormigasHeridas.Add(hormiga.hormigaACurar);
            }
            hormiga.hormigaACurar.siendoCuradaPor = null;
            hormiga.hormigaACurar = null;
        }
    }

    public void DesasignarComidaACoger(HormigaGenerica hormiga)
    {
        if (hormiga.comida != null)
        {
            hormiga.comida.transform.SetParent(null);
            hormiga.comida.laEstanLLevando = false;
            hormiga.comida.haSidoCogida = false;
            reina.SacarComidaSala(hormiga.salaDejarComida, hormiga.comida, hormiga.casillaDejarComida);
            hormiga.comida.hormigaQueLlevaLaComida = null;
            hormiga.comida = null;
            hormiga.salaDejarComida = null;
            hormiga.casillaDejarComida = null;
            hormiga.posComida = Vector3.zero;
            hormiga.posDejarComida = Vector3.zero;

            if (reina.comidaVista.Contains(hormiga.comida))
            {
                reina.comidaVista.Add(hormiga.comida);
            }
        }
    }

    // Métodos para el tratamiento de creaciones
    public void NaceHormiga(Huevo huevo)
    {
        switch (huevo.miType)
        {
            case TipoHormiga.NURSE:
                //numeroDeNursesTotal++;
                //totalHormigas++;
                Vector3 posNurse = huevo.transform.position;
                GameObject aux = Instantiate(prefabNurse, posNurse, Quaternion.identity);
                //aux.transform.position = huevo.transform.position;
                //nursesDesocupadas.Add(aux.GetComponent<Nurse>());
                //Se instanciaria el objeto de hormiga
                break;
            case TipoHormiga.OBRERA:
                //numeroDeObrerasTotal++;
                //totalHormigas++;
                Vector3 posObrera = huevo.transform.position;
                GameObject aux2 = Instantiate(prefabObrera, posObrera, Quaternion.identity);
                // se instanciaria
                break;
            case TipoHormiga.SOLDADO:
                //numeroDeSoldadosTotal++;
                //totalHormigas++;
                Vector3 posSoldado = huevo.transform.position;
                GameObject aux3 = Instantiate(prefabSoldado, posSoldado, Quaternion.identity);
                break;
        }
    }

    public void CrearComida()
    {
        if (Time.timeScale == 0)
        {
            // Nada
        }
        else if (Time.timeScale == 1)
        {
            saleComida = Random.RandomRange(0, MaxProbComida + 1);
            if (saleComida < probComida)
            {
                InstanciarComida();
            }
        }
        else if (Time.timeScale == 2)
        {
            saleComida = Random.RandomRange(0, MaxProbComida + 1);
            if (saleComida < probComida)
            {
                InstanciarComida();
            }
            saleComida = Random.RandomRange(0, MaxProbComida + 1);
            if (saleComida < probComida)
            {
                InstanciarComida();
            }
        }
        else
        {
            saleComida = Random.RandomRange(0, MaxProbComida + 1);
            if (saleComida < probComida)
            {
                InstanciarComida();
            }
            if (!par)
            {
                saleComida = Random.RandomRange(0, MaxProbComida + 1);
                if (saleComida < probComida)
                {
                    InstanciarComida();
                }
                par = true;
            }
            else
            {
                par = false;
            }
        }
        
    }

    public void InstanciarComida()
    {
        Vector3 centro = afueras.centro;
        float posX = Random.RandomRange(-(afueras.width / 2)+2, (afueras.width / 2)-2);
        float posZ = Random.RandomRange(-(afueras.heigth / 2)+2, (afueras.heigth / 2)-2);

        centro.x += posX;
        centro.z += posZ;

        GameObject aux = Instantiate(comidaPrefab, centro, Quaternion.identity);
        aux.GetComponent<Comida>().InitComida(Comida.comidaType.Trigo);
    }

    public void InstanciarComidaPos(Vector3 pos)
    {
        GameObject aux = Instantiate(comidaPrefab, pos, Quaternion.identity);
        aux.GetComponent<Comida>().InitComida(Comida.comidaType.Trigo);
    }

    // Tarea Poner Huevo de Reina

    #region Meter Cosas en salas
    public Room MeterComidaEnSala()
    {
        Room sala = GetSalaLibreComida();
        if (sala != null)
        {
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
        //Debug.Log("Hormiga A Muerto");

        GameObject aux = Instantiate(hormiga.tumbaHormiga, hormiga.transform.position, Quaternion.identity);
        aux.transform.Translate(0, 0.03f, 0);
        aux.transform.Rotate(90, 0, 0);
        if (hormiga.bocadillos.hormigaSeleccionada != null && hormiga.bocadillos.hormigaSeleccionada == this)
        {
            hormiga.bocadillos.Nada();
        }

        // Actualizamos a todos los enemigos que tenga
        foreach (EnemigoGenerico enem in hormiga.enemigosCerca)
        {
            enem.hormigasCerca.Remove(hormiga);
            if (enem.hormigaAAtacar == hormiga)
            {
                enem.hormigaAAtacar = null;
            }
        }
        // Actualizo al enemigo que estaba atacando para que lo elimine de su lista de hormigasAtacandole
        if (hormiga.enemigoAlQueAtacar != null)
        {
            hormiga.enemigoAlQueAtacar.hormigasAtacandole.Remove(hormiga);
        }
        if (hormiga.enemigoAlQueAtacarPorOrden != null)
        {
            hormiga.enemigoAlQueAtacarPorOrden.hormigasAtacandole.Remove(hormiga);
        }
        // Actualizamos a todos los huevos que tenga
        foreach (Huevo huevo in hormiga.huevosCerca)
        {
            huevo.hormigasCerca.Remove(hormiga);
        }
        // Actualizamos a todos las comidas que tenga
        foreach (Comida comida in hormiga.comidaQueHayCerca)
        {
            comida.hormigasCerca.Remove(hormiga);
        }
        // Eliminar a la hormiga de todas las listas
        if (hormigasHeridas.Contains(hormiga))
        {
            hormigasHeridas.Remove(hormiga);
        }

        // Depende de la hormiga que sea, se saca de una lista u otra
        Nurse hormigaNurse = hormiga.transform.gameObject.GetComponent(typeof(Nurse)) as Nurse;
        Soldado hormigaSoldado = hormiga.transform.gameObject.GetComponent(typeof(Soldado)) as Soldado;
        Obrera hormigaObrera = hormiga.transform.gameObject.GetComponent(typeof(Obrera)) as Obrera;

        if (hormigaNurse != null)
        {
            //Debug.Log("NURSE HA MUERTO");
            //Nurse hormigaNurse = (Nurse)hormiga;
            if (nursesDesocupadas.Contains(hormigaNurse))
            {
                nursesDesocupadas.Remove(hormigaNurse);
            }
            else if (nursesOcupadas.Contains(hormigaNurse))
            {
                nursesOcupadas.Remove(hormigaNurse);
            }
            numeroDeNursesTotal--;
        }
        else if (hormigaObrera != null)
        {
            if (obrerasDesocupadas.Contains(hormigaObrera))
            {
                obrerasDesocupadas.Remove(hormigaObrera);
            }
            else if (obrerasOcupadas.Contains(hormigaObrera))
            {
                obrerasOcupadas.Remove(hormigaObrera);
            }
            numeroDeObrerasTotal--;
        }
        else if (hormigaSoldado != null)
        {
            if (soldadosDesocupadas.Contains(hormigaSoldado))
            {
                soldadosDesocupadas.Remove(hormigaSoldado);
            }
            else if (soldadosOcupadas.Contains(hormigaSoldado))
            {
                soldadosOcupadas.Remove(hormigaSoldado);
            }
            numeroDeSoldadosTotal--;
        }
        else
        {
            // Si es la reina, sería fin de la simulación
            SceneManager.LoadScene(2);
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
            if (hormiga.hayOrdenBuscarComida)
            {
                hormiga.hayOrdenBuscarComida = false;
                reina.numHormigasBuscandoComida--;
            }
            hormiga.comida.laEstanLLevando = false;
            hormiga.comida.hormigaQueLlevaLaComida = null;
            hormiga.comida.transform.SetParent(null);
            /*hormiga.comida.misala = null;
            hormiga.comida.miTile = null;*/
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
        // Si la hormiga muere en el link
        if (hormiga.agente.isOnOffMeshLink)
        {
            NavMeshLink link = (NavMeshLink) hormiga.agente.navMeshOwner;
            if(link != null)
            {
                link.costModifier = -1;
            }
        }
        hormiga = null;
    }

    public void EnemigoHaMuerto(EnemigoGenerico enemigo)
    {
        InstanciarComidaPos(enemigo.transform.position);

        // Avisamos a la hormiga de que el enemigo ha muerto
        foreach (HormigaGenerica h in enemigo.hormigasCerca)
        {
            h.enemigosCerca.Remove(enemigo);
        }

        // Avisamos a todas las hormigas que vayan a atacar a este enemigo
        foreach (HormigaGenerica h in enemigo.hormigasAtacandole)
        {
            if (h.enemigoAlQueAtacar == enemigo)
            {
                h.enemigoAlQueAtacar = null;
            }
            if (h.enemigoAlQueAtacarPorOrden == enemigo)
            {
                h.enemigoAlQueAtacarPorOrden = null;
                h.hayOrdenDeAtacar = false;
                h.SacarDeOcupadas();
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
        if (comidaMuerta.hormigaQueLlevaLaComida != null)
        {
            GameObject aux = Instantiate(comidaMuerta.comidaMuerta, comidaMuerta.transform.position, Quaternion.identity);
            aux.transform.Translate(0, 0.03f, 0);
            aux.transform.Rotate(90, 0, 0);
            SacarComidaSala(comidaMuerta.misala, comidaMuerta, comidaMuerta.miTile);
        }
        else if (comidaMuerta.haSidoCogida)
        {
            SacarComidaSala(comidaMuerta.misala, comidaMuerta, comidaMuerta.miTile);
        }
        else
        {
            //Debug.Log("LA COMIDA SE HA MUERTO SOLA, NO SE SACA DE SALA");
        }

        // Actualizamos si la comida está siendo llevada por una hormiga (no necesario)
        if (comidaMuerta.hormigaQueLlevaLaComida != null)
        {
            if (comidaMuerta.hormigaQueLlevaLaComida.hayOrdenBuscarComida)
            {
                comidaMuerta.hormigaQueLlevaLaComida.hayOrdenBuscarComida = false;
                numHormigasBuscandoComida--;
            }
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
        comidaTotal.Remove(comida);
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
