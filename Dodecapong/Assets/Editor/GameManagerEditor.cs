using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GameManager.instance != null) {
            if (GUILayout.Button("Create New Player")) {
                GameManager.instance.GetNewPlayer();
            }

            if (GUILayout.Button("Spawn Transformer")) {
                GameManager.instance.SpawnTransformer();
            }

            if (GameManager.instance.gameState == GameManager.GameState.GAMEPLAY) {
                for (int i = 0; i < GameManager.instance.alivePlayers.Count; i++) {
                    if (GUILayout.Button("Eliminate Player " + i)) GameManager.instance.EliminatePlayer(GameManager.instance.alivePlayers[i]);
                }
            }
        }
    }
}
