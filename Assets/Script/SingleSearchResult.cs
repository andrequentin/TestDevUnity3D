using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;



public class SingleSearchResult : MonoBehaviour, IPointerClickHandler
{
    // We want to open the link of the recipe into a browser, we need an onClick event for that
    public Event onClick;

    //We also need a RawImage for thumbnail, a title and the list of ingredients (from the prefab)
    [SerializeField]
    RawImage thumb;
    [SerializeField]
    TMPro.TMP_Text title;
    [SerializeField]
    TMPro.TMP_Text ingredients;

    //To store the recipe url 
    private string url;

    //We build our search result with our result title,href and ingredient
    public void Build(Result r)
    {
        title.text = r.title;
        url = r.href;
        ingredients.text = r.ingredients;
        if (r.thumbnail.Length > 1)
        {
            // We need a couroutine for the thumbnail as it require a webrequest
            StartCoroutine(SetImage(r.thumbnail));
        }
        else
        {
            //If it has no thumbnail we made our image transparent
            Color i = thumb.color;
            i.a = 0;
            thumb.color = i;
        }
    }

    IEnumerator SetImage(string url)
    {
        // We request and load the thumbnail into the RawImange texture
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("Couldn't get thumbnail at  : " + url);

            Debug.Log(request.error);
        }
        else
        {
            thumb.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }

    // If we have a click on our object we open the url
    public void OnPointerClick(PointerEventData eventData)
    {
        Application.OpenURL(url);
    }
}
