using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace aoiRouting.Shared.UserManagement
{
    public class RegisterModel
    {
        public bool _isHashed;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        [PasswordPropertyText] public string Password { get; set; }
        public bool ModelsIsNull()
        {
            return FirstName == null || LastName == null || Email == null || Password == null;
        }
    }
}