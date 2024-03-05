using FFmpeg.AutoGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WPF_MVVM.Bases;

namespace WPF_MVVM.Helpers
{
    internal unsafe class FFMpegStreamDecoder : IDisposable
    {
        private readonly int _streamIndex;
        private readonly AVCodecContext* _codecContext;
        private readonly AVFormatContext* _formatContext;
        private readonly AVFrame* _frame;
        private readonly AVPacket* _packet;

        public FFMpegStreamDecoder(string rootPath)
        {
            ffmpeg.RootPath = rootPath;
        }
        public unsafe double GetFrameRate(string path)
        {
            var formatContext = OpenVideoFile(path);
            var videoStream = GetVideoStream(formatContext);
            AVRational frameRate = videoStream->avg_frame_rate;
            var timeBase = videoStream->time_base;
            return frameRate.num / frameRate.den;
        }
        public unsafe TimeSpan GetVideoDuration(string path)
        {
            var formatContext = OpenVideoFile(path);
            var videoStream = GetVideoStream(formatContext);
            AVRational frameRate = videoStream->avg_frame_rate;
            var duration = videoStream->duration;
            var timeBase = videoStream->time_base;
            return TimeSpan.FromTicks(duration * timeBase.num * TimeSpan.TicksPerSecond / timeBase.den);
        }

        private unsafe AVFormatContext* OpenVideoFile(string path)
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
        private unsafe AVStream* GetVideoStream(AVFormatContext* formatContext)
        {
            var videoStreamIndex = -1;
            for (int i = 0; i < formatContext->nb_streams; i++)
            {
                if (formatContext->streams[i]->codecpar->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO)
                {
                    videoStreamIndex = i;
                    break;
                }
            }

            if (videoStreamIndex == -1)
            {
                //                throw new Exception("video stream not found");
                return null;
            }

            return formatContext->streams[videoStreamIndex];
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
                ffmpeg.avcodec_close(_codecContext);
            }

            if (_formatContext != null)
            {
                var formatContext = _formatContext;
                ffmpeg.avformat_close_input(&formatContext);
            }
            ffmpeg.avformat_network_deinit();
        }
    }
}
