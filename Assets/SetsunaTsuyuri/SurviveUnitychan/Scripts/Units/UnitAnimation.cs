using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// ユニットアニメーションID
    /// </summary>
    public enum UnitAnimationId
    {
        /// <summary>
        /// 待機
        /// </summary>
        Idle = 0,

        /// <summary>
        /// 移動
        /// </summary>
        Move = 10,

        /// <summary>
        /// ダメージ
        /// </summary>
        Damaged = 20,

        /// <summary>
        /// 戦闘不能
        /// </summary>
        KnockedOut = 30,

        /// <summary>
        /// 起き上がり
        /// </summary>
        KnockedOutToUp = 31,

        /// <summary>
        /// 勝利
        /// </summary>
        Win = 1000,

        /// <summary>
        /// 敗北
        /// </summary>
        Lose = 1010
    }

    /// <summary>
    /// ユニットアニメーション
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class UnitAnimation : MonoBehaviour, IInitializable, IPausable
    {
        /// <summary>
        /// アニメーター
        /// </summary>
        protected Animator _animator = null;

        /// <summary>
        /// ID
        /// </summary>
        UnitAnimationId _id = UnitAnimationId.Idle;

        /// <summary>
        /// ID
        /// </summary>
        public UnitAnimationId Id
        {
            get => _id;
            set
            {
                _id = value;
                OnIdSet();
            }
        }

        public void Initialize()
        {
            // コンポーネント取得
            _animator = GetComponent<Animator>();
        }

        /// <summary>
        /// IDが設定されたときの処理
        /// </summary>
        private void OnIdSet()
        {
            // アニメーターが存在しなければ中止する
            if (!_animator)
            {
                return;
            }

            // アニメーターにIDを設定する
            int id = (int)Id;
            _animator.SetInteger("Id", id);
        }

        public void Pause()
        {
            // アニメーターを止める
            _animator.speed = 0.0f;
        }

        public void Resume()
        {
            // アニメーターを再開させる
            _animator.speed = 1.0f;
        }

        /// <summary>
        /// 現在のアニメーションステートインフォを取得する
        /// </summary>
        /// <param name="layer">レイヤー</param>
        /// <returns></returns>
        public AnimatorStateInfo GetCurrentAnimatorStateInfo(int layer = 0)
        {
            return _animator.GetCurrentAnimatorStateInfo(layer);
        }
    }
}
