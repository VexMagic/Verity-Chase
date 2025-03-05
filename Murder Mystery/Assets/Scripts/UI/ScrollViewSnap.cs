using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class ScrollViewSnap : MonoBehaviour
{
    public static ScrollViewSnap instance;

    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private Vector2 offset;
    [SerializeField] private Vector2 extraOffset;

    private void Awake()
    {
        instance = this;
    }

    public void SnapTo(RectTransform target, bool useExtraOffset)
    {
        Canvas.ForceUpdateCanvases();

        Vector2 tempOffset = offset;
        if (useExtraOffset)
            tempOffset += extraOffset;

        contentPanel.anchoredPosition =
            (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position)
            - (Vector2)scrollRect.transform.InverseTransformPoint(target.position) + tempOffset;
    }
}
