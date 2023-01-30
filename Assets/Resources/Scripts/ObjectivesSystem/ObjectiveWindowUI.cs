using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SDI.Objectives
{
    public class ObjectiveWindowUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI titleText;
        [SerializeField]
        private Toggle checkMark;

        public void Initialize(Objective.ObjectiveGoal obj)
        {
            titleText.text = obj.GetDescription();
            checkMark.isOn = false;
        }
        public void Check()
        {
            checkMark.isOn = true;
        }
        public void UpdateTitleText(Objective.ObjectiveGoal obj)
        {
            titleText.text = obj.GetDescription();
        }
    }
}