using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SpeechBubbleCom : MonoBehaviour
{
    public SpriteRenderer image_background;
    public RectTransform transform_textRect;

    float paddingWidth = 0.1f;
    float paddingHeight = 0.05f;

    void Start()
    {

    }

    void Update()
    {
        SetRectSize();
    }

    public void SetRectSize()
    {
        //https://forum.unity.com/threads/finding-the-size-of-a-content-size-fitter.312008/
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform_textRect);
        image_background.size = new Vector2(transform_textRect.rect.width + paddingWidth, transform_textRect.rect.height + paddingHeight);
    }
}