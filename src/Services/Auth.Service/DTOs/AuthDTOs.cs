using System.ComponentModel.DataAnnotations;

namespace Auth.Service.Dtos
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", 
            ErrorMessage = "Email must be in a valid format.")] 
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", 
            ErrorMessage = "Password must contain at least one uppercase letter, " +
            "one lowercase letter, and one digit.")]
        public string Password { get; set; }

        [Required]
        [StringLength(8)]
        [RegularExpression(@"^[a-zA-Z0-9]+$", 
            ErrorMessage = "Username can only contain alphanumeric characters.")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(20)]
        [RegularExpression(@"^[a-zA-Z]+$", 
            ErrorMessage = "Last name can only contain alphabetic characters.")]
        public string LastName { get; set; }

        [Required]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", 
            ErrorMessage = "Phone number must be in valid international format.")]
        public string PhoneNumber { get; set; }
    }


    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", 
            ErrorMessage = "Email must be in a valid format.")]
        public string Email { get; set; }


        [Required]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", 
            ErrorMessage = "Password must contain at least one uppercase letter, " +
            "one lowercase letter, and one digit.")]
        public string Password { get; set; }
    }

    public class AuthResponse
    {
        public string Token { get; set; }
        
        public string Userid { get; set; }

        public string Email { get; set; }   

        public string FirstName { get; set; }

        public string LastName { get; set; }    

        public DateTime Expires { get; set; }
    }

    public class UpdateProfileRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
    }
}