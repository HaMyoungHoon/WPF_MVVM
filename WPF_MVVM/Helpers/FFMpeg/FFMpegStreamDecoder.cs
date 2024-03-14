using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using WPF_MVVM.Bases;

namespace WPF_MVVM.Helpers.FFMpeg
{
    // throw new Exception 을 살릴까, out string error를 넣을까
    internal unsafe class FFMpegStreamDecoder : IDisposable
    {
        public FFMpegStreamDecoder(string rootPath = @"Libs\FFmpeg")
        {
            ffmpeg.RootPath = rootPath;
        }
        public unsafe void Test()
        {
            return;
        }
        public unsafe double GetSimpleVideoFrameRate(string path)
        {
            var formatContext = OpenVideoFile(path);
            AVCodec* codec = null;
            var videoStreamIndex = ffmpeg.av_find_best_stream(formatContext, AVMediaType.AVMEDIA_TYPE_VIDEO, -1, -1, &codec, 0);
            var videoStream = videoStreamIndex < 0 ? GetStream(formatContext, AVMediaType.AVMEDIA_TYPE_VIDEO) : formatContext->streams[videoStreamIndex];
            AVRational frameRate = videoStream->avg_frame_rate;
            var timeBase = videoStream->time_base;
            return frameRate.num / frameRate.den;
        }
        public unsafe TimeSpan GetSimpleVideoDuration(string path)
        {
            var formatContext = OpenVideoFile(path);
            AVCodec* codec = null;
            var videoStreamIndex = ffmpeg.av_find_best_stream(formatContext, AVMediaType.AVMEDIA_TYPE_VIDEO, -1, -1, &codec, 0);
            var videoStream = videoStreamIndex < 0 ? GetStream(formatContext, AVMediaType.AVMEDIA_TYPE_VIDEO) : formatContext->streams[videoStreamIndex];
            AVRational frameRate = videoStream->avg_frame_rate;
            var duration = videoStream->duration;
            var timeBase = videoStream->time_base;
            return TimeSpan.FromTicks(duration * timeBase.num * TimeSpan.TicksPerSecond / timeBase.den);
        }

        public unsafe AVFormatContext* OpenVideoFile(string path)
        {
            AVFormatContext* formatContext = ffmpeg.avformat_alloc_context();
            ffmpeg.avdevice_register_all();
            ffmpeg.avformat_network_init();
            if (ffmpeg.avformat_open_input(&formatContext, path, null, null) != 0)
            {
                //                throw new Exception("file open failed");
                return null;
            }

            if (ffmpeg.avformat_find_stream_info(formatContext, null) < 0)
            {
                //                throw new Exception("not find stream information.");
                return null;
            }

            return formatContext;
        }
        public unsafe void OpenVideoFile(string path, AVFormatContext** formatContext)
        {
            *formatContext = ffmpeg.avformat_alloc_context();
            ffmpeg.avdevice_register_all();
            ffmpeg.avformat_network_init();
            if (ffmpeg.avformat_open_input(formatContext, path, null, null) != 0)
            {
                //                throw new Exception("file open failed");
                *formatContext = null;
            }

            if (ffmpeg.avformat_find_stream_info(*formatContext, null) < 0)
            {
                //                throw new Exception("not find stream information.");
                *formatContext = null;
            }
        }

        public unsafe bool GetFrame(AVCodecContext* codecContext, AVFrame* frame, AVPacket* packet)
        {
            int ret = ffmpeg.avcodec_send_packet(codecContext, packet);
            if (ret < 0)
            {
                return false;
            }

            while (ret >= 0)
            {
                ret = ffmpeg.avcodec_receive_frame(codecContext, frame);
                if (ret == ffmpeg.AVERROR(ffmpeg.EAGAIN) || ret == ffmpeg.AVERROR_EOF)
                {
                    return true;
                }

                if (ret < 0)
                {
                    return false;
                }

            }

            return true;
        }
        public unsafe AVFrame* GetFrame(AVCodecContext* codecContext, AVPacket* packet, AVPixelFormat avPixelFormat)
        {
            AVFrame* frame = null;
            AVFrame* swFrame = null;
            var ret = ffmpeg.avcodec_send_packet(codecContext, packet);
            if (ret < 0)
            {
//                throw new Exception("decoding error");
                return null;
            }

            while (true)
            {
                frame = ffmpeg.av_frame_alloc();
                swFrame = ffmpeg.av_frame_alloc();
                if (frame == null || swFrame == null)
                {
                    //                throw new Exception("i think memory shortage");
                    ret = ffmpeg.AVERROR(ffmpeg.ENOMEM);
                    goto fail;
                }

                ret = ffmpeg.avcodec_receive_frame(codecContext, frame);
                if (ret == ffmpeg.AVERROR(ffmpeg.EAGAIN) || ret == ffmpeg.AVERROR_EOF)
                {
                    ffmpeg.av_frame_free(&frame);
                    ffmpeg.av_frame_free(&swFrame);
                    return null;
                }
                if (ret < 0)
                {
                    //                throw new Exception("decoding error");
                    goto fail;
                }

                var pixelFormatValue = Extensions.GetEnumValue<int, AVPixelFormat>(avPixelFormat);

                if (frame->format == pixelFormatValue)
                {
                    ret = ffmpeg.av_hwframe_transfer_data(swFrame, frame, 0);
                    if (ret < 0)
                    {
                        //                    throw new Exception("");
                        goto fail;
                    }
                    ffmpeg.av_frame_free(&frame);
                    return swFrame;
                }
                else
                {
                    ffmpeg.av_frame_free(&swFrame);
                    return frame;
                }

            fail:
                ffmpeg.av_frame_free(&frame);
                ffmpeg.av_frame_free(&swFrame);
                if (ret < 0)
                {
                    return null;
                }
            }
        }

