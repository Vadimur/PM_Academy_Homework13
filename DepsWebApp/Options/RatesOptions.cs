namespace DepsWebApp.Options
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class RatesOptions
    {
        public string BaseCurrency { get; set; }
        public bool IsValid => !string.IsNullOrWhiteSpace(BaseCurrency);
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
