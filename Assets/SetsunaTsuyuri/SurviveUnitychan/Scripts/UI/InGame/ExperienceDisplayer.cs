using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 経験値表示UI
    /// </summary>
    public class ExperienceDisplayer : GameUI
    {
        /// <summary>
        /// バー
        /// </summary>
        [SerializeField]
        Image _bar = null;

        /// <summary>
        /// セットアップする
        /// </summary>
        /// <param name="player">プレイヤー</param>
        public void SetUp(Player player)
        {
            // 経験値設定時の処理購読
            player.ExperienceSet
                .TakeUntilDestroy(this)
                .Subscribe(UpdateDisplay);

            // 表示更新
            UpdateDisplay(player);
        }

        /// <summary>
        /// 表示を更新する
        /// </summary>
        /// <param name="player">プレイヤー</param>
        public void UpdateDisplay(Player player)
        {
            // 最大レベルならゲージを満タンにする
            if (player.Level == MasterData.GameSettings.MaxLevel)
            {
                FillUp();
                return;
            }

            // 現在の経験値
            int currentExperience = player.Experience;
            
            // 次のレベルまでに必要な経験値
            int nextLevelMinExperience = player.ToMinExperience(player.Level + 1);

            // 現在のレベルから次のレベルまでに必要な残りの経験値
            int remainning = nextLevelMinExperience - currentExperience;

            // 現在のレベルにおける最低経験値
            int currentLevelMinExperience = player.ToMinExperience(player.Level);

            // 現在のレベルから次のレベルまでに必要な経験値の最大値
            int max = nextLevelMinExperience - currentLevelMinExperience;

            // 表示割合
            float fillRate = 1.0f - remainning / (float)max;
            
            // 表示更新
            _bar.fillAmount = fillRate;
        }

        /// <summary>
        /// 表示を満タンにする
        /// </summary>
        public void FillUp()
        {
            _bar.fillAmount = 1.0f;
        }
    }

}
