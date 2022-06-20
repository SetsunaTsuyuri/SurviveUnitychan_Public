using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using TMPro;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// レベル表示UI
    /// </summary>
    public class LevelDisplayer : GameUI
    {
        /// <summary>
        /// テキスト
        /// </summary>
        TextMeshProUGUI _text = null;

        protected override void Awake()
        {
            base.Awake();

            _text = GetComponentInChildren<TextMeshProUGUI>();
        }

        /// <summary>
        /// セットアップする
        /// </summary>
        /// <param name="player"></param>
        public void SetUp(Player player)
        {
            // レベル変更時の処理購読
            player.LevelChanged
                .TakeUntilDestroy(this)
                .Subscribe(UpdateDisplay);

            // 表示更新
            UpdateDisplay(player.Level);
        }

        /// <summary>
        /// 表示を更新する
        /// </summary>
        /// <param name="level">レベル</param>
        public void UpdateDisplay(int level)
        {
            _text.text = $"Lv.{level}";
        }
    }
}
