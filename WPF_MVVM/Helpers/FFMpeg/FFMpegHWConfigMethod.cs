using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_MVVM.Helpers.FFMpeg
{
    internal enum FFMpegHWConfigMethod : int
    {
        /**
         * The codec supports this format via the hw_device_ctx interface.
         *
         * When selecting this format, AVCodecContext.hw_device_ctx should
         * have been set to a device of the specified type before calling
         * avcodec_open2().
         */
        AV_CODEC_HW_CONFIG_METHOD_HW_DEVICE_CTX = 0x01,
        /**
         * The codec supports this format via the hw_frames_ctx interface.
         *
         * When selecting this format for a decoder,
         * AVCodecContext.hw_frames_ctx should be set to a suitable frames
         * context inside the get_format() callback.  The frames context
         * must have been created on a device of the specified type.
         *
         * When selecting this format for an encoder,
         * AVCodecContext.hw_frames_ctx should be set to the context which
         * will be used for the input frames before calling avcodec_open2().
         */
        AV_CODEC_HW_CONFIG_METHOD_HW_FRAMES_CTX = 0x02,
        /**
         * The codec supports this format by some internal method.
         *
         * This format can be selected without any additional configuration -
         * no device or frames context is required.
         */
        AV_CODEC_HW_CONFIG_METHOD_INTERNAL = 0x04,
        /**
         * The codec supports this format by some ad-hoc method.
         *
         * Additional settings and/or function calls are required.  See the
         * codec-specific documentation for details.  (Methods requiring
         * this sort of configuration are deprecated and others should be
         * used in preference.)
         */
        AV_CODEC_HW_CONFIG_METHOD_AD_HOC = 0x08,
    }
}
