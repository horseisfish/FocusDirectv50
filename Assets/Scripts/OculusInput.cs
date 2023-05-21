using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OculusInput : MonoBehaviour
{
    private TouchScreenKeyboard overlayKeyboard;
    public static string inputText = "";

    TextMeshPro inputfield;

    private void Start()
    {
        inputfield = GetComponent<TextMeshPro>();
    }

    public void StartInput()
    {
        overlayKeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.NumberPad);
       
    }
    public void changeText()
    {
        if (overlayKeyboard != null)
        {
            inputText = overlayKeyboard.text;
            Debug.Log(inputText);
            inputfield.text = inputText;
        }
    }

}
