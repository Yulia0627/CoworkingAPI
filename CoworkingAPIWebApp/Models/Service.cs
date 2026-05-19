using System.ComponentModel.DataAnnotations;

namespace CoworkingAPIWebApp.Models
{
    public class Service
    {
       public  int Id { get; set; }

        [Required(ErrorMessage = "Поле не повинно бути порожнім!")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Назва послуги має містити від 2 до 50 символів.")]
        [Display(Name = "Назва послуги")]
        public string? Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Поле не може бути порожнім!")]
        [Range(0.01, 10000, ErrorMessage = "Ціна повинна бути в межах від 0.01 до 10 000.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Ціна")]
        public decimal? Price { get; set; }

        [Display(Name = "Доступність")]
        public bool? IsAvailable { get; set; } = true;
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    }
}
