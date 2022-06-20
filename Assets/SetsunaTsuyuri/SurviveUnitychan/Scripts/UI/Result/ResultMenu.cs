using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// リザルトメニュー
    /// </summary>
    public class ResultMenu : SelectableGameUI<GameButton>
    {
        /// <summary>
        /// タイトルへ戻るボタン
        /// </summary>
        [SerializeField]
        GameButton _toTitle = null;

        public override void SetUp()
        {
            base.SetUp();

            _toTitle.AddOnClickListener(() =>
            {
                // タイトルへ戻る
                SceneChangeManager.ChangeScene(SceneName.Title);
            });
        }
    }
}
