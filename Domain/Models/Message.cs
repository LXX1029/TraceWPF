namespace TraceWPF.Domain.Models
{
    using System;

    public class Message
    {
        public string Content { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}

