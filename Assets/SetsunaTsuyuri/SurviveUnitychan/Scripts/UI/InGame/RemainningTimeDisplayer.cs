using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 残り時間表示UI
    /// </summary>
    public class RemainningTimeDisplayer : GameUI
    {
        /// <summary>
        /// テキスト
        /// </summary>
        TextMeshProUGUI _text = null;

        protected override void Awake()
        {
            base.Awake();

            // コンポーネント取得
            _text = GetComponentInChildren<TextMeshProUGUI>();
        }

        /// <summary>
        /// セットアップする
        /// </summary>
        /// <param name="inGame">インゲームの管理者</param>
        public void SetUp(InGameManager inGame)
        {
            // 時間変更時の処理購読
            inGame.RemainningTimeChanged
                .TakeUntilDestroy(this)
                .Subscribe(UpdateDisplay);

            // 表示更新
            UpdateDisplay(inGame.RemainningTime);
        }

        /// <summary>
        /// 表示を更新する
        /// </summary>
        /// <param name="time">時間</param>
        private void UpdateDisplay(float time)
        {
            // (00分:00秒)の形にする
            int minutes = Mathf.FloorToInt(time / 60.0f);
            int seconds = Mathf.FloorToInt(time - minutes * 60);
            string text = $"{minutes:00}:{seconds:00}";

            _text.text = text;
        }
    }
}
