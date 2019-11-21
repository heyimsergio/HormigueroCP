using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using UnityEngine.AI;

public class HormigaGenerica : PersonajeGenerico
{
    //Agente Navmesh
    protected NavMeshAgent agente;
    protected PandaBehaviour pb;
    protected Floor hormigueroDentro; //Saber donde empieza el suelo para no meterte dentro del hormiguero cuando exploras
    protected Outside hormigueroFuera;
    //bool estaDentro = true; //True: está dentro, false: esta fuera
    //bool saliendo = false;

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

    //Explorar
    protected Vector3 siguientePosicionExplorar;

    // Comer
    [Header("Comida a Comer")]
    public Comida comidaAComer;

    // Reina
    [Header("REINA")]
    public Reina reina;
    protected Vector3 posicionReina;
    //Ordenes de la reina
    public bool meHanMandadoOrden = false;
    public enum ordenes { ORDEN1, ORDEN2 };

    // sistema de vision
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
        actualizarHambre();

        actualizarSiPuedeSerCurada();

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
                        //Debug.Log(hit.collider.gameObject.transform.parent.gameObject.tag);
                        if (objetoColision.transform.parent.gameObject.GetType().Equals("Obrera"))
                        {
                            obrerasCerca = true;
                        }
                        if (objetoColision.transform.parent.gameObject.GetType().Equals("Soldado"))
                        {
                            soldadosCerca = true;
                        }
                        if (objetoColision.transform.parent.gameObject.GetType().Equals("Reina"))
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
                //Debug.DrawRay(transform.position, dir* RayDistance, Color.magenta);
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

    public void actualizarHambre()
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

