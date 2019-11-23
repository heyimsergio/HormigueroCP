using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    // Start is called before the first frame update
    public bool free; //true == free cam & false == followHormiga
    private float vel;
    Camera cam;
    private Vector3 dentroPos;
    private Vector3 fueraPos;
    BocadillosControlador bc;

    void Start()
    {
        free = true;
        vel = 20f;
        cam = this.GetComponent<Camera>();
        dentroPos = new Vector3(85, 40, 25);
        fueraPos = new Vector3(25, 40, 25);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray;
            RaycastHit hit;
            ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log("raycast");
                HormigaGenerica h = hit.transform.gameObject.GetComponent(typeof(HormigaGenerica)) as HormigaGenerica;
                if (h != null || hit.transform.gameObject.tag == "Reina")
                {
                    //Debug.Log("Es hormiga");
                    free = false;
                    target = h.gameObject;
                    bc.hormigaSeleccionada = h;
                } else
                {
                    free = true;
                    target = null;
                    bc.hormigaSeleccionada = null;
                }
            }
        }
        if (Input.GetKeyDown("2"))
        {
            free = true;
            target = null;
            bc.hormigaSeleccionada = null;
            this.transform.position = dentroPos;
        } else if (Input.GetKeyDown("1"))
        {
            free = true;
            target = null;
            bc.hormigaSeleccionada = null;
            this.transform.position = fueraPos;
        }
        if (free)
        {
            if (Input.GetKey("w"))
            {
                this.transform.Translate(0, vel * Time.deltaTime, 0);
                if (this.transform.position.z > 50)
                {
                    this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 50);
                }
            }
            if(Input.GetKey("a"))
            {
                this.transform.Translate(-vel * Time.deltaTime, 0, 0);
                if (this.transform.position.x < 0)
                {
                    this.transform.position = new Vector3(0, this.transform.position.y, this.transform.position.z);
                }
            }
            if (Input.GetKey("s"))
            {
                this.transform.Translate(0, -vel * Time.deltaTime, 0);
                if (this.transform.position.z < 0)
                {
                    this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
                }
            }
            if (Input.GetKey("d"))
            {
                this.transform.Translate(vel * Time.deltaTime, 0, 0);
                if (this.transform.position.x > 110)
                {
                    this.transform.position = new Vector3(110, this.transform.position.y, this.transform.position.z);
                }
            }

        } else if (target != null)
        {
            this.transform.position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        }
        float fov = cam.fieldOfView;
        fov -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 300;
        fov = Mathf.Clamp(fov, 15, 60);
        cam.fieldOfView = fov;
    }
}
