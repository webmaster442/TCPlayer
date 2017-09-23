using System.ComponentModel.DataAnnotations;

namespace TCPlayer.MediaLibary.DB
{
    public enum QueryOperator : int
    {
        NotSet = -1,
        [Display(Name = "=")]
        Equals,
        [Display(Name = "<")]
        Less,
        [Display(Name = "<=")]
        LessOrEqual,
        [Display(Name = ">")]
        Greater,
        [Display(Name = ">=")]
        GreaterOrEqual
    }
}
