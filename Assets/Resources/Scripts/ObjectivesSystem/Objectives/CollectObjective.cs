using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDI.Objectives
{
    public class CollectObjective : Objective.ObjectiveGoal
    {
        public string collectible;
        public override string GetDescription()
        {
            return $"Collect a {collectible}";
        }
        public override void Initialize()
        {
            base.Initialize();
            EventManager.Instance.AddListener<CollectibleGameEvent>(OnCollect);
        }
        private void OnCollect(CollectibleGameEvent eventInfo)
        {
            if (eventInfo.collectibleName == collectible)
            {
                currentAmount++;
                Evaluate();
            }
        }
    }
}   