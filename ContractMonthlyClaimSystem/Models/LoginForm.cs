namespace ContractMonthlyClaimSystem.Models
{
    public class LoginForm
    {
            public string Email { get; set; } = string.Empty;

            public required string Password { get; set; }
            public string Username { get; set; } = string.Empty;

            public string Role { get; set; } = string.Empty;


            public string ErrorMessage { get; set; } = string.Empty;

            public bool RememberMe { get; set; }

            public string ReturnUrl { get; set; } = string.Empty;


        }

    }

