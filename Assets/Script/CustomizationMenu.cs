using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UMA;
using UMA.CharacterSystem;
using Newtonsoft.Json;

public class CustomizationMenu : MonoBehaviour
{
    // Our character to customize
    DynamicCharacterAvatar mainCharacter;
    
    // UI button and slider
    [SerializeField]
    Slider heigth;
    [SerializeField]
    Slider muscle;
    [SerializeField]
    ImageColorPicker hairColor;
    [SerializeField]
    TMPro.TMP_InputField nom;

    //Our character button prefab for saving and loading menu
    [SerializeField]
    GameObject characterLoadButtonPrefab;
    [SerializeField]
    GameObject ButtonParent;
    [SerializeField]
    GameObject LoadingPanel;

    //The position of the player during the customization 
    [SerializeField]
    Transform playerStand;

    //Usefull data containers
    private Dictionary<string, DnaSetter> dna;

    //Disctionnary of saves
    private Dictionary<string, string> saves = new Dictionary<string, string>();

    //mycharacter is our character description as a string
    private string mycharacter;



    private void Start()
    {
        // Get the main character
        mainCharacter =GameManager.MainCharacter.GetComponent<DynamicCharacterAvatar>();

        //Get out saves if we have some
        if (PlayerPrefs.GetString("saves") != "")
        {
            saves = JsonConvert.DeserializeObject<Dictionary<string, string>>(PlayerPrefs.GetString("saves"));
        }else
        {
            saves = new Dictionary<string, string>();
        }
        LoadingPanel.SetActive(false);
    }


    void OnEnable()
    {
        
        GameManager.Instance.Customization();
        //Stop the character and put it at the right place for customization
        GameManager.MainCharacter.Stop();
        GameManager.MainCharacter.transform.position = playerStand.position;
        GameManager.MainCharacter.transform.rotation = playerStand.rotation;
        //Listerner for the sliders value
        heigth.onValueChanged.AddListener(delegate { HeightValueChange(); });
        muscle.onValueChanged.AddListener(delegate { MuscleValueChange(); });
    }
    void OnDisable()
    {
        GameManager.Instance.Game();
        heigth.onValueChanged.RemoveAllListeners();
        muscle.onValueChanged.RemoveAllListeners();
    }

    //When the slider has its value changed we update our character
    public void MuscleValueChange()
    {
        if (dna == null) dna = mainCharacter.GetDNA();
        dna["upperMuscle"].Set(muscle.value);
        dna["lowerMuscle"].Set(muscle.value);
        mainCharacter.BuildCharacter();
    }
    //When the slider has its value changed we update our character
    public void HeightValueChange()
    {
        if (dna == null) dna = mainCharacter.GetDNA();
        dna["height"].Set(heigth.value);
        mainCharacter.BuildCharacter();
    }
    //Whan we clicked on a color of the color picker (also see ImageColorPicker.cs)
    public void setHairColor()
    {
        mainCharacter.SetColor("Hair", hairColor.selectedColor);
        mainCharacter.BuildCharacter();
    }

    //Called by the hair buttons
    public void setHair(int v)
    {
        switch (v)
        {
            case 0:
                mainCharacter.SetSlot("Hair", "MaleHairSlick01_Recipe");
                break;
            case 1:
                mainCharacter.SetSlot("Hair", "MaleHair2");
                break;
            case 2:
                mainCharacter.SetSlot("Hair", "MaleHair3");
                break;
        }
        mainCharacter.BuildCharacter();
    }

    //To save a character -> add it to dictionnary, save dictionnary in player pref
    public void SaveCharacter()
    {
        if (nom.text.Length > 0)
        {
            mycharacter = mainCharacter.GetCurrentRecipe();
            saves[nom.text] = mycharacter;
            PlayerPrefs.SetString("saves",JsonConvert.SerializeObject(saves));
        }
    }

    private List<GameObject> tempButtons=new List<GameObject>();

    //When we click on the load button, load and show the character loading panel
    public void LoadPanel()
    {
        foreach(KeyValuePair<string,string> kv in saves)
        {
            //We create button for each characters saved, using the characterbutton prefab
            GameObject Go = Instantiate(characterLoadButtonPrefab, ButtonParent.transform);
            string arg = kv.Key;
            Go.name = arg;
            Go.GetComponentInChildren<TMPro.TMP_Text>().text = arg;
            //Button call LoadCharacter with the character name as argument
            Go.GetComponent<Button>().onClick.AddListener( ()=> LoadCharacter(arg) );
            tempButtons.Add(Go);
        }

        LoadingPanel.SetActive(true);
    }
    //Called by the exit loadingpanel button
    public void CloseLoadingPanel()
    {
        foreach (GameObject g in tempButtons)
        {
            Destroy(g);
        }
        tempButtons.Clear();
        LoadingPanel.SetActive(false);
    }
    //Called by button
    public void LoadCharacter(string s)
    {

        mainCharacter.ClearSlots();
        mainCharacter.LoadFromRecipeString(saves[s]);
        nom.text = s;
        CloseLoadingPanel();
        dna = mainCharacter.GetDNA();
        UpdateUI();
    }

    //Saving and loading utilities
    //Updating sliders value on character loading
    private void UpdateUI()
    {
        if (dna == null)
        {
            dna = mainCharacter.GetDNA();
        }
        heigth.value = dna["height"].Value;
        muscle.value = dna["upperMuscle"].Value;
    }
    //When we quit the customization menu
    //Let the character move and make him out of the trigger
    public void Quit()
    {
        GameManager.MainCharacter.unStop();
        GameManager.MainCharacter.agent.SetDestination(Vector3.zero);
        //this.gameObject.SetActive(false);
    }
}