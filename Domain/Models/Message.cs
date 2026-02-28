using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraceWPF.Domain.Models
{
    /// <summary>
    /// 消息实体类，存储系统消息内容及创建时间（如欢迎消息）。
    /// Message entity class that stores system message content and creation time (e.g., welcome messages).
    /// </summary>
    public class Message
    {
        /// <summary>
        /// 消息内容文本。
        /// The message content text.
        /// </summary>
        public string Content { get; set; } = "";

        /// <summary>
        /// 消息创建时间。
        /// The time when the message was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}

