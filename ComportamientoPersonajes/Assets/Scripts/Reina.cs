﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using UnityEngine.AI;

public class Reina : HormigaGenerica
{

    public enum TipoHormiga { NURSE, OBRERA,SOLDADO,NULL};

    #region atributos propios de la reina

    NavMeshAgent agente;
    public Vector3 siguientePosicionExplorar;

    //Poner huevos
    public int tiempoQueTardaEnPonerHuevo;
    public float tiempoQueLllevaPoniendoHuevo;
    public bool estaPoniendoHuevo = false;
    private bool tienePosicionPonerHuevo = false;
    private TipoHormiga hormigaAponer = TipoHormiga.NULL;
    private Room salaDondePonerHuevo = null;
    public GameObject PrefabHuevo;
    public Vector3 posicionParaColocarHuevo;
    public bool puedePonerHuevo;
    public int tiempoMaximoParaPonerHuevo;
    public float tiempoRestanteHuevo;

    //Cuidar huevos
    public int numeroDeNurses;
    public List<Huevo> huevosQueTienenQueSerCuidados = new List<Huevo>();

    //Atacar
    public bool hayEnemigosCerca;
    public EnemigoGenerico[] enemigosCercanos;
    public int numEnemigosCerca;

    //Curar
    public HormigaGenerica hormigasACurar;
    public HormigaGenerica hormigasDesocupadas;

    //Comer
    public Comida comida;



    
    #endregion

    #region logica global hormiguero
    public int numeroDeNursesTotal;
    public int numeroDeObrerasTotal;
    public int numeroDeSoldadosTotal;
    public List<EnemigoGenerico> enemigosTotales = new List<EnemigoGenerico>();
    public List<Comida> ComidaTotal = new List<Comida>();
    public List<Huevo> huevosTotal = new List<Huevo>();
    public HormigaGenerica[] hormigasHeridas;
    public List<Nurse> nursesOcupadas;
    public Obrera[] obrerasOcupadas;
    public Soldado[] soldadosOcupadas;
    public List<Nurse> nursesDesocupadas = new List<Nurse>();
    public Obrera[] obrerasDesocupadas;
    public Soldado[] soldadosDesocupadas;
    public int numHormigasCuidandoHuevos;
    public int numHormigasAtacando;
    public int numHormigasBuscandoComida;
    public int numHormigasCavandoTuneles;
    public int numHormigasPatrullando;
    public int numHormigasCurando;
    public int numHormigasAbriendoHormiguero;
    public int numHormigasAbriendoCerrandoHormiguero;
    public int tiempoQueQuedaParaQueLlueva;
    public int tiempoQueLlueve;
    public int tiempoGlobal;
    public int capacidadTotalDeHormigas;
    public int capacidadTotalDeComida;
    public int capacidadTotalDeHuevos;
    public int totalHormigas;
    public int totalComida;
    public int totalHuevos;
    //FALTA ESTRUCTURA DE DATOS DEL MAPA
    private Floor hormiguero;
    private Outside afueras;



    public List<Room> salasHormigas = new List<Room>();
    public List<Room> salasComida = new List<Room>();
    public List<Room> salasHuevos = new List<Room>();

    public int probComida = 10;
    public int MaxProbComida = 1000;

    float initTime;
    float actualTime;

    // CosasVistas
    public LinkedList<Comida> ComidaVista = new LinkedList<Comida>();



    //IMPORTANCIA DE OBJETIVOS
    public float importanciaHormigas;
    public float importanciaHuevos;
    public float importanciaComida;

    public float importanciaNurses;
    public float importanciaObreras;
    public float importanciaSoldados;


    //NECESIDADES DEL HORMIGUERO
    // MandarHordenes:
    enum tipoOrden { CAVAR, BUSCAR, ATACAR, CUIDAR, PATRULLAR, NADA }
    private bool HayQueCrearSalasHormigas = false;
    private bool HayQueCrearSalasComida = false;
    private bool HayQueCrearSalasHuevos = false;
    private bool HayQueCrearSalas = false;


    public bool HayQueBuscarComida = false;
    private bool HayQueAtacar = false;
    private bool HayQuePatrullar = false;
    private bool HayQueCuidarHuevos = false;

    //para ver si hay que mandar patrullar
    public bool hormigaMuerta = false;

    
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
    public float umbralComida = 0.4f;
    #endregion

    #region prefabs
    public GameObject comidaPrefab;
    public GameObject prefabNurse;
    #endregion

