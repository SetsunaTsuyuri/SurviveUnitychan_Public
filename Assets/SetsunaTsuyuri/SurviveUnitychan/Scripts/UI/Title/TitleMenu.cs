using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// タイトルメニューの管理者
    /// </summary>
    public class TitleMenu : SelectableGameUI<GameButton>
    {
        /// <summary>
        /// ゲーム開始ボタン
        /// </summary>
        [SerializeField]
        GameButton _start = null;

        /// <summary>
        /// ゲーム終了ボタン
        /// </summary>
        [SerializeField]
        GameButton _end = null;

        public override void SetUp()
        {
            base.SetUp();

            // ゲーム開始
            _start.AddOnClickListener(() =>
            {
                SceneChangeManager.ChangeScene(SceneName.InGame);
            });

#if UNITY_WEBGL
            _end.Hide();
#else
            // ゲーム終了
            _end.AddOnClickListener(() =>
            {
#if UNITY_EDITOR

                UnityEditor.EditorApplication.isPlaying = false;

#elif UNITY_STANDALONE

                Application.Quit();
#endif
            });
#endif
        }
    }
}
