using System;

namespace Utility
{
    public class ConstrainedFloat
    {
        private float value;
        private float min;
        private float max;

        public float Value
        {
            get => value;
            set => this.value = Math.Max(Math.Min(value, max), min);
        }

        public float Min
        {
            get => min;
            set => min = value;
        }

        public float Max
        {
            get => max;
            set => max = value;
        }

        public ConstrainedFloat(float initValue, float min, float max)
        {
            this.min = min;
            this.max = max;
            Value = initValue;
        }
    }
}