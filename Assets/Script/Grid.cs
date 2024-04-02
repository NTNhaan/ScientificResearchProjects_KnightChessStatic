using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public enum PieceType
    {
        NORMAL,
        BUBBLE,
        EMPTY,
        COUNT,
    }
    [System.Serializable]
    public struct PiecePrefab
    {
        public PieceType type;
        public GameObject prefab;
    }
    [System.Serializable]
    public struct PiecePosition
    {
        public PieceType type;
        public int x;
        public int y;
    };
    
    public int xDim;  
    public int yDim;  
    public float FillTime; 

    
    public PiecePrefab[] piecePrefabs;   // the array containing the image object is displayed on chessboard
    public GameObject backgroundPrefab;   // backgound for chessboard
    public PiecePosition[] initialPieces;

    private Dictionary<PieceType, GameObject> _piecePrefabDict;
    private GamePieces[,] _pieces;
    public Vector2[,] backgroundPositions;

    private GameObject prefab;  // the variable must be declared prior to use
    private float prefabWidth;
    private float prefabHeigt;
    // biến nãy sinh trong quá trình
    private GameObject newPieces;

    public Vector3 oldPrefabPos;  // tạo một biến oldprefab chứa position
    public Vector2 prefabSize;
    public Vector3 firstPrefab;
    private GamePieces pressedPiece;
    private GamePieces enteredPiece;
    // Update is called once per 
    void Start()
    {
        callChessBoard();   
        newPieces = new GameObject();
        oldPrefabPos = new Vector3(-6,-6, 0);
        prefabSize = new Vector2(0,0);
        firstPrefab = new Vector3(oldPrefabPos.x,0,0);
    }
    void Update()
    {

    } 

    public IEnumerator Fill()
    {
        while(FillStep())
        {
            yield return new WaitForSeconds(FillTime);
        }
        for(int i=0 ; i<_pieces.GetLength(0); i++)
        {
            for(int j=0 ; j<_pieces.GetLength(1) ; j++)
            {
                GamePieces piece = _pieces[i, j];
                Debug.Log("Value of arrays _piece " + _pieces[i, j] + " is " + piece.X + " " + piece.Y);
            }
        }
    }
    public bool FillStep()
    {
        Vector3 PiecePosition = new Vector3();
        bool movedPiece = false;
        for(int y = yDim-2 ; y >= 0 ; y--)
        {
            for(int x=0 ; x < xDim ; x++)
            {
                PiecePosition = backgroundPositions[y, x];
                int tmpx = Mathf.FloorToInt(PiecePosition.x);
                int tmpy = Mathf.FloorToInt(PiecePosition.y);
                GamePieces piece = _pieces[x, y];
                piece.name = "Piece(" + x + "," + y + ")" + "[" + tmpx + "," + tmpy + "]";
                if(piece.IsMoveable())
                {
                    GamePieces pieceBelow = _pieces[x, y+1];
                    if(pieceBelow.Type == PieceType.EMPTY)
                    {
                        Destroy(pieceBelow.gameObject);
                        Debug.Log("Spawn piece for the second row and fill chessboard:  " + "[" + tmpx +","+ tmpy + "]");
                        piece.MovableComponent.Move(tmpx ,tmpy+3, FillTime);
                        _pieces[x, y+1] = piece;
                        SpawnNewPiece(x, y, PieceType.EMPTY, PiecePosition);
                        movedPiece = true;
                    }   
                }
            }
        }
        for(int x=0 ; x<xDim ; x++)
        {
                int tmpx = Mathf.FloorToInt(PiecePosition.x-15);
                int tmpy = Mathf.FloorToInt(PiecePosition.y);
            GamePieces pieceBelow = _pieces[x, 0];
            if(pieceBelow.Type == PieceType.EMPTY)
            {
                Destroy(pieceBelow.gameObject);
                GameObject newPiece  = (GameObject)Instantiate(_piecePrefabDict[PieceType.NORMAL], Vector3.zero, Quaternion.identity);
                Debug.Log("Spawn piece for the top row is "+ x + ": " + "[" + tmpx +","+ tmpy + "]");
                newPiece.transform.parent = transform;
                // cập nhật thông tin của gamepiece cho prefab piece mới
                _pieces[x, 0] = newPiece.GetComponent<GamePieces>();
                _pieces[x, 0].Init(tmpx, tmpy, this, PieceType.NORMAL);
                _pieces[x, 0].MovableComponent.Move(tmpx , tmpy, FillTime);
                int randomIndex = Random.Range(0, _pieces[x, 0].ItemComponent.NumItems);
                _pieces[x, 0].ItemComponent.SetItem((ItemPieces.ItemType)randomIndex);
                movedPiece = true;
            }
            PiecePosition.x += 3;
        }
        return movedPiece;
    }
    void callChessBoard()
    { 
        backgroundPositions = new Vector2[xDim, yDim];
        _piecePrefabDict = new Dictionary<PieceType, GameObject> ();
        for(int i=0 ; i< piecePrefabs.Length ; i++)
        {
            if(!_piecePrefabDict.ContainsKey(piecePrefabs[i].type))
            {
                _piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
            }
        }
        Vector3 oldPrefabPos = new Vector3(-6,-6,-4);  // tạo một biến oldprefab chứa position
        Vector2 prefabSize = new Vector2(0,0);
        Vector3 firstPrefab = new Vector3(oldPrefabPos.x,0,0);
        _pieces = new GamePieces[xDim, yDim];
        for(int x=0 ; x < xDim ; x++)
        {
            for(int y=0 ; y < yDim ; y++)
            {
                // set POISITION cho prefabBackground
                GameObject background = (GameObject)Instantiate(backgroundPrefab, GetWorldPosition(oldPrefabPos.x, oldPrefabPos.y, -4), Quaternion.identity);

                background.name = "Prefab "+ "["+x +"," +y  + "]" + "[" + oldPrefabPos.x + "," + oldPrefabPos.y + "," + oldPrefabPos.z +"]";
                prefabSize = GetPrefabSize(background); 
  // lấy scale của prefab
                prefabWidth = Mathf.RoundToInt(prefabSize.x);
                prefabHeigt = Mathf.RoundToInt(prefabSize.y);

                background.transform.parent = transform;
                SpawnNewPiece(x, y, PieceType.EMPTY, oldPrefabPos);
                backgroundPositions[x, y] = oldPrefabPos;           // SAU KHI DUYỆT XONG THÌ CỘNG TIẾP WIDTH CỦA PREFAB ĐỂ CÓ THỂ RESET LẠI DÒNG
                oldPrefabPos.x += prefabWidth;
            }
            prefabWidth = 0;
            oldPrefabPos.x = firstPrefab.x;  // số position x khi reset về vẫn chưa hoàng hảo lắm.
            oldPrefabPos.y += prefabHeigt;
        }
        for(int i = 0; i < backgroundPositions.GetLength(0); i++)
        {
            for(int j = 0; j < backgroundPositions.GetLength(1); j++)
            {
                Debug.Log("Position at background [" + i + ", " + j + "]: " + backgroundPositions[i, j]);
            }
        }
        // Destroy(_pieces[3, 3].gameObject);
        // SpawnNewPiece(3, 3, PieceType.BUBBLE, oldPrefabPos);
        StartCoroutine(Fill());
    }


    public GamePieces SpawnNewPiece(int x, int y, PieceType type, Vector3 newposition)
    {
        // int tmpx = Mathf.FloorToInt((newposition.x + 6) /  prefabWidth);
        // int tmpy = Mathf.FloorToInt((newposition.y + 6) /  prefabWidth);

        GameObject newPiece = (GameObject)Instantiate(_piecePrefabDict[type], GetWorldPosition(newposition.x, newposition.y, -5), Quaternion.identity);
        newPiece.transform.parent = transform;
        _pieces[x, y] = newPiece.GetComponent<GamePieces>();
        _pieces[x, y].Init(x, y, this, type);
        return _pieces[x, y];
    }
    private static bool IsAdjacent(GamePieces piece1, GamePieces piece2)
    {
        int dx = Mathf.Abs(piece1.Y - piece2.Y);
        int dy = Mathf.Abs(piece1.X - piece2.X);
        return (piece1.X == piece2.X && (int)Mathf.Abs(piece1.Y - piece2.Y) == 3) || 
        (piece1.Y == piece2.Y && (int)Mathf.Abs(piece1.X - piece2.X) == 3);
        
    }
    public void ChangeData(int tmp1x, int tmp1y, GamePieces buttonPiece)
    {
        for(int i=0 ; i<xDim ; i++)
        {
            for(int j=0 ; j<yDim ; j++)
            {
                var value = _pieces[i, j];
                if(value == buttonPiece)
                {
                    tmp1x = i;
                    tmp1y = j;

                    Debug.Log("Checked data for piece: " + " arrays: " + value + " prefab: " + buttonPiece);
                }
            }
        }
    }
    public void SwapPiece(GamePieces pieces1, GamePieces pieces2)
    {
        if(!pieces1.IsMoveable() && !pieces2.IsMoveable())
        {
            Debug.Log("Can't swap piece, so piece is null " + pieces1 + " " + pieces2);
            return;
        }
        int tmp1x=0, tmp1y=0, tmp2x=0, tmp2y=0;
        ChangeData(tmp1x, tmp1y, pieces1);
        ChangeData(tmp2x, tmp2y, pieces2);
        // for(int i=0 ; i<xDim ; i++)
        // {
        //     for(int j=0 ; j<yDim ; j++)
        //     {
        //         var value = _pieces[i, j];
        //         if(value == pieces1)
        //         {
        //             tmp1x = i;
        //             tmp1y = j;
        //             Debug.Log("..........Checked 1 " + " arrays: " + value + " prefab: " + pieces1);
        //         }
        //         else if(value == pieces2)
        //         {
        //             tmp2x = i;
        //             tmp2y = j;
        //             Debug.Log("..........Checked 2 " + " arrays: " + value + " prefab: " + pieces2);
        //         }
        //     }
        // }
            _pieces[tmp1x, tmp1y] = pieces2;
            _pieces[tmp2x, tmp2y] = pieces1;
            if(GetMatch(pieces1, pieces2.X, pieces2.Y) != null || GetMatch(pieces2, pieces1.X, pieces1.Y) != null)
            {
                int piece1X = pieces1.X;
                int piece1Y = pieces1.Y;
                pieces1.MovableComponent.Move(pieces2.X, pieces2.Y, FillTime);
                pieces2.MovableComponent.Move(piece1X, piece1Y, FillTime);   
            }      
            else
            {
                _pieces[tmp1x, tmp1y] = pieces1;
                _pieces[tmp2x, tmp2y] = pieces2;
            }   
    }
    public void PressPiece(GamePieces piece)
    {
            pressedPiece = piece;
            Debug.Log("location for PressPiece: " + piece.X +" "+ piece.Y);
    }
    public void EnterPiece(GamePieces piece)
    {
        enteredPiece = piece;
        Debug.Log("location for EnterPiece: " + piece.X +" "+ piece.Y);
    }
    public void ReleasePiece(){
        if(pressedPiece == enteredPiece)
        {
            Debug.Log("Overlapping piece =((");
            return;
        }
        else
        {
            if(IsAdjacent(pressedPiece, enteredPiece))
            {
                Debug.Log("IsAdjacent is true =))");
                SwapPiece(pressedPiece, enteredPiece);
                Debug.Log("piece after swap: "+ "Piece1: " + pressedPiece.X + " " + pressedPiece.Y + "Piece2: " + enteredPiece.X + " " + enteredPiece.Y);
            }
        }
    }
    public List<GamePieces> GetMatch(GamePieces piece, int NewX, int NewY)
    {
        if(piece.IsItemed())
        {
            ItemPieces.ItemType type = piece.ItemComponent.Item;
            List<GamePieces> horizontalPieces = new List<GamePieces>();
            List<GamePieces> verticalPieces = new List<GamePieces>();
            List<GamePieces> matchingPieces = new List<GamePieces>();
            GamePieces piecetest = new GamePieces();
            horizontalPieces.Add(piece);
            int tmpx=0,tmpy=0;
            for(int dir=0 ; dir <= 1 ; dir++)
            {
                for(int count=1, xOffset=3 ; count<6; count++, xOffset+=3)
                {
                    int x;
                    if(dir == 0) x = NewX - xOffset; // left
                    else x = NewX + xOffset; // right
                    
                    if(x<0 || x>=xDim || NewY<0 || NewY>=yDim)
                    {
                        Debug.Log("position over valiable arrays: " + "X: " + x + " Y: " + NewY + " " + "Prefab: " + piece + " " + "Type: " + type);
                        break;
                    }

                    piecetest.X = x;
                    piecetest.Y = NewY;
                    Debug.Log("Data first change: " + "X: " + x + " Y: " + NewY + " " + "Prefab: " + piecetest);
                    ChangeData(tmpx, tmpy, piecetest);
                    Debug.Log("Data after change: " + "X: " + tmpx + " Y: " + tmpy + " " + "Piece: " + _pieces[tmpx, tmpy] + " " + "Prefab: " + piecetest);
                    if(_pieces[tmpx, tmpy].IsItemed() && _pieces[tmpx, tmpy].ItemComponent.Item == type)
                    {
                        horizontalPieces.Add(_pieces[tmpx, tmpy]);
                    }
                    else
                    {
                        Debug.Log("Piece is itemed: " + _pieces[tmpx, tmpy].ItemComponent.Item);
                        break;
                    }
                    
                }
            }
            if(horizontalPieces.Count >= 3)
            {
                for(int i=0 ; i<horizontalPieces.Count ; i++)
                {
                    matchingPieces.Add(horizontalPieces[i]);
                }
            }
            if(matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }

            // check verticalPiece-------------------
            verticalPieces.Add(piece);
            for(int dir=0 ; dir <= 1 ; dir++)
            {
                for(int count=1, yOffset=1 ; count<6 ;count++, yOffset++)
                {
                    int y;
                    if(dir == 0) y = NewY - yOffset; // up
                    else y = NewX + yOffset; // down
                    
                    if(y<0 || y>=yDim)
                    {
                        break;
                    }
                    piecetest.X = NewX;
                    piecetest.Y = y;
                    
                    ChangeData(tmpx, tmpy, piecetest);
                    if(_pieces[tmpx, tmpy].IsItemed() && _pieces[tmpx, tmpy].ItemComponent.Item == type)
                    {
                        verticalPieces.Add(_pieces[tmpx, tmpy]);
                    }
                    else break;
                }
            }
            if(verticalPieces.Count >= 3)
            {
                for(int i=0 ; i<verticalPieces.Count ; i++)
                {
                    matchingPieces.Add(verticalPieces[i]);
                }
            }
            if(matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }
        }
        return null;
    }

    Vector2 GetPrefabSize(GameObject prefab)
    {
        Renderer prefabRederer = prefab.GetComponent<Renderer>();
        if(prefabRederer != null)
        {
            return new Vector2(prefabRederer.bounds.size.x, prefabRederer.bounds.size.y);
        }
        else
        {
            Debug.LogError("check prefab Renderer");
            return Vector2.zero;
        }
    }   
    public Vector3 GetWorldPosition(float x, float y, float z)
    {
        return new Vector3(
            transform.position.x - xDim/2.0f + x+1.5f,
            transform.position.y + yDim/2.0f - y-2.5f,
            transform.position.z);
    }
}
