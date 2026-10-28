namespace ZeroX.SafeAreaSystem
{
    public enum SimulationDevice
    {
        /// <summary>
        /// Don't use a simulated safe area - GUI will be full screen as normal.
        /// </summary>
        None,
        /// <summary>
        /// Simulate the iPhone X and Xs (identical safe areas).
        /// </summary>
        iPhoneX,
        /// <summary>
        /// Simulate the iPhone Xs Max and XR (identical safe areas).
        /// </summary>
        iPhoneXsMax,
        /// <summary>
        /// Simulate the Google Pixel 3 XL using landscape left.
        /// </summary>
        Pixel3XL_LSL,
        /// <summary>
        /// Simulate the Google Pixel 3 XL using landscape right.
        /// </summary>
        Pixel3XL_LSR
    }
}