    #region Variables Auxiliares Durante el desarrollo
    public bool crearSala = false;
    public bool ponerHuevo = false;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        tiempoQueLllevaPoniendoHuevo = 0;
        tiempoRestanteHuevo = tiempoMaximoParaPonerHuevo;
        tamañoMaxColaConstruccionSalas = 1;
        agente = this.gameObject.GetComponent<NavMeshAgent>();
        hormiguero = GameObject.FindObjectOfType<Floor>();
        afueras = GameObject.FindObjectOfType<Outside>();
        initTime = Time.time;
        siguientePosicionExplorar = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        actualTime = Time.time - initTime;
        if (crearSala)
        {
            ConstruirSala();
            crearSala = false;
        }
        SimulateWorld();
        actualizarPercepcionesHormiguero();
        actualizarVariablesReina();
    }

     public void SimulateWorld()
    {
        crearComida();
    }

    public void actualizarPercepcionesHormiguero()
    {
        actualizarPrioridades();
        comprobarCreacionSalas();
        checkearNecesidadComida();
    }

    public void actualizarVariablesReina()
    {
        tiempoRestanteHuevo -= Time.deltaTime;
        if (tiempoRestanteHuevo < 0 && !ponerHuevo)
        {
            ponerHuevo = true;
        }
    }

    #region Funciones simulacion mundo

    public void crearComida()
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
            aux.GetComponent<Comida>().initComida(Comida.comidaType.Trigo);

        }
    }

    #endregion

    #region Funciones actualizar prioridades Reina

    public void actualizarPrioridades()
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

    public void comprobarCreacionSalas()
    {
        // comprobar si hay que crear Sala de Hormigas, se hace comprobando que haya capacidad para un 20% mas de hormigas
        if (totalHormigas * umbralCapacidadHormigas >= capacidadTotalDeHormigas - totalHormigas)
        {
            HayQueCrearSalasHormigas = true;
        }
        if (totalHuevos * umbralCapacidadHuevos >= capacidadTotalDeHuevos - totalHuevos)
        {
            HayQueCrearSalasHuevos = true;
        }
        if (totalComida * umbralCapacidadComida >= capacidadTotalDeComida - totalComida)
        {
            HayQueCrearSalasComida = true;
        }

        if (HayQueCrearSalasHormigas || HayQueCrearSalasComida || HayQueCrearSalasHuevos)
        {
            HayQueCrearSalas = true;
        }
        else
        {
            HayQueCrearSalas = false;
        }


    }

    public void checkearNecesidadComida()
    {
        if(totalHormigas * umbralComida  > totalComida)
        {
            Debug.Log(totalHormigas * umbralComida);
            if(totalComida < capacidadTotalDeComida)
            {
                HayQueBuscarComida = true;
                //Debug.Log("Hay que buscar comida");
            } else
            {
                HayQueBuscarComida = false;
            }
        }
    }

    public void checkearNecesidadCuidarHuevos()
    {
        if(huevosQueTienenQueSerCuidados.Count >= 0)
        {
            HayQueCuidarHuevos = true;
        } else
        {
            HayQueCuidarHuevos = false;
        }
    }

    public void checkearNecesidadAtacar()
    {
        if(enemigosTotales.Count >= 0)
        {
            HayQueAtacar = true;
        } else
        {
            HayQueAtacar = false;
        }
    }

    // se ha muerto una hormiga
    public void chaeckearNecesiadadPatrullar()
    {
        if (hormigaMuerta)
        {
            HayQuePatrullar = true;
        } else
        {
            HayQuePatrullar = false;
        }
    }

    #endregion





    // Devuelve la sala porque sera necesario asignarsela a las homirmigas comidas y huevos para manejarlas
    #region Meter Cosas en salas
    public Room meterComidaEnSala()
    {
        Room sala = getSalaLibreComida();
        if(sala != null)
        {
            sala.meterCosas();
            totalComida++;
        }
        return sala; 
    }

    public Room meterHormigaEnSala()
    {
        Room sala = getSalaLibreHormigas();
        if (sala != null)
        {
            sala.meterCosas();
            totalHormigas++;
        }
        return sala;
    }

    public Room meterHuevosEnSala(Room sala)
    {
        if(sala != null)
        {
            sala.meterCosas();
            totalHuevos++;
        }
        return sala;
    }
    #endregion

    #region Metodos para sacar cosas de las salas
    // necesitamos la sala dode se guardan las cosas
    public void sacarHormigaSala(Room sala)
    {
        sala.sacarCosas();
        totalHormigas--;
    }

    public void sacarComidaSala(Room sala, Comida comida)
    {
        ComidaTotal.Remove(comida);
        sala.sacarCosas();
        totalComida--;
    }

    public void sacarHuevosSala(Room sala)
    {
        sala.sacarCosas();
        totalHuevos--;
    }
    #endregion

    // devuleve una sala de cada tipo con espacio
    #region geters de salas libres
    public Room getSalaLibreHormigas()
    {
        foreach( Room aux in salasHormigas)
        {
            if (!aux.isFull)
            {
                return aux;
            }
        }
        return null;
    }

    public Room getSalaLibreComida()
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

    public Room getSalaLibreHuevos()
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
                Room Aux = getSalaLibreHuevos();
                if (Aux != null)
                {
                    salaDondePonerHuevo = Aux;
                }
                else
                {
                    Debug.Log("No hay sala de huevos");
                    tienePosicionPonerHuevo = false;
                    hormigaAponer = TipoHormiga.NULL;
                    salaDondePonerHuevo = null;
                    ponerHuevo = false;
                    Task.current.Fail();
                    return;


                }


            }



            if (!tienePosicionPonerHuevo)
            {
                posicionParaColocarHuevo = salaDondePonerHuevo.getRandomPosition();
                agente.SetDestination(posicionParaColocarHuevo);
                tienePosicionPonerHuevo = true;
            }
            float auxDistance = Vector3.Distance(this.transform.position, posicionParaColocarHuevo);
            if (auxDistance < 0.1 && tienePosicionPonerHuevo)
            {
                estaPoniendoHuevo = true;
                if(tiempoQueLllevaPoniendoHuevo < tiempoQueTardaEnPonerHuevo)
                {
                    tiempoQueLllevaPoniendoHuevo += Time.deltaTime;
                } else
                {
                    GameObject huevoAux = Instantiate(PrefabHuevo, posicionParaColocarHuevo, Quaternion.identity);
                    Huevo huevoScript = huevoAux.GetComponent<Huevo>();
                    huevoScript.init(salaDondePonerHuevo, hormigaAponer);
                    meterHuevosEnSala(salaDondePonerHuevo);
                    huevosTotal.Add(huevoScript);
                    huevoScript.miType = hormigaAponer;
                    ponerHuevo = false;
                    tienePosicionPonerHuevo = false;
                    hormigaAponer = TipoHormiga.NULL;
                    salaDondePonerHuevo = null;
                    ponerHuevo = false;
                    tiempoRestanteHuevo = tiempoMaximoParaPonerHuevo;
                    estaPoniendoHuevo = false;
                    tiempoQueLllevaPoniendoHuevo = 0;
                    Task.current.Succeed();
                }
               
            }


        } else
        {
            Task.current.Fail();
        }

    }

    [Task]
    public void Explorar()
    {
        if (this.zonaDondeEsta == 1)
        {
            //Esta fuera, tiene que entrar
            Debug.Log("Estoy fuera ais que tengo que entrar");
            Vector3 randomDirection;
            NavMeshHit aux;
            bool aux2;
            do
            {
                randomDirection = UnityEngine.Random.insideUnitSphere * 100 + this.transform.position;
                aux2 = NavMesh.SamplePosition(randomDirection, out aux, 1.0f, NavMesh.AllAreas);
            } while (aux.position.x < (hormiguero.transform.position.x - (hormiguero.width / 2)) || !aux2);
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
                    } while (!aux2 || (aux.position.x < (hormiguero.transform.position.x - (hormiguero.width / 2))));
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

    [Task]
    public void ConstruirSala()
    {
        int min = -1;


        int capacidadRestanteHormigas = capacidadTotalDeHormigas - totalHormigas;
        int capacidadRestanteComida = capacidadTotalDeComida - totalComida;
        int capacidadRestanteHuevos = capacidadTotalDeHuevos - totalHuevos;


        if (HayQueCrearSalasComida && HayQueCrearSalasHuevos && HayQueCrearSalasHormigas)
        {
            min = CompareLess3(capacidadRestanteHormigas, capacidadRestanteComida, capacidadRestanteHuevos, importanciaHormigas, importanciaComida, importanciaHuevos);
        } else if(HayQueCrearSalasHormigas && HayQueCrearSalasHuevos)
        {
            min = CompareLess3(capacidadRestanteHormigas, capacidadRestanteComida, capacidadRestanteHuevos, importanciaHormigas, importanciaComida, 0);
        } else if(HayQueCrearSalasHormigas && HayQueCrearSalasComida)
        {
            min = CompareLess3(capacidadRestanteHormigas, capacidadRestanteComida, capacidadRestanteHuevos, importanciaHormigas, 0, importanciaHuevos);
        } else if (HayQueCrearSalasComida && HayQueCrearSalasHuevos)
        {
            min = CompareLess3(capacidadRestanteHormigas, capacidadRestanteComida, capacidadRestanteHuevos, 0, importanciaComida, importanciaHuevos);
        } else if (HayQueCrearSalasHormigas)
        {
            min = 0;
        } else if (HayQueCrearSalasComida)
        {
            min = 1;
        } else if(HayQueCrearSalasHuevos)
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
                aux = hormiguero.createCorridor(Room.roomType.LIVEROOM);
                if (aux != null)
                {
                    capacidadTotalDeHormigas += aux.capacidadTotalRoom;
                    salasHormigas.Add(aux);
                    HayQueCrearSalasHormigas = false;
                    Debug.Log("Sala de Hormigas creada, la capacidad ahora es: " + capacidadTotalDeHormigas);
                    Task.current.Succeed();
                }
                else
                {
                    Task.current.Fail();
                }

                break;
            case 1:

                aux = hormiguero.createCorridor(Room.roomType.STORAGE);
                if (aux != null)
                {
                    salasComida.Add(aux);
                    capacidadTotalDeComida += aux.capacidadTotalRoom;
                    HayQueCrearSalasComida = false;
                    Debug.Log("Sala de Comida creada, la capacidad ahora es: " + capacidadTotalDeComida);
                    Task.current.Succeed();
                }
                else
                {
                    Task.current.Fail();
                }
                break;
            case 2:

                aux = hormiguero.createCorridor(Room.roomType.STORAGE);

                if (aux != null)
                {
                    salasHuevos.Add(aux);
                    capacidadTotalDeHuevos += aux.capacidadTotalRoom;
                    HayQueCrearSalasHuevos = false;
                    Debug.Log("Sala de Huevos creada, la capacidad ahora es: " + capacidadTotalDeHuevos);
                    Task.current.Succeed();
                }
                else
                {
                    Task.current.Fail();
                }


                break;
        }
    }

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

    public void NaceHuevo(Huevo huevo)
    {
        totalHuevos--;
        huevosTotal.Remove(huevo);
        if (huevosQueTienenQueSerCuidados.Contains(huevo))
        {
            huevosQueTienenQueSerCuidados.Remove(huevo);
        }
        switch(huevo.miType)
        {
            case TipoHormiga.NURSE:
                numeroDeNursesTotal++;
                totalHormigas++;
                /*
                GameObject aux = Instantiate(prefabNurse,);
                aux.transform.position = huevo.transform.position;
                nursesDesocupadas.Add(aux.GetComponent<Nurse>());
                */
                //Se instanciaria el objeto de hormiga
                break;
            case TipoHormiga.OBRERA:
                numeroDeObrerasTotal++;
                totalHormigas++;
                // se instanciaria
                break;
            case TipoHormiga.SOLDADO:
                numeroDeSoldadosTotal++;
                totalHormigas++;
                break;

        }

    }

    public void recibirAlertaComida(Comida comida)
    {
        if (!ComidaVista.Contains(comida) && !comida.laEstanLLevando)
        {
            ComidaVista.AddLast(comida);
        }
    }

    public void comidaGuardada(Comida comida, Room sala)
    {
        if (!ComidaTotal.Contains(comida))
        {
            ComidaTotal.Add(comida);
            comida.misala = sala;
            totalComida++;
            sala.meterCosas();
        }
    }

    public Comida pedirComida()
    {
      return ComidaTotal[0];
        
    }

    public void huevoNecesitaCuidado(Huevo miHuevo)
    {
        if (!huevosQueTienenQueSerCuidados.Contains(miHuevo))
        {
            huevosQueTienenQueSerCuidados.Add(miHuevo);
        }

    }

    public void huevoCuidado(Huevo miHuevo)
    {
        huevosQueTienenQueSerCuidados.Remove(miHuevo);
    }

    public void huevoMuerto(Huevo miHuevo)
    {
        huevosQueTienenQueSerCuidados.Remove(miHuevo);
        huevosTotal.Remove(miHuevo);
    }


    #region mandarOrdenes
    [Task]
    public void atacados()
    {

    }



    #endregion
    public void MandarOrden()
    {

    }
    
}
