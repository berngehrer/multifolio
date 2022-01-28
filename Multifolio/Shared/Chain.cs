
using System.ComponentModel.DataAnnotations;

namespace Multifolio.Shared
{
    public class Chain
    {
        public int Id { get; set; }
        //[Required(ErrorMessage = "Chain name is required")]
        public string Name { get; set; }
        //[Required(ErrorMessage = "Chain host is required")]
        public string Host { get; set; }
        public ChainType Type { get; set; }     
        public string Icon { get; set; }
        public string Symbol { get; set; }

        public bool AllowToken { get; set; }
    }
}
