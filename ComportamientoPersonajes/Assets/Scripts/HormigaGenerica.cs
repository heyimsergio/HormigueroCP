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
    Vector3 posDejarComida = Vector3.zero;
    //Orden Buscar Comida
    public bool hayOrdenBuscarComida = false;

    //Curar otras hormigas
    [Header("Variables Curar Hormigas")]
    public HormigaGenerica hormigaACurar;
    public int tiempoParaCurar = 0;
    public HormigaGenerica siendoCuradaPor = null;
    public Vector3 posHerida = Vector3.zero;
    public List<HormigaGenerica> hormigasCerca = new List<HormigaGenerica>();
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


    public void SistemaDeVision()
    {
        hormigasCerca = new List<HormigaGenerica>();
        obrerasCerca = false;
        soldadosCerca = false;
        reinaCerca = false;
        RaycastHit hit;

        Vector3 dir  = Vector3.zero;

        // Rayos fijos
        for (int i = 0; i < numRayosFijos; i++)
        {
            switch (i)
            {
                case 0:// delante
                    dir =  transform.TransformDirection(Vector3.forward);
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
                        hormigasCerca.Add(objetoColision.transform.parent.gameObject.GetComponent<HormigaGenerica>());
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
                    }
                    else
                    {
                        //Debug.Log("Chocandote contigo mismo");
                    }
                }
            }

        }

        // Rayos Móviles
        for(int j = 0; j < numRayosExtra; j++)
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
                        hormigasCerca.Add(objetoColision.transform.parent.gameObject.GetComponent<HormigaGenerica>());
                        //Debug.Log(hit.collider.gameObject.transform.parent.gameObject.tag);
                    }

                    else
                    {
                        //Debug.Log("Chocandote contigo mismo");
                    }
                }
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
                siguientePosicionExplorar = this.transform.position;
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
            comidaAComer = reina.pedirComida();
            if (comidaAComer != null)
            {
                agente.SetDestination(comidaAComer.transform.position);
            }
            else
            {
                Task.current.Fail();
            }
        }
        else
        {
            if (Vector3.Distance(this.transform.position, comidaAComer.transform.position) < 0.2f)
            {
                Debug.Log("He llegado a la comida");
                hambre += comidaAComer.comer();
                Task.current.Succeed();
            }
        }

    }

    /*[Task]
    public void TengoOrdenDeLaReina()
    {
        /*EnemigoGenerico enemigo = enemigosCerca[0];
        if (enemigo != null)
        {
            if (!estaLuchando)
            {
                reina.HormigaAtacando();
            }
            estaLuchando = true;
            
            //Debug.Log("Hay enemigo");
            agente.SetDestination(enemigo.transform.position);
            float distanceToTarget = Vector3.Distance(transform.position, enemigo.transform.position);
            //Debug.Log(distanceToTarget);
            if (distanceToTarget < 1.2f)
            {
                Debug.Log("Tiempo entre ataques: " + tiempoEntreAtaques);
                if (tiempoEntreAtaques <= 0)
                {
                    Debug.Log("Ataque");
                    float random = Random.Range(0, 10);
                    if (random < 9f)
                    {
                        Debug.Log("Ataque acertado");
                        enemigo.quitarVida(this.daño);
                    }
                    else
                    {
                        Debug.Log("Ataque fallido");
                    }
                    tiempoEntreAtaques = tiempoEntreAtaquesMax;
                }
                else 
                {
                    Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * 1 + enemigo.transform.position;
                    agente.SetDestination(randomDirection);
                    tiempoEntreAtaques -= Time.deltaTime;
                }
            } else
            {
                //Debug.Log("no estoy a rango para pegarle");
            }
        } else
        {
            if (estaLuchando)
            {
                reina.HomirgaDejaDeAtacar();
            }
            estaLuchando = false;
            //Debug.Log("No hay enemigo");
            enemigosCerca.RemoveAt(0);
            if (enemigosCerca.Count == 0)
            {
                hayEnemigosCerca = false;
                siguientePosicionExplorar = this.transform.position;
            }
        }
        Task.current.Succeed();
        
        Task.current.Fail();

    }
*/

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
            bool encontrado = false;
            for (int i = 0; i < hormigasCerca.Count; i++)
            {
                if (hormigasCerca[i] == null)
                {
                    hormigasCerca.RemoveAt(i);
                    i--;
                }
                else if (hormigasCerca[i].puedeCurarse() && hormigasCerca[i].siendoCuradaPor == null
                    && encontrado == false && !hormigasCerca[i].estaLuchando)
                {
                    encontrado = true;
                    hormigaACurar = hormigasCerca[i];
                    hormigasCerca[i].siendoCuradaPor = this;

                    if (reina.hormigasHeridas.Contains(hormigaACurar))
                    {
                        reina.hormigasHeridas.Remove(hormigaACurar);
                    }
                    //break;
                }
            }

            if (hormigaACurar != null)
            {
                Debug.Log("Hay Hormiga Cerca que necesita curarse");
                Task.current.Succeed();
            }
            else
            {
                Task.current.Fail();
            }
        }

        /*foreach (HormigaGenerica h in hormigasCerca)
        {
            if (h.vida < 75)
            {
                hormigaACurar = h;
                Task.current.Succeed();
            }
            else
            {
                Task.current.Fail();
            }
        }*/
    }

    [Task]
    public void CurarHormiga()
    {
        if (hormigaACurar != null)
        {
            if (posHerida == Vector3.zero)
            {
                Debug.Log("Se asigna la posicion de la hormiga a curar");
                posHerida = hormigaACurar.transform.position;
                agente.SetDestination(hormigaACurar.transform.position);
            }

            if (Vector3.Distance(this.transform.position, posHerida) < 0.2)
            {
                // Si la hormiga no ha muerto y no entra a luchar
                if (hormigaACurar.puedeCurarse() && !hormigaACurar.estaLuchando)
                {
                    Debug.Log("La hormiga puede ser curada");
                    TiempoActual -= Time.deltaTime;
                    if (TiempoActual <= 0)
                    {
                        hormigaACurar.sumarVida();
                        Debug.Log("Hormiga Curada");
                        TiempoActual = 20.0f;
                        if (hayOrdenCurarHormiga == true)
                        {
                            hayOrdenCurarHormiga = false;
                        }
                        hormigaACurar.siendoCuradaPor = null;
                        hormigaACurar = null;
                        posHerida = Vector3.zero;
                        Task.current.Succeed();
                        return;
                    }
                }
                else
                {
                    TiempoActual = 20.0f;
                    hormigaACurar = null;
                    posHerida = Vector3.zero;
                    Task.current.Fail();
                    return;
                }
            }
        }
        // Si la hormiga ha muerto
        else
        {
            TiempoActual = 20.0f;
            hormigaACurar = null;
            posHerida = Vector3.zero;
            Task.current.Fail();
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
        if (enemigosCerca.Count != 0)
        {
            if (comida != null)
            {
                comida.laEstanLLevando = false;
                comida.transform.SetParent(null);
                comida = null;
            }
            salaDejarComida = null;
            Task.current.Fail();
            return;

        }

        if(this.GetType().Equals("Obrera"))
        {
            Obrera aux = (Obrera)this;
            if (aux.hayOrdenDeCavar)
            {
                Task.current.Fail();
                return;
            }
        }

        //Debug.Log("buscar comida");
        if (this.zonaDondeEsta == 1)
        {
            //Debug.Log("Estamos fuera");
            if (comida != null)
            {
                //Debug.Log("hemos encontrado comida");
                float distComida = Vector3.Distance(transform.position, comida.transform.position);
                if (distComida < 0.2f)
                {
                    //Debug.Log("Estamos donde la comida");
                    if (salaDejarComida == null)
                    {
                        salaDejarComida = reina.getSalaLibreComida();
                        casillaDejarComida = salaDejarComida.getFreeTile();
                        posDejarComida = casillaDejarComida.gameObject.transform.position;
                        if (salaDejarComida == null)
                        {
                            comida.laEstanLLevando = false;
                            comida.transform.SetParent(null);
                            comida = null;
                            Task.current.Fail();
                        }
                    }
                    else

                    {
                        //Debug.Log("Hay sala disponible, asi que la llevamos");
                        //estaDentro = true;
                        comida.transform.SetParent(this.transform);
                        comida.transform.position = new Vector3(comida.transform.position.x, comida.transform.position.y, comida.transform.position.z);
                        agente.SetDestination(posDejarComida);
                        //Debug.Log("Llegado");
                    }

                }
                else
                {
                    //Debug.Log("Yendo a por comida");
                    agente.SetDestination(comida.transform.position);
                }
            }
            else
            {
                //Debug.Log("No hemos encontrado comida");
                //Debug.Log("Estoy fuera");
                float distanceToTarget = Vector3.Distance(transform.position, siguientePosicionExplorar);
                //Debug.Log(distanceToTarget);
                if (distanceToTarget < 0.2f)
                {
                    Vector3 randomDirection;
                    NavMeshHit aux;
                    do
                    {
                        randomDirection = UnityEngine.Random.insideUnitSphere * 50 + this.transform.position;
                    } while (!NavMesh.SamplePosition(randomDirection, out aux, 1.0f, NavMesh.AllAreas));
                    //saliendo = true;
                    agente.SetDestination(aux.position);
                    siguientePosicionExplorar = aux.position;
                }
                else
                {
                    agente.SetDestination(siguientePosicionExplorar);
                }
            }
            //
        }
        else
        {
            //Debug.Log("Estoy dentro");
            if (this.zonaDondeEsta != 1 && comida == null)
            {
                //Debug.Log("No estoy fuera");
                //Debug.Log("No tengo comida");
                //ESTO ES LO QUE PRESUMIBEMENTE ESTA MAL Y HABRA QUE CORREGIR
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
            else if (this.zonaDondeEsta == 0 && comida != null)
            {

                if (salaDejarComida != null)
                {
                    if (Vector3.Distance(this.transform.position, posDejarComida) < 0.2f)
                    {
                        //Debug.Log("Comida dejada");
                        reina.comidaGuardada(comida, salaDejarComida, casillaDejarComida);
                        comida.haSidoCogida = true;
                        comida.laEstanLLevando = false;
                        comida.transform.SetParent(null);
                        comida = null;
                        salaDejarComida = null;
                        casillaDejarComida = null;
                        Task.current.Succeed();
                    }
                    else
                    {
                        //Debug.Log("Distancia es mayor");
                    }
                }
                else
                {
                    //Debug.Log("he llegado pero nohay sala");
                }
            }
        }

    }

    #endregion
}
