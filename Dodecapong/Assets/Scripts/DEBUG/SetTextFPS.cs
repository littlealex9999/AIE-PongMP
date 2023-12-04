using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetTextFPS : MonoBehaviour
{
    public TextMeshProUGUI text;
    float updateTimer;
    int checks;
    float runningAverage;

    void Update()
    {
        updateTimer += Time.unscaledDeltaTime;
        runningAverage += 1.0f / Time.unscaledDeltaTime;
        checks++;

        if (updateTimer > 1) {
            updateTimer = 0;

            text.text = ((int)(runningAverage / checks)).ToString();

            runningAverage = 0;
            checks = 0;
        }
    }
}
