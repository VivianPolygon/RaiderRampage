using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyAIMovement))]
[CanEditMultipleObjects]
public class EnemyMovementDebugging : Editor
{
    private EnemyAIMovement movementScript;

    private void OnEnable()
    {
        movementScript = (EnemyAIMovement)target;
    }

    private void OnSceneGUI()
    {
        DrawEnemyMovementDebugInfo(movementScript);
    }

    private void DrawEnemyMovementDebugInfo(EnemyAIMovement enemyMovement)
    {
        Handles.color = Color.grey;
        Handles.ArrowHandleCap(EditorGUIUtility.GetControlID(FocusType.Passive),
            enemyMovement.transform.position, enemyMovement.transform.rotation, 4, EventType.Repaint);

        if(enemyMovement.nextNode != null)
        {
            Handles.color = Color.green;
            Handles.SphereHandleCap(EditorGUIUtility.GetControlID(FocusType.Passive),
                enemyMovement.nextNode.position, enemyMovement.nextNode.rotation, 3, EventType.Repaint);
        }

        if(enemyMovement.lastNode != null)
        {
            Handles.color = Color.gray;
            Handles.SphereHandleCap(EditorGUIUtility.GetControlID(FocusType.Passive),
                enemyMovement.lastNode.position, enemyMovement.lastNode.rotation, 3, EventType.Repaint);
        }

        Handles.Label(enemyMovement.transform.position + (Vector3.up * 3), ("Time Sense Last Node: ") + enemyMovement.timeSenseNewNode.ToString());
    }
}
