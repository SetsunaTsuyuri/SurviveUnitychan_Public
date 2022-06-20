using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 弾の移動タイプ
    /// </summary>
    public enum BulletMoveType
    {
        /// <summary>
        /// 直線
        /// </summary>
        Straight = 0
    }

    /// <summary>
    /// 弾の狙いの付け方
    /// </summary>
    public enum BulletAimingType
    {
        /// <summary>
        /// 狙わない
        /// </summary>
        None = 0,

        /// <summary>
        /// 最も近い敵を狙う
        /// </summary>
        NearestEnemy = 1
    }

    /// <summary>
    /// 弾
    /// </summary>
    public class Bullet : MonoBehaviour, IPausable
    {
        /// <summary>
        /// 移動タイプ
        /// </summary>
        public BulletMoveType MoveType { get; set; } = BulletMoveType.Straight;

        /// <summary>
        /// 貫通する
        /// </summary>
        public bool Penetrates { get; set; } = false;

        /// <summary>
        /// 威力
        /// </summary>
        public int Power { get; private set; } = 0;

        /// <summary>
        /// 速さ
        /// </summary>
        public float Speed { get; private set; } = 0.0f;

        /// <summary>
        /// 攻撃対象
        /// </summary>
        public Transform Target { get; private set; } = null;

        /// <summary>
        /// リジッドボディ
        /// </summary>
        Rigidbody _rigidbody = null;

        /// <summary>
        /// パーティクルシステム配列
        /// </summary>
        ParticleSystem[] _particleSystems = null;

        public void Pause()
        {
            foreach (var system in _particleSystems)
            {
                system.Pause();
            }
        }

        public void Resume()
        {
            foreach (var system in _particleSystems)
            {
                system.Play();
            }
        }

        private void Awake()
        {
            // コンポーネント取得
            _rigidbody = GetComponent<Rigidbody>();

            // 一時停止時の処理を購読する
            PauseManager.OnPause
                .TakeUntilDestroy(this)
                .Subscribe(_ => Pause());

            // 再開時の処理を購読する
            PauseManager.OnResume
                .TakeUntilDestroy(this)
                .Subscribe(_ => Resume());
        }

        /// <summary>
        /// セットアップする
        /// </summary>
        public void SetUp()
        {
            // コンポーネント取得
            _particleSystems = GetComponentsInChildren<ParticleSystem>();

        }

        private void FixedUpdate()
        {
            // 一時停止中なら中止
            if (PauseManager.IsPausing)
            {
                return;
            }

            // 移動
            Move();
        }

        private void OnTriggerEnter(Collider other)
        {
            // 命中可能なオブジェクトに命中した場合
            IBulletHit hit = other.GetComponentInParent<IBulletHit>();
            if (hit is not null)
            {
                // 命中時の処理を行う
                hit.OnHit(this);

                // 貫通しない場合
                if (!Penetrates)
                {
                    // 非アクティブにする
                    gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 移動する
        /// </summary>
        private void Move()
        {
            // 弾の種類によってどのように移動するかを決めて実行する
            UnityAction action = MoveType switch
            {
                // 直進
                BulletMoveType.Straight => OnStraight,
                _ => null
            };

            action?.Invoke();
        }

        /// <summary>
        /// 直線的に進む場合の処理
        /// </summary>
        private void OnStraight()
        {
            _rigidbody.MovePosition(transform.position + Speed * Time.fixedDeltaTime * transform.forward);
        }

        /// <summary>
        /// 出現する
        /// </summary>
        /// <param name="weapon">武器</param>
        /// <param name="attack">武器所有者の攻撃力</param>
        /// <param name="position">出現する位置</param>
        /// <param name="rotation">回転</param>
        /// <param name="target">攻撃対象</param>
        public void Spawn(Weapon weapon, int attack, Vector3 position, Quaternion rotation, Transform target)
        {
            // ステータス設定
            WeaponLevelData levelData = weapon.GetCurrentLevelData();
            Speed = levelData.Speed;
            Power = CalculateActualAttack(levelData.Power, attack);
            Target = target;

            // 位置
            transform.localPosition = position;

            // 回転
            transform.localRotation = rotation;

            // アクティブにする
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 実際の威力を計算する
        /// </summary>
        /// <param name="power">武器の威力</param>
        /// <param name="attack">所有者の攻撃力</param>
        /// <returns></returns>
        private int CalculateActualAttack(int power, int attack)
        {
            int result = Mathf.FloorToInt(power * (attack / 100.0f));
            return result;
        }
    }
}
