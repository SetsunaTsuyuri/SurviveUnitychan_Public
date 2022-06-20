using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// ステージのデータ
    /// </summary>
    [System.Serializable]
    public class StageData
    {
        /// <summary>
        /// 名前
        /// </summary>
        public string Name = string.Empty;

        /// <summary>
        /// クリアまでの時間
        /// </summary>
        public float TimeToClear = 180.0f;

        /// <summary>
        /// 最初に出現する敵の数
        /// </summary>
        public int InitialEnemies = 10;

        /// <summary>
        /// 出現する敵の最大数
        /// </summary>
        public int MaxEnemies = 300;

        /// <summary>
        /// 敵の増援数
        /// </summary>
        public int EnemyReinforcements = 2;

        /// <summary>
        /// 敵の増援間隔
        /// </summary>
        public float EnemyReinforcementsInterval = 1.0f;

        /// <summary>
        /// BGM名
        /// </summary>
        public string BgmName = string.Empty;

        /// <summary>
        /// 出現する敵ユニットのID
        /// </summary>
        public int[] EnemyIds = { };
    }
}
