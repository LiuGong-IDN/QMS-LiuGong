using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.Model
{
    public partial class FilePath
    {
        public int ID { get; set; }
        public string FILE_TYPE { get; set; }
        public string FILE_CATEGORY { get; set; }
        public int FILE_CATEGORY_ID { get; set; }
        public string FILE_NAME { get; set; }
        public string DIRECTORY { get; set; }
    }
}