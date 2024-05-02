using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace GymDomain.Model;

public partial class Member : Entity
{

    [Display(Name = "Підписка")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public int? SubscriptionId { get; set; }
    [Display(Name = "Зал")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public int GymId { get; set; }
    [Display(Name = "Ім'я")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string Name { get; set; } = null!;
    [Display(Name = "Телефон")]
  
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string Phone { get; set; } = null!;
    [Display(Name = "Електронна пошта")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string Email { get; set; } = null!;
    [Display(Name = "Підписка")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public virtual Subscription Subscription { get; set; }
    [Display(Name = "Зал")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public virtual Gym Gym { get; set; } = null!;
    [Display(Name = "Групові класи")]


    public virtual ICollection<GroupClass> GroupClasses { get; set; } = new List<GroupClass>();
}
