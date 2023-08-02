using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public List<TextMeshProUGUI> list = new List<TextMeshProUGUI>();

    public void UpdateScore(int playerID, int shieldLives)
    {
        list[playerID].text = "P" + playerID + " Lives " + shieldLives;
    } 
}
