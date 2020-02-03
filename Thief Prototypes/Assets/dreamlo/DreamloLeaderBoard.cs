using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DreamloLeaderBoard : MonoBehaviour
{
    readonly string dreamloWebserviceURL = "http://dreamlo.com/lb/";

    public string privateCode = "";
    public string publicCode = "";

    string highScores;

    ////////////////////////////////////////////////////////////////////////////////////////////////

    // A player named Carmine got a score of 100. If the same name is added twice, we use the higher score.
    // http://dreamlo.com/lb/(your super secret very long code)/add/Carmine/100

    // A player named Carmine got a score of 1000 in 90 seconds.
    // http://dreamlo.com/lb/(your super secret very long code)/add/Carmine/1000/90

    // A player named Carmine got a score of 1000 in 90 seconds and is Awesome.
    // http://dreamlo.com/lb/(your super secret very long code)/add/Carmine/1000/90/Awesome

    ////////////////////////////////////////////////////////////////////////////////////////////////


    public struct Score
    {
        public string playerName;
        public int score;
        public int seconds;
        public string shortText;
        public string dateString;
        public int LeaderBoardPos;
    }

    void Start()
    {
        this.highScores = "";
        if (publicCode == "")
            Debug.LogError("You forgot to set the Public Code variable for dreamloLeaderBoard");
        if (privateCode == "")
            Debug.LogError("You forgot to set the Private Code variable for dreamloLeaderBoard");
    }

    public static DreamloLeaderBoard GetSceneDreamloLeaderboard()
    {
        return FindObjectOfType<DreamloLeaderBoard>();
    }


    public static double DateDiffInSeconds(System.DateTime now, System.DateTime olderdate)
    {
        var difference = now.Subtract(olderdate);
        return difference.TotalSeconds;
    }

    System.DateTime _lastRequest = System.DateTime.Now;
    int _requestTotal = 0;

    bool TooManyRequests()
    {
        var now = System.DateTime.Now;

        if (DateDiffInSeconds(now, _lastRequest) <= 2)
        {
            _lastRequest = now;
            _requestTotal++;
            if (_requestTotal > 3)
            {
                Debug.LogError("DREAMLO Too Many Requests. Am I inside an update loop?");
                return true;
            }

        }
        else
        {
            _lastRequest = now;
            _requestTotal = 0;
        }

        return false;
    }

    public void AddScore(string playerName, int totalScore)
    {
        if (TooManyRequests()) return;

        StartCoroutine(AddScoreWithPipe(playerName, totalScore));
    }

    public void AddScore(string playerName, int totalScore, int totalSeconds)
    {
        if (TooManyRequests()) return;

        StartCoroutine(AddScoreWithPipe(playerName, totalScore, totalSeconds));
    }

    // This function saves a trip to the server. Adds the score and retrieves results in one trip.
    IEnumerator AddScoreWithPipe(string playerName, int totalScore)
    {
        playerName = Clean(playerName);

        WWW www = new WWW(dreamloWebserviceURL + privateCode + "/add-pipe/" + WWW.EscapeURL(playerName) + "/" + totalScore.ToString());
        yield return www;
        highScores = www.text;
    }

    IEnumerator AddScoreWithPipe(string playerName, int totalScore, int totalSeconds)
    {
        playerName = Clean(playerName);

        WWW www = new WWW(dreamloWebserviceURL + privateCode + "/add-pipe/" + WWW.EscapeURL(playerName) + "/" + totalScore.ToString() + "/" + totalSeconds.ToString());
        yield return www;
        highScores = www.text;
    }

    IEnumerator GetScores()
    {
        highScores = "";
        WWW www = new WWW(dreamloWebserviceURL + publicCode + "/pipe");
        yield return www;
        highScores = www.text;
    }

    IEnumerator GetSingleScore(string playerName)
    {
        highScores = "";
        WWW www = new WWW(dreamloWebserviceURL + publicCode + "/pipe-get/" + WWW.EscapeURL(playerName));
        yield return www;
        highScores = www.text;
    }

    public void LoadScores()
    {
        if (TooManyRequests())
            return;


        StartCoroutine(GetScores());
    }

    public string[] ToStringArray()
    {
        if (highScores == null)
            return null;
        if (highScores == "")
            return null;

        string[] rows = highScores.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        return rows;
    }

    public List<Score> ToListHighToLow()
    {
        List<Score> genericList = ToScoreArray();

        if (genericList == null)
            return null;

        genericList.Sort((x, y) => y.score.CompareTo(x.score));

        return genericList;
    }

    public List<Score> ToScoreArray()
    {
        string[] rows = ToStringArray();
        if (rows == null)
            return null;

        int rowcount = rows.Length;

        if (rowcount <= 0)
            return null;

        List<Score> scoreList = new List<Score>(rowcount);

        for (int i = 0; i < rowcount; i++)
        {
            string[] values = rows[i].Split(new char[] { '|' }, System.StringSplitOptions.None);

            Score current = new Score();
            current.playerName = values[0];
            current.score = 0;
            current.seconds = 0;
            current.shortText = "";
            current.dateString = "";

            if (values.Length > 1)
                current.score = CheckInt(values[1]);
            if (values.Length > 2)
                current.seconds = CheckInt(values[2]);
            if (values.Length > 3)
                current.shortText = values[3];
            if (values.Length > 4)
                current.dateString = values[4];
            if (values.Length > 5)
                current.LeaderBoardPos = CheckInt(values[5]);
            scoreList.Add(current);
        }
        return scoreList;
    }

    // Keep pipe and slash out of names
    string Clean(string s)
    {
        s = s.Replace("/", "");
        s = s.Replace("|", "");
        return s;

    }

    int CheckInt(string s)
    {
        int x = 0;

        int.TryParse(s, out x);
        return x;
    }

}
