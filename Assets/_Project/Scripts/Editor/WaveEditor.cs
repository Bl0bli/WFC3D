using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WFC3D.Editor
{
    [CustomEditor(typeof(Wave))]
    public class WaveEditor : UnityEditor.Editor

    {
        public override void OnInspectorGUI()
        {
            Debug.Log("Wave Editor");
            if (GUILayout.Button("Generate"))
            {
                Wave wave = (Wave)target;
                wave.WFC();
            }
        }
    }
}
