using System.ComponentModel.DataAnnotations;

namespace AcademyIO.Auth.API.Models
{
    /// <summary>
    /// Container for user-related view models.
    /// </summary>
    public class UserViewModel
    {
        /// <summary>
        /// View model used when registering a new user.
        /// </summary>
        public class RegisterUserViewModel
        {
            /// <summary>
            /// User's email address.
            /// </summary>
            [Required(ErrorMessage = "O campo {0} é obrigatório")]
            [EmailAddress(ErrorMessage = "O campo {0} está em formato inválido")]
            public string Email { get; set; }

            /// <summary>
            /// User's first name.
            /// </summary>
            [Required(ErrorMessage = "O campo {0} é obrigatório")]
            [StringLength(100, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 2)]
            public string FirstName { get; set; }

            /// <summary>
            /// User's last name.
            /// </summary>
            [Required(ErrorMessage = "O campo {0} é obrigatório")]
            [StringLength(100, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 2)]
            public string LastName { get; set; }

            /// <summary>
            /// User's date of birth.
            /// </summary>
            [Required(ErrorMessage = "O campo {0} é obrigatório")]
            [DataType(DataType.Date)]
            public DateTime DateOfBirth { get; set; }

            /// <summary>
            /// Indicates whether the user has admin privileges.
            /// </summary>
            public bool IsAdmin { get; set; }

            /// <summary>
            /// User's password.
            /// </summary>
            [Required(ErrorMessage = "O campo {0} é obrigatório")]
            [StringLength(100, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 6)]
            public string Password { get; set; }

            /// <summary>
            /// Confirmation of the user's password.
            /// </summary>
            [Compare("Password", ErrorMessage = "As senhas não conferem.")]
            public string ConfirmPassword { get; set; }
        }

        /// <summary>
        /// View model used for user login.
        /// </summary>
        public class LoginUserViewModel
        {
            /// <summary>
            /// User's email address for login.
            /// </summary>
            [Required(ErrorMessage = "O campo {0} é obrigatório")]
            [EmailAddress(ErrorMessage = "O campo {0} está em formato inválido")]
            public string Email { get; set; }

            /// <summary>
            /// User's password for login.
            /// </summary>
            [Required(ErrorMessage = "O campo {0} é obrigatório")]
            [StringLength(100, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 6)]
            public string Password { get; set; }
        }

        /// <summary>
        /// View model representing a tokenized user returned after authentication.
        /// </summary>
        public class UserTokenViewModel
        {
            /// <summary>
            /// User identifier.
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// User email.
            /// </summary>
            public string Email { get; set; }

            /// <summary>
            /// Claims associated with the user.
            /// </summary>
            public IEnumerable<ClaimViewModel> Claims { get; set; }
        }

        /// <summary>
        /// Response returned on successful login containing token information.
        /// </summary>
        public class LoginResponseViewModel
        {
            /// <summary>
            /// JWT or access token string.
            /// </summary>
            public string AccessToken { get; set; }

            /// <summary>
            /// Token expiration time in seconds.
            /// </summary>
            public double ExpiresIn { get; set; }

            /// <summary>
            /// Tokenized user information.
            /// </summary>
            public UserTokenViewModel UserToken { get; set; }
        }

        /// <summary>
        /// Test wrapper for login response with success flag.
        /// </summary>
        public class LoginResponseTestViewModel
        {
            /// <summary>
            /// Indicates if the operation was successful.
            /// </summary>
            public bool Success { get; set; }

            /// <summary>
            /// Payload data containing the login response.
            /// </summary>
            public LoginResponseViewModel Data { get; set; } = new();
        }

        /// <summary>
        /// Simplified representation of a claim.
        /// </summary>
        public class ClaimViewModel
        {
            /// <summary>
            /// Claim value.
            /// </summary>
            public string Value { get; set; }

            /// <summary>
            /// Claim type.
            /// </summary>
            public string Type { get; set; }
        }
    }
}