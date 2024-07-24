using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ReputationSystem))]
public class ReputationSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ReputationSystem reputationSystem = (ReputationSystem)target;
        SerializedProperty factionReputationsProperty = serializedObject.FindProperty("factionReputations");

        for (int i = 0; i < factionReputationsProperty.arraySize; i++)
        {
            SerializedProperty factionDataProperty = factionReputationsProperty.GetArrayElementAtIndex(i);
            SerializedProperty factionProperty = factionDataProperty.FindPropertyRelative("faction");
            SerializedProperty reputationProperty = factionDataProperty.FindPropertyRelative("reputation");
            SerializedProperty currentTitleIndexProperty = factionDataProperty.FindPropertyRelative("currentTitleIndex");
            SerializedProperty currentTitleNameProperty = factionDataProperty.FindPropertyRelative("currentTitleName");

            EditorGUILayout.PropertyField(factionProperty);
            EditorGUILayout.PropertyField(reputationProperty);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(currentTitleIndexProperty);
            if (EditorGUI.EndChangeCheck())
            {
                ReputationSystem.Faction faction = (ReputationSystem.Faction)factionProperty.enumValueIndex;
                int newIndex = currentTitleIndexProperty.intValue;
                currentTitleNameProperty.stringValue = ReputationSystem.GetTitleName(faction, newIndex);
            }

            string[] titles = GetTitlesForFaction((ReputationSystem.Faction)factionProperty.enumValueIndex);
            int selectedIndex = EditorGUILayout.Popup("Current Title", currentTitleIndexProperty.intValue, titles);
            if (selectedIndex != currentTitleIndexProperty.intValue)
            {
                currentTitleIndexProperty.intValue = selectedIndex;
                currentTitleNameProperty.stringValue = titles[selectedIndex];
            }

            EditorGUILayout.Space();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private string[] GetTitlesForFaction(ReputationSystem.Faction faction)
    {
        if (faction == ReputationSystem.Faction.DelphiaGuard)
        {
            return System.Enum.GetNames(typeof(ReputationSystem.DelphiaGuardTitle));
        }
        else
        {
            return System.Enum.GetNames(typeof(ReputationSystem.BanditTitle));
        }
    }
}