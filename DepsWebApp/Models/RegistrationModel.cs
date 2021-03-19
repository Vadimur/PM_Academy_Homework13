using System.ComponentModel.DataAnnotations;

namespace DepsWebApp.Models
{
    /// <summary>
    /// Model for registration
    /// </summary>
    public class RegistrationModel
    {
        /// <summary>
        /// Registration login
        /// </summary>
        [Required]
        [MinLength(6, ErrorMessage = "The field {0} must be a string with a minimum length of '{1}'.")]
        public string Login { get; set; }

        /// <summary>
        /// Registration password
        /// </summary>
        [Required]
        [MinLength(6, ErrorMessage = "The field {0} must be a string with a minimum length of '{1}'.")]
        public string Password { get; set; }
    }
}