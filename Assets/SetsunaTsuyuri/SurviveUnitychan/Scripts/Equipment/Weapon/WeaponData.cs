using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 武器のデータ
    /// </summary>
    [System.Serializable]
    public class WeaponData : EquipmentData<WeaponLevelData>
    {
        /// <summary>
        /// 弾の移動タイプ
        /// </summary>
        public BulletMoveType MoveType = BulletMoveType.Straight;

        /// <summary>
        /// 弾の狙いの付け方
        /// </summary>
        public BulletAimingType AimingType = BulletAimingType.NearestEnemy;

        /// <summary>
        /// 貫通する
        /// </summary>
        public bool Penetrates = false;

        /// <summary>
        /// 複数発射する場合の1発ごと遅延時間
        /// </summary>
        public float DelayTime = 0.0f;

        /// <summary>
        /// 弾をプールする数
        /// </summary>
        public int NumberOfBulletPool = 0;

        /// <summary>
        /// 弾のモデル
        /// </summary>
        public GameObject BulletModel = null;
    }
}