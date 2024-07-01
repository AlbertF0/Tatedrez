using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MovementTypeSo))]
public class MovementTypeSetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        MovementTypeSo movements = (MovementTypeSo)target;

        // Display the current ability in the Inspector
        EditorGUILayout.LabelField("Assigned Movement", EditorStyles.boldLabel);

        if (movements.Behaviour != null)
        {
            EditorGUILayout.LabelField("Movement Type", movements.Behaviour.GetType().Name);
        }
        else
        {
            EditorGUILayout.LabelField("No Movement assigned.");
        }

        // Add a button to change the ability
        if (GUILayout.Button("Assign Movement Type"))
        {
            GenericMenu menu = new GenericMenu();
            // Add menu items for each concrete type
            menu.AddItem(new GUIContent("Rook"), false, () => AssignMovement(movements, typeof(RookMovementBehaviour)));
            menu.AddItem(new GUIContent("Bishop"), false, () => AssignMovement(movements, typeof(BishopMovementBehaviour)));
            menu.AddItem(new GUIContent("Knight"), false, () => AssignMovement(movements, typeof(KnightMovementBehaviour)));
            menu.ShowAsContext();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

    private void AssignMovement(MovementTypeSo movementTypeHolder, System.Type movementType)
    {
        Undo.RecordObject(movementTypeHolder, "Assign Movement Type");
        movementTypeHolder.Behaviour = (AMovementBehaviour)System.Activator.CreateInstance(movementType);
        EditorUtility.SetDirty(movementTypeHolder);
    }
}