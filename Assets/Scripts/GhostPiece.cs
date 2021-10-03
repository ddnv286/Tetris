using UnityEngine.Tilemaps;
using UnityEngine;

public class GhostPiece : MonoBehaviour
{
    public Tile tile;
    public Board board;
    public Piece trackingPiece;

    public Tilemap tilemap { get; private set; }
    public Vector3Int position { get; private set; }
    public Vector3Int[] cells { get; private set; }

    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.cells = new Vector3Int[4];
    }

    private void LateUpdate ()
    {
        Clear();
        Copy();
        Drop();
        Set();
    }

    //so the logic for the ghost piece would be
    //copy the tile map of the active piece to tracking
    //kinda hard drop it so it stays at the bottom of the border
    private void Clear()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tilePosition = this.cells[i] + this.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }

    //copy all the piece's cells data to ghost piece
    private void Copy()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            this.cells[i] = this.trackingPiece.cells[i];
        } 
    }

    //iterate throughout the board to find the suitable place to 
    private void Drop()
    {
        Vector3Int position = this.trackingPiece.position;
        
        int current = position.y;
        int bottome = -this.board.boardSize.y / 2 - 1;

        //clear found position before dropping
        this.board.Clear(this.trackingPiece);
        //simulate the piece dropping
        for (int row = current; row >= bottome; row--)
        {
            position.y = row;

            if (this.board.isValidPosition(this.trackingPiece, position))
            {
                this.position = position;
            } else
            {
                break;
            }
        }
        this.board.Set(this.trackingPiece);
    }

    private void Set()
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tilePosition = this.cells[i] + this.position;
            //offset this position of the piece
            this.tilemap.SetTile(tilePosition, this.tile);
        }
    }   
}
