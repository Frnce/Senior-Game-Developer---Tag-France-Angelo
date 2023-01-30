using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDI.Objectives
{
    public class TriggerObjective : Objective.ObjectiveGoal
    {
        public string target;

        public override string GetDescription()
        {
            return $"Look for the way to {target}";
        }
        public override void Initialize()
        {
            base.Initialize();
            EventManager.Instance.AddListener<TriggerGameEvent>(OnKill);
        }
        private void OnKill(TriggerGameEvent eventInfo)
        {
            if (eventInfo.triggerName == target)
            {
                currentAmount++;
                Evaluate();
            }
        }
    }
}