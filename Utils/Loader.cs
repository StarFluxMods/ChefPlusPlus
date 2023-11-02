using System;
using System.Collections.Generic;
using System.IO;
using ChefPlusPlus.Enums;
using ChefPlusPlus.Interfaces;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ChefPlusPlus.Utils
{
    public class Loader
    {
        internal static readonly Dictionary<string, Type> effects = new();
        internal static readonly Dictionary<EffectTrigger, ITrigger> triggers = new();
        internal static string StoragePath = Path.Combine(Application.persistentDataPath, "UserData", "ChefPlusPlus");
        internal static Dictionary<EffectTrigger, List<IEffect>> loadedEffects = new();
        internal static Dictionary<EffectTrigger, List<string>> registeredConfigs = new();
        internal static List<string> registeredScopes = new();

        public static void RegisterScope(string scope)
        {
            if (!registeredScopes.Contains(scope))
                registeredScopes.Add(scope);
        }

        public static void RegisterTrigger<T>() where T : ITrigger, new()
        {
            T trigger = new();
            triggers.Add(trigger.trigger, trigger);
        }

        public static ITrigger GetTrigger(EffectTrigger type)
        {
            if (triggers.ContainsKey(type))
                return triggers[type];
            return null;
        }

        public static void RegisterEffect<T>() where T : IEffect
        {
            effects.Add(typeof(T).FullName, typeof(T));
        }

        public static void AddEffectToTrigger(EffectTrigger trigger, IEffect effect)
        {
            if (loadedEffects.ContainsKey(trigger))
                loadedEffects[trigger].Add(effect);
            else
                loadedEffects.Add(trigger, new List<IEffect> { effect });
        }

        public static List<IEffect> GetEffectsFromTrigger(EffectTrigger trigger)
        {
            if (loadedEffects.ContainsKey(trigger))
                return loadedEffects[trigger];
            return new List<IEffect>();
        }

        public static void RegisterConfig(EffectTrigger trigger, string file)
        {
            if (!Directory.Exists(StoragePath))
                Directory.CreateDirectory(StoragePath);

            if (!File.Exists(Path.Combine(StoragePath, file + ".json")))
                File.WriteAllText(Path.Combine(StoragePath, file + ".json"), "[]");

            if (registeredConfigs.ContainsKey(trigger))
                registeredConfigs[trigger].Add(file);
            else
                registeredConfigs.Add(trigger, new List<string> { file });
        }

        public static void LoadConfigs()
        {
            loadedEffects.Clear();
            foreach (EffectTrigger trigger in registeredConfigs.Keys)
            foreach (string file in registeredConfigs[trigger])
                LoadJson(file, trigger);
        }

        private static bool OpenFile(string path, out string content)
        {
            if (File.Exists(path))
            {
                content = File.ReadAllText(path);
                return true;
            }

            content = "";
            return false;
        }

        private static bool LoadJson(string file, EffectTrigger trigger)
        {
            if (!Directory.Exists(StoragePath))
                Directory.CreateDirectory(StoragePath);

            if (!File.Exists(Path.Combine(StoragePath, file + ".json")))
                File.WriteAllText(Path.Combine(StoragePath, file + ".json"), "[ ]");

            if (OpenFile(Path.Combine(StoragePath, file + ".json"), out string content))
                try
                {
                    JArray jarray = JArray.Parse(content);
                    foreach (JObject jobject in jarray)
                        if (jobject.TryGetValue("Type", out JToken token2))
                        {
                            string effect = token2.ToObject<string>();
                            if (jobject.TryGetValue("Example", out JToken token))
                            {
                                string example = token.ToObject<string>();
                                if (!string.IsNullOrEmpty(example))
                                {
                                    Mod.LogWarning($"Skipping effect in {effect}, example tag set. Did you forget to remove it?");
                                    continue;
                                }
                            }

                            object nEffect = jobject.ToObject(effects[effect]);
                            AddEffectToTrigger(trigger, (IEffect)nEffect);
                        }

                    return true;
                }
                catch
                {
                    Mod.LogWarning($"Failed to load effects from {file}, are you missing an addon?");
                }
            else
                Mod.LogWarning($"Failed to load effects from {file}, File missing.");

            return false;
        }
    }
}