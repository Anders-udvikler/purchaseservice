using Purchase.Enums;
using Purchase.Models;

namespace DTO
{
    public class OrderCreated
    {
        public string OrderId { get; set; }

        public string UserId { get; set; }

        public List<string> SalesPostGuid { get; set; }

        public OrderStatus status { get; set; }

        public OrderCreated(string User,List<string> Post)
        {
            OrderId= Guid.NewGuid().ToString();
            UserId=User;
            SalesPostGuid=Post;
            status= OrderStatus.Pending;
            
        }
    }
}