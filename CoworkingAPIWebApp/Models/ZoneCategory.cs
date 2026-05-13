using System.ComponentModel.DataAnnotations;

namespace CoworkingAPIWebApp.Models
{
    public class ZoneCategory
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Поле не повинно бути порожнім!")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Назва повинна бути від 2 до 50 символів")]
        [Display(Name = "Назва зони")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Опис не має перевищувати 1000 символів.")]
        [Display(Name = "Опис зони")]
        public string Description { get; set; } = string.Empty;

        public virtual ICollection<Place> Places { get; set; } = new List<Place>();
    }
}
