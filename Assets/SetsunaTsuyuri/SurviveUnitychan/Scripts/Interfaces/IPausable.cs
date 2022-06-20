namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 一時停止できる 
    /// </summary>
    public interface IPausable
    {
        /// <summary>
        /// 一時停止する
        /// </summary>
        void Pause();

        /// <summary>
        /// 再開する
        /// </summary>
        void Resume();
    }
}

