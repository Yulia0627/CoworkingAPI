using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace CoworkingAPIWebApp.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int PlaceId { get; set; }

        public int UserId { get; set; }
        public int StatusId { get; set; }

        [Required(ErrorMessage = "Поле не повинно бути порожнім!")]
        [DataType(DataType.DateTime, ErrorMessage = "Невірний формат дати та часу!")]
        [Display(Name = "Дата та час початку бронювання")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Поле не повинно бути порожнім!")]
        [DataType(DataType.DateTime, ErrorMessage = "Невірний формат дати та часу!")]
        [Display(Name = "Дата та час закінчення бронювання")]
        public DateTime EndTime { get; set; }

        [Required(ErrorMessage = "Поле не повинно бути порожнім.")]
        [Range(1, 200, ErrorMessage = "Кількість учасників повинна бути в межах від 1 до 200!")]
        [Display(Name = "Кількість учасників")]
        public int ParticipantsCount { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Загальна вартість")]
        public decimal TotalPrice { get; set; }

        [StringLength(500, ErrorMessage = "Поле має містити до 500 символів.")]
        [Display(Name = "Додаткові побажання")]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ValidateNever]
        public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();

        [ValidateNever]
        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
        public virtual Place? Place { get; set; } = default!;
        public virtual User? User { get; set; } = default!;
         public virtual BookingStatus? Status { get; set; } = default!;
    }
}
