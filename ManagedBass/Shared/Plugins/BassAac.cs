#if WINDOWS || LINUX || __ANDROID__
namespace ManagedBass
{
    public static partial class BassAac
    {
        /// <summary>
        /// Play audio from Mp4... default = true.
        /// </summary>
        public static bool PlayAudioFromMp4
        {
            get { return Bass.GetConfigBool(Configuration.PlayAudioFromMp4); }
            set { Bass.Configure(Configuration.PlayAudioFromMp4, value); }
        }

        /// <summary>
        /// Support Mp4 in Aac functions.
        /// </summary>
        public static bool AacSupportMp4
        {
            get { return Bass.GetConfigBool(Configuration.AacSupportMp4); }
            set { Bass.Configure(Configuration.AacSupportMp4, value); }
        }
    }
}
#endif