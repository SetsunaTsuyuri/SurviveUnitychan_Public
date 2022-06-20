using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 装備表示UI
    /// </summary>
    public class EquipmentsDisplayer : GameUI
    {
        /// <summary>
        /// 要素のプレハブ
        /// </summary>
        [SerializeField]
        EquipmentsDisplayerElement _elementPrefab = null;

        /// <summary>
        /// 武器表示UIの親
        /// </summary>
        [SerializeField]
        RectTransform _weaponsParent = null;

        /// <summary>
        /// 装飾品表示UIの親
        /// </summary>
        [SerializeField]
        RectTransform _accessoriesParent = null;

        /// <summary>
        /// 武器
        /// </summary>
        EquipmentsDisplayerElement[] _weaponElements = { };

        /// <summary>
        /// 装飾品
        /// </summary>
        EquipmentsDisplayerElement[] _accessoryElements = { };

        /// <summary>
        /// セットアップする
        /// </summary>
        /// <param name="player">プレイヤー</param>
        public void SetUp(Player player)
        {
            // 武器
            int maxWeapons = player.GetMaxWeapons();
            _weaponElements = CreateElements(maxWeapons, _weaponsParent);

            // 装飾品
            int maxAccsessories = player.GetMaxAccessories();
            _accessoryElements = CreateElements(maxAccsessories, _accessoriesParent);

            // 装備更新時の処理購読
            player.WeaponsUpdated
                .TakeUntilDestroy(this)
                .Subscribe(UpdateWeaponsDisplay);

            // 装飾品更新時の処理購読
            player.AccessoriesUpdated
                .TakeUntilDestroy(this)
                .Subscribe(UpdateAccessoriesDisplay);

            // 表示更新
            UpdateWeaponsDisplay(player.Weapons.ToArray());
            UpdateAccessoriesDisplay(player.Accessories.ToArray());
        }

        /// <summary>
        /// UI要素を作る
        /// </summary>
        /// <param name="max">最大数</param>
        /// <param name="parent">親トランスフォーム</param>
        /// <returns>UI要素配列</returns>
        private EquipmentsDisplayerElement[] CreateElements(int max, Transform parent)
        {
            EquipmentsDisplayerElement[] elements = new EquipmentsDisplayerElement[max];
            for (int i = 0; i < max; i++)
            {
                EquipmentsDisplayerElement instance = Instantiate(_elementPrefab, parent);
                elements[i] = instance;
            }

            return elements;
        }

        /// <summary>
        /// 表示をクリアする
        /// </summary>
        /// <param name="elements">UI要素配列</param>
        private void Clear(EquipmentsDisplayerElement[] elements)
        {
            foreach (var element in elements)
            {
                element.Clear();
            }
        }

        /// <summary>
        /// 武器の表示を更新する
        /// </summary>
        /// <param name="equipments">装備品配列</param>
        private void UpdateWeaponsDisplay(IEquipment[] equipments)
        {
            Clear(_weaponElements);

            for (int i = 0; i < equipments.Length; i++)
            {
                _weaponElements[i].UpdateDisplay(equipments[i]);
            }
        }

        /// <summary>
        /// 装飾品の表示を更新する
        /// </summary>
        /// <param name="equipments">装備品配列</param>
        private void UpdateAccessoriesDisplay(IEquipment[] equipments)
        {
            Clear(_accessoryElements);

            for (int i = 0; i < equipments.Length; i++)
            {
                _accessoryElements[i].UpdateDisplay(equipments[i]);
            }

        }
    }
}
