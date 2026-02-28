using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TraceWPF.Domain.Models
{
    /// <summary>
    /// 趋势数据传输对象，存储振动诊断的各项趋势指标数据，包括转速、振动总值、
    /// 故障特征频率（BPFI/BPFO/BSF/FTF）、各类峰值、温度、位移等。
    /// 
    /// Trend Data Transfer Object that stores various trend indicator data for vibration diagnostics,
    /// including RPM, vibration total value, fault characteristic frequencies (BPFI/BPFO/BSF/FTF),
    /// various peak values, temperature, displacement, etc.
    /// </summary>
    public class TrendDto
    {
        /// <summary>
        /// 默认构造函数。
        /// Default constructor.
        /// </summary>
        public TrendDto()
        {

        }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime ts { get; set; }

        /// <summary>
        /// 诊断结果
        /// </summary>
        public double res { get; set; }

        /// <summary>
        /// 转速频率
        /// </summary>
        public double fr { get; set; }

        /// <summary>
        /// 采样频率
        /// </summary>
        public int fs { get; set; }


        /// <summary>
        /// 频率线数
        /// </summary>
        public int fsxs { get; set; }

        /// <summary>
        /// 内圈故障频率
        /// </summary>
        public double bpfi { get; set; }

        /// <summary>
        /// 外圈故障频率
        /// </summary>
        public double bpfo { get; set; }

        /// <summary>
        /// 滚动体故障频率
        /// </summary>
        public double bsf { get; set; }

        /// <summary>
        /// 保持架故障频率
        /// </summary>
        public double ftf { get; set; }


        /// <summary>
        /// 转速
        /// </summary>
        public double rpm { get; set; }

        /// <summary>
        /// 振动总值
        /// </summary>
        public double rss { get; set; }
        /// <summary>
        /// 振动烈度
        /// </summary>
        public double rms { get; set; }
        /// <summary>
        /// 地脚能量值
        /// </summary>
        public double djee { get; set; }
        /// <summary>
        /// 峭度值
        /// </summary>
        public double ku { get; set; }
        /// <summary>
        /// 包络峰值
        /// </summary>
        public double blfmax { get; set; }

        /// <summary>
        /// 峰值频率包络
        /// </summary>
        public double blpfz { get; set; }
        /// <summary>
        /// 峰值频率速度
        /// </summary>
        public double plvmax { get; set; }
        /// <summary>
        /// 峰值速度
        /// </summary>
        public double plvmaxf { get; set; }
        /// <summary>
        /// 平均峰值频率
        /// </summary>
        public double meanpeakfreq { get; set; }
        /// <summary>
        /// 频率峰值幅值加速度
        /// </summary>
        public double plamaxf { get; set; }
        /// <summary>
        /// 峰值频率加速度
        /// </summary>
        public double plamax { get; set; }

        /// <summary>
        /// 预留指标值
        /// </summary>
        public double ydz { get; set; }
        /// <summary>
        /// 偏度
        /// </summary>
        public double pdz { get; set; }

        /// <summary>
        /// 计算类型（0: 默认, 其他值表示不同计算策略）
        /// Compute type (0: default, other values indicate different calculation strategies).
        /// </summary>
        public int computetype { get; set; }

        /// <summary>
        /// 运行状态（0: 停机, 1: 运行）
        /// Running status (0: stopped, 1: running).
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 健康度指标
        /// Health indicator.
        /// </summary>
        public double healthindicator { get; set; }

        /// <summary>
        /// 温度（℃）
        /// Temperature in degrees Celsius.
        /// </summary>
        public double temperature { get; set; }

        /// <summary>
        /// 采集卡序列号
        /// Data acquisition card serial number.
        /// </summary>
        public string acquistnumber { get; set; }

        /// <summary>
        /// 通道号
        /// Channel number.
        /// </summary>
        public string channelnumber { get; set; }

        /// <summary>
        /// 加速度 RMS 值
        /// Acceleration RMS value.
        /// </summary>
        public double plarms { get; set; }

        /// <summary>
        /// 加速度峰值
        /// Acceleration peak value.
        /// </summary>
        public double plafz { get; set; }
        /// <summary>
        /// 一倍频幅值
        /// </summary>
        public double ybpfz { get; set; }

        /// <summary>
        /// 二倍频幅值
        /// </summary>
        public double ebpfz { get; set; }

        /// <summary>
        /// 三倍频幅值
        /// </summary>
        public double sbpfz { get; set; }
        /// <summary>
        /// 电流
        /// </summary>
        public double electricity { get; set; }

        /// <summary>
        /// 位移
        /// </summary>
        public double displacement { get; set; }

        /// <summary>
        /// 比能
        /// </summary>
        public double specific { get; set; }

        /// <summary>
        /// 浆压
        /// </summary>
        public double pulppressure { get; set; }
        /// <summary>
        /// 计算后的采样点数
        /// </summary>
        public int sampsperchan { get; set; }

        /// <summary>
        /// 产线Id
        /// </summary>

        public int productionid { get; set; }
        /// <summary>
        /// 部门Id
        /// </summary>

        public int departmentid { get; set; }
        /// <summary>
        /// 设备Id
        /// </summary>

        public int deviceid { get; set; }
        /// <summary>
        /// 测点Id
        /// </summary>
        public int diagnosepointid { get; set; }
    }
}
