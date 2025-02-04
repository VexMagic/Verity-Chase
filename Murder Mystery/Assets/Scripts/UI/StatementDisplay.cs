using UnityEngine;
using UnityEngine.UI;

public class StatementDisplay : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite highlighted;
    [SerializeField] private Sprite normal;

    public void SetImage(bool isHighlighted)
    {
        if (isHighlighted)
            image.sprite = highlighted;
        else
            image.sprite = normal;
    }
}
