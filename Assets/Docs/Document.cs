#if(UNITY_EDITOR)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class Document : MonoBehaviour
{
    [SerializeField]
    public TextAsset doc;


}


[CustomEditor(typeof(Document))]
public class DocumentEditor : Editor
{
    TextAsset doc;

    private void OnEnable()
    {
        doc = (target as Document).doc;
    }

    //public override void OnInspectorGUI()
    //{
    //    EditorGUILayout.PropertyField(serializedObject.FindProperty("doc"));

    //    if (doc != null)
    //    {
    //        GUILayout.Label(doc.text);
    //    }
    //}


    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        root.Bind(serializedObject);

        PropertyField propertyField = new PropertyField()
        {
            bindingPath = "doc",
        };

        Label text = new Label()
        {
            style =
            {
                paddingTop = new StyleLength(10),
                whiteSpace = new StyleEnum<WhiteSpace>(StyleKeyword.Auto),
            },
        };

        propertyField.RegisterValueChangeCallback((e) =>
        {
            doc = (e.changedProperty.objectReferenceValue as TextAsset);
            text.text = doc == null ? "" : doc.text;
            text.style.display = new StyleEnum<DisplayStyle>(doc == null ? DisplayStyle.None : DisplayStyle.Flex);
        });

        root.Add(propertyField);

        root.Add(text);


        return root;
    }
}

#endif