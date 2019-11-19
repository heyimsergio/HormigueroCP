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
    protected int tiempoParaIrse;

    public float tiempoEntreAtaques;
    [HideInInspector]
    public float tiempoEntreAtaquesMax = 0.5f;

    Floor hormigueroDentro;

    // Start is called before the first frame update
    void Start()
    {
        this.zonaDondeEsta = 1;
        this.vida = 10;
        this.daño = 2;
        pb = this.gameObject.GetComponent<PandaBehaviour>();
        agente = this.gameObject.GetComponent<NavMeshAgent>();
        this.siguientePosicion = this.transform.position;
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
            HormigaGenerica hormigaCerca = hormigasCerca[0];
            if (hormigaCerca != null)
            {
                agente.SetDestination(hormigaCerca.transform.position);
                float distanceToTarget = Vector3.Distance(transform.position, hormigaCerca.transform.position);
                if (distanceToTarget < 1.2f)
                {
                    if (tiempoEntreAtaques <= 0)
                    {
                        float random = Random.Range(0, 10);
                        if (random < 9f)
                        {
                            hormigaCerca.quitarVida(this.daño);
                        }
                        else
                        {
                            //Debug.Log("Ataque fallido");
                        }
                        tiempoEntreAtaques = tiempoEntreAtaquesMax;
                    }
                    else
                    {
                        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * 1 + hormigaCerca.transform.position;
                        agente.SetDestination(randomDirection);
                        tiempoEntreAtaques -= Time.deltaTime;
                    }

                }
            }
            else
            {
                hormigasCerca.RemoveAt(0);
                if (hormigasCerca.Count == 0)
                {
                    siguientePosicion = this.transform.position;
                    Task.current.Succeed();
                }
            }
        }
    }

    public void quitarVida(int damage)
    {
        Debug.Log("Enemigo perdiendo vida");
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
        float distanceToTarget = Vector3.Distance(transform.position, this.siguientePosicion);
        //Debug.Log(distanceToTarget);
        if (distanceToTarget < 0.2f)
        {
            Vector3 randomDirection;
            NavMeshHit aux;
            bool aux2;
            do
            {
                randomDirection = UnityEngine.Random.insideUnitSphere * 50 + this.transform.position;
                aux2 = NavMesh.SamplePosition(randomDirection, out aux, 1.0f, NavMesh.AllAreas);
            } while (aux.position.x > (hormigueroDentro.transform.position.x - (hormigueroDentro.width / 2)-1) || !aux2);

            //Debug.Log("Salir hacia: " + aux.position);
            //saliendo = true;
            agente.SetDestination(aux.position);
            siguientePosicion = aux.position;
            /*
            Vector3 randomDirection;
            NavMeshHit aux;
            do
            {
                randomDirection = UnityEngine.Random.insideUnitSphere * 50 + this.transform.position;
            } while (!NavMesh.SamplePosition(randomDirection, out aux, 1.0f, NavMesh.AllAreas));


            agente.SetDestination(aux.position);
            this.siguientePosicion = aux.position;*/
        } else
        {
            agente.SetDestination(siguientePosicion);
        }
        Task.current.Succeed();
        //
    }

    
}
