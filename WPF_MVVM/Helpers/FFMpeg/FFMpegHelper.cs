using DocumentFormat.OpenXml.Drawing;
using FFmpeg.AutoGen;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_MVVM.Libs;

namespace WPF_MVVM.Helpers.FFMpeg
{
    
    internal unsafe class FFMpegHelper : IDisposable
    {
        private List<int> _videoStreamIndex;
        private List<int> _audioStreamIndex;
        private List<int> _dataStreamIndex;
        private List<int> _subTitleStreamIndex;
        private AVPixelFormat _pixelFormat;
        private AVHWDeviceType _hwDeviceType;
        private AVCodecContext* _codecContext;
        private BinaryReader _reader;
        private readonly AVBufferRef* _bufferRef;
        private readonly AVFrame* _frame;
        private readonly AVPacket* _packet;
        private readonly AVFormatContext* _formatContext;

        private const int BUFF_SIZE = 4096;

        public FFMpegHelper()
        {
            StreamDecoder = new();

            _videoStreamIndex = new();
            _audioStreamIndex = new();
            _dataStreamIndex = new();
            _subTitleStreamIndex = new();
            SelectVideoStreamIndex = -1;
            SelectAudioStreamIndex = -1;
            SelectDataStreamIndex = -1;
            SelectSubTitleStreamIndex = -1;

            _pixelFormat = AVPixelFormat.AV_PIX_FMT_NONE;
            _hwDeviceType = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE;

            _reader = new(Stream.Null);
        }

        public FFMpegStreamDecoder StreamDecoder { get; private set; }
        public int SelectVideoStreamIndex { get; private set; }
        public int SelectAudioStreamIndex { get; private set; }
        public int SelectDataStreamIndex { get; private set; }
        public int SelectSubTitleStreamIndex { get; private set; }

        public unsafe double GetSimpleVideoFrameRate(string path)
        {
            fixed (AVFormatContext** formatContext = &_formatContext)
            {
                StreamDecoder.OpenVideoFile(path, formatContext);
            }

            AVCodec* codec = null;
            var videoStreamIndex = ffmpeg.av_find_best_stream(_formatContext, AVMediaType.AVMEDIA_TYPE_VIDEO, -1, -1, &codec, 0);
            if (videoStreamIndex < 0)
            {
                return 0.0;
            }
            SelectVideoStreamIndex = videoStreamIndex;

            var videoStream = _formatContext->streams[videoStreamIndex];
            AVRational frameRate = videoStream->avg_frame_rate;
            var timeBase = videoStream->time_base;
            return frameRate.num / frameRate.den;
        }
        public unsafe TimeSpan GetSimpleDuration(string path)
        {
            fixed (AVFormatContext** formatContext = &_formatContext)
            {
                StreamDecoder.OpenVideoFile(path, formatContext);
            }

            InitStreamIndex(_formatContext);
            if (_videoStreamIndex.Count <= 0 && _audioStreamIndex.Count <= 0)
            {
                return TimeSpan.Zero;
            }

            var stream = _videoStreamIndex.Count > 0 ? _formatContext->streams[_videoStreamIndex.First()] : _formatContext->streams[_audioStreamIndex.First()];
            AVRational frameRate = stream->avg_frame_rate;
            var duration = stream->duration;
            var timeBase = stream->time_base;
            return TimeSpan.FromTicks(duration * timeBase.num * TimeSpan.TicksPerSecond / timeBase.den);
        }

        public unsafe ReadOnlySpan<byte> Test(string path)
        {
            return default;
        }

