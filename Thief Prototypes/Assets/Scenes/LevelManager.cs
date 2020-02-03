using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class LevelManager : MonoBehaviour
{
    public DreamloPublicAndPrivateKey PublicAndPrivate;
    public GameManager gameManager;
    private DreamloLeaderBoard LeaderBoard;
    private GameObject Player;
    private FirstPersonController playerController;
    private Inventory inventory;

    public LevelRequirments levelRequirments;
    public string NextLevel;
    public GameObject InGameMenu;
    public GameObject SettingsMenu;
    public GameObject LevelOverMenu;

    [Space]
    public TextMeshProUGUI LevelTotalValuableText;
    public TextMeshProUGUI LevelTotalGoldText;
    public TextMeshProUGUI PlayerTotalValuableText;
    public TextMeshProUGUI PlayerTotalGoldText;
    public TextMeshProUGUI PlayerTotalTimeText;

    [Space]
    public List<TextMeshProUGUI> LeaderBoardScores = new List<TextMeshProUGUI>();

    [Space]
    public Button nextLevelButton;
    public GameObject AddScoreObject;


    public List<Item> levelItems = new List<Item>();
    public int totalGoldInLevel;
    public int totalValuablesInLevel;

    double Timer = 0;
    bool levelEnded = false;
    // Use this for initialization
    void Start()
    {
        nextLevelButton.onClick.AddListener(() => gameManager.Openscene(NextLevel));
        levelItems.Clear();
        totalGoldInLevel = 0;
        totalValuablesInLevel = 0;

        if (levelRequirments == null)
        {
            Debug.LogWarning("No level Requirments found", this);
        }

        Item[] items = FindObjectsOfType<Item>();

        foreach (Item i in items)
        {
            if (i.itemType == Item.ItemType.GOLD)
            {
                totalGoldInLevel += i.Value;
                levelItems.Add(i);
            }
            else if (i.itemType == Item.ItemType.VALUABLE)
            {
                totalValuablesInLevel += i.Value;
                levelItems.Add(i);
            }
        }

        playerController = FindObjectOfType<FirstPersonController>();
        if (playerController == null)
        {
            Debug.LogError("UHHHH sorry about that boss we cant find the player");
        }
        else
        {
            Player = playerController.gameObject;
            inventory = Player.GetComponent<Inventory>();

            if (inventory == null)
            {
                Debug.LogError("Player has no inventory");
            }
        }

        LeaderBoard = gameObject.AddComponent<DreamloLeaderBoard>();

        LeaderBoard.privateCode = PublicAndPrivate.privateCode;
        LeaderBoard.publicCode = PublicAndPrivate.publicCode;

        DisplayLeaderBoard(LeaderBoardScores.Count);
    }

    // Update is called once per frame
    void Update()
    {
        if (!levelEnded &&
            Input.GetKeyDown(KeyCode.Q))
        {
            if (Time.timeScale > 0)
            {
                playerController.enabled = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0;
                InGameMenu.SetActive(true);
            }
            else
            {
                TurnMenuOff();
            }
        }
    }

    public void TurnMenuOff()
    {
        playerController.enabled = true;
        Time.timeScale = 1;
        InGameMenu.SetActive(false);
        SettingsMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void TurnOnEndLevelMenu()
    {
        TurnMenuOff();
        playerController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
        SetAfterLevelText();
        LevelOverMenu.SetActive(true);
    }

    public void AddScoreToLeaderBoard(TextMeshProUGUI name)
    {
        if (string.IsNullOrEmpty(name.text))
            return;

        AddScoreObject.SetActive(false);
        /*TODO Get LeaderBoard Pos*/LeaderBoard.AddScore(name.text, inventory.Value + inventory.GetGold(), Mathf.FloorToInt(Time.timeSinceLevelLoad));
        //TODO Show leaderboard around the new entry
    }

    void DisplayLeaderBoard(int maxToDisplay)
    {
        LeaderBoard.LoadScores();
        StartCoroutine(DisplayScores(maxToDisplay));
    }

    IEnumerator DisplayScores(int maxToDisplay)
    {
        //GUILayout.Label("High Scores:");
        List<DreamloLeaderBoard.Score> scoreList;

        do
        {
            scoreList = LeaderBoard.ToListHighToLow();
            yield return null;
        }
        while (scoreList == null);

        int count = 0;
        var LeaderBoardText = LeaderBoardScores.GetEnumerator();
        LeaderBoardText.MoveNext();

        foreach (DreamloLeaderBoard.Score s in scoreList)
        {
            count++;
            int min = Mathf.FloorToInt(s.seconds / 60);
            int sec = Mathf.FloorToInt(s.seconds % 60);
            string time = min.ToString("00") + ":" + sec.ToString("00");
            LeaderBoardText.Current.text = string.Format("{0}: {1}, Score: {2}, Time: {3}", s.LeaderBoardPos, s.playerName, s.score, time);
            LeaderBoardText.MoveNext();

            if (count >= maxToDisplay) break;
        }

        yield return null;
    }



    private void SetAfterLevelText()
    {
        LevelTotalValuableText.text = totalValuablesInLevel.ToString();
        LevelTotalGoldText.text = totalGoldInLevel.ToString();

        PlayerTotalValuableText.text = inventory.Value.ToString();
        PlayerTotalGoldText.text = inventory.GetGold().ToString();

        int min = Mathf.FloorToInt(Time.timeSinceLevelLoad / 60);
        int sec = Mathf.FloorToInt(Time.timeSinceLevelLoad % 60);
        PlayerTotalTimeText.text = min.ToString("00") + ":" + sec.ToString("00");
    }

    public bool TryEndLevel()
    {
        if (levelRequirments.GoldPercentage <= (inventory.Value + inventory.GetGold()) / (float)(totalGoldInLevel + totalValuablesInLevel))
        {
            levelEnded = true;
        }

        return levelEnded;
    }
}
