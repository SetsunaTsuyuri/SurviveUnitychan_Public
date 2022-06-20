using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// リザルトシーンUIの管理者
    /// </summary>
    public class ResultUIManager : MonoBehaviour
    {
        /// <summary>
        /// リザルトパネル
        /// </summary>
        ResultPanel _resultPanel = null;

        /// <summary>
        /// リザルトメニュー
        /// </summary>
        public ResultMenu ResultMenu { get; private set; } = null; 

        private void Awake()
        {
            // コンポーネント取得
            _resultPanel = GetComponentInChildren<ResultPanel>(true);
            ResultMenu = GetComponentInChildren<ResultMenu>(true);
        }

        /// <summary>
        /// セットアップする
        /// </summary>
        /// <param name="result">リザルトシーンの管理者</param>
        public void SetUp(ResultManager result)
        {
            _resultPanel.SetUp(result);
            ResultMenu.SetUp();
        }
    }
}
