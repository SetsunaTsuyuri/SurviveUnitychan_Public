using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 弾を遮るもの
    /// </summary>
    public class BulletsBlocker : MonoBehaviour, IBulletHit
    {
        public void OnHit(Bullet bullet)
        {
            // 非アクティブにする
            bullet.gameObject.SetActive(false);
        }
    }

}
