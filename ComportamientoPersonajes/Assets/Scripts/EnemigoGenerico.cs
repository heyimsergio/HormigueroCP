using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using UnityEngine.AI;

public class EnemigoGenerico : PersonajeGenerico
{
    //Agente Navmesh
    NavMeshAgent agente;
    PandaBehaviour pb;

    Reina reina = null;

    public List<HormigaGenerica> hormigasCerca = new List<HormigaGenerica>();
    public List<HormigaGenerica> hormigasAtacandole = new List<HormigaGenerica>();
    public HormigaGenerica hormigaAAtacar = null;
    protected int tiempoParaIrse;

    public float tiempoEntreAtaques;
    [HideInInspector]
    public float tiempoEntreAtaquesMax = 0.5f;

    Floor hormigueroDentro;
    Vector3 siguientePosicionExplorar;

    public bool escarabajo;

    // Start is called before the first frame update
    void Start()
    {
        if (escarabajo)
        {
            this.vida = 30;
            this.daño = 3;
            this.velocidad = 1.0f;
        } else
        {
            this.vida = 15;
            this.daño = 2;
            this.velocidad = 2.0f;
        }
        this.zonaDondeEsta = 1;

        pb = this.gameObject.GetComponent<PandaBehaviour>();
        agente = this.gameObject.GetComponent<NavMeshAgent>();
        agente.speed = this.velocidad;
        this.siguientePosicionExplorar = Vector3.zero;
        hormigueroDentro = GameObject.FindObjectOfType<Floor>();
        tiempoEntreAtaques = tiempoEntreAtaquesMax;
        reina = GameObject.FindObjectOfType<Reina>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Obrera")
        {
            Debug.Log("Colision con hormiga");
        }

        if (other.tag == "Reina" ||
            other.tag == "Nurse" ||
            other.tag == "Obrera" ||
            other.tag == "Soldado")
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
        if (other.tag == "Reina" ||
            other.tag == "Nurse" ||
            other.tag == "Obrera" ||
            other.tag == "Soldado")
        {
            HormigaGenerica aux = other.GetComponent<HormigaGenerica>();
            if (hormigasCerca.Contains(aux))
            {
                hormigasCerca.Remove(aux);
            }
        }
    }

    [Task]
    public void HayHormigasCerca()
    {
        if (hormigasCerca.Count != 0)
        {
            Task.current.Succeed();
        } else
        {
            Task.current.Fail();
        }
    }

    [Task]
    public void Atacar()
    {
        if (hormigasCerca.Count > 0)
        {
            if (hormigaAAtacar == null)
            {
                hormigaAAtacar = hormigasCerca[Random.Range(0, hormigasCerca.Count)];
            }
            //HormigaGenerica hormigaCerca = hormigasCerca[Random.Range(0, hormigasCerca.Count)];
            if (hormigaAAtacar != null)
            {
                agente.SetDestination(hormigaAAtacar.transform.position);
                float distanceToTarget = Vector3.Distance(transform.position, hormigaAAtacar.transform.position);
                if (distanceToTarget < 1.2f)
                {
                    if (tiempoEntreAtaques <= 0)
                    {
                        float random = Random.Range(0, 10);
                        if (random < 9f)
                        {
                            hormigaAAtacar.QuitarVida(this.daño);
                            //hormigaAAtacar = null;
                        }
                        else
                        {
                            //Debug.Log("Ataque fallido");
                        }
                        tiempoEntreAtaques = tiempoEntreAtaquesMax;
                    }
                    else
                    {
                        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * 1 + hormigaAAtacar.transform.position;
                        agente.SetDestination(randomDirection);
                        tiempoEntreAtaques -= Time.deltaTime;
                    }
                    Task.current.Succeed();
                    return;
                }
                else
                {
                    if (!hormigasCerca.Contains(hormigaAAtacar))
                    {
                        hormigaAAtacar = null;
                    }
                    else
                    {
                        agente.SetDestination(hormigaAAtacar.transform.position);
                    }
                    Task.current.Succeed();
                    return;
                }
            }
            else
            {
                //hormigasCerca.RemoveAt(0);
                Task.current.Fail();
                return;
            }

            /*if (hormigasCerca.Count == 0)
            {
                siguientePosicion = this.transform.position;
                Task.current.Succeed();
            }*/
        }
        else
        {
            Task.current.Fail();
            return;
        }
    }

    public void quitarVida(int damage)
    {
        //Debug.Log("Enemigo perdiendo vida");
        this.vida -= damage;
        if (vida <= 0)
        {
            reina.EnemigoHaMuerto(this);
            Destroy(this.gameObject);
        }
    }

    [Task]
    public void Explorar()
    {
        //Debug.Log("Explorar");
        hormigaAAtacar = null;

        // si esta dentro
        if (zonaDondeEsta == 0)
        {
            // sale
            siguientePosicionExplorar = Vector3.zero;
            Vector3 randomDirection;
            NavMeshHit aux;
            bool aux2;
            do
            {
                randomDirection = UnityEngine.Random.insideUnitSphere * 10 + reina.afueras.centro;
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
                    randomDirection = UnityEngine.Random.insideUnitSphere * (10) + this.transform.position;
                    aux2 = NavMesh.SamplePosition(randomDirection, out aux, 4.0f, NavMesh.AllAreas);
                } while (!aux2);
                siguientePosicionExplorar = new Vector3(aux.position.x, 0, aux.position.z);
                //Debug.Log("Posicion a la que va: " + siguientePosicionExplorar);
                agente.SetDestination(siguientePosicionExplorar);
            }
            else if (Vector3.Distance(this.transform.position, siguientePosicionExplorar) < 0.5f)
            {
                siguientePosicionExplorar = Vector3.zero;
            }
            else
            {
                agente.SetDestination(siguientePosicionExplorar);
            }

        }
        Task.current.Succeed();
    }

}