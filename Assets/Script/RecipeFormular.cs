using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;


/*
 * This is a script to manage the search fomulary for Recipe Puppy API area.
 *  
 * 
 */

public class RecipeFormular : MonoBehaviour
{
    // The input field for keyword search and ingredients
    [SerializeField]
    TMP_InputField searchField;
    [SerializeField]
    TMP_InputField ingredientField;
    //The gameobject containing the search result and ingredient list
    [SerializeField]
    GameObject ingredientList;
    [SerializeField]
    GameObject searchResultList;

    //All the prefabs we need to display the search result and the dynamic ingredient list
    [SerializeField]
    GameObject SingleResultPrefab;
    [SerializeField]
    GameObject NoResultPrefab;
    [SerializeField]
    GameObject ingredientPrefab;
    [SerializeField]
    GameObject PageHandlerPrefab;

    //The current search page
    private int currentPage = 1;
    //Our current ingredients
    private List<GameObject> ingredients;
    //Our current search results
    private List<GameObject> currentSearchResults;


    private void Start()
    {
        ingredients = new List<GameObject>();
        currentSearchResults = new List<GameObject>();
    }

    void OnEnable()
    {
        //Telling the gamemanager that we entered Recipe menu
        GameManager.Instance.Recipe();
        //Telling the main character to stop moving (so we dont leave the interaction area)
        GameManager.MainCharacter.Stop();
    }

    void OnDisable()
    {
        //Telling the gamemanager that we're back in the game
        GameManager.Instance.Game();
    }

    
    //Called when the ingredient button is clicked
    public void AddIngredient()
    {
        //we had ingredient only if there is text in the input field
        if (ingredientField.text.Length > 0)
        {
            //We initialize the new ingredient list element with its prefab, had it to our list of Ingredient GameObject
            string toAdd = ingredientField.text;
            GameObject IngGO = Instantiate(ingredientPrefab, ingredientList.transform);
            IngGO.name = toAdd;
            IngGO.GetComponentInChildren<TMP_Text>().text = toAdd;

            //We had an action for ingredient button wich will remove the ingredient
            IngGO.GetComponentInChildren<Button>().onClick.AddListener(() => RemoveIngredient(toAdd));

            ingredients.Add(IngGO);
            ingredientField.text = ""; // Clean the input field
        }
        //Refresh search  when we add an ingredient
        Search(1);
    }

    //Remove an ingredient from our list
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
        //Refresh search after we removed an ingredient
        Search(1);
    }

    //We build queries for the api
    public string BuildQuery(int page)
    {
        //We build a string list with our ingredient
        List<string> ingredientsString = new List<string>();
        foreach (GameObject g in ingredients)
        {
            ingredientsString.Add(g.name);
        }
        string searchFieldString = searchField.text;

        //We initialize our query
        string query = "http://www.recipepuppy.com/api/?";

        //if we have ingredients we add them as i=ingredint1,ingredient2
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
                    //if we also have a something in the keyword search input field we add an &
                    query += "&";
                }
            }
        }
        
        //if we have a something in the keyword search input field we add it as q=keywordsearch
        if (searchFieldString.Length > 0)
        {
            query += "q=";
            query += searchFieldString;
        }
        //We add the page number
        query += "&p=" + page.ToString();
        return query;
    }

    //This is a new search
    public void Search(int page)
    {
        //We actualise our current page(as we sometime call it from the page Handler to progress in page search)
        currentPage = page;

        // We Build our query
        string query = BuildQuery(currentPage);

        //Launch our WebRequest with the DisplaySearchResult as a callback to display our search result
        StartCoroutine(Request(query, DisplaySearchResult));

    }

    //Coroutine to do our web request
    //it has a callback function so we can use for different purpose : Display our search result, or simply check if there is result on the next page
    IEnumerator Request(string q, System.Action<PuppyRequestRoot> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(q);
        yield return request.SendWebRequest();

        
        PuppyRequestRoot result = new PuppyRequestRoot() ;
        
        //Handling two error cases
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("Erreur : " + request.result.ToString());
        }else
        {
            //We Deserialize our JSON string into our Object
            result = JsonConvert.DeserializeObject<PuppyRequestRoot>(request.downloadHandler.text);
        }
        //Call our callback with result
        callback(result);

    }

    //Time to show the results ! 
    public void DisplaySearchResult(PuppyRequestRoot result)
    {
        //Start with a clean of our result list
        CleanSearchResult();

        if (result.results.Count > 1)
        {
            //If we have at least one result 
            foreach (Result r in result.results)
            {
                GameObject Go = Instantiate(SingleResultPrefab, searchResultList.transform);

                //Single search result to handle a single result, as we need to get thumbnail an all ... 
                SingleSearchResult sro = Go.GetComponent<SingleSearchResult>();
                sro.Build(r);
                //nothing special then, we just add the gameobject to our current search list
                currentSearchResults.Add(Go);
            }

            string query = BuildQuery(currentPage + 1);
            // As said before we can also use Request Coroutine to set our page handler
            StartCoroutine(Request(query, SetPageHandler));
        }
        else
        {
            // If we have an empty result we load the NoSearchResult PRefab and build it
            List<string> ingredientsString = new List<string>();
            foreach (GameObject g in ingredients)
            {
                ingredientsString.Add(g.name);
            }
            string searchFieldString = searchField.text;

            GameObject GO = Instantiate(NoResultPrefab, searchResultList.transform);
            GO.GetComponent<NoSearchResult>().Build(searchFieldString, ingredientsString);
            //We put in the search result list so it get clean/rebuild on the next Search
            currentSearchResults.Add(GO);
        }

    }


    public void SetPageHandler(PuppyRequestRoot prr)
    {
        GameObject GoPG = Instantiate(PageHandlerPrefab, searchResultList.transform);
        //Load the PageHandler Prefab and Build it
        GoPG.GetComponent<PageHandler>().Build(currentPage, (prr.results.Count > 0), this);
        currentSearchResults.Add(GoPG);
        //We put in the search result list so it get clean/rebuild on the next Search
    }

    //Clean the search result
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

    // Call by the quit button, allow the player to move again and make him out of the area
    public void Quit()
    {
        GameManager.MainCharacter.unStop();
        GameManager.MainCharacter.agent.SetDestination(Vector3.zero);
    }
}
