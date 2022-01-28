using System.ComponentModel.DataAnnotations;

namespace Multifolio.Shared
{
    public enum DateRange
    {
        [Display(Name = "H", ResourceType = typeof(ChainType))]
        Hour = 0,
        [Display(Name = "D", ResourceType = typeof(ChainType))]
        Day = 1,
        [Display(Name = "W", ResourceType = typeof(ChainType))]
        Week = 2,
        [Display(Name = "M", ResourceType = typeof(ChainType))]
        Month = 3,
        [Display(Name = "Y", ResourceType = typeof(ChainType))]
        Year = 4,
        [Display(Name = "A", ResourceType = typeof(ChainType))]
        All = 5
    }
}
