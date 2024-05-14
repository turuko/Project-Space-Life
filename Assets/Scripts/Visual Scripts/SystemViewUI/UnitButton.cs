using System.Collections;
using System.Collections.Generic;
using Controller;
using Model.Factions;
using Model.Universe;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI NameLabel;
    [SerializeField] private TextMeshProUGUI CostLabel;
    [SerializeField] private Button RecruitButton;


    public void Init(Settlement settlement, Unit unit)
    {
        NameLabel.text = unit.Name;
        CostLabel.text = unit.RecruitmentCost.ToString();
        
        RecruitButton.onClick.AddListener(() =>
        {
            if (!(GameManager.Instance.GetPlayerParty().Gold >= unit.RecruitmentCost)) return;
            settlement.Recruit(unit, GameManager.Instance.GetPlayerCharacter());
            GameManager.Instance.GetPlayerParty().AddUnit(unit.Id);
            Debug.Log("Gold before recruit: " + GameManager.Instance.GetPlayerParty().Gold);
            GameManager.Instance.GetPlayerParty().Gold -= unit.RecruitmentCost;
            Debug.Log("Gold after recruit: " + GameManager.Instance.GetPlayerParty().Gold);
            Destroy(gameObject);
        });
    }
    
}
