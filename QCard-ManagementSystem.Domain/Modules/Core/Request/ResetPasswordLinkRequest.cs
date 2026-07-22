using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QCard.ManagementSystem.Domain.Modules.Core.Request
{
    public class ResetPasswordLinkRequest
    {
        public string Email { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
    }
}