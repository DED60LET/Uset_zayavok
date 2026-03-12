using System.ComponentModel.DataAnnotations;

namespace Uset_zayavok.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введите ФИО")]
        public string Fio { get; set; }

        [Required(ErrorMessage = "Введите телефон")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Придумайте логин")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}