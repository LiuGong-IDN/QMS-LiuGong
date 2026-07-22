using Agit.Framework.Web;
using Newtonsoft.Json;

namespace QCard.ManagementSystem.Infrastructure.Session
{
    public static class SessionManager
    {
        public static UserSession UserSession
        {
            get
            {
                try
                {
                    var context = HttpContext.Current;

                    // Cek dulu dari session
                    if (context.Session != null && context.Session.TryGetValue(AppConstants.SessionKey, out var sessionBytes))
                    {
                        var sessionJson = System.Text.Encoding.UTF8.GetString(sessionBytes);
                        return JsonConvert.DeserializeObject<UserSession>(sessionJson);
                    }

                    // Jika session null atau kosong, coba dari cookie
                    if (context.Request.Cookies.TryGetValue(AppConstants.SessionKey, out var cookieValue))
                    {
                        var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(cookieValue));
                        return JsonConvert.DeserializeObject<UserSession>(decoded);
                    }

                    return null;
                }
                catch
                {
                    return null;
                }
            }
        }

        public static long Id => UserSession?.Id ?? 1;
        public static string Current => UserSession?.Current ?? "System";
        public static string Name { get { return UserSession.Name; } }
        public static string Email { get { return UserSession.Email; } }
        public static string RoleStr { get { return UserSession.RoleStr; } }
        public static IEnumerable<string> Roles { get { return UserSession.Roles; } }
        public static string Token { get { return UserSession.Token; } }
        public static string FirstName { get { return UserSession.FirstName; } }
        public static string LastName { get { return UserSession.LastName; } }
        public static long CompanyId { get { return UserSession.CompanyId; } }
        public static string CardNo { get { return UserSession.CardNo; } }
        public static string Company { get { return UserSession.Company; } }
        public static string Pdc { get { return UserSession.Pdc; } }
        public static long PdcId { get { return UserSession.PdcId; } }

        // =======================================================
        // 🔽 Tambahkan helper di bawah ini
        // =======================================================

        /// <summary>
        /// Mengecek apakah user memiliki role yang mengandung keyword tertentu (case-insensitive).
        /// Misalnya: HasRoleLike("ADMIN") true jika Roles berisi "QCARD-ADMIN".
        /// </summary>
        public static bool HasRoleLike(string keyword)
        {
            if (UserSession?.Roles == null || string.IsNullOrWhiteSpace(keyword))
                return false;

            return UserSession.Roles.Any(r =>
                r != null && r.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Mengecek apakah user memiliki role yang persis sama (case-insensitive).
        /// Misalnya: HasRoleExact("ADMIN") hanya true jika salah satu Roles adalah "ADMIN".
        /// </summary>
        public static bool HasRoleExact(string role)
        {
            if (UserSession?.Roles == null || string.IsNullOrWhiteSpace(role))
                return false;

            return UserSession.Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
        }
    }
}
