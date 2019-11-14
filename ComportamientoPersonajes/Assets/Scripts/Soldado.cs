using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Panda;

public class Soldado : HormigaGenerica
{
    NavMeshAgent agente;
    Floor hormigueroDentro; //Saber donde empieza el suelo para no meterte dentro del hormiguero cuando exploras
    Outside hormigueroFuera;
    PandaBehaviour pb;

    //Atacar
    public bool hayEnemigosCerca;
    public List<EnemigoGenerico> enemigosCerca = new List<EnemigoGenerico>();

    //Patrullar
    public int tiempoPatrullando;
    public Vector3 centro;
    public int radio;

    //Curar
    public HormigaGenerica hormigaACurar;
    public int tiempoParaCurar;

    //Ordenes de la reina
    public Reina reina;
    public bool meHanMandadoOrden;
    public enum ordenes { ORDEN1, ORDEN2 };

    //Recoger comida
    public int reservasDeComida;
    public Comida comida;
    public Room salaDejarComida = null;
    Vector3 posDejarComida = Vector3.zero;

    // Comer
    Comida comidaAComer;

    //Buscar comida
    public Vector3 siguientePosicionBuscandoComida;
    public Vector3 almacenComida;

    //Explorar
    public Vector3 siguientePosicionExplorar;

    // Start is called before the first frame update
    void Start()
    {
        agente = this.gameObject.GetComponent<NavMeshAgent>();
        hormigueroDentro = GameObject.FindObjectOfType<Floor>();
        hormigueroFuera = GameObject.FindObjectOfType<Outside>();
        reina = GameObject.FindObjectOfType<Reina>();
        pb = this.gameObject.GetComponent<PandaBehaviour>();
        this.vida = 10;
        this.daño = 2;
        siguientePosicionExplorar = this.transform.position;
    }

    // Update is called once per frame
    void Update()
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
        }
        else if (other.tag == "Trigo")
        {
            if (comida == other)
            {
                comida = null;
            }
        }
    }

    // Tareas
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
    public void TengoOrdenDeLaReina()
    {
        Task.current.Fail();
    }

    [Task]
    public void TengoOrdenDeAtacar()
    {
        Task.current.Fail();
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

    [Task]
    public void TengoOrdenDePatrullar()
    {
        Task.current.Fail();
    }

    [Task]
    public void Patrullar()
    {
        Task.current.Fail();
    }

    [Task]
    public void HayHormigaQueCurarCerca()
    {
        Task.current.Fail();
    }

    [Task]
    public void HaceMuchoQueHabiaEnemigos()
    {
        Task.current.Fail();
    }

    [Task]
    public void Explorar()
    {
        if (this.zonaDondeEsta == 1)
        {
            //Debug.Log("Estoy fuera");
            float distanceToTarget = Vector3.Distance(transform.position, siguientePosicionExplorar);
            //Debug.Log(distanceToTarget);
            if (distanceToTarget < 0.2f)
            {
                Vector3 randomDirection;
                NavMeshHit aux;
                do
                {
                    randomDirection = UnityEngine.Random.insideUnitSphere * 10 + this.transform.position;
                } while (!NavMesh.SamplePosition(randomDirection, out aux, 1.0f, NavMesh.AllAreas));
                //saliendo = true;
                agente.SetDestination(aux.position);
                siguientePosicionExplorar = aux.position;
            }
            //
        }
        else
        {
            //Debug.Log("Estoy dentro");
            if (this.zonaDondeEsta == 0)
            {
                //Debug.Log("No estoy saliendo");
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
            else if (this.zonaDondeEsta == 3)
            {
                //Debug.Log("Estoy saliendo");
                if ((transform.position.x - (hormigueroFuera.transform.position.x + hormigueroFuera.width / 2) < 2f))
                {
                    //saliendo = false;
                    //estaDentro = false;
                }
            }
        }
        Task.current.Succeed();
    }

}
