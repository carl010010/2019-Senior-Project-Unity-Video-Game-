using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingLoader : MonoBehaviour {


    public GameManager gameManager;
    public Slider VolumeSlider;

    // Use this for initialization
    void OnEnable ()
    {
        float value;
        gameManager.audioMixer.GetFloat("volume", out value);
        VolumeSlider.value = value;
    }
}
