using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoworkingAPIWebApp.Models
{
    public class Place
    {
        public int Id { get; set; }
        public int ZoneId { get; set; }

        [Required(ErrorMessage = "Поле не повинно бути порожнім!")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Артикул повинен бути від 2 до 50 символів.")]
        [Display(Name = "Артикул місця")]
        public string Article { get; set; } = string.Empty;

        [Required(ErrorMessage = "Поле не повинно бути порожнім!")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Назва обладнання має містити від 2 до 50 символів.")]
        [Display(Name = "Назва місця")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вкажіть ціну за годину")]
        [Range(0.01, 10000, ErrorMessage = "Ціна повинна бути в межах від 0.01 до 10 000.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Ціна за годину")]
        public decimal PricePerHour { get; set; }

        [Required(ErrorMessage = "Поле не повинно бути порожнім!")]
        [Range(1, 100, ErrorMessage = "Місткість повинна бути в межах від 1 до 100!")]
        [Display(Name = "Місткість")]
        public int Capacity { get; set; }

        [StringLength(1000, ErrorMessage = "Опис не має перевищувати 1000 символів.")]
        [Display(Name = "Опис місця")]
        public string Description { get; set; } = string.Empty;

        public virtual ZoneCategory Zone { get; set; } = default!;

        [Display(Name = "Доступне для використання")]
        public bool IsAvailable { get; set; } = true;

        [JsonIgnore]
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
