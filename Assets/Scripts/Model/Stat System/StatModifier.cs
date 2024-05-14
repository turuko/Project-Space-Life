using System;

namespace Model.Stat_System
{

    public enum StatModType
    {
        None = 0,
        Flat = 100,
        PercentAdd = 200,
        PercentMult = 300
    }
    
    [Serializable]
    public class StatModifier
    {
        public float Value;

        private StatModType type;
        public StatModType Type
        {
            get => type;
            set
            {
                type = value;
                Order = (int)type;
            }
        }
        public int Order;
        public object Source;

        public StatModifier(float value, StatModType type, int order, object source)
        {
            Value = value;
            Type = type;
            Order = order;
            Source = source;
        }
        
        public StatModifier(float value, StatModType type) : this(value, type, (int)type, null) { }
        
        public StatModifier(float value, StatModType type, int order) : this(value, type, order, null) { }
        
        public StatModifier(float value, StatModType type, object source) : this(value, type, (int)type, source) { }
    }
}