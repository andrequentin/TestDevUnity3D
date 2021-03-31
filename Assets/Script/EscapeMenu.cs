using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EscapeMenu : MonoBehaviour
{

    public void Resume()
    {
        this.gameObject.SetActive(false);
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
