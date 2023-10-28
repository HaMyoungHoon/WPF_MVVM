using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WPF_MVVM.Models.RestAPI.DataGov.WthrChartInfo
{
    /// <summary>
    /// GET 지상일기도조회 
    /// http://apis.data.go.kr/1360000/WthrChartInfoService/getSurfaceChart
    /// </summary>
    internal class GetSurfaceChartResModel
    {
        public List<MainFile>? item { get; set; }
        internal class MainFile
        {
            /// <summary>
            /// 지상 일기도 이미지 "[img.png, img.png]" 형식으로 옴.
            /// xml 일 땐 node 구분해서 오면서 json으로 올 땐 이딴식으로 오네
            /// </summary>
            [JsonPropertyName("man-file")]
            public string? manFile { get; set; }

            public List<string> GetFileList()
            {
                if (manFile == null || manFile.Length == 0)
                {
                    return new();
                }

                return manFile.Replace("[", "").Replace("]", "").Split(',').ToList();
            }
        }
    }
}
