﻿using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.DTOs
{
    public class RegisterDTO
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
    }
}
