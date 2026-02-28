using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraceWPF.Domain.Models
{
    /// <summary>
    /// 图表原始数据传输对象，存储时域/频域/包络图的 X-Y 轴数据（速度、位移、加速度）。
    /// Chart original data DTO that stores X-Y axis data (speed, displacement, acceleration) for time-domain, frequency-domain, and envelope charts.
    /// </summary>
    public class ChartDataStructDto
    {
        /// <summary>
        /// 时间戳。
        /// Timestamp.
        /// </summary>
        public string ts { get; set; }

        /// <summary>
        /// X轴数据（时间/频率）。
        /// X-axis data (time / frequency).
        /// </summary>
        public double xaxis { get; set; }

        /// <summary>
        /// Y轴速度，单位：mm/s。暂用于时域、频域图。
        /// Y-axis speed in mm/s. Currently used for time-domain and frequency-domain charts.
        /// </summary>
        public double yaxisspeed { get; set; }

        /// <summary>
        /// Y轴位移，单位：微米（μm）。暂用于时域、频域图。
        /// Y-axis displacement in micrometers (μm). Currently used for time-domain and frequency-domain charts.
        /// </summary>
        public double yaxisdisplacement { get; set; }

        /// <summary>
        /// Y轴加速度，单位：m/s²。暂用于时域、频域图、包络图。
        /// Y-axis acceleration in m/s². Currently used for time-domain, frequency-domain, and envelope charts.
        /// </summary>
        public double yaxisacceleration { get; set; }

        /// <summary>
        /// 采集卡序列号。
        /// Data acquisition card serial number.
        /// </summary>
        public string acquistnumber { get; set; }

        /// <summary>
        /// 通道号。
        /// Channel number.
        /// </summary>
        public string channelnumber { get; set; }

        /// <summary>
        /// 测点 ID。
        /// Diagnose point ID.
        /// </summary>
        public int diagnosepointid { get; set; }
    }
}
