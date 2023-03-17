
using App.Services;
using UnityEngine;

namespace App.Core
{
    public static class Constants
    {
        public const string AppConfigFileName = nameof(AppConfig);
        public const string ConfigsResourcesDirectoryPath = "Assets/App/Resources/Configs";
        public const string AppConfigResourcesFilePath = "Configs/" + AppConfigFileName;
        public const string ModelGameObjectName = "Downloaded Model";
    }
}