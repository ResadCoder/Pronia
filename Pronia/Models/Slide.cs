using System.ComponentModel.DataAnnotations.Schema;

namespace Pronia.Models;



    public class Slide
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public string Subtitle { get; set; } = null!;

        public string ButtonText { get; set; } = null!;
        
        public int Order { get; set; } 
        
        public string ImagePath { get; set; } = null!;
        
        public bool ShowSlide { get; set; } = false;
        
        [NotMapped]
        public IFormFile Photo { get; set; } = null!;

    }
