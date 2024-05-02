using System.ComponentModel.DataAnnotations;

namespace GymInfrastructure.ViewModel
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "ПІБ")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Зал")]
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Range(1, int.MaxValue, ErrorMessage = "Оберіть зал")]
        public int GymId { get; set; }
        [Display(Name = "Телефон")]
        [Range(1, int.MaxValue, ErrorMessage = "Невірний номер")]
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
     
        public string Phone { get; set; } = null!;

        [Required]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Паролі не співпадають")]
        [Display(Name = "Підтвердження паролю")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
    }
}
