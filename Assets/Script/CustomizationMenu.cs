using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UMA;
using UMA.CharacterSystem;
using Newtonsoft.Json;

public class CustomizationMenu : MonoBehaviour
{
    // character
    DynamicCharacterAvatar mainCharacter;
    // UI
    [SerializeField]
    Slider heigth;
    [SerializeField]
    Slider muscle;
    [SerializeField]
    ImageColorPicker hairColor;
    [SerializeField]
    TMPro.TMP_InputField nom;

    [SerializeField]
    GameObject characterLoadButtonPrefab;
    [SerializeField]
    GameObject ButtonParent;

    [SerializeField]
    GameObject LoadingPanel;

    [SerializeField]
    Transform playerStand;

    //Usefull data containers
    private Dictionary<string, DnaSetter> dna;

    private Dictionary<string, string> saves = new Dictionary<string, string>();

    private string mycharacter;



    private void Start()
    {

        mainCharacter =GameManager.MainCharacter.GetComponent<DynamicCharacterAvatar>();
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
        GameManager.MainCharacter.Stop();
        GameManager.MainCharacter.transform.position = playerStand.position;
        GameManager.MainCharacter.transform.rotation = playerStand.rotation;

        heigth.onValueChanged.AddListener(delegate { HeightValueChange(); });
        muscle.onValueChanged.AddListener(delegate { MuscleValueChange(); });
    }
    void OnDisable()
    {
        GameManager.Instance.Game();
        heigth.onValueChanged.RemoveAllListeners();
        muscle.onValueChanged.RemoveAllListeners();
    }


    public void MuscleValueChange()
    {
        if (dna == null) dna = mainCharacter.GetDNA();

        dna["upperMuscle"].Set(muscle.value);
        dna["lowerMuscle"].Set(muscle.value);
        mainCharacter.BuildCharacter();


    }
    public void HeightValueChange()
    {
        if (dna == null) dna = mainCharacter.GetDNA();

        dna["height"].Set(heigth.value);
        mainCharacter.BuildCharacter();

    }

    public void setHairColor()
    {
        mainCharacter.SetColor("Hair", hairColor.selectedColor);
        mainCharacter.BuildCharacter();

    }

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
    public void LoadPanel()
    {

        foreach(KeyValuePair<string,string> kv in saves)
        {
            GameObject Go = Instantiate(characterLoadButtonPrefab, ButtonParent.transform);
            string arg = kv.Key;
            Go.name = arg;
            Go.GetComponentInChildren<TMPro.TMP_Text>().text = arg;
            Go.GetComponent<Button>().onClick.AddListener( ()=> LoadCharacter(arg) );
            tempButtons.Add(Go);
        }

        LoadingPanel.SetActive(true);
    }
    public void CloseLoadingPanel()
    {
        foreach (GameObject g in tempButtons)
        {
            Destroy(g);
        }
        tempButtons.Clear();
        LoadingPanel.SetActive(false);
    }
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

    private void UpdateUI()
    {
        if (dna == null)
        {
            dna = mainCharacter.GetDNA();
        }
        heigth.value = dna["height"].Value;
        muscle.value = dna["upperMuscle"].Value;
    }

    public void Quit()
    {
        GameManager.MainCharacter.unStop();
        GameManager.MainCharacter.agent.SetDestination(Vector3.zero);
        //this.gameObject.SetActive(false);
    }
}