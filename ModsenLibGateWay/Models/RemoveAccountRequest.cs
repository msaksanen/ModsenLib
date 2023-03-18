namespace ModsenLibGateWay.Models
{
    /// <summary>
    /// RemoveAccountRequest
    /// </summary>
    public class RemoveAccountRequest
    {
        ///
        public string? Pwd { get; set; }
        ///
        public string? Keyword { get; set; }
        ///
        public string? SystemInfo { get; set; } =String.Empty;
    }
}
