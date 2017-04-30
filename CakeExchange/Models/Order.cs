using System;
using System.ComponentModel.DataAnnotations;

namespace CakeExchange.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "Значение не может быть отрицательным")]
        [Required(ErrorMessage = "Введите цену")]
        public float Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Количество не может быть меньше 1")]
        [Required(ErrorMessage = "Введите количество")]
        public int Count { get; set; }

        [StringLength(40, ErrorMessage = "Длина не должна превышать 40 символов")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Введите email")]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage = "Введите корректный email")]
        public string Email { get; set; }
        public OrderType Type { get; set; }
        public DateTime AdditionTime { get; set; }


        public Order()
        {
        }

        public Order(int id, float price, int count, string email, OrderType type, DateTime additionalTime)
        {
            Id = id;
            Price = price;
            Count = count;
            Email = email;
            Type = type;
            AdditionTime = additionalTime;
        }

        public override string ToString()
        {
            return $"Price: {Price}, Count: {Count}, Email: {Email}";
        }
    }

    public enum OrderType
    {
        Buy,
        Sell
    }
}