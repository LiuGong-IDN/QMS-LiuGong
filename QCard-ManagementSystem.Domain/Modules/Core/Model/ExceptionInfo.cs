using System;
using System.Collections.Generic;
using System.Text;

namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    public class ExceptionInfo
    {
        public ExceptionInfo(string title, string message)
        {
            Title = title;
            Message = message;
            Exception = !string.IsNullOrEmpty(message);
        }
        public bool Exception { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
