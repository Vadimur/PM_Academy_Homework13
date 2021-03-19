using System;

namespace DepsWebApp.Options
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class NbuClientOptions
    {
        public string BaseAddress { get; set; }

        public bool IsValid => !string.IsNullOrWhiteSpace(BaseAddress) &&
                               Uri.TryCreate(BaseAddress, UriKind.Absolute, out _);
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
