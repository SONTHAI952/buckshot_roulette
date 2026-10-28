namespace ZeroX.ProgressSystem
{
    public enum AnimateValueMode
    {
        /// <summary>
        /// Animate value always equal to value
        /// </summary>
        Instant,
        
        /// <summary>
        /// Animate value is controller by progressor
        /// </summary>
        Smooth,
        
        /// <summary>
        /// Animate value is controlled by external source
        /// </summary>
        Custom
    }
}