//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//[System.Serializable]
//public class HierarchyStyle
//{
//    public string targetName;

//    public Color textColor = Color.white;
//    public FontStyle textFontStyle;
//    public Color backgroundColor = new Color(0.225f, 0.225f, 0.225f, 1f);
//}

//[UnityEditor.InitializeOnLoad]
//public class HierarchyColor : MonoBehaviour
//{

//    [SerializeField]
//    public List<HierarchyStyle> styles;

//    HierarchyColor()
//    {
//        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;

//        styles = new List<HierarchyStyle>();

//        HierarchyStyle s = new HierarchyStyle();
//        s.targetName = "Camera_";
//        s.textColor = new Color(1f, 0.4f, 0f, 1f);
//        styles.Add(s);

//        s = new HierarchyStyle();
//        s.targetName = "Light";
//        s.textColor = Color.yellow;
//        s.textFontStyle = FontStyle.Bold;
//        styles.Add(s);

//        s = new HierarchyStyle();
//        s.targetName = "Gizmo_";
//        s.textColor = Color.green;
//        styles.Add(s);

//        s = new HierarchyStyle();
//        s.targetName = "Manager";
//        s.textColor = Color.red;
//        s.textFontStyle = FontStyle.Bold;
//        styles.Add(s);

//        s = new HierarchyStyle();
//        s.targetName = "Panel_";
//        s.textColor = Color.magenta;
//        s.textFontStyle = FontStyle.Bold;
//        styles.Add(s);

//        s = new HierarchyStyle();
//        s.targetName = "Button_";
//        s.textColor = Color.cyan;
//        styles.Add(s);

//        s = new HierarchyStyle();
//        s.targetName = "Toggle_";
//        s.textColor = Color.cyan;
//        styles.Add(s);

//        s = new HierarchyStyle();
//        s.targetName = "Slider_";
//        s.textColor = Color.cyan;
//        styles.Add(s);

//        s = new HierarchyStyle();
//        s.targetName = "Text_";
//        s.textColor = Color.green;
//        styles.Add(s);

//        s = new HierarchyStyle();
//        s.targetName = "Radio_";
//        s.textColor = Color.green;
//        styles.Add(s);
//    }

//    private void OnHierarchyWindowItemOnGUI(int instanceID, Rect rect)
//    {

//        Object target = EditorUtility.InstanceIDToObject(instanceID);
//        if (target == null)
//            return;

//        rect.x += 18f;

//        for (int i = 0; i < styles.Count; i++)
//        {
//            if (target.name.ToLower().Contains(styles[i].targetName.ToLower()))
//            {

//                Rect bgRect = new Rect(rect.x, rect.y, rect.width, rect.height);

//                if (Selection.Contains(instanceID))
//                    EditorGUI.DrawRect(bgRect, new Color(0.17f, 0.4f, 0.6f, 1f));
//                // Hover
//                else if (rect.Contains(Event.current.mousePosition))
//                {
//                    EditorGUI.DrawRect(bgRect, new Color(0.3f, 0.3f, 0.3f, 1f));
//                }
//                // Normal
//                else
//                    EditorGUI.DrawRect(bgRect, styles[i].backgroundColor);

//                Color color;
//                if ((target as GameObject).activeInHierarchy)
//                    color = styles[i].textColor;
//                else
//                    color = styles[i].textColor * 0.75f;
//                EditorGUI.LabelField(rect, target.name, new GUIStyle()
//                {
//                    normal = new GUIStyleState() { textColor = color },
//                    fontStyle = styles[i].textFontStyle
//                }
//                );
//                break;
//            }
//        }
//    }
//}
