using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GymDomain.Model;

public partial class Gym: Entity
{

    [Display(Name = "Розклад")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string Schedule { get; set; } = null!;
    [Display(Name = "Адреса")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string Adress { get; set; } = null!;

    [Display(Name = "Тренажери та оснащення")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string Equipment { get; set; } = null!;

    public virtual ICollection<GroupClass> GroupClasses { get; set; } = new List<GroupClass>();

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public virtual ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();

    public virtual ICollection<Member> Members { get; set; } = new List<Member>();
}
