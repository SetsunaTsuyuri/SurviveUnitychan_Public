using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 一時停止処理の管理者
    /// </summary>
    public class PauseManager : Singleton<PauseManager>, IInitializable
    {
        /// <summary>
        /// 一時停止時の処理
        /// </summary>
        readonly Subject<Unit> _onPause = new();

        /// <summary>
        /// 一時停止時の処理
        /// </summary>
        public static IObservable<Unit> OnPause
        {
            get => Instance._onPause;
        }

        /// <summary>
        /// 再開時の処理
        /// </summary>
        readonly Subject<Unit> _onResume = new();

        /// <summary>
        /// 再開時の処理
        /// </summary>
        public static IObservable<Unit> OnResume
        {
            get => Instance._onResume;
        }

        /// <summary>
        /// 一時停止中である
        /// </summary>
        bool _isPausing = false;

        /// <summary>
        /// 一時停止中である
        /// </summary>
        public static bool IsPausing
        {
            get => Instance._isPausing;
        }

        /// <summary>
        /// 一時停止する
        /// </summary>
        public static void Pause()
        {
            Instance.PauseInner();
        }

        /// <summary>
        /// 一時停止する
        /// </summary>
        private void PauseInner()
        {
            _onPause.OnNext(Unit.Default);
            _isPausing = true;
        }

        /// <summary>
        /// 再開する
        /// </summary>
        public static void Resume()
        {
            Instance.ResumeInner();
        }

        /// <summary>
        /// 再開する
        /// </summary>
        private void ResumeInner()
        {
            _onResume.OnNext(Unit.Default);
            _isPausing = false;
        }

        /// <summary>
        /// 一時停止と再開を切り替える
        /// </summary>
        public static void Switch()
        {
            if (IsPausing)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
}
