using SDI.Enums;
using SDI.Objectives;
using SDI.Players;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace SDI.Managers
{
    public class ObjectiveManager : NetworkBehaviour
    {
        public static ObjectiveManager Instance;
        [SerializeField]
        private GameObject objectiveSingleUI;
        [SerializeField]
        private Transform container;
        [SerializeField]
        private List<Objective> guardObjectives = new List<Objective>();
        [SerializeField]
        private List<Objective> thiefObjectives = new List<Objective>();

        private List<Objective> currentObjectives = new List<Objective>();

        private void Awake()
        {
            Instance = this; 
        }
        public void AssignCurrentObjectives(Roles roles)
        {
            if(roles == Roles.GUARD)
            {
                currentObjectives.AddRange(guardObjectives);
            }
            else
            {
                currentObjectives.AddRange(thiefObjectives);
            }
        }
        public void InitializeObjectives()
        {

            foreach (var objective in currentObjectives)
            {
                objective.Initialize();
                objective.objectiveCompleted.AddListener(OnObjectiveCompleted);
                objective.objectiveUpdated.AddListener(OnObjectiveUpdated);
                GameObject go = Instantiate(objectiveSingleUI, container);
                go.GetComponent<ObjectiveWindowUI>().Initialize(objective.objectives[0]);
            }
        }
        public void Collect(string collectName)
        {
            EventManager.Instance.QueueEvent(new CollectibleGameEvent(collectName));
        }
        public void Switch(string switchName)
        {
            EventManager.Instance.QueueEvent(new SwitchGameEvent(switchName));
        }
        public void Trigger(string triggerName, bool hasKey)
        {
            EventManager.Instance.QueueEvent(new TriggerGameEvent(triggerName));
        }
        public void Kill(string target)
        {
            EventManager.Instance.QueueEvent(new KillGameEvent(target));
        }
        private bool CheckIfAllCompleted()
        {
            for (int i = 0; i < currentObjectives.Count; i++)
            {
                if (!currentObjectives[i].Completed)
                {
                    return false;
                }
            }
            return true;
        }
        private void OnObjectiveCompleted(Objective obj)
        {
            Debug.Log("Objective Complete");
            container.GetChild(currentObjectives.IndexOf(obj)).GetComponent<ObjectiveWindowUI>().Check();
            if (CheckIfAllCompleted())
            {
                ObjectiveCompleteServerRpc();
                ShowWinCardClientRpc();
            }
        }
        [ServerRpc(RequireOwnership = false)]
        private void ObjectiveCompleteServerRpc()
        {
            if (NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<PlayerRole>().roles.Value == Roles.GUARD)
            {
                GameManager.Instance.isGameFinished.Value = 1;
            }
            else
            {
                GameManager.Instance.isGameFinished.Value = 2;
            }
        }
        [ClientRpc]
        private void ShowWinCardClientRpc()
        {
            GameManager.Instance.ShowWinCard(1);
        }
        private void OnObjectiveUpdated(Objective obj)
        {
            container.GetChild(currentObjectives.IndexOf(obj)).GetComponent<ObjectiveWindowUI>().UpdateTitleText(obj.objectives[0]);
        }
    }
}