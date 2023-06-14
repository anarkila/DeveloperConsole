using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Anarkila.DeveloperConsole
{
    [CustomPropertyDrawer(typeof(AssembliesNameAttribute), true)]
    public class AssembliesNameAttributeDrawer : PropertyDrawer
    {
        private bool _initialized = false;
        private string[] _assembliesNames;
        private int _assembliesFlag;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            if (!_initialized);
                Init(property);
            EditorGUI.BeginChangeCheck();
            _assembliesFlag = EditorGUI.MaskField(position, "Included assemblies", _assembliesFlag, _assembliesNames);
            if (EditorGUI.EndChangeCheck())
            {
                List<string> assembliesNamesSelected = new();
                for (int i = 0; i < _assembliesNames.Length; i++)
                {
                    string name = _assembliesNames[i];
                    int layer = 1 << i;
                    bool selected = (_assembliesFlag & layer) != 0;
                    if (selected) 
                        assembliesNamesSelected.Add(name);
                }

                property.stringValue = string.Join(";", assembliesNamesSelected);
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override bool CanCacheInspectorGUI(SerializedProperty property) => false;

        private void Init(SerializedProperty property)
        {
            _assembliesFlag = 0;
            var assemblies = UnityEditor.Compilation.CompilationPipeline.GetAssemblies(UnityEditor.Compilation.AssembliesType.Player);
            var assembliesNamesTemp = new List<string>();
            for (int i = 0; i < assemblies.Length; i++) 
                assembliesNamesTemp.Add(assemblies[i].name);

            var assambliesSelected = property.stringValue.Split(";");
            for (int i = 0; i < assembliesNamesTemp.Count; i++)
            {
                if (assambliesSelected.Any(x => x == assembliesNamesTemp[i]))
                    _assembliesFlag |= 1 << i;
            }

            _assembliesNames = assembliesNamesTemp.ToArray();
            _initialized = true;
        }
    }
}