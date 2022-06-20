using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// リザルトシーンの管理者
    /// </summary>
    public class ResultManager : MonoBehaviour
    {
        /// <summary>
        /// リザルトシーンUIの管理者
        /// </summary>
        [SerializeField]
        ResultUIManager _ui = null;

        /// <summary>
        /// プレイヤー
        /// </summary>
        public Player Player { get; set; } = null;

        /// <summary>
        /// 生存時間
        /// </summary>
        public float SurvivalTime { get; set; } = 0.0f;

        /// <summary>
        /// 敵撃破数
        /// </summary>
        public int DefeatedEnemiesCount { get; set; } = 0;

        private void Start()
        {
            // UIセットアップ
            _ui.SetUp(this);

            string seName = "敗北";
            UnitAnimationId animationId = UnitAnimationId.Lose;

            // プレイヤーが勝利した場合
            if (PlayerHasWon())
            {
                seName = "勝利";
                animationId = UnitAnimationId.Win;
            }

            AudioManager.PlaySE(seName);
            if (Player)
            {
                Player.SetAnimationId(animationId);
            }

            // リザルトメニューを選択する
            _ui.ResultMenu.Select();
        }

        /// <summary>
        /// プレイヤーが勝利した
        /// </summary>
        /// <returns></returns>
        public bool PlayerHasWon()
        {
            return Player && Player.IsLiving();
        }
    }
}
