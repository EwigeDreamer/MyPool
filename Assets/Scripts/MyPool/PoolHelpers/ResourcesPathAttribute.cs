using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MyHelpers
{
    public class ResourcesPathAttribute : PropertyAttribute { }
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ResourcesPathAttribute))]
    public class ResourcesPathDrawer : PropertyDrawer
    {
        ResourcesPathAttribute RpAttribute { get { return (ResourcesPathAttribute)attribute; } }
        float LineHeight { get { return EditorGUIUtility.singleLineHeight; } }
        float LineSpacing { get { return EditorGUIUtility.standardVerticalSpacing; } }
        float LabelWidth { get { return EditorGUIUtility.labelWidth; } }
        
        private class SerializedString
        {
            SerializedProperty serProp = null;
            public string String
            {
                get { return serProp != null ? serProp.stringValue : null; }
                set { if (serProp != null && serProp.stringValue != value) { serProp.stringValue = value; serProp.serializedObject.ApplyModifiedProperties(); } }
            }
            public SerializedString(SerializedProperty property) { serProp = property; }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
                return LineHeight * 2f + LineSpacing;
            return LineHeight;
        }

        public override void OnGUI(Rect rect, SerializedProperty serProp, GUIContent label)
        {
            if (serProp.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(EditorGUI.IndentedRect(rect), "Use ResourcesPath attribute with string");
                return;
            }


            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            Rect rect1 = rect;
            rect1.yMax -= LineHeight + LineSpacing;
            Rect rect2 = rect;
            rect2.yMin += LineHeight + LineSpacing;
            Rect label1, field1;
            Rect label2, field2;
            GetRectsPair(rect1, indentLevel, out label1, out field1);
            GetRectsPair(rect2, indentLevel, out label2, out field2);
            GUI.Box(EditorGUI.IndentedRect(new Rect() { xMin = field1.xMin, xMax = field2.xMax, yMin = field1.yMin, yMax = field2.yMax }), "");

            SerializedString pathProp = new SerializedString(serProp);
            Object asset = null;
            bool editable = false;
            if (!string.IsNullOrEmpty(pathProp.String))
            {
                asset = Resources.Load(pathProp.String);
                if (asset == null)
                {
                    GUIStyle warningStyle = new GUIStyle(GUI.skin.label);
                    warningStyle.fontStyle = FontStyle.Bold;
                    warningStyle.normal.textColor = Color.red;
                    warningStyle.alignment = TextAnchor.MiddleRight;
                    EditorGUI.LabelField(label2, "Invalid path!", warningStyle);
                    editable = true;
                }
            }

            EditorGUI.LabelField(label1, "Resources path");
            Object obj = EditorGUI.ObjectField(field1, asset, typeof(Object), false);
            if (obj != asset)
            {
                asset = obj;
                pathProp.String = GetResourcesPath(asset);
            }
            EditorGUI.BeginDisabledGroup(!editable);
            pathProp.String = EditorGUI.TextField(field2, pathProp.String);
            EditorGUI.EndDisabledGroup();

            EditorGUI.indentLevel = indentLevel;
        }

        void GetRectsPair(Rect source, int indentLevel, out Rect label, out Rect field)
        {
            int indTmp = EditorGUI.indentLevel;
            EditorGUI.indentLevel = indentLevel;
            label = EditorGUI.IndentedRect(source);
            label.xMax = source.xMin + LabelWidth;
            field = source;
            field.xMin += LabelWidth;
            EditorGUI.indentLevel = indTmp;
        }
        string GetResourcesPath(Object obj)
        {
            if (obj == null) return string.Empty;
            string assetPath = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError(string.Format("\"{0}\" is not an asset!", obj.name));
                return string.Empty;
            }
            string resourcesPath = string.Empty;
            string resSign = "/Resources/";
            string slashSign = "/";
            string dotSign = ".";
            if (assetPath.Contains(resSign))
            {
                int firstCut = assetPath.IndexOf(resSign) + resSign.Length;
                int lastCut = assetPath.Length;
                if (assetPath.Contains(dotSign))
                {
                    int lastSlash = assetPath.LastIndexOf(slashSign);
                    int lastDot = assetPath.LastIndexOf(dotSign);
                    if (lastDot > lastSlash)
                        lastCut = lastDot;
                }
                resourcesPath = assetPath.Substring(firstCut, lastCut - firstCut);
            }
            else
            {
                Debug.LogError(string.Format("Asset is not in \"Resources/\"!"));
            }
            return resourcesPath;
        }

    }
#endif
}
