using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDI.Objectives
{
    public class KillObjective : Objective.ObjectiveGoal
    {
        public string target;
        public override string GetDescription()
        {
            return $"Find and Kill the {target}";
        }
        public override void Initialize()
        {
            base.Initialize();
            EventManager.Instance.AddListener<KillGameEvent>(OnKill);
        }
        private void OnKill(KillGameEvent eventInfo)
        {
            if(eventInfo.targetName == target)
            {
                currentAmount++;
                Evaluate();
            }
        }
    }
}