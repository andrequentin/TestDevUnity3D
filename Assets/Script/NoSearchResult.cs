using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoSearchResult : MonoBehaviour
{

    [SerializeField]
    TMPro.TMP_Text title;
    [SerializeField]
    TMPro.TMP_Text ingredients;

    //Nothing special here just a way to tell the user that there is no result for the current search

    public void Build(string keyword,List<string> ingredient)
    {

        title.text = "There is no result for ";
        ingredients.text = "";
        if (keyword.Length > 0)
        {
            title.text += "keywords : " + keyword;
            
        }
        if(keyword.Length > 0 && ingredient.Count > 0)
        {
            title.text += " and ";
        }
        if(ingredient.Count > 0)
        {
            title.text += "ingredients : ";
        }
        for (int i = 0; i < ingredient.Count; i++)
        {
            ingredients.text += ingredient[i];
            if (i != ingredient.Count - 1)
            {
                ingredients.text += ",";
            }
            
        }

    }
}
