
using System.ComponentModel.DataAnnotations;
using TaApostado.Utils;

namespace TaApostado.DTOs.InputModels
{
    public class InputModelUser
    {

        [Required]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "The field name is invalid.")]
        [StringLengthAttribute(16, MinimumLength = 3, ErrorMessage = "The field name is invalid.")]
        public string name { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "The field last name is invalid.")]
        [StringLengthAttribute(20, MinimumLength = 3, ErrorMessage = "The field last name is invalid.")]
        public string lastName { get; set; }

        [Required]
        [CPFUtils(ErrorMessage = "The field cpf is invalid.")]
        public string cpf { get; set; }

        [Required]
        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", ErrorMessage = "The field email is invalid.")]
        public string email { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$", ErrorMessage = "The field password is invalid.")]
        public string password { get; set; }
    }
}
