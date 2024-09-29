using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class 玩家代码 : MonoBehaviour
{
    public bool over = false;

    public Vector3 zeroPointPos;
    public float cellWidth;
    public PieceColor color = PieceColor.Black;

    private int row;
    private int column;

    public GameObject BlackPiece;
    public GameObject WhitePiece;

    public List<棋子代码> CurrentPieceList = new List<棋子代码>();

    public GameObject gameOverText;
    public GameObject restartButton;
    // Start is called before the first frame update
    void Start()
    {
        gameOverText = GameObject.Find("GameOverText");
        if (gameOverText != null)
        {
            gameOverText.SetActive(false);
        }
        restartButton = GameObject.Find("Button");
        if (restartButton != null)
        {
            restartButton.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //鼠标点击
        if (Input.GetMouseButtonDown(0) && over == false) 
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -Camera.main.transform.position.z;

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            Vector3 offsetPos = worldPos - zeroPointPos;
            column = (int)Mathf.Round(offsetPos.y / cellWidth);
            row = (int)Mathf.Round(offsetPos.x / cellWidth);

            int[] RowColumnValue = { row, column };

            //判断是否越界
            if (row < 0 || row > 14 || column < 0 || column > 14) return;
            //判断是否已经存在棋子
            CurrentPieceList=GameObject.FindObjectsOfType<棋子代码>().ToList();
            foreach(var piece in CurrentPieceList)
            {
                if(piece.row == row && piece.column == column)
                {
                    return;
                }
            }

            Vector3 piecePos = new Vector3(row * cellWidth, column * cellWidth, zeroPointPos.z) + zeroPointPos;

            //生成棋子
            GameObject newPiece;
            棋子代码 currentPiece = new 棋子代码() ;
            if (color == PieceColor.Black)
            {
                if (BlackPiece != null)
                {
                    newPiece = Instantiate(BlackPiece, piecePos, BlackPiece.transform.rotation);
                    color= PieceColor.White;

                    // 获取棋子代码脚本并设置行和列值
                    棋子代码 pieceScript = newPiece.GetComponent<棋子代码>();
                    pieceScript.SetRowColumnValue(new int[] { row, column });

                    currentPiece = newPiece.GetComponent<棋子代码>();
                }
            }
            else
            {
                if (WhitePiece != null)
                {
                    newPiece = Instantiate(WhitePiece, piecePos, WhitePiece.transform.rotation);
                    color= PieceColor.Black;
                     
                    // 获取棋子代码脚本并设置行和列值
                    棋子代码 pieceScript = newPiece.GetComponent<棋子代码>();
                    pieceScript.SetRowColumnValue(new int[] { row, column });

                    currentPiece = newPiece.GetComponent<棋子代码>();
                }
            }

            //判断五子相连
            CurrentPieceList=GameObject.FindObjectsOfType<棋子代码>().ToList();
            bool isFive = JudgeFivePiece(CurrentPieceList, currentPiece);
            if (isFive)
            {
                //游戏结束
                if (gameOverText != null)
                {
                    gameOverText.SetActive(true);
                    restartButton.SetActive(true);
                    over = true;
                }
            }
        }
    }

    bool JudgeFivePiece(List<棋子代码> currentList,棋子代码 currentPiece)
    {
        bool result=false;
        List<棋子代码>CurrentColorList=currentList.Where(en=>en.color==currentPiece.color).ToList();
        var UpList = GetSamePieceByDirection(CurrentColorList, currentPiece, DirectionPiece.Up);
        var DownList = GetSamePieceByDirection(CurrentColorList, currentPiece, DirectionPiece.Down);
        var LeftList = GetSamePieceByDirection(CurrentColorList, currentPiece, DirectionPiece.Left);
        var RightList = GetSamePieceByDirection(CurrentColorList, currentPiece, DirectionPiece.Right);
        var LeftUpList = GetSamePieceByDirection(CurrentColorList, currentPiece, DirectionPiece.LeftUp);
        var RightUpList = GetSamePieceByDirection(CurrentColorList, currentPiece, DirectionPiece.RightUp);
        var LeftDownList = GetSamePieceByDirection(CurrentColorList, currentPiece, DirectionPiece.LeftDown);
        var RightDownList = GetSamePieceByDirection(CurrentColorList, currentPiece, DirectionPiece.RightDown);

/*        print(UpList.Count + ":" + DownList.Count + ":" + LeftList.Count + ":" + RightList.Count + ":" + LeftUpList.Count + ":" + RightUpList.Count + ":" + LeftDownList.Count + ":" + RightDownList.Count);
*/

        if (UpList.Count + DownList.Count >= 4 ||
            LeftList.Count + RightList.Count >= 4 ||
            LeftUpList.Count + RightDownList.Count >= 4 ||
            RightUpList.Count + LeftDownList.Count >= 4) 
        {
            result = true;
        }

        return result;
    }

    List<棋子代码> GetSamePieceByDirection(List<棋子代码>currentColorList,棋子代码 currentPiece,DirectionPiece direction)
    {
        List<棋子代码> result=new List<棋子代码> ();
        switch (direction)
        {
            case DirectionPiece.Up:
                foreach (var item in currentColorList)
                {
                    if (item.row == currentPiece.row && item.column == currentPiece.column + 1) 
                    {
                        result.Add(item);
                        var resultList=GetSamePieceByDirection(currentColorList,item,DirectionPiece.Up);
                        result.AddRange(resultList);
                    }
                }
                break;
            case DirectionPiece.Down:
                foreach (var item in currentColorList)
                {
                    if (item.row == currentPiece.row && item.column == currentPiece.column - 1) 
                    {
                        result.Add(item);
                        var resultList = GetSamePieceByDirection(currentColorList, item, DirectionPiece.Down);
                        result.AddRange(resultList);
                    }
                }
                break;
            case DirectionPiece.Left:
                foreach (var item in currentColorList)
                {
                    if (item.row == currentPiece.row - 1 && item.column == currentPiece.column) 
                    {
                        result.Add(item);
                        var resultList = GetSamePieceByDirection(currentColorList, item, DirectionPiece.Left);
                        result.AddRange(resultList);
                    }
                }
                break;
            case DirectionPiece.Right:
                foreach (var item in currentColorList)
                {
                    if (item.row == currentPiece.row + 1 && item.column == currentPiece.column) 
                    {
                        result.Add(item);
                        var resultList = GetSamePieceByDirection(currentColorList, item, DirectionPiece.Right);
                        result.AddRange(resultList);
                    }
                }
                break;
            case DirectionPiece.LeftUp:
                foreach (var item in currentColorList)
                {
                    if (item.row == currentPiece.row - 1 && item.column == currentPiece.column + 1)
                    {
                        result.Add(item);
                        var resultList = GetSamePieceByDirection(currentColorList, item, DirectionPiece.LeftUp);
                        result.AddRange(resultList);
                    }
                }
                break;
            case DirectionPiece.RightUp:
                foreach (var item in currentColorList)
                {
                    if (item.row == currentPiece.row + 1 && item.column == currentPiece.column + 1)
                    {
                        result.Add(item);
                        var resultList = GetSamePieceByDirection(currentColorList, item, DirectionPiece.RightUp);
                        result.AddRange(resultList);
                    }
                }
                break;
            case DirectionPiece.LeftDown:
                foreach (var item in currentColorList)
                {
                    if (item.row == currentPiece.row - 1 && item.column == currentPiece.column - 1)
                    {
                        result.Add(item);
                        var resultList = GetSamePieceByDirection(currentColorList, item, DirectionPiece.LeftDown);
                        result.AddRange(resultList);
                    }
                }
                break;
            case DirectionPiece.RightDown:
                foreach (var item in currentColorList)
                {
                    if (item.row == currentPiece.row + 1 && item.column == currentPiece.column - 1)
                    {
                        result.Add(item);
                        var resultList = GetSamePieceByDirection(currentColorList, item, DirectionPiece.RightDown);
                        result.AddRange(resultList);
                    }
                }
                break;
        }

        return result;
    }
}

public enum DirectionPiece
{
    Up=0,
    Down=1, 
    Left=2, 
    Right=3,
    LeftUp=4,
    RightUp=5,
    LeftDown=6,
    RightDown=7,
}
