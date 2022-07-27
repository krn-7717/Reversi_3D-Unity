using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OthelloSystem : MonoBehaviour
{

    const int FIELD_SIZE_X  = 8;
    const int FIELD_SIZE_Y  = 8;

    //ブロックの状態
    public enum eCharacterState {
        None,
        Face,
        Back,

        Max
    };

    //ボードの実体
    private GameObject _boardObject = null;

    //ブロックの実体
    private GameObject[,] _fieldCharactersObject = new GameObject[FIELD_SIZE_Y, FIELD_SIZE_X];
    private OthelloCharacter[,] _fieldCharacters = new OthelloCharacter[FIELD_SIZE_Y, FIELD_SIZE_X];

    //最終的なブロックの状態
    private eCharacterState[,] _fieldCharactersStateFinal = new eCharacterState[FIELD_SIZE_Y, FIELD_SIZE_X];

    //カーソルの実体
    private GameObject _cursorObject = null;

    [SerializeField] GameObject _boardPrefab = null;
    [SerializeField] GameObject _characterPrefab = null;
    [SerializeField] GameObject _cursorPrefab = null;

    //カーソル制御用
    private int _cursorX = 0;
    private int _cursorY = 0;

    //ターン制御
    private eCharacterState _turn = eCharacterState.Back;

    //ひっくり返す対象
    class Position {
        public int _x;
        public int _y;

        public Position(int x, int y) {
            _x = x;
            _y = y;
        }
    }

    //ひっくり返す処理の方向
    int[] TURN_CHECK_X = new int[] {-1, -1, 0, 1, 1, 1, 0, -1};
    int[] TURN_CHECK_Y = new int[] {0, 1, 1, 1, 0, -1, -1, -1};

    // Start is called before the first frame update
    void Start()
    {
        //初期化状態の設定
        for (int i = 0; i < FIELD_SIZE_Y; i++) {
            for (int j = 0; j < FIELD_SIZE_X; j++) {
                //ブロックの実体
                GameObject newObject = GameObject.Instantiate<GameObject>(_characterPrefab);
                OthelloCharacter newCharacter = newObject.GetComponent<OthelloCharacter>();
                newObject.transform.localPosition = new Vector3(-(FIELD_SIZE_X - 1) * 0.5f + j, 0.125f, -(FIELD_SIZE_Y - 1) * 0.5f + i);
                _fieldCharactersObject[i, j] = newObject;
                _fieldCharacters[j, i] = newCharacter;

                //ブロックの状態
                _fieldCharactersStateFinal[i, j] = eCharacterState.None;
            }
            _fieldCharactersStateFinal[3, 3] = eCharacterState.Face;
            _fieldCharactersStateFinal[4, 3] = eCharacterState.Back;
            _fieldCharactersStateFinal[3, 4] = eCharacterState.Back;
            _fieldCharactersStateFinal[4, 4] = eCharacterState.Face;
        }
        //ボードの生成
        _boardObject = GameObject.Instantiate<GameObject>(_boardPrefab);

        //カーソルの生成
        _cursorObject = GameObject.Instantiate<GameObject>(_cursorPrefab);

    }

    // Update is called once per frame
    void Update()
    {
        //カーソルの移動
        int deltaX = 0;
        int deltaY = 0;
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            deltaY += 1;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            deltaY -= 1;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            deltaX -= 1;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            deltaX += 1;
        }
        _cursorX += deltaX;
        _cursorY += deltaY;
        _cursorObject.transform.localPosition = new Vector3(-(FIELD_SIZE_X - 1) * 0.5f + _cursorX, 0.5f, -(FIELD_SIZE_Y - 1) * 0.5f + _cursorY);

        if (Input.GetKeyDown(KeyCode.Return)) {
            if (0 <= _cursorX && _cursorX < FIELD_SIZE_X && 0 <= _cursorY && _cursorY < FIELD_SIZE_Y && _fieldCharactersStateFinal[_cursorY, _cursorX] == eCharacterState.None && Turn(false) > 0) {
                _fieldCharactersStateFinal[_cursorY, _cursorX] = _turn;
                Turn(true);
                _turn = ((_turn == eCharacterState.Back) ? eCharacterState.Face : eCharacterState.Back);
            }
        }
        
        //ブロックの状態を更新
        UpdateCharacterState();
    }

    int Turn(bool isTurn) {
        //相手の色
        eCharacterState opponentColor = ((_turn == eCharacterState.Back) ? eCharacterState.Face : eCharacterState.Back);

        //ひっくり返せる数
        bool isValidTurn = false; //ひっくり返せるかどうか
        List<Position> positionList = new List<Position>();
        int count = 0;

        int deltaX = 0, deltaY = 0;
        for (int i = 0; i < TURN_CHECK_X.Length; i++) {
            int x = _cursorX;
            int y = _cursorY;

            deltaX = TURN_CHECK_X[i];
            deltaY = TURN_CHECK_Y[i];

            isValidTurn = false;
            positionList.Clear();

            while (true) {
                x += deltaX;
                y += deltaY;
                if (!(0 <= x && x < FIELD_SIZE_X && 0 <= y && y < FIELD_SIZE_Y)) {
                    //範囲外
                    break;
                }
                if (_fieldCharactersStateFinal[y, x] == opponentColor) {
                    //ひっくり返す対象
                    positionList.Add(new Position(x, y));    
                } else if (_fieldCharactersStateFinal[y, x] == _turn) {
                    //ひっくり返せる
                    isValidTurn = true;
                    break;
                } else {
                    //なにもない
                    break;
                }
            }
            //実際のひっくり返す処理
            if (isValidTurn) {
                count += positionList.Count;

                if (isTurn) {
                    for (int j = 0; j < positionList.Count; j++) {
                        Position pos = positionList[j];
                        _fieldCharactersStateFinal[pos._y, pos._x] = _turn;
                    }
                }
            }
        }
        return count;
    }

    void UpdateCharacterState() {
        //ブロックの状態反映
        for (int i = 0; i < FIELD_SIZE_Y; i++) {
            for (int j = 0; j < FIELD_SIZE_X; j++) {
                //ブロックの状態
                _fieldCharacters[i, j].SetState(_fieldCharactersStateFinal[j, i]);
            }
        }
    }
}
