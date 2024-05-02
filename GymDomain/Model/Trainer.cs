using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GymDomain.Model;

public partial class Trainer : Entity
{
    [Display(Name = "Зал")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public int GymId { get; set; }



    [Display(Name = "Повне ім'я")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string TrainerName { get; set; } = null!;
    [Display(Name = "Телефон")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Range(0, int.MaxValue, ErrorMessage = "Не може бути від'ємним")]
    public string Phone { get; set; } = null!;
    [Display(Name = "Пошта")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string Email { get; set; } = null!;
    [Display(Name = "Зал")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public virtual Gym Gym { get; set; } = null!;



    [Display(Name = "Групові класи")]
    public virtual ICollection<GroupClass> GroupClasses { get; set; } = new List<GroupClass>();
}
