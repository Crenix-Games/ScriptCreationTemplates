using System.IO;
using UnityEditor;
using UnityEngine;

internal static class UpmHelper
{
    internal static string GetTemplatesPath(string packageId)
    {
        var path = $"Packages/{packageId}/Editor/Templates";
        if (!Directory.Exists(path))
        {
            path = $"Assets/UPM/Editor/Templates";
        }

        return path;
    }
}