        public unsafe void GetSimpleVideoSet(string path)
        {
            int ret = 0;
            this.Dispose();
            _reader = new(new FileStream(path, FileMode.Open));

            fixed (AVPacket** packet = &_packet)
            {
                *packet = ffmpeg.av_packet_alloc();
            }

            AVCodec* codec;
            fixed (AVFormatContext** formatContext = &_formatContext)
            {
                StreamDecoder.OpenVideoFile(path, formatContext);
            }
            ffmpeg.av_find_best_stream(_formatContext, AVMediaType.AVMEDIA_TYPE_VIDEO, -1, -1, &codec, 0);
            if (codec == null)
            {
                return;
            }

            fixed (AVCodecContext** codecContext = &_codecContext)
            {
                *codecContext = ffmpeg.avcodec_alloc_context3(codec);
                if (_codecContext == null)
                {
                    return;
                }
            }

            ret = ffmpeg.avcodec_open2(_codecContext, codec, null);
            if (ret < 0)
            {
                return;
            }

            fixed (AVFrame** frame = &_frame)
            {
                *frame = ffmpeg.av_frame_alloc();
            }
        }
        public unsafe byte[] GetSimpleVideoFrameData()
        {
            if (CheckSetVideo())
            {
                return Array.Empty<byte>();
            }

            int ret = 0;
            var buff = new byte[BUFF_SIZE + ffmpeg.AV_INPUT_BUFFER_PADDING_SIZE];
            AVCodecParserContext* parser = ffmpeg.av_parser_init(Extensions.GetEnumValue<int, AVCodecID>(_codecContext->codec->id));

            var parseRet = true;
            while (parseRet)
            {
                var dataSize = _reader.Read(buff, 0, BUFF_SIZE);
                if (dataSize == 0)
                {
                    return Array.Empty<byte>();
                }

                fixed (byte* buffPtr = buff)
                {
                    byte* buffPtrData = buffPtr;
                    while (dataSize > 0)
                    {
                        ret = ffmpeg.av_parser_parse2(parser, _codecContext, &_packet->data, &_packet->size, buffPtrData, dataSize, ffmpeg.AV_NOPTS_VALUE, ffmpeg.AV_NOPTS_VALUE, 0);
                        if (ret < 0)
                        {
                            break;
                        }

                        buffPtrData += ret;
                        dataSize -= ret;

                        if (_packet->size != 0)
                        {
                            parseRet = StreamDecoder.GetFrame(_codecContext, _frame, _packet);
                            if (parseRet == false)
                            {
                                break;
                            }

                            return StreamDecoder.GetFrameToByteArray(_frame);
                        }
                    }
                }
            }

            ffmpeg.av_parser_close(parser);

            return Array.Empty<byte>();
        }
        public unsafe void GetSimpleHWVideoSet(string path, AVHWDeviceType deviceType = AVHWDeviceType.AV_HWDEVICE_TYPE_CUDA)
        {
            this.Dispose();
            fixed (AVFormatContext** formatContext = &_formatContext)
            {
                StreamDecoder.OpenVideoFile(path, formatContext);
            }

            InitStreamIndex(_formatContext);

            AVCodec* codec;
            var videoStreamIndex = ffmpeg.av_find_best_stream(_formatContext, AVMediaType.AVMEDIA_TYPE_VIDEO, -1, -1, &codec, 0);
            if (videoStreamIndex < 0)
            {
                return;
            }
            SelectVideoStreamIndex = videoStreamIndex;

            var videoStream = _formatContext->streams[videoStreamIndex];
            _pixelFormat = StreamDecoder.GetPixelFormat(_formatContext, videoStreamIndex, deviceType);
            if (_pixelFormat == AVPixelFormat.AV_PIX_FMT_NONE)
            {
                return;
            }

            fixed (AVCodecContext** codecContext = &_codecContext)
            {
                *codecContext = ffmpeg.avcodec_alloc_context3(codec);
                if (codecContext == null)
                {
                    return;
                }
                var ret = ffmpeg.avcodec_parameters_to_context(_codecContext, videoStream->codecpar);
                if (ret < 0)
                {
                    return;
                }

                ret = ffmpeg.avcodec_open2(_codecContext, codec, null);
                if (ret < 0)
                {
                    return;
                }
                _codecContext->get_format = (AVCodecContext_get_format_func)GetHWFormat;
                CodecDecoderInit(_codecContext, deviceType);
            }

            fixed(AVFrame** frame = &_frame)
            {
                *frame = ffmpeg.av_frame_alloc();
            }
            fixed(AVPacket** packet = &_packet)
            {
                *packet = ffmpeg.av_packet_alloc();
            }
        }
        public unsafe AVFrame* GetSimpleHWVideoFrame()
        {
            int ret = 0;
            if (CheckSetHWVideo() == false)
            {
                return null;
            }

            while (ret >= 0)
            {
                ret = ffmpeg.av_read_frame(_formatContext, _packet);
                if (ret < 0)
                {
                    StreamDecoder.GetFrame(_codecContext, null, _pixelFormat);
                    return null;
                }

                if (SelectVideoStreamIndex == _packet->stream_index)
                {
                    return StreamDecoder.GetFrame(_codecContext, _packet, _pixelFormat);
                }
            }

            return null;
        }
        public unsafe OpenCvSharp.Mat GetSimpleHWVideoFrameData()
        {
            int ret = 0;
            if (CheckSetHWVideo() == false)
            {
                return new OpenCvSharp.Mat();
            }

            while (ret >= 0)
            {
                ret = ffmpeg.av_read_frame(_formatContext, _packet);
                if (ret < 0)
                {
                    StreamDecoder.GetFrame(_codecContext, null, _pixelFormat);
                    return new OpenCvSharp.Mat();
                }

                if (SelectVideoStreamIndex == _packet->stream_index)
                {
                    var frame = StreamDecoder.GetFrame(_codecContext, _packet, _pixelFormat);
                    if (frame == null)
                    {
                        continue;
                    }

                    var buffer = StreamDecoder.GetFrameToBytePtr(frame);
                    OpenCvSharp.Mat rgbMat = new();
                    OpenCvSharp.Mat nvMat = new(frame->height * 3 / 2, frame->width, OpenCvSharp.MatType.CV_8UC1, new IntPtr(buffer));
                    OpenCvSharp.Cv2.CvtColor(nvMat, rgbMat, OpenCvSharp.ColorConversionCodes.YUV2BGR_NV12);
                    ffmpeg.av_freep(&buffer);
                    return rgbMat;
                }
            }

            return new OpenCvSharp.Mat();
        }
        public bool CheckSetVideo()
        {
            if (_codecContext == null)
            {
                return false;
            }
            if (_packet == null)
            {
                return false;
            }
            if (_frame == null)
            {
                return false;
            }

            return true;
        }
        public bool CheckSetHWVideo()
        {
            if (_formatContext == null)
            {
                return false;
            }
            if (_packet == null)
            {
                return false;
            }
            if (_codecContext == null)
            {
                return false;
            }
            if (_pixelFormat == AVPixelFormat.AV_PIX_FMT_NONE)
            {
                return false;
            }
            if (SelectVideoStreamIndex == -1)
            {
                return false;
            }

            return true;
        }

