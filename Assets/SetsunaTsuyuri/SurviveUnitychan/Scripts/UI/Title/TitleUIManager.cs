using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// タイトルシーンの管理者
    /// </summary>
    public class TitleUIManager : MonoBehaviour
    {
        /// <summary>
        /// タイトルメニュー
        /// </summary>
        public TitleMenu TitleMenu { get; private set; } = null;

        private void Awake()
        {
            // コンポーネント取得
            TitleMenu = GetComponentInChildren<TitleMenu>(true);
        }

        /// <summary>
        /// セットアップする
        /// </summary>
        public void SetUp()
        {
            TitleMenu.SetUp();
        }
    }
}
