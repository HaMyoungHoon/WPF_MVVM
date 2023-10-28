using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace WPF_MVVM.Models.RestAPI.DataGov.WthrChartInfo
{
    /// <summary>
    /// GET 보조일기도조회
    /// http://apis.data.go.kr/1360000/WthrChartInfoService/getAuxillaryChart
    /// </summary>
    internal class GetAuxillaryChartResModel
    {
        public List<N500R30IFile>? item { get; set; }
        internal class N500R30IFile
        {
            /// <summary>
            /// 보조 일기도 이미지 "[img.png, img.png]" 형식으로 옴.
            /// xml 일 땐 node 구분해서 오면서 json으로 올 땐 이딴식으로 오네
            /// </summary>
            [JsonPropertyName("n500-file")]
            public string? n500File { get; set; }
            /// <summary>
            /// 보조 일기도 이미지 "[img.png, img.png]" 형식으로 옴.
            /// xml 일 땐 node 구분해서 오면서 json으로 올 땐 이딴식으로 오네
            /// </summary>
            [JsonPropertyName("r30i-file")]
            public string? r30iFile { get; set; }

            public List<string> GetFileList()
            {
                if (n500File != null && n500File.Length > 0)
                {
                    return n500File.Replace("[", "").Replace("]", "").Split(',').ToList();
                }

                if (r30iFile != null && r30iFile.Length > 0)
                {
                    return r30iFile.Replace("[", "").Replace("]", "").Split(',').ToList();
                }

                return new();
            }
        }
    }
}
