using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using UnityEngine.AI;

public class Nurse : HormigaGenerica
{
    //Atacar
    public int numeroDeObrerasCerca = 0;
    public int numeroDeSoldadosCerca = 0;
    public bool reinaEstaCerca = false;
    public bool hayHuevosCerca = false;
    public bool hayEnemigosCerca = false;
    public List <EnemigoGenerico> enemigosCerca = new List<EnemigoGenerico>();
    public float tiempoEntreAtaques;
    [HideInInspector]
    public float tiempoEntreAtaquesMax = 0.5f;

    //Cuidar de huevos
    public int tiempoCuidandoHuevos = 2;
    public Huevo huevoACuidar = null;
    public Vector3 posHuevo = Vector3.zero;
    public float TiempoActual;

    //Recoger comida
    public int numeroDeObreras = 0;
    public Comida comida;
    public Room salaDejarComida = null;
    Vector3 posDejarComida = Vector3.zero;

    //Curar otras hormigas
    public HormigaGenerica hormigaACurar;
    public int tiempoParaCurar = 0;

    //Buscar comida
    public Vector3 siguientePosicionBuscandoComida;
    public Vector3 almacenComida;

    //Ordenes de la reina
    public bool meHanMandadoOrden = false;
    public enum ordenes {ORDEN1, ORDEN2};

    // Start is called before the first frame update
    void Start()
    {
        TiempoActual = tiempoCuidandoHuevos;
        hormigueroDentro = GameObject.FindObjectOfType<Floor>();
        hormigueroFuera = GameObject.FindObjectOfType<Outside>();
        reina = GameObject.FindObjectOfType<Reina>();
        pb = this.gameObject.GetComponent<PandaBehaviour>();
        agente = this.gameObject.GetComponent<NavMeshAgent>();
        this.vida = 10;
        this.daño = 2;
        tiempoEntreAtaquesMax = 0.5f;
        this.tiempoEntreAtaques = tiempoEntreAtaquesMax;
        siguientePosicionExplorar = this.transform.position;
    }

    private void Update()
    {
        actualizarHambre();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemigo")
        {
            EnemigoGenerico aux = other.GetComponent<EnemigoGenerico>();
            hayEnemigosCerca = true;
            aux.hormigaCerca = this;
            if (!enemigosCerca.Contains(aux))
            {
                enemigosCerca.Add(aux);
            }
        } else if (other.tag == "Trigo")
        {
            if (comida == null)
            {
                Comida aux = other.GetComponent<Comida>();
                if (!aux.laEstanLLevando && !aux.haSidoCogida)
                {
                    reina.recibirAlertaComida(aux);
                    comida = aux;
                    aux.laEstanLLevando = true;
                }

            } else
            {
                reina.recibirAlertaComida(other.GetComponent<Comida>());
            }
        } else if( other.tag == "Huevo")
        {
            Huevo aux =  other.GetComponent<Huevo>();
            if (aux.puedeSerCuidado)
            {
                huevoACuidar = aux;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemigo")
        {
            EnemigoGenerico aux = other.GetComponent<EnemigoGenerico>();
            if (enemigosCerca.Contains(aux))
            {
                enemigosCerca.Remove(aux);
            }
            if (enemigosCerca.Count == 0)
            {
                hayEnemigosCerca = false;
            }
            siguientePosicionExplorar = this.transform.position;
        } else if (other.tag == "Trigo")
        {
            if (comida == other)
            {
                comida = null;
            }
        }
    }


    [Task]
    public void HayHuevosCerca()
    {
        if (hayHuevosCerca)
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
        if (numeroDeObrerasCerca > 0 || numeroDeSoldadosCerca > 0)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void TengoOrdenDeLaReina()
    {
        EnemigoGenerico enemigo = enemigosCerca[0];
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
        
    }

    [Task]
    public void TengoOrdenDeCuidarHuevos()
    {
        Task.current.Fail();
    }

    [Task]
    public void CuidarHuevos()
    {
        if (posHuevo == Vector3.zero)
        {
            agente.SetDestination(huevoACuidar.transform.position);
            posHuevo = huevoACuidar.transform.position;
        }

        if (Vector3.Distance(this.transform.position, posHuevo) < 0.2)
        {
            if (huevoACuidar.puedeSerCuidado)
            {
                TiempoActual -= Time.deltaTime;
                if (TiempoActual <= 0)
                {
                    huevoACuidar.cuidar();
                    TiempoActual = tiempoCuidandoHuevos;
                    huevoACuidar = null;
                    posHuevo = Vector3.zero;
                    Task.current.Succeed();
                }
            }
            else
            {
                TiempoActual = tiempoCuidandoHuevos;
                huevoACuidar = null;
                posHuevo = Vector3.zero;
                Task.current.Fail();
            }
        }
    }

    [Task]
    public void TengoOrdenDeCurarHormiga()
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

    // Ahora voy a cambiarlos para que se lo pregunten a la reina, pero deberian saberl solas.
    [Task]
    public void HayHuevosCercaQueNecesitanCuidados()
    {
        Debug.Log("Pregunto por cuidar Huevos");
        Debug.Log("La cuenta de huevos sin cuidar es: " + reina.huevosQueTienenQueSerCuidados.Count);
        if (huevoACuidar != null)
        {
            Task.current.Succeed();
        }
        else if (reina.huevosQueTienenQueSerCuidados.Count > 0)
        {
            huevoACuidar = reina.huevosQueTienenQueSerCuidados[Random.Range(0, reina.huevosQueTienenQueSerCuidados.Count)];
            Task.current.Succeed();
            return;
        }

        Task.current.Fail();
    }

    [Task]
    public void HayHormigaQueCurarCerca()
    {
        Task.current.Fail();
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
            } while (aux.position.x < (hormigueroDentro.transform.position.x - (hormigueroDentro.width / 2)) || !aux2);
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
                Debug.Log("Estoy dentro, asi que exploro " + siguientePosicionExplorar + " " + transform.position);
                float distanceToTarget = Vector3.Distance(this.transform.position, siguientePosicionExplorar);
                Debug.Log(distanceToTarget);
                if (distanceToTarget < 1.8f)
                {
                    Debug.Log("Esta cerca");
                    Vector3 randomDirection;
                    NavMeshHit aux;
                    bool aux2;
                    do
                    {
                        randomDirection = UnityEngine.Random.insideUnitSphere * 10 + this.transform.position;
                        aux2 = NavMesh.SamplePosition(randomDirection, out aux, 1.0f, NavMesh.AllAreas);
                    } while (!aux2 || (aux.position.x < (hormigueroDentro.transform.position.x - (hormigueroDentro.width / 2))));
                    //saliendo = true;
                    agente.SetDestination(aux.position);
                    siguientePosicionExplorar = aux.position;
                }
                else
                {
                    Debug.Log("Esta lejos, voy hacia el");
                    agente.SetDestination(siguientePosicionExplorar);

                }
            }
        }
        Task.current.Succeed();
    }


    


}
