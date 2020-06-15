namespace Telepuz.Models.Network
{
    public static class ApiOptions
    {
        public static readonly string IpAddress = "35.228.119.156";
        public static readonly string Port = "5000";

        public static string Url => $"ws://{IpAddress}:{Port}";
    }
}
