using System;
using System.Collections.Generic;

namespace Uset_zayavok;

public partial class Comment
{
    public int Commentid { get; set; }

    public string? Message { get; set; }

    public int? Masterid { get; set; }

    public int? Requestid { get; set; }

    public virtual User? Master { get; set; }

    public virtual Request? Request { get; set; }
}
