using System.ComponentModel.DataAnnotations;

namespace Member.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter your username")]
        public string id { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        public string password { get; set; }
    }
}
