using SplitWise.Domain.Comman;

namespace SplitWise.Domain.Entities
{
    public class User: BaseEntity
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }   
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
    }
}
