
namespace Multifolio.Shared
{
    public class AccountToken
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AccountId { get; set; }
        public string ContractKey { get; set; }
        //public string ContractType { get; set; }
        public string Icon { get; set; }
        public string Symbol { get; set; }
        public int Decimals { get; set; }
    }
}
