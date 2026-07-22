namespace QCard.ManagementSystem.Infrastructure.Session
{
    [Serializable]
    public class UserSession
    {
        public long Id { get; set; }
        public string Current { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string RoleStr { get; set; }
        public string[] Roles { get { return RoleStr == null ? null : RoleStr.Split(','); } }
        public string Token { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public long CompanyId { get; set; }
        public string CardNo { get; set; }
        public string Company { get; set; }
        public long PdcId{ get; set; }
        public string Pdc { get; set; }
    }
}
