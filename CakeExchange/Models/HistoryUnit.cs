using System;

namespace CakeExchange.Models
{
    public class HistoryUnit
    {
        public int Id { get; set; }
        public DateTime LeadTime { get; set; }
        public DateTime PurchaseTime { get; set; }
        public DateTime SaleTime { get; set; }
        public float Price { get; set; }
        public int Count { get; set; }
        public string PurchaserEmail { get; set; }
        public string SellerEmail { get; set; }

        public override string ToString()
        {
            return $"Id: {Id} LeadTime: {LeadTime} PurchaseTime: {PurchaseTime} SaleTime: {SaleTime}" +
                   $"Price: {Price} Count: {Count} PurchaserEmail: {PurchaserEmail} SellerEmail: {SellerEmail}";
        }
    }
}