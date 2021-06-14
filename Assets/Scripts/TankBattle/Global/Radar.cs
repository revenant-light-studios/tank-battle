using System.Collections.Generic;
using UnityEngine;

namespace TankBattle.Global
{
    public class Radar
    {
        public delegate void OnDetectableObjectAddedDelegate(DetectableObject obj);
        public OnDetectableObjectAddedDelegate OnDetectableObjectAdded;

        public delegate void OnDetectableObjectRemovedDelegate(DetectableObject obj);
        public OnDetectableObjectAddedDelegate OnDetectableObjectRemoved;

        private static Radar _instance;

        public static Radar Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Radar();
                }
                
                return _instance;
            }
        }
        
        private Radar() {}

        public readonly List<DetectableObject> DetectableObjects = new List<DetectableObject>();
        
        public void AddDetectableObject(DetectableObject _detectableObject)
        {
            lock (DetectableObjects)
            {
                DetectableObjects.Add(_detectableObject);
                // Debug.LogFormat($"Detectable {_detectableObject.name} added to DetectableObjects");
            }
            
            OnDetectableObjectAdded?.Invoke(_detectableObject);
        }

        public void RemoveDetectableObject(DetectableObject _detectableObject)
        {
            lock (DetectableObjects)
            {
                DetectableObjects.Remove(_detectableObject);    
                // Debug.LogFormat($"Detectable {_detectableObject.name} removed from DetectableObjects");
            }
            OnDetectableObjectRemoved?.Invoke(_detectableObject);
        }
    }
}