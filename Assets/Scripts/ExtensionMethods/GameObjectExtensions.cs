using System.Collections.Generic;
using UnityEngine;

namespace ExtensionMethods
{
    public static class GameObjectExtensions
    {
        public static void SetLayerRecursively(this GameObject obj, int layer) {
            obj.layer = layer;
 
            foreach (Transform child in obj.transform) {
                child.gameObject.SetLayerRecursively(layer);
            }
        }

        public static Collider[] GetAllColliders(this GameObject obj)
        {
            return obj.GetComponentsInChildren<Collider>();
        }

    }
}