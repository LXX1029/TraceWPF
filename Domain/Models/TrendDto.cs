using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TraceWPF.Domain.Models
{
    public class TrendDto
    {
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
        public double ydz { get; set; }
        /// <summary>
        /// 偏度
        /// </summary>
        public double pdz { get; set; }
        public int computetype { get; set; }
        public int status { get; set; }
        public double healthindicator { get; set; }
        public double temperature { get; set; }
        public string acquistnumber { get; set; }
        public string channelnumber { get; set; }
        public double plarms { get; set; }
        public double plafz { get; set; } // 加速度峰值
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
