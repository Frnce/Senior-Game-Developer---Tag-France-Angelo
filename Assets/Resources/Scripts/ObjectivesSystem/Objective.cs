using SDI.Objectives;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.Events;

namespace SDI.Objectives
{
    [CreateAssetMenu(fileName = "Objective", menuName = "New Objective", order = 0)]
    public class Objective : ScriptableObject
    {
        [System.Serializable]
        public struct Info
        {
            public string name;
            public string description;
        }

        [Header("Info")] public Info information;

        public bool Completed { get; protected set; }
        public ObjectiveCompletedEvent objectiveCompleted;
        public ObjectiveUpdateEvent objectiveUpdated;

        public abstract class ObjectiveGoal : ScriptableObject
        {
            protected string description;
            public int currentAmount { get; protected set; }
            public int requiredAmount = 1;

            public bool Completed { get; protected set; }
            [HideInInspector] public UnityEvent objectiveCompleted;
            [HideInInspector] public UnityEvent objectiveUpdated;

            public virtual string GetDescription()
            {
                return description;
            }
            public virtual void Initialize()
            {
                currentAmount = 0;
                Completed = false;
                objectiveCompleted = new UnityEvent();
                objectiveUpdated = new UnityEvent();
            }

            protected void Evaluate()
            {
                objectiveUpdated.Invoke();
                if (currentAmount >= requiredAmount)
                {
                    Complete();
                }
            }
            private void Complete()
            {
                Completed = true;
                objectiveCompleted.Invoke();
                objectiveCompleted.RemoveAllListeners();
            }
        }
        public List<ObjectiveGoal> objectives;

        public void Initialize()
        {
            Completed = false;
            objectiveCompleted = new ObjectiveCompletedEvent();
            objectiveUpdated = new ObjectiveUpdateEvent();

            foreach (var objective in objectives)
            {
                objective.Initialize();
                objective.objectiveCompleted.AddListener(delegate { CheckObjectives(); });
                objective.objectiveUpdated.AddListener(delegate { CheckObjectives(); });
            }
        }

        private void CheckObjectives()
        {
            Completed = objectives.All(g => g.Completed);
            if (Completed)
            {
                objectiveCompleted.Invoke(this);
                objectiveCompleted.RemoveAllListeners();
            }
            else
            {
                objectiveUpdated.Invoke(this);
            }
        }
    }
}
public class ObjectiveCompletedEvent : UnityEvent<Objective> { }
public class ObjectiveUpdateEvent : UnityEvent<Objective> { }

#if UNITY_EDITOR
[CustomEditor(typeof(Objective))]
public class ObjectiveEditor : Editor
{
    SerializedProperty m_ObjectiveInfoProperty;

    List<string> m_ObjectiveGoalType;
    SerializedProperty m_ObjectiveGoalListProperty;

    [MenuItem("Assets/Objectives", priority = 0)]
    public static void CreateObjective()
    {
        var newObjective = CreateInstance<Objective>();

        ProjectWindowUtil.CreateAsset(newObjective, "objective.asset");
    }
    private void OnEnable()
    {
        m_ObjectiveInfoProperty = serializedObject.FindProperty(nameof(Objective.information));

        m_ObjectiveGoalListProperty = serializedObject.FindProperty(nameof(Objective.objectives));

        var lookup = typeof(Objective.ObjectiveGoal);
        m_ObjectiveGoalType = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(lookup))
            .Select(type => type.Name)
            .ToList();
    }
    public override void OnInspectorGUI()
    {
        var child = m_ObjectiveInfoProperty.Copy();
        var depth = child.depth;
        child.NextVisible(true);

        EditorGUILayout.LabelField("Objective Info", EditorStyles.boldLabel);
        while (child.depth > depth)
        {
            EditorGUILayout.PropertyField(child, true);
            child.NextVisible(false);
        }

        int choice = EditorGUILayout.Popup("Add new Objective Goal", -1, m_ObjectiveGoalType.ToArray());

        if (choice != -1)
        {
            var newInstance = ScriptableObject.CreateInstance(m_ObjectiveGoalType[choice]);

            AssetDatabase.AddObjectToAsset(newInstance, target);

            m_ObjectiveGoalListProperty.InsertArrayElementAtIndex(m_ObjectiveGoalListProperty.arraySize);
            m_ObjectiveGoalListProperty.GetArrayElementAtIndex(m_ObjectiveGoalListProperty.arraySize - 1).objectReferenceValue = newInstance;
        }
        Editor ed = null;
        int toDelete = -1;
        for (int i = 0; i < m_ObjectiveGoalListProperty.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();

            var item = m_ObjectiveGoalListProperty.GetArrayElementAtIndex(i);
            SerializedObject obj = new SerializedObject(item.objectReferenceValue);

            Editor.CreateCachedEditor(item.objectReferenceValue, null, ref ed);
            ed.OnInspectorGUI();
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("-", GUILayout.Width(32)))
            {
                toDelete = i;
            }
            EditorGUILayout.EndHorizontal();
        }
        if(toDelete != -1)
        {
            var item = m_ObjectiveGoalListProperty.GetArrayElementAtIndex(toDelete).objectReferenceValue;
            DestroyImmediate(item, true);

            m_ObjectiveGoalListProperty.DeleteArrayElementAtIndex(toDelete);
            m_ObjectiveGoalListProperty.DeleteArrayElementAtIndex(toDelete);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif