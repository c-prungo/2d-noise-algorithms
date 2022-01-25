using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (MapGenerator))]
public class MapGeneratorEditor : Editor
{

    MapGenerator mapGen;
    Editor noiseEditor;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector ();
        DrawSettingsEditor (mapGen.noiseSettings, ref mapGen.noiseSettingsFoldout, ref noiseEditor);

        if (GUILayout.Button ("Generate")) {
            mapGen.GenerateMap ();
        }
    }

    void DrawSettingsEditor(Object settings, ref bool foldout, ref Editor editor)
    {
        foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
        using (var check = new EditorGUI.ChangeCheckScope()) {

            if (foldout) {
                CreateCachedEditor(settings, null, ref editor);
                editor.OnInspectorGUI();

                if (check.changed && mapGen.autoUpdate)
                {
                    mapGen.GenerateMap ();
                }
            }
        }
    }

    private void OnEnable()
    {
        mapGen = (MapGenerator)target;
    }
}
