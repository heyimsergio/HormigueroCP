using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    DataController dc;

    public void VolverAlMenu()
    {
        dc = FindObjectOfType<DataController>();
        Destroy(dc.gameObject);
        SceneManager.LoadScene(0);
    }
}
