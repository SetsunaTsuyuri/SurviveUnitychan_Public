using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// リザルトパネル
    /// </summary>
    public class ResultPanel : GameUI
    {
        /// <summary>
        /// 勝利メッセージの文字列
        /// </summary>
        [SerializeField]
        string _winMessageText = "勝利！";

        /// <summary>
        /// 勝利メッセージの色
        /// </summary>
        [SerializeField]
        Color _winMessageColor = Color.yellow;

        /// <summary>
        /// 敗北メッセージの文字列
        /// </summary>
        [SerializeField]
        string _loseMessageText = "敗北……。";

        /// <summary>
        /// 敗北メッセージの色
        /// </summary>
        [SerializeField]
        Color _loseMessageColor = Color.red;

        /// <summary>
        /// 生存時間のテキスト
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _survivalTime = null;

        /// <summary>
        /// 敵撃破数のテキスト
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _defeatedEnemiesCount = null;

        /// <summary>
        /// リザルトテキスト
        /// </summary>
        [SerializeField]
        TextMeshProUGUI _resultText = null;

        /// <summary>
        /// セットアップする
        /// </summary>
        /// <param name="result">リザルトシーンの管理者</param>
        public void SetUp(ResultManager result)
        {
            // 生存時間の表示
            int minutes = Mathf.FloorToInt(result.SurvivalTime / 60.0f);
            int seconds = Mathf.FloorToInt(result.SurvivalTime - minutes * 60);
            _survivalTime.text = $"{minutes:00}:{seconds:00}";

            // 敵撃破数の表示
            _defeatedEnemiesCount.text = result.DefeatedEnemiesCount.ToString();

            // 勝敗の表示
            string text = _loseMessageText;
            Color color = _loseMessageColor;
            if (result.PlayerHasWon())
            {
                text = _winMessageText;
                color = _winMessageColor;
            }
            _resultText.text = text;
            _resultText.color = color;
        }
    }
}
