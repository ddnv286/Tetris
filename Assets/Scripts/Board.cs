using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public TetrominoData[] tetrominoes;
    public Piece activatePiece { get; private set; }
    public Vector3Int spawnPosition;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public RectInt Bounds
    {
        get
        {
            //bound is from the very bottom-left to the top-right so we could take 
            //the position of the bottom-left corner and calc up the area (the size of the board)
            Vector2Int position = new Vector2Int(-this.boardSize.x / 2, -this.boardSize.y / 2);
            return new RectInt(position, this.boardSize);
        }
    }

    //run initialize
    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activatePiece = GetComponentInChildren<Piece>();
        //initialize each piece of tetromino
        for (int i = 0; i < this.tetrominoes.Length; i++ )
        {
            this.tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece();
    }

    //pick a random element from the tetromino array
    public void SpawnPiece ()
    {
        int random = Random.Range(0, this.tetrominoes.Length);
        TetrominoData data = this.tetrominoes[random];
        //give spawn position
        this.activatePiece.Initialize(this, this.spawnPosition, data);
        
        //check valid spawn, if not then it's game over
        if(isValidPosition(this.activatePiece, this.spawnPosition))
        {
            //get reference to game piece
            Set(this.activatePiece);
        } else
        {
            GameOver();
        }
    }

    //haven't added anything fancy, just clear the entire board when game is over
    private void GameOver()
    {
        this.tilemap.ClearAllTiles();
    }

    public void Set (Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            //offset this position of the piece
            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }

    public bool isValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = this.Bounds;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            //out of bound
            //using built-in func in RectInt
            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            //another tile occupied
            if (this.tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    //we'll check this from the bottom rows, if it isn't full we'll check the one right upper it
    public void ClearLines ()
    {
        RectInt bounds = this.Bounds;
        int row = bounds.yMin;
        while (row < bounds.yMax)
        {
            if (isLineFull(row))
            {
                LineClear(row);
            }
            else
            {
                row++;
            }
        }
    }

    private bool isLineFull (int row)
    {
        RectInt bounds = this.Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            if (!this.tilemap.HasTile(position))
            {
                return false;
            }
        }
        return true;
    }

    private void LineClear(int row)
    {
        //iterate from the bottom line to the top line and check if that line is full of tiles
        RectInt bounds = this.Bounds;
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            this.tilemap.SetTile(position, null);
        }

        //then get all the tiles from the upper lines
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = this.tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                this.tilemap.SetTile(position, above);
            }

            row++;
        }
    }
}