using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CopyTextCom : MonoBehaviour
{
    public TextMeshProUGUI targetText;
    public List<TextMeshPro> list_Copy = new List<TextMeshPro>();
    string originText;

    void Start()
    {
        
    }

    void Update()
    {
        CopyText();
    }

    void CopyText()
    {
        originText = targetText.text;
        for(int i = 0; i < list_Copy.Count; i++)
        {
            list_Copy[i].text = originText;
        }
    }
}
