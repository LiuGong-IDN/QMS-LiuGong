using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.Request
{
    public partial class PasswordResetTokenRequest
    {
        public string Email { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiredAt { get; set; }
        public bool IsUsed { get; set; }
    }
}