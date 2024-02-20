using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

[BepInPlugin("dev.mooskyfish.modlist", "Mod List", "1.1")]
[BepInProcess("Bug Fables.exe")]
public class ModList : BaseUnityPlugin
{
    public void Awake()
    {
        var harmony = new Harmony("dev.mooskyfish.modlist");
        harmony.PatchAll();
    }

    [HarmonyPatch(typeof(StartMenu), "SetMenuText")]
    static class StartMenuVersion
    {
        static void Postfix(StartMenu __instance)
        {
            var y = -3.2473f;
            var menu1 = (Transform)AccessTools.Field(typeof(StartMenu), "menu1").GetValue(__instance);
            if (menu1.transform.Find("Text: |size,0.45||halfline||color,4||font,0|v1.1.2"))
            {
                Destroy(menu1.transform.Find("Text: |size,0.45||halfline||color,4||font,0|v1.1.2").gameObject);
            }
            MainManager.instance.StartCoroutine(MainManager.SetText("|size,0.45||halfline||color,4||font,0|v" + Application.version + $" - BepInEx v{typeof(Paths).Assembly.GetName().Version}", new Vector3(-8.75f, -3.55f, 10f), menu1));
            foreach (var mod in BepInEx.Bootstrap.Chainloader.PluginInfos.Values)
            {
                if (mod.Metadata.Name == "Mod List")
                    continue;
                var Name = mod.Metadata.Name;
                string Version;
                var ModType = mod.Instance.GetType();
                if (ModType.GetField("Version") != null)
                    Version = (string)AccessTools.Field(ModType, "Version").GetValue(ModType);
                else
                    Version = mod.Metadata.Version.ToString();
                MainManager.instance.StartCoroutine(MainManager.SetText($"|size,0.45||halfline||color,4||font,0|{Name} v{Version}", new Vector3(-8.75f, y, 10f), menu1));
                y += 0.28f;
            }
        }
    }
}
