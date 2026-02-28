using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraceWPF.Domain.Models
{
    /// <summary>
    /// 加速度数据传输对象，存储振动采集的原始加速度值及关联信息。
    /// Acceleration Data Transfer Object that stores raw acceleration values from vibration acquisition and associated information.
    /// </summary>
    public class AccelerationDto
    {
        /// <summary>
        /// 时间戳。
        /// Timestamp.
        /// </summary>
        public string ts { get; set; }

        /// <summary>
        /// 原始加速度值。
        /// Original acceleration value.
        /// </summary>
        public double orginalvalue { get; set; }

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
        /// 设备 ID。
        /// Device ID.
        /// </summary>
        public int deviceId { get; set; }

        /// <summary>
        /// 测点 ID。
        /// Diagnose point ID.
        /// </summary>
        public string diagnosepointId { get; set; }

    }
}
