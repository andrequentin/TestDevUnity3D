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

    private static Player mainCharacter = null;

  
    public static Player MainCharacter{
        get { return mainCharacter; }
        set { mainCharacter = value; }
    }

    public GameObject escapeCanvas;



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
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if( Input.GetKey(KeyCode.Escape))
        {
            escapeCanvas.SetActive(true);
        }
    }
    
  

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
