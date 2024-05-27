using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    private bool _inverse;

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
    private TimeBar.Role role;
    [SerializeField] public TimeBar timeswap;
    void Awake()
    {
        newPieces = new GameObject();
        oldPrefabPos = new Vector3(-6, -6, 0);
        prefabSize = new Vector2(0, 0);
        firstPrefab = new Vector3(oldPrefabPos.x, 0, 0);
        role = TimeBar.Role.Player;
        timeswap = FindObjectOfType<TimeBar>();
    }
    void Start()
    {
        _piecePrefabDict = new Dictionary<PieceType, GameObject>();
        for (int i = 0; i < piecePrefabs.Length; i++)
        {
            if (!_piecePrefabDict.ContainsKey(piecePrefabs[i].type))
            {
                _piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
            }
        }
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                GameObject bg = (GameObject)Instantiate(backgroundPrefab, GetWorldPosition(x * 2, y * 2, 0), Quaternion.identity);
                bg.transform.parent = transform;
            }
        }
        _pieces = new GamePieces[xDim, yDim];
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                SpawnNewPiece(x, y, PieceType.EMPTY);
            }
        }
        StartCoroutine(Fill());
    }

    public IEnumerator Fill()
    {
        // for (int i = 0; i < _pieces.GetLength(0); i++)
        // {
        //     for (int j = 0; j < _pieces.GetLength(1); j++)
        //     {
        //         GamePieces piece = _pieces[i, j];
        //         Debug.Log("Value of arrays _piece " + _pieces[i, j] + " is " + piece.X + " " + piece.Y);
        //     }
        // }
        bool needRefill = true;
        while (needRefill)
        {
            yield return new WaitForSeconds(FillTime);
            while (FillStep())
            {
                _inverse = !_inverse;
                yield return new WaitForSeconds(FillTime);
            }
            needRefill = ClearAllValidMatches();
        }
    }
    public bool FillStep()
    {
        bool movedPiece = false;
        for (int y = yDim - 2; y >= 0; y--)
        {
            for (int x = 0; x < xDim; x++)
            {
                GamePieces piece = _pieces[x, y];
                piece.name = "Piece(" + x + "," + y + ")" + "[" + piece.X + "," + piece.Y + "]";
                if (piece.IsMoveable())
                {
                    GamePieces pieceBelow = _pieces[x, y + 1];

                    if (pieceBelow.Type == PieceType.EMPTY)
                    {
                        Destroy(pieceBelow.gameObject);
                        //Debug.Log("Spawn piece for the second row and fill chessboard:  " + "[" + tmpx + "," + tmpy + "]");
                        piece.MovableComponent.Move(x, y + 1, FillTime);
                        _pieces[x, y + 1] = piece;
                        SpawnNewPiece(x, y, PieceType.EMPTY);
                        movedPiece = true;
                    }
                }
            }
        }
        for (int x = 0; x < xDim; x++)
        {
            GamePieces pieceBelow = _pieces[x, 0];
            if (pieceBelow.Type == PieceType.EMPTY)
            {
                Destroy(pieceBelow.gameObject);
                GameObject newPiece = (GameObject)Instantiate(_piecePrefabDict[PieceType.NORMAL], GetWorldPosition(x, -1, 0), Quaternion.identity);
                newPiece.transform.parent = transform;
                // cập nhật thông tin của gamepiece cho prefab piece mới
                _pieces[x, 0] = newPiece.GetComponent<GamePieces>();
                _pieces[x, 0].Init(x, -1, this, PieceType.NORMAL);
                _pieces[x, 0].MovableComponent.Move(x, 0, FillTime);
                int randomIndex = UnityEngine.Random.Range(0, _pieces[x, 0].ItemComponent.NumItems);
                _pieces[x, 0].ItemComponent.SetItem((ItemPieces.ItemType)randomIndex);
                movedPiece = true;
            }
        }
        return movedPiece;
    }
    void callChessBoard()
    {
        backgroundPositions = new Vector2[xDim, yDim];
        _piecePrefabDict = new Dictionary<PieceType, GameObject>();
        for (int i = 0; i < piecePrefabs.Length; i++)
        {
            if (!_piecePrefabDict.ContainsKey(piecePrefabs[i].type))
            {
                _piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
            }
        }
        Vector3 oldPrefabPos = new Vector3(-6, -6, -4);  // tạo một biến oldprefab chứa position
        Vector2 prefabSize = new Vector2(0, 0);
        Vector3 firstPrefab = new Vector3(oldPrefabPos.x, 0, 0);
        _pieces = new GamePieces[xDim, yDim];
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                // set POISITION cho prefabBackground
                GameObject background = (GameObject)Instantiate(backgroundPrefab, GetWorldPosition(oldPrefabPos.x, oldPrefabPos.y, -4), Quaternion.identity);

                background.name = "Prefab " + "[" + x + "," + y + "]" + "[" + oldPrefabPos.x + "," + oldPrefabPos.y + "," + oldPrefabPos.z + "]";
                prefabSize = GetPrefabSize(background);
                // lấy scale của prefab
                prefabWidth = Mathf.RoundToInt(prefabSize.x);
                prefabHeigt = Mathf.RoundToInt(prefabSize.y);

                background.transform.parent = transform;
                SpawnNewPiece(x, y, PieceType.EMPTY);
                backgroundPositions[x, y] = oldPrefabPos;           // SAU KHI DUYỆT XONG THÌ CỘNG TIẾP WIDTH CỦA PREFAB ĐỂ CÓ THỂ RESET LẠI DÒNG
                oldPrefabPos.x += prefabWidth;
            }
            prefabWidth = 0;
            oldPrefabPos.x = firstPrefab.x;  // số position x khi reset về vẫn chưa hoàng hảo lắm.
            oldPrefabPos.y += prefabHeigt;
        }
        for (int i = 0; i < backgroundPositions.GetLength(0); i++)
        {
            for (int j = 0; j < backgroundPositions.GetLength(1); j++)
            {
                Debug.Log("Position at background [" + i + ", " + j + "]: " + backgroundPositions[i, j]);
            }
        }
        // Destroy(_pieces[3, 3].gameObject);
        // SpawnNewPiece(3, 3, PieceType.BUBBLE, oldPrefabPos);
        //StartCoroutine(Fill());
    }


    public GamePieces SpawnNewPiece(int x, int y, PieceType type)
    {
        GameObject newPiece = (GameObject)Instantiate(_piecePrefabDict[type], GetWorldPosition(x, y, 0), Quaternion.identity);
        newPiece.transform.parent = transform;
        _pieces[x, y] = newPiece.GetComponent<GamePieces>();
        _pieces[x, y].Init(x, y, this, type);
        return _pieces[x, y];
    }
    private static bool IsAdjacent(GamePieces piece1, GamePieces piece2)
    {
        return (piece1.X == piece2.X && (int)Mathf.Abs(piece1.Y - piece2.Y) == 1) ||
        (piece1.Y == piece2.Y && (int)Mathf.Abs(piece1.X - piece2.X) == 1);

    }
    public void SwapPiece(GamePieces piece1, GamePieces piece2)
    {
        if (!piece1.IsMoveable() && !piece2.IsMoveable())
        {
            Debug.Log("Can't swap piece, so piece is null " + piece1 + " " + piece2);
            return;
        }
        _pieces[piece1.X, piece1.Y] = piece2;
        _pieces[piece2.X, piece2.Y] = piece1;
        var match1 = GetMatch(piece1, piece2.X, piece2.Y);
        var match2 = GetMatch(piece2, piece1.X, piece1.Y);
        if (match1 != null || match2 != null)
        {
            int piece1X = piece1.X;
            int piece1Y = piece1.Y;
            piece1.MovableComponent.Move(piece2.X, piece2.Y, FillTime);
            piece2.MovableComponent.Move(piece1X, piece1Y, FillTime);
            ClearAllValidMatches();
            StartCoroutine(Fill());
        }
        else
        {
            StartCoroutine(SwapPiecesBack(piece1, piece2, FillTime));
        }
    }
    IEnumerator SwapPiecesBack(GamePieces piece1, GamePieces piece2, float delay)
    {
        yield return new WaitForSeconds(delay);
        _pieces[piece1.X, piece1.Y] = piece2;
        _pieces[piece2.X, piece2.Y] = piece1;
        int piece1X = piece1.X;
        int piece1Y = piece1.Y;
        int piece2X = piece2.X;
        int piece2Y = piece2.Y;
        piece1.MovableComponent.Move(piece2.X, piece2.Y, FillTime);
        piece2.MovableComponent.Move(piece1X, piece1Y, FillTime);

        // Wait for another delay
        yield return new WaitForSeconds(delay);

        // Swap back
        _pieces[piece1.X, piece1.Y] = piece1;
        _pieces[piece2.X, piece2.Y] = piece2;
        piece1.MovableComponent.Move(piece1X, piece1Y, FillTime);
        piece2.MovableComponent.Move(piece2X, piece2Y, FillTime);
    }
    public List<GamePieces> GetMatch(GamePieces piece, int NewX, int NewY)
    {
        if (piece.IsItemed())
        {
            ItemPieces.ItemType type = piece.ItemComponent.Item;
            List<GamePieces> horizontalPieces = new List<GamePieces>();
            List<GamePieces> verticalPieces = new List<GamePieces>();
            List<GamePieces> matchingPieces = new List<GamePieces>();
            horizontalPieces.Add(piece);
            for (int dir = 0; dir <= 1; dir++)
            {
                for (int xOffset = 1; xOffset < xDim; xOffset++)
                {
                    int x;
                    if (dir == 0) x = NewX - xOffset; // left
                    else x = NewX + xOffset; // right
                    if (x < 0 || x >= xDim || xOffset >= xDim)
                    {
                        break;
                    }
                    if (_pieces[x, NewY].IsItemed() && _pieces[x, NewY].ItemComponent.Item == type)
                    {
                        horizontalPieces.Add(_pieces[x, NewY]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (horizontalPieces.Count >= 3)
            {
                for (int i = 0; i < horizontalPieces.Count; i++)
                {
                    matchingPieces.Add(horizontalPieces[i]);
                    Debug.Log("PieceTypeHorizontalList: " + horizontalPieces[i].ItemComponent.Item);
                }
            }

            // Traverse vertically if we found a match (3 or more pieces)
            if (horizontalPieces.Count >= 3)
            {
                for (int i = 0; i < horizontalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int yOffset = 1; yOffset < yDim; yOffset++)
                        {
                            int y;
                            if (dir == 0) y = NewY - yOffset;
                            else y = NewY + yOffset;
                            if (y < 0 || y >= yDim) break;
                            if (_pieces[horizontalPieces[i].X, y].IsItemed() && _pieces[horizontalPieces[i].X, y].ItemComponent.Item == type)
                            {
                                verticalPieces.Add(_pieces[horizontalPieces[i].X, y]);
                            }
                            else break;
                        }
                    }
                    if (verticalPieces.Count < 2)
                        verticalPieces.Clear();
                    else
                    {
                        for (int j = 0; j < verticalPieces.Count; j++)
                        {
                            matchingPieces.Add(verticalPieces[j]);
                        }
                        break;
                    }
                }
            }
            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }
            horizontalPieces.Clear();
            verticalPieces.Clear();
            //check verticalPiece-------------------
            verticalPieces.Add(piece);
            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yOffset = 1; yOffset < yDim; yOffset++)
                {
                    int y;
                    if (dir == 0) y = NewY - yOffset; // left
                    else y = NewY + yOffset; // right
                    if (y < 0 || y >= yDim || yOffset >= yDim)
                    {
                        break;
                    }
                    if (_pieces[NewX, y].IsItemed() && _pieces[NewX, y].ItemComponent.Item == type)
                    {
                        verticalPieces.Add(_pieces[NewX, y]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    matchingPieces.Add(verticalPieces[i]);
                    Debug.Log("PieceTypeVerticalList: " + verticalPieces[i].ItemComponent.Item);
                }
            }
            // Traverse horizontal if we found a match (3 or more pieces)
            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xOffset = 1; xOffset < xDim; xOffset++)
                        {
                            int x;
                            if (dir == 0) x = NewX - xOffset; // left
                            else x = NewX + xOffset; //right
                            if (x < 0 || x >= xDim) break;
                            if (_pieces[x, verticalPieces[i].Y].IsItemed() && _pieces[x, verticalPieces[i].Y].ItemComponent.Item == type)
                            {
                                verticalPieces.Add(_pieces[x, verticalPieces[i].Y]);
                            }
                            else break;
                        }
                    }
                    if (horizontalPieces.Count < 2)
                        horizontalPieces.Clear();
                    else
                    {
                        for (int j = 0; j < horizontalPieces.Count; j++)
                        {
                            matchingPieces.Add(horizontalPieces[j]);
                        }
                        break;
                    }
                }
            }
            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }
        }
        return null;
    }
    public bool ClearAllValidMatches()
    {
        bool needRefill = false;
        for (int y = 0; y < yDim; y++)
        {
            for (int x = 0; x < xDim; x++)
            {
                if (_pieces[x, y].IsClearable())
                {
                    List<GamePieces> match = GetMatch(_pieces[x, y], x, y);
                    if (match != null)
                    {
                        for (int i = 0; i < match.Count; i++)
                        {
                            if (ClearPiece(match[i].X, match[i].Y))
                            {
                                needRefill = true;
                                Debug.Log("PieceCleared: " + match[i].X + " " + match[i].Y + " " + "Piece: " + match[i].ItemComponent.Item);
                            }
                        }
                    }
                }
            }
        }
        return needRefill;
    }
    public bool ClearPiece(int x, int y)
    {
        if (_pieces[x, y].IsClearable() && !_pieces[x, y].ClearableComponent.IsBeingCleared)
        {
            _pieces[x, y].ClearableComponent.Clear();
            SpawnNewPiece(x, y, PieceType.EMPTY);
            return true;
        }
        return false;
    }
    Vector2 GetPrefabSize(GameObject prefab)
    {
        Renderer prefabRederer = prefab.GetComponent<Renderer>();
        if (prefabRederer != null)
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
            transform.position.x - xDim / 2.0f + x - 3,
            transform.position.y + yDim / 2.0f - y + 2,
            transform.position.z);
    }
    public void PressPiece(GamePieces piece)
    {
        pressedPiece = piece;
        Debug.Log("location for PressPiece: " + piece.X + " " + piece.Y);
    }
    public void EnterPiece(GamePieces piece)
    {
        enteredPiece = piece;
        Debug.Log("location for EnterPiece: " + piece.X + " " + piece.Y);
    }
    public void ReleasePiece()
    {
        if (pressedPiece == enteredPiece)
        {
            Debug.Log("Overlapping piece =((");
            return;
        }
        else
        {
            if (IsAdjacent(pressedPiece, enteredPiece))
            {
                Debug.Log("IsAdjacent is true =))");
                SwapPiece(pressedPiece, enteredPiece);
                //Debug.Log("piece after swap: "+ "Piece1: " + pressedPiece.X + " " + pressedPiece.Y + "Piece2: " + enteredPiece.X + " " + enteredPiece.Y);
            }
            timeswap.SwapRole();
        }
    }
}




