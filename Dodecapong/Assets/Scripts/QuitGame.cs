using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void Quit()
    {
#if UNITY_EDITOR
        Debug.Log("Application Quit");
#endif

        Application.Quit();
    }
}
