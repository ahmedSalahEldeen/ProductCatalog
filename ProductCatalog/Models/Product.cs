using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductCatalog.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required, MaxLength(200)]
        public string Name { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public string CreatedByUserId { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        public int DurationInMinutes { get; set; }  //setting duration time in minutes 
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please upload an image.")]
        [NotMapped] // Prevent this from being mapped to the database
        public IFormFile ImageFile { get; set; }

        // This will store the file path in the database
        public string ImageFilePath { get; set; }
        //setting relations & navigation property
        public byte CategoryId { get; set; }
        public Category Category { get; set; }

    }
}
