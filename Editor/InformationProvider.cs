using System;
using System.IO;
using System.Linq;
using System.Reflection;

internal static class InformationProvider
{
    public static string ClassName(string path)
    {
        return Path.GetFileNameWithoutExtension(path).Replace(" ", string.Empty);
    }

    public static string Namespace(string path)
    {
        var nameSpace = "namespace ";
        var folders = path.Replace(".cs", string.Empty).Split('/');

        var scriptsFolder = folders.FirstOrDefault(_ => _.ToLower().Contains("script"));
        var index = Array.IndexOf(folders, scriptsFolder) + 1;

        if (index < 0 || index >= folders.Length - 1) return "namespace Scripts {";

        for (; index < folders.Length - 1; index++)
        {
            nameSpace += folders[index];
            nameSpace += index < folders.Length - 2 ? "." : "{";
        }

        return nameSpace;
    }


    public static string UserName()
    {
        const string UNITY_CONNECT_ASMDEF = "UnityEditor.Connect.UnityConnect";
        try
        {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.EditorWindow));
            var unityConnectAssembly = assembly.CreateInstance(UNITY_CONNECT_ASMDEF, false,
                BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);
            var type = unityConnectAssembly?.GetType();

            // Get user info object from UnityConnect.
            var userInfo = type?.GetProperty("userInfo")?.GetValue(unityConnectAssembly, null);

            // Retrieve user id from user info.
            var userInfoType = userInfo?.GetType();
            return userInfoType.GetProperty("displayName").GetValue(userInfo, null) as string;
        }
        catch
        {
            return Environment.UserName;
        }
    }
}