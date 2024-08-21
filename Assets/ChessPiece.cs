using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ChessPiece : MonoBehaviour
{
    public enum Side
    {
        White,
        Black
    }


    public enum State
    {
        OnBoard,
        PickedUp,
        Taken
    }

    public enum PieceType
    {
        WhiteKing,
        WhiteQueen,
        WhiteBishop,
        WhiteKnight,
        WhiteRook,
        WhitePawn,
        BlackKing,
        BlackQueen,
        BlackBishop,
        BlackKnight,
        BlackRook,
        BlackPawn,
        Null
    }

    Dictionary<PieceType, int> piecesToFileIndex = new Dictionary<PieceType, int>()
    {
        {PieceType.WhiteKing,  0},
        {PieceType.WhiteQueen, 1},
        {PieceType.WhiteBishop,2},
        {PieceType.WhiteKnight,3},
        {PieceType.WhiteRook,  4},
        {PieceType.WhitePawn,  5},
        {PieceType.BlackKing,  6},
        {PieceType.BlackQueen, 7},
        {PieceType.BlackBishop,8},
        {PieceType.BlackKnight,9},
        {PieceType.BlackRook,  10 },
        {PieceType.BlackPawn,  11 },
    };


    State state;
    BoxCollider2D boxCollider;
    bool moved;
    Vector2Int oldCoordinate;
    Vector2Int coordinate;
    public Grid grid;
    public SpriteRenderer spriteRenderer;
    public Vector2Int startCoordinate;
    public PieceType type;
    public BoardState bs;

    private void Start()
    {
        if(spriteRenderer == null)
        {

             spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        spriteRenderer.sprite = LoadSprite();
        state = State.OnBoard;
        boxCollider = GetComponent<BoxCollider2D>();
        if (grid == null)
        {
            grid = FindObjectOfType<Grid>();
        }
        oldCoordinate = startCoordinate;
        coordinate = startCoordinate;
        AlignPieces();
        moved = false;
    }


    private void Update()
    {
        DragPiece();
        
    }

    Sprite LoadSprite()
    {
        Sprite[] pieces = Resources.LoadAll<Sprite>("Pieces/Chess_Pieces_Sprite");
        return pieces[piecesToFileIndex[type]];
    }

    void AlignPieces()
    {
        float cellSizeX = grid.cellSize.x;
        float cellSizeY = grid.cellSize.y;

        if (coordinate.x >= 0 && coordinate.y >= 0 && coordinate.x < 8 && coordinate.y < 8)
        {
            oldCoordinate = coordinate;
        }
        else
        {
            coordinate = oldCoordinate;
        }
        Vector3 squareCenter = grid.CellToWorld(new Vector3Int(coordinate.x, coordinate.y, -1));
        squareCenter.z = -1;
        squareCenter += new Vector3(cellSizeX * 0.5f, cellSizeY * 0.5f, 0);
        transform.position = squareCenter;
    }

    void SnapToGrid()
    {
        float cellSizeX = grid.cellSize.x;
        float cellSizeY = grid.cellSize.y;

        // Calculate column and row indices
        coordinate.x = Mathf.FloorToInt((transform.position.x - grid.transform.position.x) / cellSizeX);
        coordinate.y = Mathf.FloorToInt((transform.position.y - grid.transform.position.y) / cellSizeY);
        if (coordinate.x >= 0 && coordinate.y >= 0 && coordinate.x < 8 && coordinate.y < 8 && CheckMoveValidity()) 
        {
            oldCoordinate = coordinate;
            moved = true;
        }
        else
        {
            coordinate = oldCoordinate;
        }
        Vector3 squareCenter = grid.CellToWorld(new Vector3Int(coordinate.x, coordinate.y, -1));
        squareCenter.z = -1;
        squareCenter += new Vector3(cellSizeX * 0.5f, cellSizeY * 0.5f, 0);
        transform.position = squareCenter; 
    }


    void DragPiece()
    {
        if (state == State.OnBoard)
        {
            return;
        }
        else if(state == State.PickedUp)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = -2;
            this.transform.position = mousePos;
        }
        
    }

    void SwitchState()
    {
        switch (state)
        {
            case State.OnBoard:
                state = State.PickedUp; break;
            case State.PickedUp:
                state = State.OnBoard; break;
        }
    }

    KeyValuePair<bool, PieceType> CheckOverlapPiece()
    {
        foreach(KeyValuePair<string, ChessPiece> piece in bs.chessPieces)
        {
            if(coordinate == piece.Value.coordinate && name != piece.Key)
            {
                return new KeyValuePair<bool, PieceType>( true, piece.Value.type);
            }

        }
        return new KeyValuePair<bool, PieceType>(false, PieceType.Null); ;
    }

    bool CheckBlockedPiece()
    {
        foreach (KeyValuePair<string, ChessPiece> piece in bs.chessPieces)
        {
            Vector2Int move = coordinate - oldCoordinate;
            Vector2Int vectorToPiece = piece.Value.coordinate - oldCoordinate;
            if(vectorToPiece.magnitude < move.magnitude 
                && Vector2IntUtilities.IsAlign(move, vectorToPiece))
            {
                return true;
            }
        }
        return false;
    }


    bool CheckMoveValidity()
    {
        
        switch(type)
        {
            case PieceType.BlackPawn:
                if (moved)
                {
                    if (coordinate.y < oldCoordinate.y && coordinate.y > oldCoordinate.y - 2 && coordinate.x == oldCoordinate.x && !CheckOverlapPiece().Key)
                    {
                        return true;
                    }
                }
                else
                {
                    if (coordinate.y < oldCoordinate.y && coordinate.y > oldCoordinate.y - 3 && coordinate.x == oldCoordinate.x && !CheckBlockedPiece())
                    {
                        return true;
                    }
                }
                
                break;
            case PieceType.WhitePawn:
                if (moved)
                {
                    if (coordinate.y > oldCoordinate.y && coordinate.y < oldCoordinate.y + 2 && coordinate.x == oldCoordinate.x && !CheckOverlapPiece().Key)
                    {
                        return true;
                    }
                }
                else
                {
                    if (coordinate.y > oldCoordinate.y && coordinate.y < oldCoordinate.y + 3 && coordinate.x == oldCoordinate.x && !CheckBlockedPiece())
                    {
                        return true;
                    }
                }
                break;
            case PieceType.BlackBishop:
                if(Mathf.Abs((coordinate -  oldCoordinate).x) == Mathf.Abs((coordinate - oldCoordinate).y) && !CheckBlockedPiece())
                {
                    return true;
                }
                break;
            case PieceType.WhiteBishop:
                if (Mathf.Abs((coordinate - oldCoordinate).x) == Mathf.Abs((coordinate - oldCoordinate).y) && !CheckBlockedPiece())
                {
                    return true;
                }
                break;
            case PieceType.WhiteRook:
                if((coordinate - oldCoordinate).x == 0 || (coordinate - oldCoordinate).y == 0) 
                { 
                    return true; 
                }
                break;
            case PieceType.BlackRook:
                if ((coordinate - oldCoordinate).x == 0 || (coordinate - oldCoordinate).y == 0)
                {
                    return true;
                }
                break;
            case PieceType.WhiteKing:
                if((coordinate - oldCoordinate).magnitude < 2)
                {
                    return true;
                }
                break;
            case PieceType.BlackKing:
                if ((coordinate - oldCoordinate).magnitude < 2)
                {
                    return true;
                }
                break;
            case PieceType.WhiteKnight:
                if((Mathf.Abs((coordinate - oldCoordinate).x) == 1 
                    && Mathf.Abs((coordinate - oldCoordinate).y) == 2) 
                    || 
                    (Mathf.Abs((coordinate - oldCoordinate).y) == 1 
                    && Mathf.Abs((coordinate - oldCoordinate).x) == 2))
                {
                    return true;
                }
                break;
        }

        return false;
    }

    private void OnMouseDown()
    {
        SwitchState();   
    }
    private void OnMouseUp()
    {
        SwitchState();
        SnapToGrid();
    }
    private void OnValidate()
    {
        oldCoordinate = startCoordinate;
        coordinate = startCoordinate;
        AlignPieces();
        spriteRenderer.sprite = LoadSprite();
    }
}
