using UnityEngine;
using BepInEx;
using HarmonyLib;
using BepInEx.Configuration;
using System.Linq;

[BepInPlugin("dev.mooskyfish.modlist", "Mod List", "1.2.1")]
[BepInProcess("Bug Fables.exe")]
public class ModList : BaseUnityPlugin
{
    public static ConfigEntry<bool> ShowModsCount;
    public void Awake()
    {
        var harmony = new Harmony("dev.mooskyfish.modlist");
        ShowModsCount = Config.Bind("Config", "Show Mods Count", true, "");
        harmony.PatchAll();
    }

    [HarmonyPatch(typeof(StartMenu), "SetMenuText")]
    static class StartMenuVersion
    {
        static void Postfix(StartMenu __instance)
        {
            var y = -3.3f;
            var menu1 = (Transform)AccessTools.Field(typeof(StartMenu), "menu1").GetValue(__instance);
            if (menu1.transform.Find($"Text: |size,0.45||halfline||color,4||font,0|v{Application.version}"))
                Destroy(menu1.transform.Find($"Text: |size,0.45||halfline||color,4||font,0|v{Application.version}").gameObject);
            MainManager.instance.StartCoroutine(MainManager.SetText("|size,0.45||halfline||color,4||font,0|v" + Application.version + $" - BepInEx v{typeof(Paths).Assembly.GetName().Version}", new Vector3(-8.75f, -3.55f, 10f), menu1));
            if (ShowModsCount.Value)
            {
                var count = BepInEx.Bootstrap.Chainloader.PluginInfos.Values.Count(mod => mod.Instance.enabled);
                MainManager.instance.StartCoroutine(MainManager.SetText($"|single||size,0.45||halfline||color,4||font,0|Mods Loaded: {count}", new Vector3(-8.75f, y, 10f), menu1));
                return;
            }
            foreach (var mod in BepInEx.Bootstrap.Chainloader.PluginInfos.Values)
            {
                if (mod.Metadata.Name == "Mod List" || !mod.Instance.enabled)
                    continue;
                var name = mod.Metadata.Name;
                string version;
                var modType = mod.Instance.GetType();
                if (modType.GetField("Version") != null)
                    version = (string)AccessTools.Field(modType, "Version").GetValue(modType);
                else
                    version = mod.Metadata.Version.ToString();
                MainManager.instance.StartCoroutine(MainManager.SetText($"|single||size,0.45||halfline||color,4||font,0|{name} v{version}", new Vector3(-8.75f, y, 10f), menu1));
                y += 0.35f;
            }
        }
    }
}