    public void actualizarSiPuedeSerCurada()
    {
        if (puedeCurarse())
        {
            puedeSerCurada = true;
        }
        else
        {
            puedeSerCurada = false;
        }

        if (necesitaCurarse())
        {
            necesitaSerCurada = true;
            if (!reina.hormigasHeridas.Contains(this))
            {
                if (this.GetType().Equals("Nurse"))
                {
                    Nurse a = (Nurse)this;
                    reina.nursesDesocupadas.Remove(a);
                    if (!reina.nursesOcupadas.Contains(a))
                    {
                        reina.nursesOcupadas.Add(a);
                    }

                }
                else if (this.GetType().Equals("Obrera"))
                {
                    Debug.Log("Una obrera debe ser curada");
                    Obrera a = (Obrera)this;
                    reina.obrerasDesocupadas.Remove(a);
                    if (!reina.obrerasOcupadas.Contains(a))
                    {
                        reina.obrerasOcupadas.Add(a);
                    }
                }
                else if (this.GetType().Equals("Soldado"))
                {
                    /*Soldado a = (Soldado)this;
                    reina.soldadosDesocupadas.Remove(a);
                    if (!reina.soldadosOcupadas.Contains(a))
                    {
                        reina.soldadosOcupadas.Add(a);
                    }*/
                }
                else
                {
                    // Es la reina
                }

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

    private bool puedeCurarse()
    {
        return (this.vida < umbralPuedeCurarse);
    }

    private bool necesitaCurarse()
    {
        return (this.vida < umbralNecesitaCurarse);
    }

    public void quitarVida(int damage)
    {
        Debug.Log("Hormiga perdiendo vida");
        this.vida -= damage;
        if (vida <= 0)
        {
            reina.HormigaHaMuerto(this);
            Destroy(this.gameObject);
        }
    }

    public void sumarVida()
    {
        Debug.Log("Hormiga ganando vida");
        this.vida += cantidadDeCura;
        if (this.vida > 10)
        {
            this.vida = 10;
        }

        if (vida > umbralNecesitaCurarse)
        {
            necesitaSerCurada = false;
            if (!this.hayOrdenBuscarComida || !this.hayOrdenCurarHormiga ||
                !this.hayOrdenDeAtacar || this.hayOrdenCuidarHuevos || this.hayOrdenDeCavar)
            {
                if (this.GetType().Equals("Nurse"))
                {
                    Nurse a = (Nurse)this;
                    reina.nursesOcupadas.Remove(a);
                    if (!reina.nursesDesocupadas.Contains(a))
                    {
                        reina.nursesDesocupadas.Add(a);
                    }

                }
                else if (this.GetType().Equals("Obrera"))
                {
                    Obrera a = (Obrera)this;
                    reina.obrerasOcupadas.Remove(a);
                    if (!reina.obrerasDesocupadas.Contains(a))
                    {
                        reina.obrerasDesocupadas.Add(a);
                    }
                }
                else if (this.GetType().Equals("Soldado"))
                {
                    /*Soldado a = (Soldado)this;
                    reina.soldadosOcupadas.Remove(a);
                    if (!reina.soldadosDesocupadas.Contains(a))
                    {
                        reina.soldadosDesocupadas.Add(a);
                    }*/
                }
                else
                {
                    // Es la reina
                }
            }
        }

    }



    #region Tareas Globales Hormigas

    [Task]
    public void HayEnemigosCerca()
    {
        if (enemigosCerca.Count != 0)
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
        if (enemigosCerca.Count > 0)
        {
            if (enemigoAlQueAtacar == null)
            {
                enemigoAlQueAtacar = enemigosCerca[0];
            }

            if (enemigoAlQueAtacar != null)
            {
                agente.SetDestination(enemigoAlQueAtacar.transform.position);
                float distanceToTarget = Vector3.Distance(transform.position, enemigoAlQueAtacar.transform.position);
                if (comida != null)
                {
                    comida.transform.SetParent(null);
                    comida = null;
                    posDejarComida = Vector3.zero;
                    salaDejarComida = null;
                    casillaDejarComida = null;
                    posComida = Vector3.zero;
                }
                if (distanceToTarget < 1.2f)
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
                    }
                    else
                    {
                        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * 1 + enemigoAlQueAtacar.transform.position;
                        agente.SetDestination(randomDirection);
                        tiempoEntreAtaques -= Time.deltaTime;
                    }

                }
            }
            else
            {
                enemigosCerca.RemoveAt(0);
            }
            // Aún pueden quedarte enemigos a los que atacar
            if (enemigosCerca.Count == 0)
            {
                //siguientePosicionExplorar = this.transform.position;
                Task.current.Succeed();
            }
        }
        else
        {
            Task.current.Fail();
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
                Vector3 direccionEnemigo = enemigoCerca.transform.position - this.transform.position;
                Vector3 direccionContraria = direccionEnemigo * -1;
                agente.SetDestination(this.transform.position + direccionContraria);
            }
            else
            {
                Task.current.Succeed();
            }
            Debug.Log("Huir");
        }
        else
        {
            Task.current.Fail();
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
        if (reina.ComidaTotal.Count <= 0)
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
        if (comidaAComer == null)
        {
            //Debug.Log("No tengo comida aun ");
            //comidaAComer = reina.pedirComida();
            if (reina.ComidaTotal.Count > 0)
            {
                //Debug.Log("Hay comida");
                comidaAComer = reina.ComidaTotal[Random.Range(0,reina.ComidaTotal.Count)];
                //reina.ComidaTotal.Remove(comidaAComer);
            }
            // Comprobamos si podemos o no acceder a comida
            if (comidaAComer != null)
            {
                Debug.Log("VOY A LA PUTA COMIDA");
                agente.SetDestination(comidaAComer.transform.position);
                Task.current.Succeed();
                return;
            }
            else
            {
                Debug.Log("Axwelfeo");
                Task.current.Fail();
                return;
            }
        }
        else
        {
            if (Vector3.Distance(this.transform.position, comidaAComer.transform.position) < 0.2f)
            {
                Debug.Log("He llegado a la comida");
                hambre += comidaAComer.comer();
                comidaAComer = null;
                Task.current.Succeed();
                return;
            } else
            {
                agente.SetDestination(comidaAComer.transform.position);
                Debug.Log("Posicion comida a comer: " + comidaAComer.transform.position);
            }
            Debug.Log("Intentando ira porla comida");
            Task.current.Succeed();
            return;
        }

    }

    [Task]
    public void TengoOrdenDeCurarHormiga()
    {
        if (hayOrdenCurarHormiga)
        {
            // Todo lo que esté haciendo si tengo una orden más prioritaria, lo corto
            // Si estabas cuidando un huevo
            if (huevoACuidar != null)
            {
                huevoACuidar.siendoCuidadoPor = null;
                if (huevoACuidar.necesitaCuidados && !reina.huevosQueTienenQueSerCuidados.Contains(huevoACuidar))
                {
                    reina.huevosQueTienenQueSerCuidados.Add(huevoACuidar);
                }
            }
            huevoACuidar = null;
            posHuevo = Vector3.zero;
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
                        Debug.Log("Hay Hormiga Cerca que puede ser o necesita curarse");
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

            if (Vector3.Distance(this.transform.position, posHerida) < 1.5f)
            {
                TiempoActual -= Time.deltaTime;
                // Si ha pasado el tiempo de cura
                if (TiempoActual <= 0)
                {
                    hormigaACurar.sumarVida();
                    Debug.Log("Hormiga Curada");
                    // Reseteas todos los valores
                    TiempoActual = tiempoParaCurar;
                    posHerida = Vector3.zero;
                    hormigaACurar.siendoCuradaPor = null;
                    hormigaACurar = null;
                    // Si se trataba de una orden de curar hormiga
                    if (hayOrdenCurarHormiga == true)
                    {
                        hayOrdenCurarHormiga = false;
                    }
                    Task.current.Succeed();
                    return;
                }

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
            }
            Task.current.Fail();
            return;
        }
    }

    [Task]
    public void TengoOrdenDeBuscarComida()
    {
        Task.current.Fail();
    }

    [Task]
    public void TengoOrdenDeAtacar()
    {
        Task.current.Fail();
    }

    [Task]
    public void BuscarComida()
    {
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
                if (Vector3.Distance(this.transform.position, posDejarComida) < 0.2f)
                {
                    reina.comidaGuardada(comida, salaDejarComida, casillaDejarComida);
                    comida.haSidoCogida = true;
                    comida.transform.SetParent(null);
                    comida.hormigaQueLlevaLaComida = null;
                    // Reseteamos los datos de la hormiga
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
                    Task.current.Succeed();
                    return;
                }
            }
        }
        else
        {
            // La comida ha muerto, reseteo datos
            salaDejarComida = null;
            casillaDejarComida = null;
            posComida = Vector3.zero;
            posDejarComida = Vector3.zero;
            comida = null;
            Task.current.Fail();
        }
    }

    #endregion
}
