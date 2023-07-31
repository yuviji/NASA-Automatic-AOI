using System;
using LinqToDB.Mapping;

namespace UserManagement.Models
{
    [Table(Name = "users")]
    public record User
    {
        public User(Guid userId, string firstName, string lastName, string email, DateTime created)
        {
            UserID = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Created = created;
        }
        [PrimaryKey] [Column(Name = "id")] public Guid UserID { get; set; }
        [Column(Name = "first_name")] public string FirstName { get; set; }
        [Column(Name = "last_name")] public string LastName { get; set; }
        [Column(Name = "email")] public string Email { get; set; }
        [Column(Name = "created")] public DateTime Created { get; set; }
    }
}