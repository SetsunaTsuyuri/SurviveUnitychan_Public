using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 武器のレベルデータ
    /// </summary>
    [System.Serializable]
    public class WeaponLevelData : EquipmentLevelData
    {
        /// <summary>
        /// 発射数
        /// </summary>
        public int Shots = 0;

        /// <summary>
        /// 攻撃力
        /// </summary>
        public int Power = 0;

        /// <summary>
        /// 速さ
        /// </summary>
        public float Speed = 0.0f;

        /// <summary>
        /// 再使用までに必要な時間
        /// </summary>
        public float Recharge = 0.0f;
    }
}
