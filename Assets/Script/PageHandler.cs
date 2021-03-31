using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageHandler : MonoBehaviour
{
    [SerializeField]
    public Button next;
    [SerializeField]
    public Button prev;
    [SerializeField]
    public TMPro.TMP_Text num;

    public void Build(int n, bool ne,RecipeFormular rf)
    {
        if (ne)
        {
            next.onClick.AddListener(delegate { rf.Search(n + 1); });
        }
        else
        {
            next.gameObject.SetActive(false);
        }
        if (n>1)
        {
            prev.onClick.AddListener(delegate { rf.Search(n - 1); });
        }
        else
        {
            prev.gameObject.SetActive(false);
        }
        num.text = n.ToString();

    }

}
