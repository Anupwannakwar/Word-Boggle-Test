using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    #region Enums
    public enum Levels
    {
        Level1,
        Level2,
        Level3,
        Endless
    }
    #endregion

    #region Declarations
    public List<Tile> Column1 = new List<Tile>();
    public List<Tile> Column2 = new List<Tile>();
    public List<Tile> Column3 = new List<Tile>();
    public List<Tile> Column4 = new List<Tile>();

    public List<string> UsedWords = new List<string>();

    public string[] LevelLetters = new  string [16];

    public TMP_Text SelectedText;
    public TMP_Text ScoreText;
    public TMP_Text GameEndText;

    string wordlist;

    public Levels levels;

    int score = 0;

    public SpawnManager spawnManager;

    public bool isEndless = false;

    bool BonusUsed = false;

    public bool TimerOn = false;

    public bool GameOver = false;

    public bool LevelWon = false;

    //timer variables
    public TMP_Text timecount;
    private float SecCount = 60;
    public float min = 4;
    private int sec = 0;
    #endregion

    #region Singleton
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    #endregion

    private void Start()
    {
        var _path = Application.streamingAssetsPath + "/wordlist.txt";
        UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(_path);
        www.SendWebRequest();
        while (!www.isDone)
        {
        }
        wordlist = www.downloadHandler.text;
    }

    private void Update()
    {
        if(!GameOver)
        {
            if (TimerOn)
            {
                timecount.gameObject.SetActive(true);

                SecCount -= Time.deltaTime;

                sec = (int)SecCount;
                if (sec < 10)
                    timecount.text = min + ":0" + sec;
                else
                    timecount.text = min + ":" + sec;


                if (sec == 0)
                {
                    min -= 1;
                    SecCount = 60;
                }

                if (min == -1 && sec == 0)
                    GameOver = true;
            }

            if((levels == Levels.Level1 || levels == Levels.Level3) && score >= 7)
            {
                LevelWon = true;
                GameOver = true;
                UsedWords.Clear();
            }

            if (levels == Levels.Level2 && score >= 20)
            {
                LevelWon = true;
                GameOver = true;
                UsedWords.Clear();
            }
        }
        else
        {
            StartCoroutine(GameMessage());
        }
    }

    public void UpdateSelectedText(Tile tile)
    {
        SelectedText.text = SelectedText.text + tile.TileText.text;
        if(tile.BugActive)
        {
            BonusUsed = true;
        }
    }

    public IEnumerator ResetTiles(bool isReset)
    {
        SelectedText.text = "";
        Column1.ForEach(x => x.isSelected = false);
        Column2.ForEach(x => x.isSelected = false);
        Column3.ForEach(x => x.isSelected = false);
        Column4.ForEach(x => x.isSelected = false);

        if(isReset)
        {
            Column1.ForEach(x => x.TileImage.color = new Color32(255, 23, 0, 255));
            Column2.ForEach(x => x.TileImage.color = new Color32(255, 23, 0, 255));
            Column3.ForEach(x => x.TileImage.color = new Color32(255, 23, 0, 255));
            Column4.ForEach(x => x.TileImage.color = new Color32(255, 23, 0, 255));

            yield return new WaitForSeconds(0.4f);
        }

        Column1.ForEach(x => x.TileImage.color = new Color32(255, 255, 255, 255));
        Column2.ForEach(x => x.TileImage.color = new Color32(255, 255, 255, 255));
        Column3.ForEach(x => x.TileImage.color = new Color32(255, 255, 255, 255));
        Column4.ForEach(x => x.TileImage.color = new Color32(255, 255, 255, 255));
    }

    public IEnumerator GameMessage()
    {
        if(LevelWon)
        {
            GameEndText.gameObject.SetActive(true);
            GameEndText.text = " Player Won!";
        }
        else if(GameOver)
        {
            GameEndText.gameObject.SetActive(true);
            GameEndText.text = " Player Lost!";
        }
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("Main Menu");
    }

    public void CheckWord()
    {
        if(SelectedText.text.Length >= 3)
        {
            var wordlistArray = wordlist.Split('\n');

            foreach (var line in wordlistArray)
            {
                if (string.Equals(line, SelectedText.text.ToLower()) && !UsedWords.Contains(SelectedText.text))
                {
                    if(isEndless)
                    {
                        DestroyUsedTiles();
                    }
                    else
                    {
                        DestroyNearbyBlocks();
                    }

                    UsedWords.Add(SelectedText.text);

                    UpdateScore();
                    StartCoroutine(ResetTiles(false));
                    return;
                }

            }

            BonusUsed = false;

            StartCoroutine(ResetTiles(true));
        } 
        else
        {
            BonusUsed = false;
            StartCoroutine(ResetTiles(true));
        }
    }

    public void DeselectTiles()
    {
        StartCoroutine(ResetTiles(false));
    }

    public void UpdateScore()
    {
        if(levels == Levels.Level1 || levels == Levels.Level3)
        {
            score += 1;
            ScoreText.text = "Words : " + score.ToString();
        }
        else if(levels == Levels.Level2)
        {
            if (BonusUsed == true)
            {
                score += 5;
            }
            else
            {
                score += 1;
            }
            ScoreText.text = "Score : " + score.ToString();
        }
        else if(levels == Levels.Endless)
        {
            score += 1;
            ScoreText.text = "Score : " + score.ToString();
        }

        spawnManager.SpawnTiles();
        BonusUsed = false;
    }

    public void DestroyUsedTiles()
    {
        SelectedText.text = "";

        List<Tile> tempList = new List<Tile>();

        tempList.AddRange(Column1.FindAll(x => x.isSelected));
        tempList.AddRange(Column2.FindAll(x => x.isSelected));
        tempList.AddRange(Column3.FindAll(x => x.isSelected));
        tempList.AddRange(Column4.FindAll(x => x.isSelected));     

        Column1.RemoveAll(x => x.isSelected);
        Column2.RemoveAll(x => x.isSelected);
        Column3.RemoveAll(x => x.isSelected);
        Column4.RemoveAll(x => x.isSelected);


        foreach (var item in tempList)
        {
            Destroy(item.gameObject);
        }

    }

    public void DestroyNearbyBlocks()
    {
        for (int i = 0; i < Column1.Count; i++)
        {
            if (Column1[i].isSelected)
            {
                if (i == 0)
                    Column1[i + 1].BlockDestroyed = true;
                else if (i == 3)
                    Column1[i - 1].BlockDestroyed = true;
                else
                {
                    Column1[i + 1].BlockDestroyed = true;
                    Column1[i - 1].BlockDestroyed = true;
                }
            }
        }

        for (int i = 0; i < Column2.Count; i++)
        {
            if (Column2[i].isSelected)
            {
                if (i == 0)
                    Column2[i + 1].BlockDestroyed = true;
                else if (i == 3)
                    Column2[i - 1].BlockDestroyed = true;
                else
                {
                    Column2[i + 1].BlockDestroyed = true;
                    Column2[i - 1].BlockDestroyed = true;
                }
            }
        }

        for (int i = 0; i < Column3.Count; i++)
        {
            if (Column3[i].isSelected)
            {
                if (i == 0)
                    Column3[i + 1].BlockDestroyed = true;
                else if (i == 3)
                    Column3[i - 1].BlockDestroyed = true;
                else
                {
                    Column3[i + 1].BlockDestroyed = true;
                    Column3[i - 1].BlockDestroyed = true;
                }
            }
        }

        for (int i = 0; i < Column4.Count; i++)
        {
            if (Column4[i].isSelected)
            {
                if (i == 0)
                    Column4[i + 1].BlockDestroyed = true;
                else if (i == 3)
                    Column4[i - 1].BlockDestroyed = true;
                else
                {
                    Column4[i + 1].BlockDestroyed = true;
                    Column4[i - 1].BlockDestroyed = true;
                }
            }
        }
    }

    public void SetLevelWords()
    {
        if (!isEndless)
        {
            Column1[0].TileText.text = LevelLetters[0];
            Column1[1].TileText.text = LevelLetters[1];
            Column1[2].TileText.text = LevelLetters[2];
            Column1[3].TileText.text = LevelLetters[3];

            Column2[0].TileText.text = LevelLetters[4];
            Column2[1].TileText.text = LevelLetters[5];
            Column2[2].TileText.text = LevelLetters[6];
            Column2[3].TileText.text = LevelLetters[7];

            Column3[0].TileText.text = LevelLetters[8];
            Column3[1].TileText.text = LevelLetters[9];
            Column3[2].TileText.text = LevelLetters[10];
            Column3[3].TileText.text = LevelLetters[11];

            Column4[0].TileText.text = LevelLetters[12];
            Column4[1].TileText.text = LevelLetters[13];
            Column4[2].TileText.text = LevelLetters[14];
            Column4[3].TileText.text = LevelLetters[15];
        }
    }
}
