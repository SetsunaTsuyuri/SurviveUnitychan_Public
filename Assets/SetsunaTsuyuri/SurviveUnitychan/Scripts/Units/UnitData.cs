using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// ユニットのデータ
    /// </summary>
    [System.Serializable]
    public class UnitData
    {
        /// <summary>
        /// 名前
        /// </summary>
        public string Name = string.Empty;

        /// <summary>
        /// 最大生命力
        /// </summary>
        public int MaxLife = 0;

        /// <summary>
        /// 素早さ
        /// </summary>
        public int MoveSpeed = 0;

        /// <summary>
        /// モデル
        /// </summary>
        public GameObject Model = null;
    }
}
