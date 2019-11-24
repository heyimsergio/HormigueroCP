using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Huevo : MonoBehaviour
{
    public Reina.TipoHormiga miType;
    public float tiempoParaNacer;
    public float tiempoQueAguantaSinCuidar;
    public float maxTimeParaCuidar;
    public bool necesitaCuidados = false;
    public bool puedeSerCuidado = false;
    public HormigaGenerica siendoCuidadoPor = null;
    public int umbralDeAvisoCuidarHuevo;
    public int umbralDePoderseCuidar;
    public Room myRoom;
    public TileScript myTile;
    private Reina miReina;
    Collider huevoCollider;
    public List<HormigaGenerica> hormigasCerca = new List<HormigaGenerica>();

    public GameObject huevoF;

    // Start is called before the first frame update
    void Start()
    {
        miReina = GameObject.FindObjectOfType<Reina>();
        huevoCollider = GetComponent<Collider>();
        huevoCollider.isTrigger = true;

        // Variables huevo
        maxTimeParaCuidar = 150;
        tiempoQueAguantaSinCuidar = maxTimeParaCuidar - Random.Range(0, 20);
        tiempoParaNacer = 150 + Random.Range(0, 100);
        umbralDeAvisoCuidarHuevo = 70;
        umbralDePoderseCuidar = 100;
    }

    public void Init(Room aux, Reina.TipoHormiga tipo, TileScript tile)
    {
        myRoom = aux;
        miType = tipo;
        myTile = tile;
    }

    // Update is called once per frame
    void Update()
    {
        if(tiempoQueAguantaSinCuidar < umbralDePoderseCuidar)
        {
            puedeSerCuidado = true;
        }

        //Avisamos de que hay que cuidar al huevo
        if(tiempoQueAguantaSinCuidar < umbralDeAvisoCuidarHuevo)
        {
            necesitaCuidados = true;
            if (siendoCuidadoPor == null)
            {
                if (!miReina.huevosQueTienenQueSerCuidados.Contains(this))
                {
                    miReina.huevosQueTienenQueSerCuidados.Add(this);
                }
            }
        }

        // Huevo muerto
        if(tiempoQueAguantaSinCuidar < 0)
        {
            //Debug.Log("Huevo Muerto");
            miReina.HuevoHaMuerto(this);
            GameObject aux = Instantiate(huevoF, this.transform.position, Quaternion.identity);
            aux.transform.Translate(0, 0.03f, 0);
            aux.transform.Rotate(90, 0, 0);
            Destroy(this.gameObject);
        }
        // Nace Huevo
        else if (tiempoParaNacer < 0)
        {
            miReina.NaceHormiga(this);
            miReina.HuevoHaMuerto(this);
            Destroy(this.gameObject);
        }
        tiempoQueAguantaSinCuidar -= Time.deltaTime;
        tiempoParaNacer -= Time.deltaTime;
    }

    public void Cuidar()
    {
        if (necesitaCuidados)
        {
            necesitaCuidados = false;
            miReina.huevosQueTienenQueSerCuidados.Remove(this);
        }
        puedeSerCuidado = false;
        tiempoQueAguantaSinCuidar = maxTimeParaCuidar;
    }
}
