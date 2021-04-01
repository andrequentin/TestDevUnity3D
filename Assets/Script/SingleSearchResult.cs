using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

public class SingleSearchResult : MonoBehaviour, IPointerClickHandler
{

    public Event onClick;

    [SerializeField]
    RawImage thumb;
    [SerializeField]
    TMPro.TMP_Text title;
    [SerializeField]
    TMPro.TMP_Text ingredients;

    private string url;


    public void Build(Result r)
    {
        title.text = r.title;
        url = r.href;
        ingredients.text = r.ingredients;
        if (r.thumbnail.Length > 1)
        {
            StartCoroutine(SetImage(r.thumbnail));
        }
        else
        {
            Color i = thumb.color;
            i.a = 0;
            thumb.color = i;
        }
    }

    IEnumerator SetImage(string url)
    {
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


    public void OnPointerClick(PointerEventData eventData)
    {
        Application.OpenURL(url);
    }
}
