using FluentValidation.Results;
using System;
using System.Collections.Generic;

namespace QCard.ManagementSystem.Models
{
    public class Result
    {
        public bool IsSaved { get; set; }
        public bool IsSuccess { get; set; }
        public string Messages { get; set; }
        public ValidationResult Validate { get; set; }
        public List<ValidationResult> Validates { get; set; }
        public string DocumentNo { get; set; }
        public List<string> ListDocumentNo { get; set; }
        public Guid OutID { get; set; }
        public List<Guid> ListOutID { get; set; }
    }
}
