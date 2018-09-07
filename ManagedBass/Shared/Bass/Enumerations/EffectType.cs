using ManagedBass.Fx;

namespace ManagedBass
{
    /// <summary>
    /// FX effect types, use with <see cref="Bass.ChannelSetFX" />.
    /// </summary>
    public enum EffectType
    {
        #region DirectX
        /// <summary>
        /// DX8 Chorus.
        /// </summary>
        DXChorus,

#if WINDOWS
        /// <summary>
        /// DX8 Compressor
        /// </summary>
        DXCompressor,
#endif

        /// <summary>
        /// DX8 Distortion.
        /// </summary>
        DXDistortion,

        /// <summary>
        /// DX8 Echo.
        /// </summary>
        DXEcho,

        /// <summary>
        /// DX8 Flanger.
        /// </summary>
        DXFlanger,

#if WINDOWS
        /// <summary>
        /// DX8 Gargle.
        /// </summary>
        DXGargle,

        /// <summary>
        /// DX8 I3DL2 (Interactive 3D Audio Level 2) reverb.
        /// </summary>
        DX_I3DL2Reverb,
#endif

        /// <summary>
        /// DX8 Parametric equalizer.
        /// </summary>
        DXParamEQ,

        /// <summary>
        /// DX8 Reverb.
        /// </summary>
        DXReverb,
        #endregion

        #region BassFx
        /// <summary>
        /// <see cref="BassFx"/>: Channel Volume Ping-Pong (multi channel).
        /// </summary>
        Rotate = 0x10000,

        /// <summary>
        /// <see cref="BassFx"/>: Volume control (multi channel).
        /// </summary>
        Volume = 0x10003,

        /// <summary>
        /// <see cref="BassFx"/>: Peaking Equalizer (multi channel).
        /// </summary>
        PeakEQ = 0x10004,

        /// <summary>
        /// <see cref="BassFx"/>: Channel Swap/Remap/Downmix (multi channel).
        /// </summary>
        Mix = 0x10007,

        /// <summary>
        /// <see cref="BassFx"/>: Dynamic Amplification (multi channel).
        /// </summary>
        Damp = 0x10008,

        /// <summary>
        /// <see cref="BassFx"/>: Auto WAH (multi channel).
        /// </summary>
        AutoWah = 0x10009,

        /// <summary>
        /// <see cref="BassFx"/>: Phaser (multi channel).
        /// </summary>
        Phaser = 0x1000b,

        /// <summary>
        /// <see cref="BassFx"/>: Chorus (multi channel).
        /// </summary>
        Chorus = 0x1000d,

        /// <summary>
        /// <see cref="BassFx"/>: Distortion (multi channel).
        /// </summary>
        Distortion = 0x10010,

        /// <summary>
        /// <see cref="BassFx"/>: Dynamic Range Compressor (multi channel).
        /// </summary>
        Compressor = 0x10011,

        /// <summary>
        /// <see cref="BassFx"/>: Volume Envelope (multi channel).
        /// </summary>
        VolumeEnvelope = 0x10012,

        /// <summary>
        /// <see cref="BassFx"/>: BiQuad filters (multi channel).
        /// </summary>
        BQF = 0x10013,

        /// <summary>
        /// <see cref="BassFx"/>: Echo/Reverb 4 (multi channel).
        /// </summary>
        Echo = 0x10014,

        /// <summary>
        /// <see cref="BassFx"/>: Pitch Shift using FFT (multi channel).
        /// </summary>
        PitchShift = 0x10015,

        /// <summary>
        /// <see cref="BassFx"/>: Pitch Shift using FFT (multi channel).
        /// </summary>
        Freeverb = 0x10016
        #endregion
    }
}
