using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankBattle.InputManagers
{
    public static class InputSystemExtensions
    {
        public static string SaveOverridesToJSON(this InputActionAsset inputActionAsset)
        {
            BindingOverrideListJson list = new BindingOverrideListJson();
            foreach (InputActionMap inputActionMap in inputActionAsset.actionMaps)
            {
                foreach (InputBinding inputBinding in inputActionMap.bindings)
                {
                    if (!string.IsNullOrEmpty(inputBinding.overridePath))
                    {
                        BindingOverrideJson @override = BindingOverrideJson.FromBinding(inputBinding);
                        list.bindings.Add(@override);
                    }
                }
            }

            return JsonUtility.ToJson(list);
        }

        public static void LoadOverridesFromJSON(this InputActionAsset inputActionAsset, string json)
        {
            BindingOverrideListJson list = JsonUtility.FromJson<BindingOverrideListJson>(json);
            
            Dictionary<Guid, BindingOverrideJson> overrides = new Dictionary<Guid, BindingOverrideJson>();
            foreach (BindingOverrideJson bindingOverrideJson in list.bindings)
            {
                overrides.Add(new Guid(bindingOverrideJson.id), bindingOverrideJson);
            }

            foreach (InputActionMap inputActionMap in inputActionAsset.actionMaps)
            {
                var bindings = inputActionMap.bindings;
                for(int i=0; i < bindings.Count; i++)
                {
                    if (overrides.TryGetValue(bindings[i].id, out BindingOverrideJson bindingOverrideJson))
                    {
                        inputActionMap.ApplyBindingOverride(i,
                            new InputBinding
                            {
                                overridePath = bindingOverrideJson.path
                            });
                    }
                }
            }
        }
        
        [Serializable]
        internal class BindingOverrideListJson
        {
            public List<BindingOverrideJson> bindings = new List<BindingOverrideJson>();
        }

        [Serializable]
        internal struct BindingOverrideJson
        {
            // We save both the "map/action" path of the action as well as the binding ID.
            // This gives us two avenues into finding our target binding to apply the override
            // to.
            public string action;
            public string id;
            public string path;
            public string interactions;
            public string processors;

            public static BindingOverrideJson FromBinding(InputBinding binding)
            {
                return new BindingOverrideJson
                {
                    action = binding.action,
                    id = binding.id.ToString(),
                    path = binding.overridePath,
                    interactions = binding.overrideInteractions,
                    processors = binding.overrideProcessors,
                };
            }
        }
    }
}