        public unsafe byte* GetFrameToBytePtr(AVFrame* frame)
        {
            var size = ffmpeg.av_image_get_buffer_size((AVPixelFormat)frame->format, frame->width, frame->height, 1);
            byte* buffer = (byte*)ffmpeg.av_malloc((ulong)size);
            if (buffer == null)
            {
                //                throw new Exception("i think memory shortage");
                ffmpeg.av_frame_free(&frame);
                return null;
            }

            byte_ptrArray4* frameData = (byte_ptrArray4*)&frame->data;
            int_array4* lineSize = (int_array4*)&frame->linesize;
            var ret = ffmpeg.av_image_copy_to_buffer(buffer, size, *frameData, *lineSize, (AVPixelFormat)frame->format, frame->width, frame->height, 1);
            if (ret < 0)
            {
                //                throw new Exception("buffer copy error");
                ffmpeg.av_frame_free(&frame);
                ffmpeg.av_freep(&buffer);
                return null;
            }

            return buffer;
        }
        public unsafe byte[] GetFrameToByteArray(AVFrame* frame)
        {
            var size = ffmpeg.av_image_get_buffer_size((AVPixelFormat)frame->format, frame->width, frame->height, 1);
            byte* buffer = (byte*)ffmpeg.av_malloc((ulong)size);
            if (buffer == null)
            {
                //                throw new Exception("i think memory shortage");
                ffmpeg.av_frame_free(&frame);
                return Array.Empty<byte>();
            }

            byte_ptrArray4* frameData = (byte_ptrArray4*)&frame->data;
            int_array4* lineSize = (int_array4*)&frame->linesize;
            var ret = ffmpeg.av_image_copy_to_buffer(buffer, size, *frameData, *lineSize, (AVPixelFormat)frame->format, frame->width, frame->height, 1);
            if (ret < 0)
            {
                //                throw new Exception("buffer copy error");
                ffmpeg.av_frame_free(&frame);
                ffmpeg.av_freep(&buffer);
                return Array.Empty<byte>();
            }

            ReadOnlySpan<byte> spanByte = new Span<byte>(buffer, size);
            var byteArray = spanByte.ToArray();
            ffmpeg.av_frame_free(&frame);
            ffmpeg.av_freep(&buffer);

            return byteArray;
        }

        public unsafe AVPixelFormat GetPixelFormat(AVFormatContext* formatContext, int videoStreamIndex = -1, AVHWDeviceType deviceType = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE, FFMpegHWConfigMethod configMethod = FFMpegHWConfigMethod.AV_CODEC_HW_CONFIG_METHOD_HW_DEVICE_CTX)
        {
            AVCodec* codec = GetCodec(formatContext, AVMediaType.AVMEDIA_TYPE_VIDEO, videoStreamIndex);
            return GetPixelFormat(codec, deviceType, configMethod);
        }
        public unsafe AVPixelFormat GetPixelFormat(AVCodec* codec, AVHWDeviceType deviceType = AVHWDeviceType.AV_HWDEVICE_TYPE_NONE, FFMpegHWConfigMethod configMethod = FFMpegHWConfigMethod.AV_CODEC_HW_CONFIG_METHOD_HW_DEVICE_CTX)
        {
            AVCodecHWConfig* codecConfig = null;
            int i = 0;
            var configMethodFlag = Extensions.GetEnumValue<int, FFMpegHWConfigMethod>(configMethod);
            do
            {
                codecConfig = ffmpeg.avcodec_get_hw_config(codec, i);
                if (codecConfig == null)
                {
                    return AVPixelFormat.AV_PIX_FMT_NONE;
                }
                if ((codecConfig->methods & configMethodFlag) == configMethodFlag && codecConfig->device_type == deviceType)
                {
                    return codecConfig->pix_fmt;
                }
                i++;
            } while (codecConfig != null);

            if (codecConfig == null)
            {
//                throw new Exception($"{Marshal.PtrToStringUTF8(new IntPtr(codec->name))} not support");
            }

            return AVPixelFormat.AV_PIX_FMT_NONE;
        }

        public unsafe AVCodec* GetCodec(AVFormatContext* formatContext, AVMediaType avMediaType, int wantedStreamNb = -1, int relatedStream = -1, int flag = 0)
        {
            AVCodec* codec;
            var ret = ffmpeg.av_find_best_stream(formatContext, avMediaType, wantedStreamNb, relatedStream, &codec, flag);
            if (ret < 0)
            {
                //                throw new Exception("video codec not found");
                return null;
            }

            return codec;
        }

        public unsafe AVStream* GetStream(AVFormatContext* formatContext, AVMediaType avMediaType = AVMediaType.AVMEDIA_TYPE_UNKNOWN)
        {
            var stream = -1;
            for (int i = 0; i < formatContext->nb_streams; i++)
            {
                if (formatContext->streams[i]->codecpar->codec_type == avMediaType)
                {
                    stream = i;
                    break;
                }
            }

            if (stream == -1)
            {
                //                throw new Exception("video stream not found");
                return null;
            }

            return formatContext->streams[stream];
        }

        public List<int> GetStreamIndex(AVFormatContext* formatContext, AVMediaType avMediaType = AVMediaType.AVMEDIA_TYPE_UNKNOWN)
        {
            List<int> ret = new();
            for (int i = 0; i < formatContext->nb_streams; i++)
            {
                if (formatContext->streams[i]->codecpar->codec_type == avMediaType)
                {
                    ret.Add(i);
                }
            }

            return ret;
        }

        public void Dispose()
        {
            ffmpeg.avformat_network_deinit();
        }
    }
}
