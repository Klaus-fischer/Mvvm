namespace Mvvm.VisualTest
{
    using Mvvm.Core;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class SimpleErrorViewModel : ValidationViewModel
    {
        public SimpleErrorViewModel()
        {
            this.ValidateOnPropertyChanged = true;
        }

        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DisplayName("Passwort")]
        [StringLength(12, MinimumLength = 8, ErrorMessage = "{0} must have at least 8 and less than 12 character.")]
        public string Password { get; set; }

        protected override string ValidateProperty(string propertyName)
        {
            if (propertyName == nameof(Username))
            {
                if (Username?.Length > 5)
                {
                    return string.Empty;
                }
                else
                {
                    return "Username must have at least 5 character.";
                }
            }

            return base.ValidateProperty(propertyName);
        }
    }
}