        public bool IsSupportDeviceType(AVHWDeviceType deviceType, out string error)
        {
            error = "";
            if (deviceType == AVHWDeviceType.AV_HWDEVICE_TYPE_NONE)
            {
                error = $"{ffmpeg.av_hwdevice_get_type_name(deviceType)} not support";
                return false;
            }

            return true;
        }

        private void InitStreamIndex(AVFormatContext* formatContext)
        {
            InitVideoStreamIndex(formatContext);
            InitAudioStremIndex(formatContext);
            InitDataStremIndex(formatContext);
            InitSubTitleStremIndex(formatContext);
        }
        private void InitVideoStreamIndex(AVFormatContext* formatContext)
        {
            _videoStreamIndex = StreamDecoder.GetStreamIndex(formatContext, AVMediaType.AVMEDIA_TYPE_VIDEO);
            SelectVideoStreamIndex = _videoStreamIndex.Count > 0 ? _videoStreamIndex.First() : -1;
        }
        private void InitAudioStremIndex(AVFormatContext* formatContext)
        {
            _audioStreamIndex = StreamDecoder.GetStreamIndex(formatContext, AVMediaType.AVMEDIA_TYPE_AUDIO);
            SelectAudioStreamIndex = _audioStreamIndex.Count > 0 ? _audioStreamIndex.First() : -1;
        }
        private void InitDataStremIndex(AVFormatContext* formatContext) 
        {
            _dataStreamIndex = StreamDecoder.GetStreamIndex(formatContext, AVMediaType.AVMEDIA_TYPE_DATA);
            SelectDataStreamIndex = _dataStreamIndex.Count > 0 ? _dataStreamIndex.First() : -1;
        }
        private void InitSubTitleStremIndex(AVFormatContext* formatContext)
        {
            _subTitleStreamIndex = StreamDecoder.GetStreamIndex(formatContext, AVMediaType.AVMEDIA_TYPE_SUBTITLE);
            SelectSubTitleStreamIndex = _subTitleStreamIndex.Count > 0 ? _subTitleStreamIndex.First() : -1;
        }
        private unsafe int CodecDecoderInit(AVCodecContext* codecContext, AVHWDeviceType type)
        {
            int ret = 0;
            fixed(AVBufferRef** buffer = &_bufferRef)
            {
                ret = ffmpeg.av_hwdevice_ctx_create(buffer, type, null, null, 0);
                if (ret < 0)
                {
                    //                throw new Exception("failed create hw device");
                    return ret;
                }
                codecContext->hw_device_ctx = ffmpeg.av_buffer_ref(*buffer);
            }

            return ret;
        }

        public AVPixelFormat GetHWFormat(AVCodecContext* codecContext, AVPixelFormat* pixelFormat)
        {
            AVPixelFormat* ret;
            for (ret = pixelFormat; *ret != AVPixelFormat.AV_PIX_FMT_NONE; ret++)
            {
                if (*ret == _pixelFormat)
                {
                    return *ret;
                }
            }

            return AVPixelFormat.AV_PIX_FMT_NONE;
        }

        public void Dispose()
        {
            if (_frame != null)
            {
                ffmpeg.av_frame_unref(_frame);
                ffmpeg.av_free(_frame);
            }

            if (_packet != null)
            {
                ffmpeg.av_packet_unref(_packet);
                ffmpeg.av_free(_packet);
            }

            if (_codecContext != null)
            {
                fixed (AVCodecContext** codecContext =  &_codecContext)
                {
                    ffmpeg.avcodec_free_context(codecContext);
                    ffmpeg.avcodec_close(*codecContext);
                }
            }

            if (_bufferRef != null)
            {
                var buffer = _bufferRef;
                ffmpeg.av_buffer_unref(&buffer);
//                ffmpeg.av_free(_bufferRef);
            }

            if (_formatContext != null)
            {
                var formatContext = _formatContext;
                ffmpeg.avformat_close_input(&formatContext);
            }

            _reader.Dispose();

            StreamDecoder.Dispose();
        }
    }
}
