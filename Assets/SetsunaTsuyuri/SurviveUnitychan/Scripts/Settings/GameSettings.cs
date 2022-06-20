using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// ゲーム設定
    /// </summary>
    [CreateAssetMenu(fileName = "GameSettings", menuName ="GameSettings")]
    public class GameSettings : ScriptableObject
    {
        /// <summary>
        /// ユニットの移動速度
        /// </summary>
        [field: SerializeField]
        public float UnitMoveSpeed { get; private set; } = 10.0f;

        /// <summary>
        /// 被ダメージ後のプレイヤーの気絶時間
        /// </summary>
        [field: SerializeField]
        public float PlayerStunnedTime { get; private set; } = 0.5f;

        /// <summary>
        /// 被ダメージ後のプレイヤーの無敵時間
        /// </summary>
        [field: SerializeField]
        public float PlayerInvincibleTime { get; private set; } = 3.0f;

        /// <summary>
        /// ユニットが点滅する間隔
        /// </summary>
        [field: SerializeField]
        public float UnitBlinkingInterval { get; private set; } = 0.05f;

        /// <summary>
        /// 最小レベル
        /// </summary>
        [field: SerializeField]
        public int MinLevel { get; private set; } = 1;

        /// <summary>
        /// 最大レベル
        /// </summary>
        [field: SerializeField]
        public int MaxLevel { get; private set; } = 99;

        /// <summary>
        /// レベルアップに必要な経験値の基本値
        /// </summary>
        [field: SerializeField]
        public int LevelUpExperience { get; private set; } = 100;

        /// <summary>
        /// 1レベル毎の必要経験値上昇倍率
        /// </summary>
        [field: SerializeField]
        public float LevelUpExperienceRate { get; private set; } = 0.5f;

        /// <summary>
        /// レベルアップしたときに得られる装備品の最大数
        /// </summary>
        [field: SerializeField]
        public int AvailableEquipmentsOnLevelUp { get; private set; } = 3;

        /// <summary>
        /// ユニットの生命力限界値
        /// </summary>
        [field: SerializeField]
        public int UnitLifeLimit { get; private set; } = 99999;

        /// <summary>
        /// 新規装備品のレベル表示
        /// </summary>
        [field: SerializeField]
        public string NewEquipmentLevelDisplay { get; private set; } = string.Empty;

        /// <summary>
        /// アイテムの初期移動速度
        /// </summary>
        [field: SerializeField]
        public float InitialItemMoveSpeed { get; private set; } = 5.0f;

        /// <summary>
        /// アイテムの加速度
        /// </summary>
        [field: SerializeField]
        public float ItemMoveAcceleration { get; private set; } = 1.0f;
    }
}
