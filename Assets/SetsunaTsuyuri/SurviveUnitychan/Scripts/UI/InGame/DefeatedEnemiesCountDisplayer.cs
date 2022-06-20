using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using TMPro;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 敵撃破数表示UI
    /// </summary>
    public class DefeatedEnemiesCountDisplayer : GameUI
    {
        /// <summary>
        /// テキスト
        /// </summary>
        TextMeshProUGUI _text = null;

        protected override void Awake()
        {
            base.Awake();

            _text = GetComponentInChildren<TextMeshProUGUI>(true);
        }

        /// <summary>
        /// セットアップする
        /// </summary>
        /// <param name="inGame">インゲームの管理者</param>
        public void SetUp(InGameManager inGame)
        {
            // 敵撃破数変化時の処理購読
            inGame.DefeatedEnemiesCountChanged
                .TakeUntilDestroy(this)
                .Subscribe(UpdateDisplay);

            // 表示更新
            UpdateDisplay(inGame.DefeatedEnemiesCount);
        }

        /// <summary>
        /// 表示を更新する
        /// </summary>
        /// <param name="value">値</param>
        public void UpdateDisplay(int value)
        {
            _text.text = $"Defeated {value}";
        }
    }
}
