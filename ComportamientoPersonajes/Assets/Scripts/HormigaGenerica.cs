using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using UnityEngine.AI;

public class HormigaGenerica : PersonajeGenerico
{
    //Bocadillos
    public BocadillosControlador bocadillos;
    public bool bocadillosFound = false;

    //Agente Navmesh
    public NavMeshAgent agente;
    public int priority;
    protected PandaBehaviour pb;
    protected Floor hormigueroDentro; //Saber donde empieza el suelo para no meterte dentro del hormiguero cuando exploras
    protected Outside hormigueroFuera;

    // ATRIBUTOS ////////////////////////////////////////////////////////////////////////////////////////////////

    // Atributos de las hormigas generales
    [Header("Atributos generales hormiga")]
    public float hambre = 300;
    public float umbralHambre = 200;
    public bool tengoHambre = false;
    public float umbralHambreMaximo = 100;
    public bool tengoMuchaHambre = false;
    public float pesoQuePuedenTransportar;
    public bool estaLuchando = false;
    protected float tiempoEntreAtaques;
    [HideInInspector]
    protected float tiempoEntreAtaquesMax = 0.5f;
    protected float TiempoActual;
    public Room miSala;

    // Atacar
    [Header("Variables ataque")]
    //public EnemigoGenerico enemigoCerca = null;
    public List<EnemigoGenerico> enemigosCerca = new List<EnemigoGenerico>();
    //Orden atacar
    public bool hayOrdenDeAtacar = false;
    public EnemigoGenerico enemigoAlQueAtacar = null;
    public bool reinaCerca = false;
    public bool obrerasCerca = false;
    public bool soldadosCerca = false;

    //Buscar Comida
    [Header("Variables Búsqueda Comida")]
    public Vector3 siguientePosicionBuscandoComida;
    public Comida comida;
    public Room salaDejarComida = null;
    public TileScript casillaDejarComida = null;
    public Vector3 posComida = Vector3.zero;
    public Vector3 posDejarComida = Vector3.zero;
    public List<Comida> comidaQueHayCerca = new List<Comida>();
    //Orden Buscar Comida
    public bool hayOrdenBuscarComida = false;

    //Curar otras hormigas
    [Header("Variables Curar Hormigas")]
    public float tiempoParaCurar = 10.0f;
    public Vector3 posHerida = Vector3.zero;
    public List<HormigaGenerica> hormigasCerca = new List<HormigaGenerica>();
    public HormigaGenerica hormigaACurar = null;
    public HormigaGenerica siendoCuradaPor = null;
    public int cantidadDeCura = 3;
    public bool puedeSerCurada = false;
    public bool necesitaSerCurada = false;
    public int umbralPuedeCurarse = 9;
    public int umbralNecesitaCurarse = 3;
    //Orden Curar Hormiga
    public bool hayOrdenCurarHormiga = false;

    //Cuidar Huevos
    public float tiempoCuidandoHuevos = 10.0f;
    public Huevo huevoACuidar = null;
    public Vector3 posHuevo = Vector3.zero;
    public List<Huevo> huevosCerca = new List<Huevo>();
    // Orden Cuidar Huevos
    public bool hayOrdenCuidarHuevos = false;

    // Huir cuando te ataquen
    bool hayHormigasAtacandoAlEnemigo = false;

    //Explorar
    [Header("Explorar")]
    public Vector3 siguientePosicionExplorar;

    // Comer
    [Header("Comida a Comer")]
    public Comida comidaAComer;

    // Reina
    [Header("REINA")]
    public Reina reina;
    protected Vector3 posicionReina;
    //Ordenes de la reina
    public bool meHanMandadoOrden = false;

    // Sistema de vision
    int numRayosExtra = 3;
    int numRayosFijos = 4;
    int RayDistance = 10;

    // Cavar
    public bool hayOrdenDeCavar = false;



