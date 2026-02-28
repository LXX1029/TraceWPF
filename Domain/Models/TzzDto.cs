using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraceWPF.Domain.Models
{
    /// <summary>
    /// 特征值（TZZ）数据传输对象，存储振动诊断中的特征值列表数据。
    /// Feature value (TZZ) Data Transfer Object that stores feature value list data in vibration diagnostics.
    /// </summary>
    public class TzzDto
    {
        /// <summary>
        /// 时间戳。
        /// Timestamp.
        /// </summary>
        public string ts { get; set; }

        /// <summary>
        /// 特征值列表（JSON 或逗号分隔的字符串）。
        /// Feature value list (JSON or comma-separated string).
        /// </summary>
        public string tzzlist { get; set; }

        /// <summary>
        /// 故障类型 ID。
        /// Failure type ID.
        /// </summary>
        public int failureid { get; set; }

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
        public int deviceid { get; set; }

        /// <summary>
        /// 测点 ID。
        /// Diagnose point ID.
        /// </summary>
        public int diagnosepointid { get; set; }
    }
}
