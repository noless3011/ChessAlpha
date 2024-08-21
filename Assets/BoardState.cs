using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class BoardState : MonoBehaviour
{
    public GameObject[] chessObjectPieces;
    public Dictionary<string, ChessPiece> chessPieces;
    private void Start()
    {
        chessPieces = new();
        for (int i = 0; i < chessObjectPieces.Length; i++)
        {
            ChessPiece cp = chessObjectPieces[i].GetComponent<ChessPiece>();
            string name = chessObjectPieces[i].name;
            chessPieces.Add(name, cp);
        }
    }

}
