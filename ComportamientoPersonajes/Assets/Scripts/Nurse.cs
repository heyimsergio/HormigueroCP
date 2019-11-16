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
    public List<Huevo> huevosCerca = new List<Huevo>();
    public int numeroDeObrerasCerca = 0;
    public int numeroDeSoldadosCerca = 0;
    public bool reinaCerca = false;

    //Comer
    // float hambre
    // int reina.totalComida

    // Orden de la reina
    // bool meHanMandadoOrden
    public bool hayOrdenCuidarHuevos = false;
    // bool hayOrdenCurarHormiga
    // bool hayOrdenBuscarComida

    //Cuidar de huevos
    public float tiempoCuidandoHuevos = 20.0f;
    public Huevo huevoACuidar = null;
    public Vector3 posHuevo = Vector3.zero;
    // float TiempoActual;

    //Curar A Una Hormiga
    // HormigaGenerica hormigaACurar
    // int tiempoParaCurar
    public List<HormigaGenerica> hormigasCerca = new List<HormigaGenerica>();

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
        tiempoCuidandoHuevos = 20.0f;
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
        // Si encuentras un enemigo y no está en la lista de enemigos
        if (other.tag == "Enemigo")
        {
            numeroDeObrerasCerca = GameObject.FindGameObjectsWithTag("Obrera").Length;
            numeroDeSoldadosCerca = GameObject.FindGameObjectsWithTag("Soldado").Length;

            EnemigoGenerico aux = other.GetComponent<EnemigoGenerico>();
            if (!enemigosCerca.Contains(aux))
            {
                reina.recibirAlertaEnemigo(aux);
                enemigosCerca.Add(aux);
            }
        }
        else if (other.tag == "Trigo")
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

            }
            else
            {
                reina.recibirAlertaComida(other.GetComponent<Comida>());
            }
        }
        else if (other.tag == "Huevo")
        {
            Huevo aux = other.GetComponent<Huevo>();
            if (!huevosCerca.Contains(aux))
            {
                huevosCerca.Add(aux);
                Debug.Log("Añado huevo a la lista");
            }
        }
        else if (other.tag == "Reina")
        {
            reinaCerca = true;
            Debug.Log("Reina cerca");
        }
        else if (other.tag == "Nurse" || other.tag == "Obrera" || other.tag == "Soldado")
        {
            HormigaGenerica aux = other.GetComponent<HormigaGenerica>();
            if (!hormigasCerca.Contains(aux))
            {
                hormigasCerca.Add(aux);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Si un enemigo sale de nuestro collider, y estabamos luchando con el actualizar la lista
        if (other.tag == "Enemigo")
        {
            EnemigoGenerico aux = other.GetComponent<EnemigoGenerico>();
            if (enemigosCerca.Contains(aux))
            {
                enemigosCerca.Remove(aux);
            }
            // Si no hay más enemigos
            if (enemigosCerca.Count == 0)
            {
                siguientePosicionExplorar = this.transform.position;
            }
        }
        else if (other.tag == "Trigo")
        {
            if (comida == other)
            {
                comida = null;
            }
        }
        else if (other.tag == "Huevo")
        {
            Huevo aux = other.GetComponent<Huevo>();
            if (huevosCerca.Contains(aux))
            {
                huevosCerca.Remove(aux);
                Debug.Log("Elimino huevo de la lista");
            }
        }
        else if (other.tag == "Reina")
        {
            reinaCerca = false;
            Debug.Log("Reina cerca");
        }
        else if (other.tag == "Nurse" || other.tag == "Obrera" || other.tag == "Soldado")
        {
            HormigaGenerica aux = other.GetComponent<HormigaGenerica>();
            if (hormigasCerca.Contains(aux))
            {
                hormigasCerca.Remove(aux);
            }
        }
    }

    /*private void OnTriggerStay(Collider other)
    {
        numeroDeObrerasCerca = GameObject.FindGameObjectsWithTag("Obrera").Length;
        numeroDeSoldadosCerca = GameObject.FindGameObjectsWithTag("Soldado").Length;
    }*/

    #region Tareas Nurse

    // HayEnemigosCerca()

    [Task]
    public void HayHuevosCerca()
    {
        if (huevosCerca.Count != 0)
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

    [Task]
    public void TengoOrdenDeLaReina()
    {
        if (hayOrdenCuidarHuevos || hayOrdenCurarHormiga || hayOrdenBuscarComida)
        {
            Task.current.Succeed();
        }
        else
        {
            if (reina.nursesOcupadas.Contains(this))
            {
                reina.nursesOcupadas.Remove(this);
                reina.nursesDesocupadas.Add(this);
            }
            Task.current.Fail();
        }
    }

    [Task]
    public void TengoOrdenDeCuidarHuevos()
    {
        if (hayOrdenCuidarHuevos)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void CuidarHuevos()
    {
        if (huevoACuidar != null)
        {
            if (posHuevo == Vector3.zero)
            {
                Debug.Log("Se asigna la posicion del huevo a curar");
                posHuevo = huevoACuidar.transform.position;
                agente.SetDestination(huevoACuidar.transform.position);
            }
            if (Vector3.Distance(this.transform.position, posHuevo) < 0.2)
            {
                // Si el huevo no ha muerto
                if (huevoACuidar.puedeSerCuidado)
                {
                    Debug.Log("El huevo puede ser cuidado");
                    TiempoActual -= Time.deltaTime;
                    if (TiempoActual <= 0)
                    {
                        huevoACuidar.cuidar();
                        Debug.Log("Huevo Cuidado");
                        TiempoActual = tiempoCuidandoHuevos;
                        posHuevo = Vector3.zero;
                        if (hayOrdenCuidarHuevos == true)
                        {
                            hayOrdenCuidarHuevos = false;
                        }
                        huevoACuidar.siendoCuidadoPor = null;
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
        // Si el huevo ha muerto o nacido
        else
        {
            TiempoActual = tiempoCuidandoHuevos;
            huevoACuidar = null;
            posHuevo = Vector3.zero;
            Task.current.Fail();
        }
    }

    // TengoOrdenDeCurarHormiga()
    // CurarHormiga()
    // TengoOrdenDeBuscarComida()
    // BuscarComida()

    [Task]
    public void HayHuevosCercaQueNecesitanCuidados()
    {
        if (hayOrdenCuidarHuevos == false)
        {
            bool encontrado = false;
            for (int i = 0; i < huevosCerca.Count; i++)
            {
                if (huevosCerca[i] == null)
                {
                    huevosCerca.RemoveAt(i);
                    i--;
                }
                else if (huevosCerca[i].necesitaCuidados && huevosCerca[i].siendoCuidadoPor == null && encontrado == false)
                {
                    encontrado = true;
                    huevoACuidar = huevosCerca[i];
                    huevosCerca[i].siendoCuidadoPor = this;

                    if (reina.huevosQueTienenQueSerCuidados.Contains(huevoACuidar))
                    {
                        reina.huevosQueTienenQueSerCuidados.Remove(huevoACuidar);
                    }
                    //break;
                }
            }

            if (huevoACuidar != null)
            {
                Task.current.Succeed();
                Debug.Log("Hay Huevo Cerca que necesita cuidado");
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

    // HayHormigaQueCurarCerca() --> habrá que pasarlo a la generica
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
                Task.current.Succeed();
                Debug.Log("Hay Hormiga Cerca que necesita curarse");
            }
            else
            {
                Task.current.Fail();
            }
        }

            foreach (HormigaGenerica h in hormigasCerca)
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
        }
    }

    [Task]
    public void Explorar()
    {
        if (this.zonaDondeEsta == 1)
        {
            //Esta fuera, tiene que entrar
            //Debug.Log("Estoy fuera ais que tengo que entrar");
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
                    } while (!aux2 || (aux.position.x < (hormigueroDentro.transform.position.x - (hormigueroDentro.width / 2))));
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

    #endregion
}
