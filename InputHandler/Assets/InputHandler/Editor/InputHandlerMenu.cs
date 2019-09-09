using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class InputHandlerMenu : EditorWindow
{
    static EditorWindow window;

    [MenuItem("Input Handler/Setup Input Manager")]
    public static void SetupInputManager()
    {
        window = GetWindow<InputHandlerMenu>("Confirmation");
    }


    private void OnGUI()
    {
        GUILayout.Label("This will populate the Input Manager with around 200 entries, do you wish to continue?");
        GUILayout.Space(10);

        bool Recov = true;
        Recov = GUILayout.Toggle(Recov, "Create backup of old InputManager before making new?");
        if (GUILayout.Button("Yes"))
        {
            if (Recov)
                this.SaveCopyOfInputManager();
            this.FillManagerWithJoysticks();
        }
        GUILayout.Space(10);
        if (GUILayout.Button("No"))
        {
            window.Close();
        }
    }

    private void SaveCopyOfInputManager()
    {
        var manager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0];

        string finalName = this.GetUniqueName("InputManagerBackup", Application.dataPath + "/", ".txt");
        File.Copy("ProjectSettings/InputManager.asset", Application.dataPath + "/" + finalName);
    }

    private string GetUniqueName(string name, string folderPath, string extension)
    {
        string validatedName = name + extension;
        int number = 1;
        while (File.Exists(folderPath + validatedName))
        {
            validatedName = string.Format("{0} [{1}]" + extension, name, number++);
        }
        return validatedName;
    }

    private void FillManagerWithJoysticks()
    {
        File.AppendAllText("ProjectSettings/InputManager.asset", InputManagerEntries.Entries);
    }

    [MenuItem("Input Handler/Setup Scene")]
    public static void Setup()
    {

        GameObject existingHandler = GameObject.Find("InputHandler");
        if (existingHandler == null)
        {
            existingHandler = new GameObject();
            existingHandler.name = "InputHandler";
            existingHandler.AddComponent<InputHandler>();
        }

        try
        {
            Input.GetKeyDown("joystick 1 button 1");
            Input.GetAxisRaw("joystick 1 analog 1");
        }
        catch
        {
            Debug.Log("Could not read button 1 on joystick 1, have you run setup of Input Manager?");
        }

    }

    [MenuItem("Input Handler/Export To Package")]
    public static void CreatePackage()
    {
        AssetDatabase.ExportPackage("Assets/InputHandler", "InputHandlerPackage.unitypackage", ExportPackageOptions.Recurse);
    }
}
