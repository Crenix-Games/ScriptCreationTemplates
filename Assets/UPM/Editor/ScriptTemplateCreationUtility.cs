using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;
using System.IO;
using System.Text;

internal class ScriptTemplateCreationUtility
{
    private static readonly Texture2D ScriptIcon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;

    [MenuItem("Assets/Create/Script Template/Class", false, 80)]
    private static void CreateClassBehaviour() => CreateTemplate("MyClass.cs");

    [MenuItem("Assets/Create/Script Template/Mono Behaviour", false, 80)]
    private static void CreateMonoBehaviour() => CreateTemplate("MyMonoBehaviour.cs");

    [MenuItem("Assets/Create/Script Template/Scriptable Object", false, 80)]
    private static void CreateScriptableObject() => CreateTemplate("MyScriptableObject.cs");

    private static void CreateTemplate(string template)
    {
        var path = $"{UpmHelper.GetTemplatesPath("com.crenixgames.scriptcreationtemplate")}/{template}.txt";

        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
            0,
            ScriptableObject.CreateInstance<DoCreateCodeFile>(),
            template,
            ScriptIcon,
            path
        );
    }

    public class DoCreateCodeFile : UnityEditor.ProjectWindowCallback.EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            var o = CreateScript(pathName, resourceFile);
            ProjectWindowUtil.ShowCreatedAsset(o);
        }
    }

    private static Object CreateScript(string pathName, string templatePath)
    {
        if (File.Exists(templatePath))
        {
            var templateText = File.ReadAllText(templatePath);

            templateText = templateText.Replace("#NOTRIM#", string.Empty);

            templateText = templateText.Replace("#SCRIPTNAME#", InformationProvider.ClassName(pathName));
            templateText = templateText.Replace("#COMPANY#", Application.companyName);

            templateText = templateText.Replace("#DEVELOPERNAME#", InformationProvider.UserName());
            
            templateText = templateText.Replace("#FOLDERNAMESPACE#", InformationProvider.Namespace(pathName));
            templateText = templateText.Replace("#ENDNAMESPACE#", "}");

            templateText = templateText.Replace("#CREATIONDATE#", System.DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            templateText = templateText.Replace("#PROJECTNAME#", PlayerSettings.productName);
            
            var writer = new StreamWriter(Path.GetFullPath(pathName), false, new UTF8Encoding(true, false));
            writer.Write(templateText);
            writer.Close();

            AssetDatabase.ImportAsset(pathName);
            return AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
        }

        Debug.LogError($"The template file was not found: {templatePath}");
        return null;
    }
}