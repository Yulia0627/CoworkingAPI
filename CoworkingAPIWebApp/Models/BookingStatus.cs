using System.ComponentModel.DataAnnotations;

namespace CoworkingAPIWebApp.Models
{
    public class BookingStatus
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле не повинно бути порожнім!")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Назва повинна бути від 2 до 50 символів")]
        [Display(Name = "Назва статусу")]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
