namespace QCard.ManagementSystem.Infrastructure.Extension
{
    public class ContextInfo
    {
        public string _area;
        public string _controller;
        public string _action;

        public ContextInfo(string area, string controller, string action)
        {
            _area = area;
            _controller = controller;
            _action = action;
        }
        public string Area { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string PermissionKey { get; }
    }
}