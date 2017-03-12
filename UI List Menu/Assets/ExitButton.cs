using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : MonoBehaviour {

    void Click()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }

}
