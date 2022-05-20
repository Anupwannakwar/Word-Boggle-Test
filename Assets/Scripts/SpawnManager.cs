using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnManager : MonoBehaviour
{
    #region Declaration
    public Tile Tile;
    public Transform CanvasParent;

    public Transform SpawnPoint1;
    public Transform SpawnPoint2;
    public Transform SpawnPoint3;
    public Transform SpawnPoint4;
    #endregion

    private void Start()
    {
        SpawnTiles();
    }

    public void SpawnTiles()
    {
        StartCoroutine(SpawnIndividualTiles(GameManager.Instance.Column1, SpawnPoint1, 1));
        StartCoroutine(SpawnIndividualTiles(GameManager.Instance.Column2, SpawnPoint2, 2));
        StartCoroutine(SpawnIndividualTiles(GameManager.Instance.Column3, SpawnPoint3, 3));
        StartCoroutine(SpawnIndividualTiles(GameManager.Instance.Column4, SpawnPoint4, 4));
    }

    public IEnumerator SpawnIndividualTiles(List<Tile> CheckList , Transform spawnpoint , int ListNo)
    {
        Vector3 spawnPos = spawnpoint.transform.position;
     
        for (int i = CheckList.Count ; i < 4; i++)
        {
            Tile item = Instantiate(Tile, spawnPos, Quaternion.identity);
            item.transform.SetParent(CanvasParent);

            switch(ListNo)
            {
                case 1:
                    GameManager.Instance.Column1.Add(item);
                    break;
                case 2:
                    GameManager.Instance.Column2.Add(item);
                    break;
                case 3:
                    GameManager.Instance.Column3.Add(item);
                    break;
                case 4:
                    GameManager.Instance.Column4.Add(item);
                    break;
            }

            yield return new WaitForSeconds(1.0f);
        }
        if (ListNo == 4)
        {
            GameManager.Instance.SetLevelWords();
        }
    }
}
