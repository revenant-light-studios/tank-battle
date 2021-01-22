using System;
using UnityEngine;

namespace ExtensionMethods
{
    /**
     * Extracted from https://www.loekvandenouweland.com/content/unity-find-gameobject-in-hierarchy.html
     */
    public static class TransformExtensions
    {
        public static Transform FirstOrDefault(this Transform transform, Func<Transform, bool> query)
        {
            if (query(transform)) {
                return transform;
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                var result = FirstOrDefault(transform.GetChild(i), query);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public static void ClearChildren(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

    }
}