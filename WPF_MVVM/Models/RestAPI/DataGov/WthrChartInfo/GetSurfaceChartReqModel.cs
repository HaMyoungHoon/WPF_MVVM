using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_MVVM.Models.RestAPI.DataGov.WthrChartInfo
{
    /// <summary>
    /// GET 지상일기도조회 
    /// http://apis.data.go.kr/1360000/WthrChartInfoService/getSurfaceChart
    /// 근데 default가 의미 없는 게 dataType 빼곤 다 입력 해야 함.
    /// </summary>
    internal class GetSurfaceChartReqModel
    {
        /// <summary>
        /// 인증키
        /// </summary>
        public string serviceKey { get; set; }
        /// <summary>
        /// 한 페이지 결과 수
        /// default : 10
        /// </summary>
        public int numOfRows { get; set; }
        /// <summary>
        /// 페이지 번호
        /// default : 1
        /// </summary>
        public int pageNo { get; set; }
        /// <summary>
        /// 요청자료형식 (XML / JSON)
        /// default : XML
        /// </summary>
        public string? dataType { get; set; }
        /// <summary>
        /// 지상코드 - 뭔소린지 모르겠음
        /// 3: 지상 03
        /// 12: 지상 12
        /// 24: 지상 24시간예상
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 년월일(YYYYMMDD)
        /// 입력안하면 현재 년월일
        /// </summary>
        public string time { get; set; }
    }
}
