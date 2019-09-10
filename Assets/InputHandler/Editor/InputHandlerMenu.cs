using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class InputHandlerMenu : EditorWindow
{
    static EditorWindow window;
    static int NumberOfGamepadssToAdd = 10;
    [MenuItem("Input Handler/Setup Input Manager")]
    public static void SetupInputManager()
    {
        window = GetWindow<InputHandlerMenu>("Confirmation");
        window.maxSize = new Vector2(600, 200);
        window.minSize = new Vector2(600, 200);
    }


    private void OnGUI()
    {
        GUI.skin.label.wordWrap = true;
        GUILayout.Label("This will populate the Input Manager with 20 entries per gamepad supported, do you wish to continue?");
        GUILayout.Space(10);

        bool Recov = true;
        Recov = GUILayout.Toggle(Recov, "Create backup of old InputManager before making new?");
        GUILayout.Space(10);

        GUILayout.Label("How many gamepads do you want to support? Note that if you have more gamepads connected, than you support, you may run into issues, so recommended would be around 10 or so.");
        NumberOfGamepadssToAdd = EditorGUILayout.IntSlider(NumberOfGamepadssToAdd, 1, 16);
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
        StringBuilder sb = new StringBuilder();
        for (int i = 1; i <= NumberOfGamepadssToAdd; i++)
        {
            sb.Append("\n");
            for (int x = 0; x < 20; x++)
            {
                sb.Append(string.Format(
                    @"  - serializedVersion: 3
    m_Name: joystick {0} analog {1}
    descriptiveName:
    descriptiveNegativeName: 
    negativeButton:
    positiveButton: 
    altNegativeButton:
    altPositiveButton: 
    gravity: 0
    dead: 0.001
    sensitivity: 1
    snap: 0
    invert: 0
    type: 2
    axis: {1}
    joyNum: {0} ", i, x));
                sb.Append("\n");
            }
        }

        File.AppendAllText("ProjectSettings/InputManager.asset", sb.ToString());

        AssetDatabase.Refresh();
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
