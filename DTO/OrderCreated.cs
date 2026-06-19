using Purchase.Enums;
using Purchase.Models;

namespace DTO
{
    public class OrderCreated
    {
        public string OrderId { get; set; } = Guid.NewGuid().ToString();

        public string UserId { get; set; }

        public List<string> SalesPostGuid { get; set; }

        public string Email { get; set; }

        public OrderCreated(string User,List<string> Post,string email)
        {
            OrderId= Guid.NewGuid().ToString();
            UserId=User;
            SalesPostGuid=Post;
            Email=email;
            
            
        }
    }
}