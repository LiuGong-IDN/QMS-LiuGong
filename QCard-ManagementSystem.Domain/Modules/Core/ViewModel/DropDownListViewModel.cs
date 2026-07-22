using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.ViewModel
{
    public class DropDownListViewModel
    {
        public Guid ID { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }

    }
}
