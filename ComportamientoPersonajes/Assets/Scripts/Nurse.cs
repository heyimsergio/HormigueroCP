using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using UnityEngine.AI;

public class Nurse : HormigaGenerica
{
    #region Variables Nurse
    //Atacar
    // bool hayEnemigosCerca
    public bool hayHuevosCerca = false;
    public int numeroDeObrerasCerca = 0;
    public int numeroDeSoldadosCerca = 0;
    public bool reinaEstaCerca = false;

    //Comer
    // float hambre
    // int reina.totalComida

    // Orden de la reina
        // bool meHanMandadoOrden
    public bool hayOrdenCuidarHuevos = false;
        // bool hayOrdenCurarHormiga
        // bool hayOrdenBuscarComida

    //Cuidar de huevos
    public int tiempoCuidandoHuevos = 2;
    public Huevo huevoACuidar = null;
    public Vector3 posHuevo = Vector3.zero;
    public float TiempoActual;

    //Curar A Una Hormiga
        // HormigaGenerica hormigaACurar
        // int tiempoParaCurar

    // Buscar Comida
        // Vector3 siguientePosicionBuscandoComida
        // Comida comida;
        // Room salaDejarComida = null;
        // posDejarComida = Vector3.zero;

    // Comer
        // Comida comidaAComer

    // Explorar
        // Vector3 siguientePosicionExplorar

    #endregion

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

    // HayEnemigosCerca()

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

    // ReinaEnPeligro()
    // Atacar()
    // Huir()
    // TengoMuchaHambre()
    // TengoHambre()
    // HayComida()
    // Comer()
    // TengoOrdenDeLaReina()

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

    // TengoOrdenDeCurarHormiga()
    // CurarHormiga()
    // TengoOrdenDeBuscarComida()
    // BuscarComida()

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

    // HayHormigaQueCurarCerca()

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
