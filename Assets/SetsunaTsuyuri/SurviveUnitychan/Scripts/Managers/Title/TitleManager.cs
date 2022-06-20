using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// タイトルシーンの管理者
    /// </summary>
    public class TitleManager : MonoBehaviour
    {
        /// <summary>
        /// タイトルシーンUIの管理者
        /// </summary>
        [SerializeField]
        TitleUIManager _ui = null;

        private void Start()
        {
            // UIをセットアップする
            _ui.SetUp();

            // タイトルメニューを選択する
            _ui.TitleMenu.Select();
        }
    }
}
