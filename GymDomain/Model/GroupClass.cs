using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GymDomain.Model;

public partial class GroupClass : Entity
{
    [Display(Name = "Номер кімнати")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Range(0, int.MaxValue, ErrorMessage = "Не може бути від'ємним")]
    public int Room { get; set; }
    [Display(Name = "Розклад")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string Schedule { get; set; } = null!;

    [Display(Name = "Назва")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string Name { get; set; } = null!;
    [Display(Name = "Ціна за місяць,$")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Range(0, int.MaxValue, ErrorMessage = "Не може бути від'ємним")]
    public int Price { get; set; }
    [Display(Name = "Зал")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public int GymId { get; set; }
    [Display(Name = "Зал")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public virtual Gym Gym { get; set; } = null!;

    public virtual ICollection<Member> Members { get; set; } = new List<Member>();

    public virtual ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();
}
