using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 味方のデータ
    /// </summary>
    [System.Serializable]
    public class AllyData : UnitData
    {
        /// <summary>
        /// 攻撃力
        /// </summary>
        public int Attack = 100;

        /// <summary>
        /// 武器の再使用速度
        /// </summary>
        public int RechargeSpeed = 100;

        /// <summary>
        /// 初期武器ID
        /// </summary>
        public int[] InitialWeaponIds = { };

        /// <summary>
        /// レベルアップで得られる武器ID
        /// </summary>
        public int[] AvailableWeaponIds = { };

        /// <summary>
        /// 初期装飾品ID
        /// </summary>
        public int[] InitialAccessoryIds = { };

        /// <summary>
        /// レベルアップで得られる装飾品ID
        /// </summary>
        public int[] AvailableAccesoryIds = { };
    }
}
