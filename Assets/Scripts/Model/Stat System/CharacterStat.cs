using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace Model.Stat_System
{
    [Serializable]
    public class CharacterStat
    {
        public CharacterStat()
        {
            statModifiers = new List<StatModifier>();
            StatModifiers = statModifiers.AsReadOnly();
        }
        
        public CharacterStat(float baseValue) : this()
        {
            BaseValue = baseValue;
        }

        protected float lastBaseValue = float.MinValue;
        public float BaseValue;

        protected readonly List<StatModifier> statModifiers;
        public readonly ReadOnlyCollection<StatModifier> StatModifiers;

        protected bool isDirty = true;
        protected float _value;
        
        public virtual float Value
        {
            get
            {
                if (isDirty || lastBaseValue != BaseValue)
                {
                    lastBaseValue = BaseValue;
                    _value = CalculateFinalValue();
                    isDirty = false;
                }

                return _value;
            }
        }
        
        public virtual void AddModifier(StatModifier mod)
        {
            isDirty = true;
            statModifiers.Add(mod);
            statModifiers.Sort(CompareModifierOrder);
        }

        protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
        {
            if (a.Order < b.Order)
                return -1;
            else if (a.Order > b.Order)
                return 1;
            return 0;
        }

        public virtual bool RemoveModifier(StatModifier mod)
        {
            if (statModifiers.Remove(mod))
            {
                isDirty = true;
                return true;
            }
            return false;
        }

        public virtual bool RemoveAllModifiersFromSource(object source)
        {
            bool didRemove = false;

            for (int i = statModifiers.Count - 1; i >= 0; i--)
            {
                if (statModifiers[i].Source == source)
                {
                    isDirty = true;
                    didRemove = true;
                    statModifiers.RemoveAt(i);
                }
            }

            return didRemove;
        }

        protected virtual float CalculateFinalValue()
        {
            float finalValue = BaseValue;
            float sumPercentAdd = 0f;

            for (int i = 0; i < statModifiers.Count; i++)
            {
                var mod = statModifiers[i];
                switch (mod.Type)
                {
                    case StatModType.Flat:
                        finalValue += mod.Value;
                        break;
                    case StatModType.PercentMult:
                        sumPercentAdd += mod.Value;

                        if (i + 1 >= statModifiers.Count || statModifiers[i + 1].Type != StatModType.PercentAdd)
                        {
                            finalValue *= 1 + sumPercentAdd;
                            sumPercentAdd = 0f;
                        }
                        break;
                    case StatModType.PercentAdd:
                        finalValue *= 1 + mod.Value;
                        break;
                }
            }

            return (float)Math.Round(finalValue, 4);
        }
    }
}