// void callChessBoard()
//     {
//         backgroundPositions = new Vector2[xDim, yDim];
//         _piecePrefabDict = new Dictionary<PieceType, GameObject>();
//         for (int i = 0; i < piecePrefabs.Length; i++)
//         {
//             if (!_piecePrefabDict.ContainsKey(piecePrefabs[i].type))
//             {
//                 _piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
//             }
//         }
//         Vector3 oldPrefabPos = new Vector3(-6, -6, -4);  // tạo một biến oldprefab chứa position
//         Vector2 prefabSize = new Vector2(0, 0);
//         Vector3 firstPrefab = new Vector3(oldPrefabPos.x, 0, 0);
//         _pieces = new GamePieces[xDim, yDim];
//         for (int x = 0; x < xDim; x++)
//         {
//             for (int y = 0; y < yDim; y++)
//             {
//                 // set POISITION cho prefabBackground
//                 GameObject background = (GameObject)Instantiate(backgroundPrefab, GetWorldPosition(oldPrefabPos.x, oldPrefabPos.y, -4), Quaternion.identity);

//                 background.name = "Prefab " + "[" + x + "," + y + "]" + "[" + oldPrefabPos.x + "," + oldPrefabPos.y + "," + oldPrefabPos.z + "]";
//                 prefabSize = GetPrefabSize(background);
//                 // lấy scale của prefab
//                 prefabWidth = Mathf.RoundToInt(prefabSize.x);
//                 prefabHeigt = Mathf.RoundToInt(prefabSize.y);

//                 background.transform.parent = transform;
//                 SpawnNewPiece(x, y, PieceType.EMPTY, oldPrefabPos);
//                 backgroundPositions[x, y] = oldPrefabPos;           // SAU KHI DUYỆT XONG THÌ CỘNG TIẾP WIDTH CỦA PREFAB ĐỂ CÓ THỂ RESET LẠI DÒNG
//                 oldPrefabPos.x += prefabWidth;
//             }
//             prefabWidth = 0;
//             oldPrefabPos.x = firstPrefab.x;  // số position x khi reset về vẫn chưa hoàng hảo lắm.
//             oldPrefabPos.y += prefabHeigt;
//         }
//         for (int i = 0; i < backgroundPositions.GetLength(0); i++)
//         {
//             for (int j = 0; j < backgroundPositions.GetLength(1); j++)
//             {
//                 Debug.Log("Position at background [" + i + ", " + j + "]: " + backgroundPositions[i, j]);
//             }
//         }
//         // Destroy(_pieces[3, 3].gameObject);
//         // SpawnNewPiece(3, 3, PieceType.BUBBLE, oldPrefabPos);
//         //StartCoroutine(Fill());
//     }