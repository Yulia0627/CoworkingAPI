using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoworkingAPIWebApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле не повинно бути порожнім!")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Поле має містити від 2 до 50 символів.")]
        [Display(Name = "Прізвище та ім'я")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Поле не повинно бути порожнім!")]
        [EmailAddress(ErrorMessage = "Невірний формат пошти!")]
        [Display(Name = "Електронна пошта")]
        public string Email { get; set; } = string.Empty;
        public bool isAdmin { get; set; }

        [Display(Name = "Дата реєстрації в системі")]
        public DateTime RegisteredAt { get; set; } = DateTime.Now;

        [JsonIgnore]
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
