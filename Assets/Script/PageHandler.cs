using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageHandler : MonoBehaviour
{
    // Our page handler need : a previous button, a next button, and page number
    [SerializeField]
    public Button next;
    [SerializeField]
    public Button prev;
    [SerializeField]
    public TMPro.TMP_Text num;

    public void Build(int n, bool ne,RecipeFormular rf)
    {
        // ne is true if there is a next page, if so, we set up the onClick action to search for the next page
        if (ne)
        {
            next.onClick.AddListener(delegate { rf.Search(n + 1); });
        }
        else
        {
            //else we dont need it
            next.gameObject.SetActive(false);
        }
        //If our page number is > 1, we have a previous page so we set up the onClick action to search for the previous page
        if (n>1)
        {
            prev.onClick.AddListener(delegate { rf.Search(n - 1); });
        }
        else
        {   
            //else we dont need it
            prev.gameObject.SetActive(false);
        }
        //Finally the current page number
        num.text = n.ToString();

    }

}
