using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

[Serializable]
public class MenuTextPair
{
    public Slider sliderWidget;
    public TMP_Text labelWidget;
    public List<string> customValues = new List<string>();

    public void UpdateText()
    {
        if (sliderWidget != null && labelWidget != null)
        {
            if (customValues == null || customValues.Count == 0)
            {
                labelWidget.text = sliderWidget.value.ToString();

            }
            else
            {
                labelWidget.text = customValues[(int)sliderWidget.value];
            }

        }
    }
}

public class PresetScrollbar
{
    public Button presetButton;
    public TMP_Text presetLabel;
    public TMP_Text presetHoverLabel;


}

public class menuweeee : MonoBehaviour
{


    public GameObject fieldSettings;
    public GameObject playerSettings;
    public GameObject scoreSettings;
    public GameObject ballSettings;




    public List<MenuTextPair> menuTextPairs;

    // Start is called before the first frame update
    void Start()
    {
        ballSettings.SetActive(false);
        fieldSettings.SetActive(false);
        playerSettings.SetActive(false);
        scoreSettings.SetActive(false);
    }

    public void Update()
    {
        foreach(MenuTextPair thisPair in menuTextPairs)
        {
            thisPair.UpdateText();
        }

        
    }

    // Buttons at top, enabling/disabling panels
    public void BallSettingsEnable()
    {
        ballSettings.SetActive(true);
        fieldSettings.SetActive(false);
        playerSettings.SetActive(false);
        scoreSettings.SetActive(false);
    }

    public void FieldSettingsEnable()
    {
        ballSettings.SetActive(false);
        fieldSettings.SetActive(true);
        playerSettings.SetActive(false);
        scoreSettings.SetActive(false);
    }

    public void PlayerSettingsEnable()
    {
        ballSettings.SetActive(false);
        fieldSettings.SetActive(false);
        playerSettings.SetActive(true);
        scoreSettings.SetActive(false);
    }

    public void ScoreSettingsEnable()
    {
        ballSettings.SetActive(false);
        fieldSettings.SetActive(false);
        playerSettings.SetActive(false);
        scoreSettings.SetActive(true);
    }

    // Pushing slider values to text 

}
