using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;

namespace BC.PAYMENT.CORE
{
    public sealed class Singleton
    {
        private static readonly Lazy<Singleton> _instance = new Lazy<Singleton>(() => new Singleton());

        public static Singleton Instance => _instance.Value;
        private readonly IConfiguration _configuration;
        private Singleton()
        {
            // Initialize here
        }
        // Add properties and methods to store super-admin data
        public string Role { get; set; } 
        public string Username { get; set; }
        public string UserPassword { get; set; }    
        public string AppCode { get; set; }    
    }
}
