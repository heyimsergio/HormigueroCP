using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using UnityEngine.AI;

public class HormigaGenerica : PersonajeGenerico
{
    //Agente Navmesh
    public NavMeshAgent agente;
    public PandaBehaviour pb;
    public Floor hormigueroDentro; //Saber donde empieza el suelo para no meterte dentro del hormiguero cuando exploras
    public Outside hormigueroFuera;
    //bool estaDentro = true; //True: está dentro, false: esta fuera
    //bool saliendo = false;

    // ATRIBUTOS ////////////////////////////////////////////////////////////////////////////////////////////////

    // Atributos de las hormigas generales
    protected float hambre = 200;
    public float pesoQuePuedenTransportar;
    public bool estaLuchando = false;
    public float tiempoEntreAtaques;
    [HideInInspector]
    public float tiempoEntreAtaquesMax = 0.5f;

    // Atacar
    public bool hayEnemigosCerca = false;
    public List<EnemigoGenerico> enemigosCerca = new List<EnemigoGenerico>();
    //Orden atacar
    public bool hayOrdenDeAtacar = false;
    public EnemigoGenerico enemigoAlQueAtacar = null;

    //Buscar Comida
    public Vector3 siguientePosicionBuscandoComida;
    public Comida comida;
    public Room salaDejarComida = null;
    Vector3 posDejarComida = Vector3.zero;
    //Orden Buscar Comida
    public bool hayOrdenBuscarComida = false;

    //Curar otras hormigas
    public HormigaGenerica hormigaACurar;
    public int tiempoParaCurar = 0;
    //Orden Curar Hormiga
    public bool hayOrdenCurarHormiga = false;

    //Explorar
    public Vector3 siguientePosicionExplorar;

    // Comer
    public Comida comidaAComer;

    // Reina
    public Reina reina;
    protected Vector3 posicionReina;
    //Ordenes de la reina
    public bool meHanMandadoOrden = false;
    public enum ordenes { ORDEN1, ORDEN2 };


    // CODIGO ///////////////////////////////////////////////////////////////////////////////////////////////

    // Start is called before the first frame update
    void Start()
    {
        this.zonaDondeEsta = 0;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void actualizarHambre()
    {
        hambre -= Time.deltaTime;
    }

    public bool quitarVida(int damage)
    {
        Debug.Log("Hormiga perdiendo vida");
        this.vida -= damage;
        if (vida <= 0)
        {
            Destroy(this.gameObject);
            return true;
        }
        return false;
    }

    // Tareas Panda
    [Task]
    public void HayEnemigosCerca()
    {
        if (hayEnemigosCerca)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void Atacar()
    {
        EnemigoGenerico enemigo = enemigosCerca[0];
        if (enemigo != null)
        {
            //Debug.Log("Hay enemigo");
            agente.SetDestination(enemigo.transform.position);
            float distanceToTarget = Vector3.Distance(transform.position, enemigo.transform.position);
            //Debug.Log(distanceToTarget);
            if (distanceToTarget < 1.2f)
            {
                //Debug.Log("Quitar vida");
                float random = Random.Range(0, 10);
                if (random < 9f)
                {
                    enemigo.quitarVida(this.daño);
                }
                else
                {
                    Debug.Log("Ataque fallido");
                }
            }
            else
            {
                //Debug.Log("no estoy a rango para pegarle");
            }
        }
        else
        {
            //Debug.Log("No hay enemigo");
            enemigosCerca.RemoveAt(0);
            if (enemigosCerca.Count == 0)
            {
                hayEnemigosCerca = false;
                siguientePosicionExplorar = this.transform.position;
            }
        }
        Task.current.Succeed();

    }

    [Task]
    public void TengoHambre()
    {
        //Debug.Log("Hola");
        //Debug.Log(this.hambre + " : hambre");
        if (this.hambre <= 75)
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
        if (this.hambre <= 30)
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
        if (reina.totalComida <= 0)
        {
            Task.current.Fail();
        }
        else
        {
            Task.current.Succeed();
        }
    }

    [Task]
    public void ReinaEnPeligro()
    {
        if (reina.hayEnemigosCerca == true)
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
        Debug.Log("comer");
        if (comidaAComer == null)
        {
            Debug.Log("Tengo comida");
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
                if (comidaAComer.usosDeLaComida == 0)
                {
                    reina.sacarComidaSala(comidaAComer.misala, comidaAComer);
                    Destroy(comidaAComer.gameObject);
                    comidaAComer = null;
                    Debug.Log("Comida destruida");
                    Task.current.Succeed();
                }

            }
        }

    }

    [Task]
    public void Huir()
    {
        Task.current.Succeed();
        Debug.Log("Huir");
    }

    [Task]
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
        */
        Task.current.Fail();

    }

    [Task]
    public void TengoOrdenDeCurarHormiga()
    {
        Task.current.Fail();
    }

    [Task]
    public void HayHormigaQueCurarCerca()
    {
        Task.current.Fail();
    }

    [Task]
    public void CurarHormiga()
    {
        Task.current.Fail();
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
        if (hayEnemigosCerca)
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
                        posDejarComida = salaDejarComida.getRandomPosition();
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
                        reina.comidaGuardada(comida, salaDejarComida);
                        comida.haSidoCogida = true;
                        comida.laEstanLLevando = false;
                        comida.transform.SetParent(null);
                        comida = null;
                        salaDejarComida = null;
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
}
