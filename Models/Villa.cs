using System.ComponentModel.DataAnnotations;

namespace RestFullApi.Models
{
    public class Villa
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        public DateTime CreateDate { get; set; }

       
    }
}
