using System;
using Model.Factions;
using UnityEngine;
using UnityEngine.UI;

namespace Visual_Scripts.Factions
{
    public class BannerVisual : MonoBehaviour
    {
        public Banner Banner;

        public GameObject Background, Icon;

        public bool setBanner = false;

        private void OnEnable()
        {
            if (Banner == null)
            {
                return;
            }
            var backgroundImage = Background.GetComponent<Image>();
            backgroundImage.sprite = Banner.Shape;
            backgroundImage.color = Banner.SecondaryColor;

            var iconImage = Icon.GetComponent<Image>();
            iconImage.sprite = Banner.Icon;
            iconImage.color = Banner.PrimaryColor;
        }

        private void Update()
        {
            if (setBanner)
            {
                var backgroundImage = Background.GetComponent<Image>();
                backgroundImage.sprite = Banner.Shape;
                backgroundImage.color = Banner.SecondaryColor;

                var iconImage = Icon.GetComponent<Image>();
                iconImage.sprite = Banner.Icon;
                iconImage.color = Banner.PrimaryColor;
                setBanner = false;
            }
        }
    }
}