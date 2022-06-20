using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 敵
    /// </summary>
    public class Enemy : Unit<Enemy, EnemyData>, IBulletHit
    {
        /// <summary>
        /// 倒されたとき
        /// </summary>
        readonly Subject<Enemy> _killed = new();

        /// <summary>
        /// 倒されたとき
        /// </summary>
        public System.IObservable<Enemy> Killed
        {
            get => _killed;
        }

        public override void RefreshStatus()
        {
            base.RefreshStatus();

            MoveSpeed = Data.MoveSpeed;
        }

        public void OnHit(Bullet bullet)
        {
            // ダメージを受ける
            TakeDamage(bullet.Power);
        }

        public override void BeKnockedOut()
        {
            // 倒されたことを通知する
            _killed.OnNext(this);

            // 非アクティブ化
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            // 接触可能なオブジェクトに触れている場合
            IEnemyHit hit = other.GetComponentInParent<IEnemyHit>();
            if (hit is not null)
            {
                // 接触時の処理を行う
                hit.OnHit(this);
            }
        }
    }
}
