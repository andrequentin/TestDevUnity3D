using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ImageColorPicker : MonoBehaviour, IPointerClickHandler
{

    //ImageColorPicker for the customization menu
    public Color selectedColor;


    //Event for onclick event to select color and callback on click action
    [Serializable]
    public class ColorEvent : UnityEvent<Color> { }
    public ColorEvent OnColorPicked = new ColorEvent();

    //On click -> get color , call calback action
    public void OnPointerClick(PointerEventData eventData)
    {
        selectedColor = GetColor(GetPointerUVPosition());
        OnColorPicked.Invoke(selectedColor);
    }

    //Get the color at the pointer position
    private Color GetColor(Vector2 pos)
    {
        Texture2D texture = GetComponent<Image>().sprite.texture;
        Color selected = texture.GetPixelBilinear(pos.x, pos.y);
        selected.a = 1; 
        return selected;
    }

    //Get the pointer position as 2D coordinate of the image
    Vector2 GetPointerUVPosition()
    {
        Vector3[] imageCorners = new Vector3[4];
        gameObject.GetComponent<RectTransform>().GetWorldCorners(imageCorners);
        float texWidth = imageCorners[2].x - imageCorners[0].x;
        float texHeight = imageCorners[2].y - imageCorners[0].y;
        float uvX = (Input.mousePosition.x - imageCorners[0].x) / texWidth;
        float uvY = (Input.mousePosition.y - imageCorners[0].y) / texHeight;
        return new Vector2(uvX, uvY);
    }
}
