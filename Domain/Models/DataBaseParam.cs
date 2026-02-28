using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TraceWPF.Domain.Models
{
    /// <summary>
    /// TDengine 数据库创建参数模型，包含 KEEP、DURATION、CACHESIZE、BUFFER 等配置项。
    /// 继承 ObservableObject 以支持 UI 数据绑定。
    /// 
    /// TDengine database creation parameter model containing KEEP, DURATION, CACHESIZE, BUFFER, and other configuration options.
    /// Inherits ObservableObject to support UI data binding.
    /// </summary>
    public partial class DataBaseParam : ObservableObject
    {
        //[ObservableProperty]
        //private string targetDbName = "onlineanalysisv20";
        /// <summary>
        /// 数据保留天数，可选值：7/14/30/60/90/120/180/365。
        /// Data retention days. Options: 7/14/30/60/90/120/180/365.
        /// </summary>
        [ObservableProperty]
        private int keepDays = 90;
        /// <summary>
        /// 数据文件存储周期（天），可选值：7/14/21/28。
        /// Data file storage duration in days. Options: 7/14/21/28.
        /// </summary>
        [ObservableProperty]
        private int duration = 7;
        /// <summary>
        /// 缓存大小（MB），可选值：1024/2048/4096/8192。
        /// Cache size in MB. Options: 1024/2048/4096/8192.
        /// </summary>
        [ObservableProperty]
        private int cachesize = 2048;
        /// <summary>
        /// 写入缓冲区大小（MB），可选值：1024/2048/4096/8192。
        /// Write buffer size in MB. Options: 1024/2048/4096/8192.
        /// </summary>
        [ObservableProperty]
        private int buffer = 4096;
        /// <summary>
        /// 内存页数，可选值：4096/8192/16384/32768。
        /// Number of memory pages. Options: 4096/8192/16384/32768.
        /// </summary>
        [ObservableProperty]
        private int pages = 8192;
        /// <summary>
        /// 内存页大小（KB），可选值：16/32。
        /// Memory page size in KB. Options: 16/32.
        /// </summary>
        [ObservableProperty]
        private int pagesize = 32;
        /// <summary>
        /// 每个数据块最大行数，范围：200-10000。
        /// Maximum rows per data block. Range: 200-10000.
        /// </summary>
        [ObservableProperty]
        private int maxrows = 10000;
        /// <summary>
        /// 虚拟节点组数，最高至 CPU 核数的 3/4。
        /// Number of virtual node groups. Maximum is 3/4 of CPU cores.
        /// </summary>
        [ObservableProperty]
        private int vgroups = 2;
        /// <summary>
        /// SST 文件合并触发阈值，可选值：4/8/16。
        /// SST file merge trigger threshold. Options: 4/8/16.
        /// </summary>
        [ObservableProperty]
        private int stt_trigger = 16;
    }
}
