using System.ComponentModel.DataAnnotations;
namespace bakery_web_page_CSI340.Models
{
    
    public class MenuItems
    {
        public string MenuItemId { get; set; }
        [Required]
        [StringLength(60, ErrorMessage = "Name should be no more than 60 characters.")]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Must be greater than 1 or 1")]
        public string Price { get; set; }
        [Required]
        public string NumberOfCalories { get; set; }
        [Required]
        public string IsVegan { get; set; }
        [Required]
        public string IsVegeterian { get; set; }


    }
}
