using System.ComponentModel.DataAnnotations;

namespace ModsenLibAPI.Models
{
    /// <summary>
    /// AddBookRequestModel
    /// </summary>
    public class AddBookRequestModel
    {
        ///
        [MaxLength(50)]
        public string? Title { get; set; }
        /// 
        [MaxLength(25)]
        public string? Author { get; set; }
        /// 
        [MaxLength(256)]
        public string? Description { get; set; }
        /// 
        [MaxLength(25)]
        public string? Genre { get; set; }
        /// 
        [RegularExpression(@"\d{1,3}-\d{1,5}-\d{1,8}-\d{1,6}-[0-9]|\d{1,5}-\d{1,8}-\d{1,6}-[0-9]")]  
        public string? ISBN { get; set; }
    }
}
