using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
	public Vector3Int position { get ; private set; }
	public	TetrominoData data { get ; private set; }
	public	Board board { get ; private set; }
	
	
	public int rotattionIndex{ get; private set;}
	
	
	public Vector3Int[] cells{ get ; private set; }
	
	
	public float stepDelay =1f;
	private float stepTime;
	
	public float lockDelay = 0.5f;
	private float lockTime;
	
	
	// Start is called before the first frame update
	void Start()
	{
        
	}

	// Update is called once per frame
	void Update()
	{
		this.board.Clear(this);
		
		this.lockTime += Time.time + this.stepDelay;
    	
		if (Input.GetKeyDown(KeyCode.E)) {
			Rotate(1);
		} else if (Input.GetKeyDown(KeyCode.Q)) {
			Rotate(-1);
		}    	
		if (Input.GetKeyDown(KeyCode.A)) {
			move(Vector2Int.left);
		} else if (Input.GetKeyDown(KeyCode.D)) {
			move(Vector2Int.right);
		}
    	
    	
		if (Input.GetKeyDown(KeyCode.S)) {
			move(Vector2Int.down);
		}
    	
    	
		// hard drop
		if (Input.GetKeyDown(KeyCode.Space)) {
			while (move(Vector2Int.down)) {
				continue;
			}
			
			Lock();
		}
		
		
		if(Time.time >= this.stepTime)
		{
			Step();
			
		}
    	
		this.board.Set(this);
	}
    
    
	public void Initialize(Vector3Int position, TetrominoData data, Board board)
	{
		this.lockTime = 0f;
    
		this.position = position;
		this.data = data;
		this.board = board;
		this.rotattionIndex = 0;
    	
		if (this.cells == null) {
			this.cells = new Vector3Int[data.cells.Length];
		}
    	
    	
		for (int i = 0; i < data.cells.Length; i++) {
			this.cells[i] = (Vector3Int)data.cells[i];
		}
	}
    
    
    
	private bool move(Vector2Int translateVect)
	{
		Vector3Int newPos = this.position;
		newPos.x += translateVect.x;
		newPos.y += translateVect.y;
    	
		bool valid = this.board.IsValidPosition(this, newPos);
    	
		if (valid){
			this.position = newPos;
			this.lockTime = 0f;
		}
    	
    	
		return valid;
	}
    
    
	private void Rotate(int direction)
	{
		int originalRotationIndex = this.rotattionIndex;
		this.rotattionIndex = Wrap(rotattionIndex + direction, 0, 4);
		
		ApplyRotationMatrix(direction);
		if(!TestWallkicks(rotattionIndex , direction))
		{
		   	this.rotattionIndex = originalRotationIndex;
		   	ApplyRotationMatrix(-direction);
		}
		

	}
	
	private void ApplyRotationMatrix(int direction)
	{
				for(int i = 0 ; i < cells.Length ; i++)
		{
			Vector3 cell = this.cells[i];
			int x,y;
			
			
			switch(data.tetromino)
			{
					
                case Tetromino.I:
                case Tetromino.O:

                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction)
                                        + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) 
                                        + (cell.y * Data.RotationMatrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) 
                                         + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) 
                                         + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }

            cells[i] = new Vector3Int(x, y, 0);
			
		
			
			
		}
	}
    
	private int Wrap(int input, int min, int max)
	{
		if (input < min) {
			return max - (min - input) % (max - min);
		} 
		else {
			return min + (input - min) % (max - min);
		}
	}
	
	private bool TestWallkicks(int rotattionIndex , int rotationDirection)
	{
		int wallkicksIndex = GetWallkicksIndex(rotattionIndex , rotationDirection);
		
		for(int i = 0 ; i < this.data.wallkicks.GetLength(1); i++)
		{
			Vector2Int translation = this.data.wallkicks[wallkicksIndex, i];
			
			
			if(move(translation))
				return true;
		}
		
		return false;
	}
    
	private int GetWallkicksIndex(int rotattionIndex , int rotationDirection)
	{
		int wallKicks = rotattionIndex *2;
		if(rotationDirection <0)
			wallKicks--;
		
		return Wrap(wallKicks , 0 , this.data.wallkicks.GetLength(0));
	}
	
	
	private void Step()
	{
		this.stepTime = Time.time + this.stepDelay;
		move(Vector2Int.down);
		
		if(this.lockTime >= this.lockDelay)
		{
			Lock();
		}
	}
	
	private void Lock()
	{
		this.board.Set(this);
		this.board.ClearLines();
		this.board.SpawnPiece();
		
	}

    
    
    
    
}
