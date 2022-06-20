using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// アイテム
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Item : MonoBehaviour
    {
        /// <summary>
        /// 経験値
        /// </summary>
        public int Experience { get; set; } = 0;

        /// <summary>
        /// 追跡できる
        /// </summary>
        public bool CanTrackTarget { get; set; } = false;

        /// <summary>
        /// 出現した時間
        /// </summary>
        public float SpawnedTime { get; set; } = 0.0f;

        /// <summary>
        /// 移動速度
        /// </summary>
        float _speed = 0.0f;

        /// <summary>
        /// リジッドボディ
        /// </summary>
        Rigidbody _rigidbody = null;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnTriggerEnter(Collider other)
        {
            // 接触可能なオブジェクトに命中した場合
            IItemHit hit = other.GetComponentInParent<IItemHit>();
            if (hit is not null)
            {
                // 接触時の処理を行う
                hit.OnHit(this);
            }
        }

        /// <summary>
        /// 出現する
        /// </summary>
        /// <param name="enemy">敵</param>
        public void Spawn(Enemy enemy)
        {
            // アイテムの追跡フラグをfalseにする
            CanTrackTarget = false;

            // 経験値設定
            Experience = enemy.Data.Experience;

            // 出現時間設定
            SpawnedTime = Time.time;

            // 移動速度初期化
            _speed = MasterData.GameSettings.InitialItemMoveSpeed;

            // 位置設定
            transform.localPosition = enemy.transform.position;
            
            // アクティブ化
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 追跡する
        /// </summary>
        /// <param name="targetPosition">追跡対象の位置</param>
        public void Track(Vector3 targetPosition)
        {
            // 速度を決定する
            _speed += MasterData.GameSettings.ItemMoveAcceleration * Time.deltaTime;
            Vector3 direction = (targetPosition - transform.position).normalized;            
            Vector3 velocity = _speed * Time.deltaTime * direction;

            // 移動する
            _rigidbody.MovePosition(transform.position + velocity);
        }
    }
}
