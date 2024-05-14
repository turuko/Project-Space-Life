using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Model
{
    public class StandardRotationController: IDataPersistence
    {
        //Time in seconds for one rotation to pass
        private float standardRotationLength;

        private float timeSinceLastRotation;

        //Amount of rotations that have gone by since the start of the game
        private int rotationsPassed;

        private Action<StandardRotationController> RotationsChangedCb;

        private Dictionary<object, List<int>> events;

        public StandardRotationController()
        {
            rotationsPassed = 0;
            standardRotationLength = 15f;
            timeSinceLastRotation = standardRotationLength;
            InstanceTracker<IDataPersistence>.RegisterInstance(this);
            events = new Dictionary<object, List<int>>();
        }

        ~StandardRotationController()
        {
            InstanceTracker<IDataPersistence>.UnregisterInstance(this);
        }

        public int LogEvent(object source)
        {
            if (!events.ContainsKey(source))
            {
                events.Add(source, new List<int>());
            }

            var sourceEvents = events[source];
            sourceEvents.Add(rotationsPassed);
            Debug.Log("StandardRotationController::LogEvent :" + rotationsPassed);
            return sourceEvents.Count - 1;
        }

        public int TimeSinceEvent(object source, int eventIndex)
        {
            if (events.ContainsKey(source))
            {
                var sourceEvents = events[source];
                if (eventIndex >= 0 && eventIndex < sourceEvents.Count)
                {
                    return rotationsPassed - sourceEvents[eventIndex];
                }
            }

            return -1;
        }
        
        public void UpdateTimeSinceRotation(float deltaTime)
        {
            if (timeSinceLastRotation <= 0)
            {
                timeSinceLastRotation = standardRotationLength;
                rotationsPassed++;
                RotationsChangedCb(this);
            }

            timeSinceLastRotation -= deltaTime;
        }

        public void RegisterChangedCB(Action<StandardRotationController> cb)
        {
            RotationsChangedCb += cb;
        }
        
        public void UnregisterChangedCB(Action<StandardRotationController> cb)
        {
            RotationsChangedCb -= cb;
        }

        public int GetRotations()
        {
            return rotationsPassed;
        }

        public float GetStandardRotationLength()
        {
            return standardRotationLength;
        }
        
        
        public float GetTimeSinceLastRotation()
        {
            return timeSinceLastRotation;
        }

        public void LoadData(GameData data)
        {
            timeSinceLastRotation = data.rotationData.Value.timeSinceLastRotation;
            rotationsPassed = data.rotationData.Value.rotationsPassed;
        }

        public void SaveData(GameData data)
        {
            var rotationData = new GameData.RotationData
            {
                rotationsPassed = rotationsPassed,
                timeSinceLastRotation = timeSinceLastRotation
            };

            data.rotationData = rotationData;
        }
    }
}