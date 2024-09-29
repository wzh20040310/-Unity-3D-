using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // 导入 UI 相关类

public class 按钮代码 : MonoBehaviour
{
    // 引用玩家代码脚本
    public 玩家代码 playerScript;

    public void RestartGame()
    {
        // 1. 清除棋子克隆体
        GameObject[] allPieces = GameObject.FindGameObjectsWithTag("Piece");
        foreach (GameObject piece in allPieces)
        {
            Destroy(piece);
        }

        // 2. 重置游戏状态
        playerScript.over = false; // 重置游戏结束状态
        playerScript.color = PieceColor.Black; // 重置玩家颜色
        playerScript.CurrentPieceList.Clear(); // 清空棋子列表

        // 3. 隐藏游戏结束文本和重启按钮
        playerScript.gameOverText.SetActive(false);
        playerScript.restartButton.SetActive(false);
    }
}
