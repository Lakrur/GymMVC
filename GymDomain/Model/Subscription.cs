using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GymDomain.Model;

public partial class Subscription : Entity
{

    [Display(Name = "Спортзал")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public int GymId { get; set; }
    [Display(Name = "Назва")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string Type { get; set; } = null!;
    [Display(Name = "Опис")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string Description { get; set; } = null!;
    [Display(Name = "Ціна,$")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Range(0, int.MaxValue, ErrorMessage = "Не може бути від'ємним")]
    public int Price { get; set; }
    [Display(Name = "Тривалість(місяців) ")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Range(0, int.MaxValue, ErrorMessage = "Не може бути від'ємним")]
    public int Duration { get; set; }
    [Display(Name = "Спортзал")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public virtual Gym Gym { get; set; } = null!;

    public virtual ICollection<Member> Members { get; set; } = new List<Member>();
}
