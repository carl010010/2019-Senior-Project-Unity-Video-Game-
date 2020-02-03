using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EndLevelScript : MonoBehaviour
{
    LevelManager levelManager;

    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();

        if (levelManager == null)
        {
            Debug.LogError("Could not find level manager");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (levelManager.TryEndLevel())
        {
            //levelEndEvent.Invoke();
            levelManager.TurnOnEndLevelMenu();
        }
    }
}
