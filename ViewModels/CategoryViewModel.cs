using ProjectCMS.Models;
﻿using System.ComponentModel.DataAnnotations;

namespace ProjectCMS.ViewModels
{
    public class CategoryViewModel
    {

        [Required]
        [MinLength(3, ErrorMessage = "Name length must be more than 3 characters"), MaxLength(20, ErrorMessage = "Name length must be less than 20 characters ")]
        public string Name { get; set; }

        [Required]
        [MinLength(15, ErrorMessage = "Content length must be more than 15 characters"), MaxLength(50, ErrorMessage = "Name length must be less than 50 characters ")]
        public string Content { get; set; }

      //  public DateTime AddedDate { get; set; } = DateTime.Now;
     
    }
}
