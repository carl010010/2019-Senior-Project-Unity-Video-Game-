using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SceneLoader", menuName = "ScriptableObject/SceneLoader", order = 1)]
public class GameManager : ScriptableObject
{
    public AudioMixer audioMixer;

    public void Openscene(string sceneName)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
