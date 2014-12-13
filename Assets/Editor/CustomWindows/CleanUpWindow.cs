using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class CleanUpWindow : EditorWindow
{
    bool groupEnabled = false;
    List<string> usedAssets = new List<string>();
    List<string> includedDependencies = new List<string>();
    private Vector2 scrollPos;
    private List<Object> unUsed;
    private Dictionary<string, List<Object>> unUsedArranged;
    private bool needToBuild = false;

    // Add menu named "CleanUpWindow" to the Window menu  
    [MenuItem("Window/CleanUpWindow")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:  
        CleanUpWindow window = (CleanUpWindow)EditorWindow.GetWindow(typeof(CleanUpWindow));
        window.Show();
    }

    void OnGUI()
    {
        if (needToBuild)
        {
            GUI.color = Color.red;
            GUILayout.Label("Are you sure you remembered to build project? Because you really need to...", EditorStyles.boldLabel);
        }

        if (!needToBuild)
        {
            GUI.color = Color.red;
            if (GUILayout.Button("Clear EditorLog - THIS DELETES THE BUILD LOG"))
            {
                
                clearEditorLog();
                needToBuild = true;
            }
            GUI.color = Color.white;
        }

        GUI.color = Color.white;
        if (GUILayout.Button("Load EditorLog"))
        {
            loadEditorLog();
        }

        if (!needToBuild)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            if (groupEnabled)
            {
                GUILayout.Label("DEPENDENCIES");
                for (int i = 0; i < includedDependencies.Count; i++)
                {
                    EditorGUILayout.LabelField(i.ToString(), includedDependencies[i]);
                }
            }
            EditorGUILayout.EndVertical();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.BeginVertical();

            if (groupEnabled)
            {
                if (unUsedArranged != null)
                {
                    foreach (KeyValuePair<string, List<Object>> objList in unUsedArranged)
                    {
                        if (objList.Value.Count >= 1)
                        {
                            GUILayout.Label(objList.Key.ToUpper());
                            for (int i = 0; i < objList.Value.Count; i++)
                            {
                                EditorGUILayout.ObjectField(objList.Value[i], typeof(Object),false);
                            }
                        }
                    }
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
        }

    }

    private void clearEditorLog()
    {
        
        string LocalAppData = string.Empty;
        string UnityEditorLogfile = string.Empty;

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            LocalAppData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            UnityEditorLogfile = LocalAppData + "\\Unity\\Editor\\Editor.log";
        }
        else if (Application.platform == RuntimePlatform.OSXEditor)
        {
            LocalAppData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            UnityEditorLogfile = LocalAppData + "/Library/Logs/Unity/Editor.log";
        }

        try
        {
            // Have to use FileStream to get around sharing violations!
            //System.IO.File.WriteAllText(UnityEditorLogfile, string.Empty);
            FileStream FS = new FileStream(UnityEditorLogfile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            //StreamReader SR = new StreamReader(FS);
            StreamWriter SW = new StreamWriter(FS);
            
            SW.Write(string.Empty);
            SW.Flush();
            SW.Close();
        }
        catch (System.Exception E)
        {
            Debug.LogError("Error: " + E);
        }
    }

    private void loadEditorLog()
    {
        UsedAssets.GetLists(ref usedAssets, ref includedDependencies);

        if (usedAssets.Count == 0)
        {
            needToBuild = true;
        }
        else
        {
            compareAssetList(UsedAssets.GetAllAssets());
            groupEnabled = true;
            needToBuild = false;
        }
    }

    private void compareAssetList(string[] assetList)
    {

        unUsed = new List<Object>();

        unUsedArranged = new Dictionary<string, List<Object>>();
        unUsedArranged.Add("plugins", new List<Object>());
        unUsedArranged.Add("editor", new List<Object>());
        unUsedArranged.Add("some other folder", new List<Object>());

        for (int i = 0; i < assetList.Length; i++)
        {
            if (!usedAssets.Contains(assetList[i]))
            {

                Object objToFind = AssetDatabase.LoadAssetAtPath(assetList[i], typeof(Object));
                unUsed.Add(objToFind);
                unUsedArranged[getArrangedPos(objToFind)].Add(objToFind);
            }
        }
    }

    private string getArrangedPos(Object value)
    {
        string path = AssetDatabase.GetAssetPath(value).ToLower();

        if (path.Contains("/plugins/"))
        {
            return "plugins";
        }
        else if (path.Contains("/editor/"))
        {
            return "editor";
        }
        else
        {
            return "some other folder";
        }
    }
}