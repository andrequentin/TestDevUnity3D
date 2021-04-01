using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    enum GameState
    {
        GAME,
        CUSTOMIZATION,
        RECIPE
    }



    private GameState GM;

    public static GameManager Instance = null;

    private static PlayerController mainCharacter = null;

  
    public static PlayerController MainCharacter{
        get { return mainCharacter; }
        set { mainCharacter = value; }
    }

    public GameObject escapeCanvas;


    //To be the only one gamanger
    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            GameManager.Instance = this;
        }
        else
        {
            Destroy(escapeCanvas);
            Destroy(gameObject);
        }
        GM = GameState.GAME;

    }

    //To open an escape menu and leave application
    void Update()
    {
        if( Input.GetKey(KeyCode.Escape))
        {
            escapeCanvas.SetActive(true);
        }
    }
    
  
    //GameState transition
    public void Game()
    {
        GM = GameState.GAME;
        mainCharacter.isControlActive = true;
    }

    public void Recipe()
    {
        GM = GameState.RECIPE;
        mainCharacter.isControlActive = false;
    }

    public void Customization()
    {
        GM = GameState.CUSTOMIZATION;
        mainCharacter.isControlActive = false;
    }
}
