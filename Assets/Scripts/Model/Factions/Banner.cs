using UnityEngine;

namespace Model.Factions
{
    [CreateAssetMenu(fileName = "New Banner", menuName = "Custom/Banner")]
    public class Banner : ScriptableObject
    {
        [SerializeField] private string id;

        public string Id
        {
            get => id;
            set => id = value;
        }
            
        [SerializeField] private Color primaryColor;
        public Color PrimaryColor => primaryColor;

        [SerializeField] private Color secondaryColor;
        public Color SecondaryColor => secondaryColor;

        [SerializeField] private Sprite shape;
        public Sprite Shape => shape;

        [SerializeField] private Sprite icon;
        public Sprite Icon => icon;
    }
}