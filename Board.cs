
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
	public TetrominoData[] tetrominoes;
	public Tilemap tilemap;
	
	public Piece activePiece{ get ; private set; }
	public Vector3Int spawnPosition;
	
	
	public Vector2Int boardSize = new Vector2Int(10,20);
	public RectInt Bounds
	{
		get
		{
			Vector2Int position = new Vector2Int(-this.boardSize.x/2 , -this.boardSize.y/ 2);
			
			return (new RectInt( position, this.boardSize));
		}
	}
	
	void Awake()
	{
		this.tilemap = GetComponentInChildren<Tilemap>();
		this.activePiece = GetComponentInChildren<Piece>();
		for (int i = 0; i < tetrominoes.Length; i++) {
			this.tetrominoes[i].Initialize();
		}
		

	}
	// Start is called before the first frame update
	void Start()
	{
		SpawnPiece();
        
	}

	// Update is called once per frame
	void Update()
	{
        
	}
    
    
	public void SpawnPiece()
	{
		int randomIndex = Random.Range(0, this.tetrominoes.Length);
		TetrominoData data = this.tetrominoes[randomIndex];
		//this.activePiece.Intitialize(this ,spawnPosition , data);
		activePiece.Initialize(this.spawnPosition, data, this);
    	

		
		if(IsValidPosition(this.activePiece ,spawnPosition)){
			Set(this.activePiece);}
		else
			GameOver();
	}
    
	public void Set(Piece piece)
	{
		for (int i = 0; i < piece.cells.Length; i++) {
			Vector3Int tileposition = (Vector3Int)piece.cells[i] + piece.position;
			this.tilemap.SetTile(tileposition, piece.data.tile);
		}
	}
    
   	public void Clear(Piece piece)
	{
		for (int i = 0; i < piece.cells.Length; i++) {
			Vector3Int tileposition = (Vector3Int)piece.cells[i] + piece.position;
			this.tilemap.SetTile(tileposition, null);
		}
	} 
	public bool IsValidPosition(Piece piece, Vector3Int position)
	{
		RectInt bounds = this.Bounds;
		for (int i = 0; i < piece.cells.Length; i++) {
			
			Vector3Int tilePosition = piece.cells[i] + position;
			
			if(!bounds.Contains((Vector2Int)tilePosition))
			{
				return false;
			}
			
			if(this.tilemap.HasTile(tilePosition))
			{
				return false;
			}
			
			
		}
		
		
		return true;
	}
	
	
	public void ClearLines()
	{
		RectInt bounds = this.Bounds;
		int row = bounds.yMin;
		
		
		while(row < bounds.yMax)
		{
			if(IsFullLine(row)){
				LineClear(row);
			}
			else
			{
				row++;
			}
			
		}
	}
	
	
	private bool IsFullLine(int row)
	{
		RectInt bounds = this.Bounds;
		for(int col = bounds.xMin ; col < bounds.xMax ; col++)
		{
			Vector3Int position = new Vector3Int(col,row,0);
			
			
			if(!this.tilemap.HasTile(position))
			{
				return false;
			}
		}
		
		
		return true;
	}
	
	
	private void LineClear(int row)
	{
		RectInt bounds = this.Bounds;
		for(int col = bounds.xMin ; col < bounds.xMax ; col++)
		{
			Vector3Int position = new Vector3Int(col,row,0);
			this.tilemap.SetTile(position , null);
		}
			
		while(row < bounds.yMax)
			{
				for(int col = bounds.xMin ; col < bounds.xMax ; col++)
				{
					//                                     ******
					Vector3Int position = new Vector3Int(col,row+1,0);
					TileBase above = this.tilemap.GetTile(position);
					
					//                            ***
					position = new Vector3Int(col,row,0);
					this.tilemap.SetTile(position , above);
					
					
				}
				
				row++;
		}
	}
	
	
	private void GameOver()
	{
		this.tilemap.ClearAllTiles();
	}
}
