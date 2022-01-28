
using System.ComponentModel.DataAnnotations;

namespace Multifolio.Shared
{
    public class Account
    {
        public int Id { get; set; }
        //[Required(ErrorMessage = "Account name is required")]
        public string Name { get; set; }
        //[Required(ErrorMessage = "Account parent is required")]
        public int ChainId { get; set; }
        //[Required(ErrorMessage = "Account key is required")]
        public string PublicKey { get; set; }
    }
}
