using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// インゲームシーンUIの管理者
    /// </summary>
    public class InGameUIManager : MonoBehaviour
    {
        /// <summary>
        /// 残り時間表示
        /// </summary>
        RemainningTimeDisplayer _remainningTimeDisplayer = null;

        /// <summary>
        /// レベル表示
        /// </summary>
        LevelDisplayer _levelDisplayer = null; 

        /// <summary>
        /// 生命力表示
        /// </summary>
        LifeDisplayer _lifeDisplayer = null;
        
        /// <summary>
        /// 装備表示
        /// </summary>
        EquipmentsDisplayer _equipmentsDisplayer = null;

        /// <summary>
        /// 敵撃破数表示
        /// </summary>
        DefeatedEnemiesCountDisplayer _defeatedEnemiesCountDisplayer = null;

        /// <summary>
        /// 経験値表示
        /// </summary>
        public ExperienceDisplayer Experience { get; private set; } = null;

        /// <summary>
        /// 装備選択ボタンの管理者
        /// </summary>
        public EquipmentSelectionButtonsManager EquipmentSelection { get; private set; } = null;

        private void Awake()
        {
            // コンポーネント取得
            _remainningTimeDisplayer = GetComponentInChildren<RemainningTimeDisplayer>(true);
            _levelDisplayer = GetComponentInChildren<LevelDisplayer>(true);
            _lifeDisplayer = GetComponentInChildren<LifeDisplayer>(true);
            _equipmentsDisplayer = GetComponentInChildren<EquipmentsDisplayer>(true);
            _defeatedEnemiesCountDisplayer = GetComponentInChildren<DefeatedEnemiesCountDisplayer>(true);
            Experience = GetComponentInChildren<ExperienceDisplayer>(true);
            EquipmentSelection = GetComponentInChildren<EquipmentSelectionButtonsManager>(true);
        }

        /// <summary>
        /// セットアップする
        /// </summary>
        /// <param name="inGame">インゲームの管理者</param>
        public void SetUp(InGameManager inGame)
        {
            // タイマー
            _remainningTimeDisplayer.SetUp(inGame);

            // 敵撃破数表示
            _defeatedEnemiesCountDisplayer.SetUp(inGame);

            // プレイヤー
            Player player = inGame.Units.Player;

            // レベル表示
            _levelDisplayer.SetUp(player);

            // 経験値表示
            Experience.SetUp(player);

            // 生命力表示
            _lifeDisplayer.SetUp(player);

            // 装備表示
            _equipmentsDisplayer.SetUp(player);

            // 装備選択
            EquipmentSelection.SetUp(player);
        }
    }
}
