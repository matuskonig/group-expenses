using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class TodoItem
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }
    }
}