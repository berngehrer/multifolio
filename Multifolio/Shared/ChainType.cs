using System.ComponentModel.DataAnnotations;

namespace Multifolio.Shared
{
    public enum ChainType
    {
        [Display(Name = "Bitcoin", ResourceType = typeof(ChainType))]
        Bitcoin = 1,
        [Display(Name = "Ethereum", ResourceType = typeof(ChainType))]
        Ethereum = 2,
        [Display(Name = "Solana", ResourceType = typeof(ChainType))]
        Solana = 3,
        [Display(Name = "Avalanche", ResourceType = typeof(ChainType))]
        Avalanche = 4,
        [Display(Name = "Stellar", ResourceType = typeof(ChainType))]
        Stellar = 5,
        [Display(Name = "Algorand", ResourceType = typeof(ChainType))]
        Algorand = 6,
        [Display(Name = "Coinbase", ResourceType = typeof(ChainType))]
        Coinbase = 7,
        [Display(Name = "XRP", ResourceType = typeof(ChainType))]
        XRP = 8,
        [Display(Name = "Cardano", ResourceType = typeof(ChainType))]
        Cardano = 9
    }
}
