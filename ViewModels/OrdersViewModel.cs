using System.Globalization;
using StanaGO.Enums;

namespace StanaGO.ViewModels
{
    public class OrdersViewModel
    {
        public int Id { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public ProductStatus Status { get; set; } = ProductStatus.Queued;

        public bool CustomerConfirmed { get; set; }

        public bool SellerConfirmed { get; set; }

        public bool CanSellerConfirm =>
            !SellerConfirmed && CustomerConfirmed;

        public bool IsCompleted =>
            SellerConfirmed && CustomerConfirmed;

    }
}
