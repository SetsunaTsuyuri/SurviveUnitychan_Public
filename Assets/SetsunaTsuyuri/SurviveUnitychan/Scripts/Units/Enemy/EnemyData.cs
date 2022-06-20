using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 敵のデータ
    /// </summary>
    [System.Serializable]
    public class EnemyData : UnitData
    {
        /// <summary>
        /// 接触時のダメージ
        /// </summary>
        public int TouchDamage = 1;

        /// <summary>
        /// 獲得できる経験値
        /// </summary>
        public int Experience = 0;
    }
}
