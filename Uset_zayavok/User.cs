using System;
using System.Collections.Generic;

namespace Uset_zayavok;

public partial class User
{
    public int Userid { get; set; }

    public string? Fio { get; set; }

    public string? Phone { get; set; }

    public string? Login { get; set; }

    public string? Password { get; set; }

    public string? Type { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Request> RequestClients { get; set; } = new List<Request>();

    public virtual ICollection<Request> RequestMasters { get; set; } = new List<Request>();
}
