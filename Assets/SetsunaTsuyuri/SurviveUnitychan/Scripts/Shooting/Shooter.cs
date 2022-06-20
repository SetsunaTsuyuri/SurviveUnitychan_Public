using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 弾の発射装置
    /// </summary>
    public class Shooter : MonoBehaviour
    {
        /// <summary>
        /// 発射する(非同期)
        /// </summary>
        /// <param name="weapon">武器</param>
        /// <param name="attack">武器所有者の攻撃力</param>
        /// <param name="units">ユニットの管理者</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async UniTask FireAsync(Weapon weapon, int attack, UnitsManager units, CancellationToken token)
        {
            try
            {
                // 弾の数
                int shots = weapon.GetCurrentLevelData().Shots;
                if (shots <= 0)
                {
                    return;
                }

                // 出現地点それぞれの角度の差
                float angleDifference = 360.0f / shots;

                for (int i = 0; i < shots; i++)
                {
                    // 弾の回転
                    Quaternion rotation = Quaternion.identity;
                    Vector3 direction = Vector3.zero;

                    // 攻撃対象
                    Transform target = null;

                    switch (weapon.Data.AimingType)
                    {
                        case BulletAimingType.None:

                            // 弾の個数に応じて拡散させる
                            float angle = (90.0f - angleDifference * i) * Mathf.Deg2Rad;
                            direction.x = Mathf.Cos(angle);
                            direction.z = Mathf.Sin(angle);
                            break;

                        case BulletAimingType.NearestEnemy:

                            // 最も近い敵のいる方向に向く
                            target = units.SerchForTheNearestEnemyTrandform(transform);
                            if (target is not null)
                            {
                                direction = (target.position - transform.position).normalized;
                                direction.y = 0.0f;
                            }
                            break;
                    }
                    rotation = Quaternion.FromToRotation(Vector3.forward, direction).normalized;

                    // 弾を出現させる
                    Bullet bullet = weapon.BulletPool.FirstOrDefault(x => !x.isActiveAndEnabled);
                    if (bullet)
                    {
                        bullet.Spawn(weapon, attack, transform.position, rotation, target);
                    }

                    // 武器の遅延時間だけ待つ
                    float elapsedTime = 0.0f;
                    while (elapsedTime < weapon.Data.DelayTime)
                    {
                        if (!PauseManager.IsPausing)
                        {
                            elapsedTime += Time.deltaTime;
                        }
                        await UniTask.Yield(token);
                    }
                }
            }
            catch (OperationCanceledException) { }
        }
    }
}
