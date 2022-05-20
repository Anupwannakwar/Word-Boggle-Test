using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    #region Declarations
    public bool isSelected = false;

    public Dictionary<int, string> Letters = new Dictionary<int, string>(){
                                {1, "A"},
                                {2, "B"},
                                {3, "C"},
                                {4, "D"},
                                {5, "E"},
                                {6, "F"},
                                {7, "G"},
                                {8, "H"},
                                {9, "I"},
                                {10, "J"},
                                {11, "K"},
                                {12, "L"},
                                {13, "M"},
                                {14, "N"},
                                {15, "O"},
                                {16, "P"},
                                {17, "Q"},
                                {18, "R"},
                                {19, "S"},
                                {20, "T"},
                                {21, "U"},
                                {22, "V"},
                                {23, "W"},
                                {24, "X"},
                                {25, "Y"},
                                {26, "Z"},
    };

    public TMP_Text TileText;

    public Button TileButton;

    public Image TileImage;

    public GameObject block;

    public GameObject bug;

    public bool BlockActive = false;
    public bool BlockDestroyed = false;

    public bool BugActive = false;

    public Animator anim;
    #endregion


    private void Start()
    {
        if(GameManager.Instance.isEndless)
        {
            int key = Random.Range(1, 27);
            TileText.text = Letters[key];
        }
        else
        {
            int flag = Random.Range(1, 5);
            if (flag == 1)
            {
                BlockActive = true;
            }

            if (BlockActive)
                block.gameObject.SetActive(true);

            // Bugs Random Spawn
            if(GameManager.Instance.levels != GameManager.Levels.Level1)
            {
                int BugFlag = Random.Range(1, 6);
                if (BugFlag == 1 && !BlockActive)
                {
                    BugActive = true;
                }

                if (BugActive)
                    bug.gameObject.SetActive(true);
            }       
        }
        
        TileButton.onClick.AddListener(SelectTile);  
    }

    private void Update()
    {
        if(BlockActive)
        {
            if(BlockDestroyed)
            {
                anim.SetBool("OnBurst", true);
                BlockActive = false;
            }
        }
    }

    public void SelectTile()
    {
        if(!GameManager.Instance.GameOver)
        {
            if (!isSelected)
            {
                isSelected = true;

                TileImage.color = new Color32(0, 255, 206, 255);

                GameManager.Instance.UpdateSelectedText(this);
            }
        }   
    }
}
