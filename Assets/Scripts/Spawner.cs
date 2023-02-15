using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject corePiece;
    [SerializeField] private List<Sprite> piecesSprites = new List<Sprite>();
    private int piecesQuantity;
    public static Spawner Instance;
    private void Awake() {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        piecesQuantity = piecesSprites.Count;
    }

    private List<int> RandomPiece()
    {   
        List<int> newPieceList = new List<int>();

        for (int i = 0; i < piecesQuantity; i++)
        {
            newPieceList.Add(Random.Range(0,piecesQuantity + 1));
        }

        return newPieceList;
    }

    public Transform CreatePiece()
    {
        List<int> newPieceList = RandomPiece();

        GameObject spawnedPiece = Instantiate(corePiece, transform.position, transform.rotation);

        int listPosition = 0;
        List<Transform> deleteList = new List<Transform>(); 
        foreach (Transform block in spawnedPiece.transform)
        {
            int pieceCode = newPieceList[listPosition];
            if(pieceCode == 0)
            {
                deleteList.Add(block);
            }
            else
            {
                block.GetComponent<SpriteRenderer>().sprite = piecesSprites[pieceCode-1];
                block.GetComponent<Block>().SetBlockCode(pieceCode);
            }
            listPosition ++;
        }

        //Delete no Code Pieces
        foreach(Transform block in deleteList)
        {
            block.parent = null;
            Destroy(block.gameObject);
        }

        //Relocate the 1 color piece
        if (spawnedPiece.transform.childCount == 1)
        {
            spawnedPiece.transform.GetChild(0).transform.position = spawnedPiece.transform.TransformPoint(0,-1,0);
        }

        return spawnedPiece.transform;
    }

    private bool CanSpawn()
    {
        return true;
    }
}
