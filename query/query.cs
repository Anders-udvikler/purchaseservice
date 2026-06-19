using HotChocolate;
using Furnitures;
using Purchase.Models;
using service;
using models;
using Stripe;
using service.Grapql;

namespace query
{
    public class Query
    {


        public async Task<Order> GetOrderById(
            [Service] OrderService repo,string id
        )
        {
            var order = await repo.GetOrderById(id);
            return order;
        }

        public async Task<List<EventEnvelope<Order>>> GetEnvelopes(
            [Service] EventEnvelopeService<Order> repo,string id
        )
        {
            var order = await repo.GetAllEnvelopes();
            return order;
        }

        public async Task<EventEnvelope<Order>> GetEventById(
            [Service] EventEnvelopeService<Order> repo,string id
        )
        {
            var envelope = await repo.GetEventById(id);
            return envelope;
        }

        
    }
}