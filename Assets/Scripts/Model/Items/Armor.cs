using UnityEngine;
using UnityEngine.Serialization;

namespace Model.Items
{
    
    [CreateAssetMenu(fileName = "New Armor", menuName = "Custom/Armor")]
    public class Armor : Item
    {
        [FormerlySerializedAs("armorValue")] [SerializeField] private float laserArmorValue;
        [SerializeField] private float meleeArmorValue;
        [SerializeField] private float psychicResistance;

        public float LaserArmorValue
        {
            get => laserArmorValue;
            set => laserArmorValue = value;
        }

        public float MeleeArmorValue
        {
            get => meleeArmorValue;
            set => meleeArmorValue = value;
        }

        public float PsychicResistance
        {
            get => psychicResistance;
            set => psychicResistance = value;
        }
    }
}