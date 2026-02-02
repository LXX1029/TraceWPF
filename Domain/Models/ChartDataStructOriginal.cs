using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraceWPF.Domain.Models
{
    public class ChartDataStructDto
    {
        public string ts { get; set; }

        /// <summary>
        /// X轴数据  时间/频率
        /// </summary>
        public double xaxis { get; set; }

        /// <summary>
        /// Y轴速度 单位：mm/s
        /// 暂用于 时域、频域图
        /// </summary>
        public double yaxisspeed { get; set; }

        /// <summary>
        /// Y轴位移 单位：微米
        /// 暂用于 时域、频域图
        /// </summary>
        public double yaxisdisplacement { get; set; }

        /// <summary>
        /// Y轴加速度  单位：米每次方秒
        /// 暂用于 时域、频域图、包络图
        /// </summary>
        public double yaxisacceleration { get; set; }

        /// <summary>
        /// 采集卡序列号
        /// </summary>
        public string acquistnumber { get; set; }

        /// <summary>
        /// 通道号
        /// </summary>
        public string channelnumber { get; set; }

        /// <summary>
        /// 测点Id
        /// </summary>
        public int diagnosepointid { get; set; }
    }
}
