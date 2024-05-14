using System.Collections.Generic;

namespace Utility
{
    public class InstanceTracker<T> where T : class
    {
        private static List<T> instances = new List<T>();

        public static void RegisterInstance(T instance)
        {
            if (!instances.Contains(instance))
            {
                instances.Add(instance);
            }
        }

        public static void UnregisterInstance(T instance)
        {
            instances.Remove(instance);
        }

        public static List<T> GetAllInstances()
        {
            return new List<T>(instances);
        }
    }
}