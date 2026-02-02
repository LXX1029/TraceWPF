using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TraceWPF.Domain.Models
{
    public partial class DataBaseParam : ObservableObject
    {
        //[ObservableProperty]
        //private string targetDbName = "onlineanalysisv20";
        /// <summary>
        /// 7/14/30/60/90/120/180/365
        /// </summary>
        [ObservableProperty]
        private int keepDays = 90;
        /// <summary>
        /// 7/14/21/28
        /// </summary>
        [ObservableProperty]
        private int duration = 7;
        /// <summary>
        /// 1024/2048/4096/8192
        /// </summary>
        [ObservableProperty]
        private int cachesize = 2048;
        /// <summary>
        /// 1024/2048/4096/8192
        /// </summary>
        [ObservableProperty]
        private int buffer = 4096;
        /// <summary>
        /// 4096/8192/16384/32768
        /// </summary>
        [ObservableProperty]
        private int pages = 8192;
        /// <summary>
        /// 16/32
        /// </summary>
        [ObservableProperty]
        private int pagesize = 32;
        /// <summary>
        /// 200-10000
        /// </summary>
        [ObservableProperty]
        private int maxrows = 10000;
        /// <summary>
        /// 最高至CPU核数的3/4
        /// </summary>
        [ObservableProperty]
        private int vgroups = 2;
        /// <summary>
        /// 4/8/16
        /// </summary>
        [ObservableProperty]
        private int stt_trigger = 16;
    }
}
