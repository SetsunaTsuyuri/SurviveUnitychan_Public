using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 生命力表示UI
    /// </summary>
    public class LifeDisplayer : GameUI
    {
        /// <summary>
        /// 生命力表示のプレハブ
        /// </summary>
        [SerializeField]
        Image _lifePrefab = null;

        /// <summary>
        /// 生命力表示の親トランスフォーム
        /// </summary>
        [SerializeField]
        RectTransform _lifeParent = null;

        /// <summary>
        /// 最大生命力表示のプレハブ
        /// </summary>
        [SerializeField]
        Image _maxLifePrefab = null;

        /// <summary>
        /// 最大生命力表示の親トランスフォーム
        /// </summary>
        [SerializeField]
        RectTransform _maxLifeParent = null;

        /// <summary>
        /// 生命力表示のオブジェクトプール
        /// </summary>
        Image[] _lifePool = { };

        /// <summary>
        /// 最大生命力表示のオブジェクトプール
        /// </summary>
        Image[] _maxLifePool = { };

        /// <summary>
        /// セットアップする
        /// </summary>
        public void SetUp(Player player)
        {
            // 生命力表示
            _lifePool = CreatePool(_lifePrefab, _lifeParent, player.MaxLife);

            // 最大生命力表示
            _maxLifePool = CreatePool(_maxLifePrefab, _maxLifeParent, player.MaxLife);

            // 生命力設定時の処理購読
            player.LifeSet
                .TakeUntilDestroy(this)
                .Subscribe(x => UpdateDisplay(_lifePool, x));

            // 最大生命力設定時の処理購読
            player.MaxLifeSet
                .TakeUntilDestroy(this)
                .Subscribe(x => UpdateDisplay(_maxLifePool, x));

            // 表示更新
            UpdateDisplay(_lifePool, player.Life);
            UpdateDisplay(_maxLifePool, player.MaxLife);
        }

        /// <summary>
        /// 要素を作る
        /// </summary>
        /// <param name="prefab">プレハブ</param>
        /// <param name="parent">親トランスフォーム</param>
        /// <param name="maxLife">最大生命力</param>
        private Image[] CreatePool(Image prefab, RectTransform parent, int maxLife)
        {
            // プレイヤーの最大生命力の数だけオブジェクトプーリングする
            Image[] pool = new Image[maxLife];
            
            for (int i = 0; i < maxLife; i++)
            {
                Image instance = Instantiate(prefab, parent);
                instance.enabled = false;
                pool[i] = instance;
            }

            return pool;
        }

        /// <summary>
        /// 表示を更新する
        /// </summary>
        /// <param name="pool">オブジェクトプール</param>
        /// <param name="life">生命力</param>
        public void UpdateDisplay(Image[] pool, int life)
        {
            // 全て非表示にする
            foreach (var element in pool)
            {
                element.enabled = false;
            }

            // 必要な数だけ表示する
            Image[] elementsToBeDisplayed = pool
                .Take(life)
                .ToArray();

            foreach (var element in elementsToBeDisplayed)
            {
                element.enabled = true;
            }
        }
    }
}
