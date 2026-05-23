using System.ComponentModel.DataAnnotations;

namespace EmployeeMVC.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int Age { get; set; }
        public decimal Salary { get; set; }
        public string Designation { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public string ImageUrl { get; set; }
    }
}