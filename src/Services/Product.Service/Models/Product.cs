using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Product.Service.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }


        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }  

        public int CategoryId { get; set; } 

        public Category Category { get; set; }

        [MaxLength(500)]
        public string ImageUrl { get; set; }   
        
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdateAt { get; set; } = DateTime.UtcNow;   
    }


    public class Category
    {
        public int Id { get; set; }


        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;  

        public ICollection<Product> Products { get; set; }
    }
}
