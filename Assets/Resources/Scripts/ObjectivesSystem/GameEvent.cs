using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDI.Objectives
{
    public abstract class GameEvent
    {
        public string eventDescription;
    }
    public class KillGameEvent : GameEvent
    {
        public string targetName;
        public KillGameEvent(string targetName)
        {
            this.targetName = targetName;
        }
    }
    public class CollectibleGameEvent : GameEvent
    {
        public string collectibleName;
        public CollectibleGameEvent(string name)
        {
            collectibleName = name;
        }
    }
    public class SwitchGameEvent : GameEvent
    {
        public string switchName;
        public SwitchGameEvent(string switchName)
        {
            this.switchName = switchName;
        }
    }
    public class TriggerGameEvent: GameEvent
    {
        public string triggerName;
        public TriggerGameEvent(string triggerName)
        {
            this.triggerName = triggerName;
        }
    }
}
