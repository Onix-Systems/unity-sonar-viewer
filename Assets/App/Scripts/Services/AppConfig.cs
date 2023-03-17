
using UnityEngine;
using App.Networking;
using App.Services.ModelARViewing;
using App.UI;
using App.Core;
using App.Services.Input;

namespace App.Services
{
    public class AppConfig : ScriptableObject
    {
        [field: Header("General: ")]
        [field: SerializeField] public int TargetFrameRate { get; private set; } = 60;
        [field: SerializeField] public string ContactUsUrl { get; private set; } = "https://onix-systems.com/contact-us"; 

        [field: Space]
        [field: Header("Configs: ")]
        [field: SerializeField] public InputConfig InputConfig { get; private set; }
        [field: SerializeField] public ARViewerConfig ARViewerConfig { get; private set; }
        [field: SerializeField] public NetworkConfig NetworkConfig { get; private set; }
        [field: SerializeField] public UIConfig UIConfig { get; private set; }
        [field: SerializeField] public DefaultsConfig Defaults { get; private set; }
    }
}