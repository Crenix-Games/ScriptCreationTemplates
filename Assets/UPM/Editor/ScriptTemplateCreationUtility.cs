using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;
using System.IO;
using System.Text;

public class ScriptTemplateCreationUtility {
    private static Texture2D scriptIcon = (EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D);

    [MenuItem("Assets/Create/Crenix/Class", false, 80)]
    private static void CreateClassBehaviour() {
        string template = "Crenix_Class.cs";
        CreateTemplate(template);
    }
    
    [MenuItem("Assets/Create/Crenix/Mono Behaviour", false, 80)]
    private static void CreateMonoBehaviour() {
        string template = "Crenix_MonoBehaviour.cs";
        CreateTemplate(template);
    }

    [MenuItem("Assets/Create/Crenix/Singleton", false, 80)]
    private static void CreateSingleton() {
        string template = "Crenix_Singleton.cs";
        CreateTemplate(template);
    }

    [MenuItem("Assets/Create/Crenix/Scriptable Object", false, 80)]
    private static void CreateScriptableObject() {
        string template = "Crenix_ScriptableObject.cs";
        CreateTemplate(template);
    }
    private static void CreateTemplate(string template) {
        var path = UpmHelper.GetTemplatesPath("com.crenixgames.scriptcreationtemplate") + "/" + template+".txt";
        CreateFromTemplate(
            template,
            path
        );
    }
    
    

    public static void CreateFromTemplate(string initialName, string templatePath) {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
            0,
            ScriptableObject.CreateInstance<DoCreateCodeFile>(),
            initialName,
            scriptIcon,
            templatePath
        );
    }
    /// Inherits from EndNameAction, must override EndNameAction.Action
    public class DoCreateCodeFile : UnityEditor.ProjectWindowCallback.EndNameEditAction {
        public override void Action(int instanceId, string pathName, string resourceFile) {
            Object o = CreateScript(pathName, resourceFile);
            ProjectWindowUtil.ShowCreatedAsset(o);
        }
    }

    /// <summary>Creates Script from Template's path.</summary>
    internal static UnityEngine.Object CreateScript(string pathName, string templatePath) {
        string className = Path.GetFileNameWithoutExtension(pathName).Replace(" ", string.Empty);
        string templateText = string.Empty;

        UTF8Encoding encoding = new UTF8Encoding(true, false);

        if (File.Exists(templatePath)) {
            /// Read procedures.
            StreamReader reader = new StreamReader(templatePath);
            templateText = reader.ReadToEnd();
            reader.Close();

            templateText = templateText.Replace("#SCRIPTNAME#", className);
            templateText = templateText.Replace("#NOTRIM#", string.Empty);
            /// You can replace as many tags you make on your templates, just repeat Replace function
            /// e.g.:
            /// templateText = templateText.Replace("#NEWTAG#", "MyText");

            /// Write procedures.

            StreamWriter writer = new StreamWriter(Path.GetFullPath(pathName), false, encoding);
            writer.Write(templateText);
            writer.Close();

            AssetDatabase.ImportAsset(pathName);
            return AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
        } else {
            Debug.LogError(string.Format("The template file was not found: {0}", templatePath));
            return null;
        }
    }
}