    // CODIGO ///////////////////////////////////////////////////////////////////////////////////////////////

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!bocadillosFound)
        {
            bocadillos = FindObjectOfType<BocadillosControlador>();
            if (bocadillos != null)
            {
                Debug.Log("Encontrado");
                bocadillosFound = true;
            }
        }

        ActualizarHambre();

        ActualizarSiPuedeSerCurada();

        SistemaDeVision();
    }

    // Para detección de las hormigas (lista hormigasCerca)
    public void SistemaDeVision()
    {
        hormigasCerca = new List<HormigaGenerica>();
        obrerasCerca = false;
        soldadosCerca = false;
        reinaCerca = false;
        RaycastHit hit;

        Vector3 dir = Vector3.zero;

        // Rayos fijos
        for (int i = 0; i < numRayosFijos; i++)
        {
            switch (i)
            {
                case 0:// delante
                    dir = transform.TransformDirection(Vector3.forward);
                    break;
                case 1: // detras
                    dir = transform.TransformDirection(Vector3.back);
                    break;
                case 2: // izq
                    dir = transform.TransformDirection(Vector3.left);
                    break;
                case 3: //der
                    dir = transform.TransformDirection(Vector3.right);
                    break;
            }

            if (Physics.Raycast(transform.position, dir, out hit, RayDistance))
            {
                GameObject objetoColision = hit.collider.gameObject;

                if (objetoColision.transform.parent != null)
                {
                    if (objetoColision.transform.parent.gameObject != this.gameObject)
                    {
                        Obrera hormigaObrera = objetoColision.transform.parent.gameObject.GetComponent<Obrera>();
                        Soldado hormigaSoldado = objetoColision.transform.parent.gameObject.GetComponent<Soldado>();
                        Reina hormigaReina = objetoColision.transform.parent.gameObject.GetComponent<Reina>();
                        if (hormigaObrera != null)
                        {
                            obrerasCerca = true;
                        }
                        if (hormigaSoldado != null)
                        {
                            soldadosCerca = true;
                        }
                        if (hormigaReina != null)
                        {
                            reinaCerca = true;
                        }
                        if (!hormigasCerca.Contains(objetoColision.transform.parent.gameObject.GetComponent<HormigaGenerica>()))
                        {
                            hormigasCerca.Add(objetoColision.transform.parent.gameObject.GetComponent<HormigaGenerica>());
                        }
                    }
                    else
                    {
                        //Debug.Log("Chocandote contigo mismo");
                    }
                }
            }

        }

        // Rayos Móviles
        for (int j = 0; j < numRayosExtra; j++)
        {
            dir = new Vector3(Random.Range(-100, 101), 0, Random.Range(-100, 101));
            dir = dir.normalized;
            if (Physics.Raycast(transform.position, dir, out hit, RayDistance))
            {
                GameObject objetoColision = hit.collider.gameObject;

                if (objetoColision.transform.parent != null)
                {
                    if (objetoColision.transform.parent.gameObject != this.gameObject)
                    {
                        Obrera hormigaObrera = objetoColision.transform.parent.gameObject.GetComponent<Obrera>();
                        Soldado hormigaSoldado = objetoColision.transform.parent.gameObject.GetComponent<Soldado>();
                        Reina hormigaReina = objetoColision.transform.parent.gameObject.GetComponent<Reina>();
                        if (hormigaObrera != null)
                        {
                            obrerasCerca = true;
                        }
                        if (hormigaSoldado != null)
                        {
                            soldadosCerca = true;
                        }
                        if (hormigaReina != null)
                        {
                            reinaCerca = true;
                        }
                        if (!hormigasCerca.Contains(objetoColision.transform.parent.gameObject.GetComponent<HormigaGenerica>()))
                        {
                            hormigasCerca.Add(objetoColision.transform.parent.gameObject.GetComponent<HormigaGenerica>());
                        }
                        //Debug.Log(hit.collider.gameObject.transform.parent.gameObject.tag);
                    }

                    else
                    {
                        //Debug.Log("Chocandote contigo mismo");
                    }
                }
                //Debug.DrawRay(transform.position, dir * RayDistance, Color.magenta);
            }
        }
    }

    public void ActualizarHambre()
    {
        hambre -= Time.deltaTime;
        if (hambre >= umbralHambre)
        {
            tengoHambre = false;
            tengoMuchaHambre = false;
        }
        if (hambre < umbralHambre)
        {
            tengoHambre = true;
            tengoMuchaHambre = false;
        }
        if (hambre < umbralHambreMaximo)
        {
            tengoHambre = true;
            tengoMuchaHambre = true;
        }
        if (hambre < 0)
        {
            reina.HormigaHaMuerto(this);
            Destroy(this.gameObject);
        }
    }

    public void ActualizarSiPuedeSerCurada()
    {
        if (PuedeCurarse())
        {
            puedeSerCurada = true;
        }
        else
        {
            puedeSerCurada = false;
        }

        if (NecesitaCurarse())
        {
            necesitaSerCurada = true;
            if (!reina.hormigasHeridas.Contains(this))
            {
                // Debo eliminar a la hormiga toda orden que tenga
                reina.DesasignarComidaACoger(this);
                reina.DesasignarHormigaACurar(this);
                reina.DesasignarHuevoACurar(this);

                if (hayOrdenBuscarComida)
                {
                    hayOrdenBuscarComida = false;
                    reina.numHormigasBuscandoComida--;
                }
                else if (hayOrdenCuidarHuevos)
                {
                    hayOrdenCuidarHuevos = false;
                    reina.numHormigasCuidandoHuevos--;
                }
                else if (hayOrdenDeCavar)
                {
                    hayOrdenDeCavar = false;
                    reina.numHormigasCavandoTuneles--;
                }

                SacarDeDesocupadas();

                if (siendoCuradaPor == null)
                {
                    reina.hormigasHeridas.Add(this);
                }
            }
        }
        else
        {
            necesitaSerCurada = false;
        }
    }

    private bool PuedeCurarse()
    {
        return (this.vida < umbralPuedeCurarse);
    }

    private bool NecesitaCurarse()
    {
        return (this.vida < umbralNecesitaCurarse);
    }

    public void QuitarVida(int damage)
    {
        Debug.Log("Hormiga perdiendo vida");
        this.vida -= damage;
        if (vida <= 0)
        {
            reina.HormigaHaMuerto(this);
            Destroy(this.gameObject);
        }
    }

    public void SumarVida()
    {
        Debug.Log("Hormiga ganando vida");
        /*this.vida = cantidadDeCura;
        if (this.vida > 10)
        {
            this.vida = 10;
        }*/
        this.vida = 10;

        if (vida > umbralNecesitaCurarse)
        {
            necesitaSerCurada = false;
            if (!this.hayOrdenBuscarComida || !this.hayOrdenCurarHormiga ||
                !this.hayOrdenDeAtacar || this.hayOrdenCuidarHuevos || this.hayOrdenDeCavar)
            {
                SacarDeOcupadas();
            }
        }
    }

    #region Tareas Globales Hormigas

    [Task]
    public void HayEnemigosCerca()
    {
        if (enemigosCerca.Count > 0)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void ReinaEnPeligro()
    {
        if (reina.enemigosCerca.Count != 0 && reinaCerca == true)
        {
            enemigoAlQueAtacar = reina.enemigosCerca[0];
            Task.current.Succeed();
        }
        else
        {
            enemigoAlQueAtacar = null;
            Task.current.Fail();
        }
    }

    [Task]
    public void Atacar()
    {
        if (bocadillos.hormigaSeleccionada != null && bocadillos.hormigaSeleccionada == this)
        {
            bocadillos.Atacar();
        }
        if (enemigoAlQueAtacar == null)
        {
            if (enemigosCerca.Count > 0)
            {
                enemigoAlQueAtacar = enemigosCerca[0];
                if (enemigoAlQueAtacar.hormigasAtacandole.Contains(this))
                {
                    enemigoAlQueAtacar.hormigasAtacandole.Add(this);
                }
                Task.current.Succeed();
                return;
            }
            else
            {
                // No hay enemigo al que atacar
                Task.current.Fail();
                return;
            }
        }
        else
        {
            // Elimino todo lo que esté haciendo, pero no le quito la orden

            // reina.DesasignarComidaACoger(this);
            // Suelto la comida, pero no la desasigno
            if (comida != null && comida.laEstanLLevando)
            {
                comida.transform.SetParent(null);
                comida.laEstanLLevando = false;
                posComida = Vector3.zero;
                posDejarComida = Vector3.zero;
            }

            // reina.DesasignarHormigaACurar(this);
            // Dejo de curar a la hormgia pero no la desasigno
            if (hormigaACurar != null)
            {
                // No hace falta hacer nada
            }

            // reina.DesasignarHuevoACurar(this);
            // Dejo de cuidar un huevo pero no lo desasigno
            if (huevoACuidar != null)
            {
                // No hace falta hacer nada
            }

            // Asigno la posición a la que hay que ir
            agente.SetDestination(enemigoAlQueAtacar.transform.position);

            // En el momento que esté cerca
            if (Vector3.Distance(transform.position, enemigoAlQueAtacar.transform.position) < 1.2f)
            {
                if (tiempoEntreAtaques <= 0)
                {
                    float random = Random.Range(0, 10);
                    if (random < 9f)
                    {
                        enemigoAlQueAtacar.quitarVida(this.daño);
                    }
                    else
                    {
                        //Debug.Log("Ataque fallido");
                    }
                    tiempoEntreAtaques = tiempoEntreAtaquesMax;
                    Task.current.Succeed();
                    return;
                }
                else
                {
                    Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * 1 + enemigoAlQueAtacar.transform.position;
                    agente.SetDestination(randomDirection);
                    tiempoEntreAtaques -= Time.deltaTime;
                    Task.current.Succeed();
                    return;
                }
            }
            else
            {
                // Te estás acercando al enemigo
                agente.SetDestination(enemigoAlQueAtacar.transform.position);
                Task.current.Succeed();
                return;
            }
        }
    }

    [Task]
    public void Huir()
    {
        if (enemigosCerca.Count > 0)
        {
            EnemigoGenerico enemigoCerca = enemigosCerca[0];
            if (enemigoCerca != null)
            {
                // Elimino todo lo que esté haciendo, pero no le quito la orden

                // reina.DesasignarComidaACoger(this);
                // Suelto la comida, pero no la desasigno
                if (comida != null && comida.laEstanLLevando)
                {
                    comida.transform.SetParent(null);
                    comida.laEstanLLevando = false;
                    posComida = Vector3.zero;
                    posDejarComida = Vector3.zero;
                }

                // reina.DesasignarHormigaACurar(this);
                // Dejo de curar a la hormgia pero no la desasigno
                if (hormigaACurar != null)
                {
                    // No hace falta hacer nada
                }

                // reina.DesasignarHuevoACurar(this);
                // Dejo de cuidar un huevo pero no lo desasigno
                if (huevoACuidar != null)
                {
                    // No hace falta hacer nada
                }

                Vector3 direccionEnemigo = enemigoCerca.transform.position - this.transform.position;
                Vector3 direccionContraria = direccionEnemigo * -1;
                agente.SetDestination(this.transform.position + direccionContraria);
                Debug.Log("Estoy huyendo");
            }
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
    public void EstaHerida()
    {
        if (puedeSerCurada)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void EstaMuyHerida()
    {
        if (necesitaSerCurada)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void EstaSiendoCurada()
    {
        if (siendoCuradaPor != null)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void NoMoverse()
    {
        if (bocadillos.hormigaSeleccionada != null && bocadillos.hormigaSeleccionada == this)
        {
            if (!this.agente.isOnOffMeshLink)
            {
                bocadillos.EstaHerido();
            }
            else
            {
                bocadillos.Nada();
            }
        }
        //siguientePosicionExplorar = this.transform.position;
        agente.SetDestination(this.transform.position);
        Task.current.Succeed();
    }

    [Task]
    public void TengoHambre()
    {
        if (tengoHambre)
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
        if (tengoMuchaHambre)
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
        if (reina.comidaTotal.Count <= 0)
        {
            Task.current.Fail();
        }
        else
        {
            Task.current.Succeed();
        }
    }

    [Task]
    public void ReinaTieneMuchaHambre()
    {
        if (reina.tengoMuchaHambre)
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
        if (bocadillos.hormigaSeleccionada != null && bocadillos.hormigaSeleccionada == this)
        {
            if (!this.agente.isOnOffMeshLink)
            {
                bocadillos.Comer();
            }
            else
            {
                bocadillos.Nada();
            }
        }
        if (comidaAComer == null)
        {
            //Debug.Log("No tengo comida aun ");
            //comidaAComer = reina.pedirComida();
            if (reina.comidaTotal.Count > 0)
            {
                //Debug.Log("Hay comida");
                comidaAComer = reina.comidaTotal[Random.Range(0, reina.comidaTotal.Count)];
                //reina.ComidaTotal.Remove(comidaAComer);
            }
            // Comprobamos si podemos o no acceder a comida
            if (comidaAComer != null)
            {
                //Debug.Log("VOY A LA PUTA COMIDA");
                agente.SetDestination(comidaAComer.transform.position);
                Task.current.Succeed();
                return;
            }
            else
            {
                Task.current.Fail();
                return;
            }
        }
        else
        {
            if (Vector3.Distance(this.transform.position, comidaAComer.transform.position) < 0.2f)
            {
                //Debug.Log("He llegado a la comida");
                hambre += comidaAComer.Comer();
                comidaAComer = null;
                Task.current.Succeed();
                return;
            }
            else
            {
                agente.SetDestination(comidaAComer.transform.position);
                //Debug.Log("Posicion comida a comer: " + comidaAComer.transform.position);
            }
            //Debug.Log("Intentando ira porla comida");
            Task.current.Succeed();
            return;
        }

    }

    [Task]
    public void TengoOrdenDeCurarHormiga()
    {
        if (hayOrdenCurarHormiga)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void HayHormigaQueCurarCerca()
    {
        if (hayOrdenCurarHormiga == false)
        {
            // Si no tengo ninguna hormiga asignada para curar, miro las que hay alrededor
            if (hormigaACurar == null)
            {
                // Recorremos la lista de hormigas cercanas
                foreach (HormigaGenerica h in hormigasCerca)
                {
                    // Si alguna hormiga PUEDE ser curada y no tiene a nadie asignado, se lo asigno e indico a la hormiga quien lo cura
                    if (h.siendoCuradaPor == null && h.puedeSerCurada)
                    {
                        hormigaACurar = h;
                        h.siendoCuradaPor = this;
                        // Si la reina lo tiene en su lista de hormigas heridas, lo borro
                        reina.hormigasHeridas.Remove(hormigaACurar);
                        Task.current.Succeed();
                        return;
                    }
                }
            }
            else
            {
                // Si tenemos una hormiga a curar asignada
                Task.current.Succeed();
                return;
            }
        }
        // Si no encuentra hormiga, o tiene orden
        Task.current.Fail();
    }

    [Task]
    public void CurarHormiga()
    {
        if (bocadillos.hormigaSeleccionada != null && bocadillos.hormigaSeleccionada == this)
        {
            if (!this.agente.isOnOffMeshLink)
            {
                bocadillos.Curar();
            }
            else
            {
                bocadillos.Nada();
            }
        }
        if (hormigaACurar != null)
        {
            // Si es la primera vez, no tengo asignada la posicion de la hormiga a curar
            if (posHerida == Vector3.zero)
            {
                Debug.Log("Se asigna la posicion de la hormiga a curar");
                TiempoActual = tiempoParaCurar;
                posHerida = hormigaACurar.transform.position;
                agente.SetDestination(hormigaACurar.transform.position);
                Task.current.Succeed();
                return;
            }

            if (Vector3.Distance(this.transform.position, hormigaACurar.transform.position) < 3.8f && this.zonaDondeEsta == hormigaACurar.zonaDondeEsta)
            {
                if (Vector3.Distance(this.transform.position, hormigaACurar.transform.position) < 2.5f)
                {
                    agente.SetDestination(this.transform.position);
                }
                TiempoActual -= Time.deltaTime;
                // Si ha pasado el tiempo de cura
                if (TiempoActual <= 0)
                {
                    hormigaACurar.SumarVida();
                    //Debug.Log("Hormiga Curada");
                    // Reseteas todos los valores
                    TiempoActual = tiempoParaCurar;
                    posHerida = Vector3.zero;
                    hormigaACurar.siendoCuradaPor = null;
                    hormigaACurar = null;
                    // Si se trataba de una orden de curar hormiga
                    if (hayOrdenCurarHormiga == true)
                    {
                        hayOrdenCurarHormiga = false;
                        SacarDeOcupadas();
                    }
                    Task.current.Succeed();
                    return;
                }
                Task.current.Succeed();
                return;
            }
            else
            {
                TiempoActual = tiempoParaCurar;
                agente.SetDestination(hormigaACurar.transform.position);
            }
            Task.current.Succeed();
            return;
        }
        // Si la hormiga ha muerto, se devuelve Fail para que siga con el BT
        else
        {
            TiempoActual = tiempoParaCurar;
            hormigaACurar = null;
            posHerida = Vector3.zero;
            if (hayOrdenCurarHormiga == true)
            {
                hayOrdenCurarHormiga = false;
                SacarDeOcupadas();
            }
            Task.current.Fail();
            return;
        }
    }

    [Task]
    public void TengoOrdenDeBuscarComida()
    {
        if (hayOrdenBuscarComida)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void TengoOrdenDeAtacar()
    {
        if (hayOrdenDeAtacar)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void BuscarComida()
    {
        if (bocadillos.hormigaSeleccionada != null && bocadillos.hormigaSeleccionada == this)
        {
            if (!this.agente.isOnOffMeshLink)
            {
                bocadillos.BuscarComida();
            }
            else
            {
                bocadillos.Nada();
            }
        }
        if (comida != null)
        {
            // Si no he cogido la comida aún, me dirijo a ella
            if (!comida.laEstanLLevando)
            {
                if (posComida == Vector3.zero)
                {
                    posComida = comida.transform.position;
                    agente.SetDestination(comida.transform.position);
                    Task.current.Succeed();
                    return;
                }

                // En este caso, voy hasta donde está la comida, cuando llego
                if (Vector3.Distance(this.transform.position, posComida) < 0.2f)
                {
                    // Muevo la comida con la hormiga y le actualizo que están llevando esa comida
                    comida.transform.SetParent(this.transform);
                    comida.laEstanLLevando = true;
                    Task.current.Succeed();
                    return;
                }
                else
                {
                    // Te sigues acercando a la comida
                    agente.SetDestination(posComida);
                    Task.current.Succeed();
                    return;
                }
            }
            // Si ya he cogido la comida, me dirijo a la casilla libre que he tomado
            else
            {
                // Guardo la posición a donde me dirijo para dejar la comida
                if (posDejarComida == Vector3.zero)
                {
                    posDejarComida = casillaDejarComida.transform.position;
                    agente.SetDestination(posDejarComida);
                    Task.current.Succeed();
                    return;
                }
                // Cuando llego a la casilla, suelto la comida y actualizo el haSidoCogida
                if (Vector3.Distance(this.transform.position, posDejarComida) < 0.2f && zonaDondeEsta == 0)
                {
                    reina.ComidaGuardada(comida, salaDejarComida, casillaDejarComida);
                    comida.CogerComida(salaDejarComida, casillaDejarComida);
                    comida.haSidoCogida = true;
                    comida.transform.SetParent(null);
                    comida.hormigaQueLlevaLaComida = null;
                    // Reseteamos los datos de la hormiga
                    if (hayOrdenBuscarComida)
                    {
                        hayOrdenBuscarComida = false;
                        SacarDeOcupadas();
                        reina.numHormigasBuscandoComida--;
                    }
                    comida = null;
                    salaDejarComida = null;
                    casillaDejarComida = null;
                    posComida = Vector3.zero;
                    posDejarComida = Vector3.zero;
                    Task.current.Succeed();
                    return;
                }
                else
                {
                    // Te sigues acercando a la comida
                    agente.SetDestination(posDejarComida);
                    Task.current.Succeed();
                    return;
                }
            }
        }
        else
        {
            // La comida ha muerto o tengo que dejar la comida, reseteo datos
            salaDejarComida = null;
            casillaDejarComida = null;
            posComida = Vector3.zero;
            posDejarComida = Vector3.zero;
            comida = null;
            if (hayOrdenBuscarComida)
            {
                hayOrdenBuscarComida = false;
                SacarDeOcupadas();
                reina.numHormigasBuscandoComida--;
            }
            Task.current.Fail();
        }
    }

    #endregion

    // Al quitar una orden
    public void SacarDeDesocupadas()
    {
        Nurse hormigaNurse = this.transform.gameObject.GetComponent(typeof(Nurse)) as Nurse;
        Obrera hormigaObrera = this.transform.gameObject.GetComponent(typeof(Obrera)) as Obrera;
        Soldado hormigaSoldado = this.transform.gameObject.GetComponent(typeof(Soldado)) as Soldado;

        if (hormigaNurse != null)
        {
            if (reina.nursesDesocupadas.Remove(hormigaNurse))
            {
                reina.nursesOcupadas.Add(hormigaNurse);
            }
        }
        else if (hormigaObrera != null)
        {
            if (reina.obrerasDesocupadas.Remove(hormigaObrera))
            {
                reina.obrerasOcupadas.Add(hormigaObrera);
            }
        }
        else if (hormigaSoldado != null)
        {
            if (reina.soldadosDesocupadas.Remove(hormigaSoldado))
            {
                reina.soldadosOcupadas.Add(hormigaSoldado);
            }
        }
    }

    public void SacarDeOcupadas()
    {
        Nurse hormigaNurse = this.transform.gameObject.GetComponent(typeof(Nurse)) as Nurse;
        Obrera hormigaObrera = this.transform.gameObject.GetComponent(typeof(Obrera)) as Obrera;
        Soldado hormigaSoldado = this.transform.gameObject.GetComponent(typeof(Soldado)) as Soldado;

        if (hormigaNurse != null)
        {
            if (reina.nursesOcupadas.Remove(hormigaNurse))
            {
                reina.nursesDesocupadas.Add(hormigaNurse);
            }
        }
        else if (hormigaObrera != null)
        {
            if (reina.obrerasOcupadas.Remove(hormigaObrera))
            {
                reina.obrerasDesocupadas.Add(hormigaObrera);
            }
        }
        else if (hormigaSoldado != null)
        {
            if (reina.soldadosOcupadas.Remove(hormigaSoldado))
            {
                reina.soldadosDesocupadas.Add(hormigaSoldado);
            }
        }
    }
}
