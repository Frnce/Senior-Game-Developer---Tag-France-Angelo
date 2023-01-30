using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDI.Objectives
{
    public class SwitchObjective : Objective.ObjectiveGoal
    {
        public string switches;
        public override string GetDescription()
        {
            return $"Open {currentAmount}/{requiredAmount} Hidden {switches}";
        }
        public override void Initialize()
        {
            base.Initialize();
            EventManager.Instance.AddListener<SwitchGameEvent>(OnSwitch);
        }
        private void OnSwitch(SwitchGameEvent eventInfo)
        {
            if(eventInfo.switchName == switches)
            {
                currentAmount++;
                Evaluate();
            }
        }
    }
}
