using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;

public class RecipeFormular : MonoBehaviour
{
    [SerializeField]
    TMP_InputField searchField;
    [SerializeField]
    TMP_InputField ingredientField;
    [SerializeField]
    GameObject SingleResultPrefab;
    [SerializeField]
    GameObject ingredientPrefab;
    [SerializeField]
    GameObject ingredientList;
    [SerializeField]
    GameObject searchResultList;
    [SerializeField]
    GameObject PageHandlerPrefab;

    private List<GameObject> ingredients;
    private int currentPage=1;
    private List<GameObject> currentSearchResults;


    private void Start()
    {
        ingredients = new List<GameObject>();
        currentSearchResults = new List<GameObject>();
    }

   


    void OnEnable()
    {

        GameManager.Instance.Recipe();
        GameManager.MainCharacter.Stop();
    }
    void OnDisable()
    {
        GameManager.Instance.Game();
    }

    

    public void AddIngredient()
    {
        if (ingredientField.text.Length > 0)
        {
            string toAdd = ingredientField.text;
            GameObject IngGO = Instantiate(ingredientPrefab, ingredientList.transform);
            IngGO.name = toAdd;
            IngGO.GetComponentInChildren<TMP_Text>().text = toAdd;
            IngGO.GetComponentInChildren<Button>().onClick.AddListener(() => RemoveIngredient(toAdd));

            ingredients.Add(IngGO);
            ingredientField.text = "";
        }
        Search(1);
    }
    public void RemoveIngredient(string s)
    {
        List<GameObject> toremove = new List<GameObject>();
        foreach(GameObject g in ingredients)
        {
            if(g.name == s)
            {
                toremove.Add(g);           
            }
        }
        foreach(GameObject g in toremove)
        {
            ingredients.Remove(g);
            Destroy(g);
        }
        Search(1);
    }

    public string BuildQuery(int page)
    {
        List<string> ingredientsString = new List<string>();
        foreach (GameObject g in ingredients)
        {
            ingredientsString.Add(g.name);
        }
        string searchFieldString = searchField.text;

        string query = "http://www.recipepuppy.com/api/?";

        if (ingredientsString.Count > 0)
        {
            query += "i=";
            for (int i = 0; i < ingredientsString.Count; i++)
            {
                query += ingredientsString[i];
                if (i != ingredientsString.Count - 1)
                {
                    query += ",";
                }
                else if (searchFieldString.Length > 0)
                {
                    query += "&q=";
                }
            }
        }

        if (searchFieldString.Length > 0)
        {
            query += searchFieldString;
        }

        query += "&p=" + page.ToString();
        Debug.Log("Query : " + query);
        return query;
    }

    public void Search(int page)
    {
        currentPage = page;

        string query = BuildQuery(currentPage);

        StartCoroutine(Request(query, DisplaySearchResult));

    }

    IEnumerator Request(string q, System.Action<PuppyRequestRoot> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(q);
        yield return request.SendWebRequest();
        PuppyRequestRoot result = new PuppyRequestRoot() ;
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            //Debug.Log("Erreur : " + request.result.ToString());
        }else
        {
            //Debug.Log("Success : \n" + request.downloadHandler.text);
            result = JsonConvert.DeserializeObject<PuppyRequestRoot>(request.downloadHandler.text);
        }
        callback(result);

    }


    public void DisplaySearchResult(PuppyRequestRoot result)
    {

        CleanSearchResult();
        foreach(Result r in result.results)
        {
            GameObject Go = Instantiate(SingleResultPrefab, searchResultList.transform);
            SearchResultOne sro = Go.GetComponent<SearchResultOne>();
            sro.Build(r);
            currentSearchResults.Add(Go);
        }

        string query = BuildQuery(currentPage + 1);
        StartCoroutine(Request(query, SetPageHandler));

    }
    public void SetPageHandler(PuppyRequestRoot prr)
    {
     
        
        GameObject GoPG = Instantiate(PageHandlerPrefab, searchResultList.transform);
        GoPG.GetComponent<PageHandler>().Build(currentPage, (prr.results.Count > 0), this);
        currentSearchResults.Add(GoPG);
    }
  

    public void CleanSearchResult()
    {
        List<GameObject> toremove = new List<GameObject>();
        foreach (GameObject g in currentSearchResults)
        {
            toremove.Add(g);
        }
        foreach (GameObject g in toremove)
        {
            currentSearchResults.Remove(g);
            Destroy(g);
        }
        currentSearchResults.Clear();
    }

    public void Quit()
    {
        GameManager.MainCharacter.unStop();
        GameManager.MainCharacter.agent.SetDestination(Vector3.zero);
    }
}
