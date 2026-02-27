using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uset_zayavok.Models;

public partial class Request
{
    public int Requestid { get; set; }

    public DateOnly? Startdate { get; set; }

    public string? Hometechtype { get; set; }

    public string? Hometechmodel { get; set; }

    public string? Problemdescryption { get; set; }

    public string? Requeststatus { get; set; }

    public DateOnly? Completiondate { get; set; }

    public string? Repairparts { get; set; }

    public int? Masterid { get; set; }

    public int? Clientid { get; set; }

   
    public virtual User? Client { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

   
    public virtual User? Master { get; set; }
}
