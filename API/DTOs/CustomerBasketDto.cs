

namespace API.DTOs
{
    public class CustomerBasketDto
    {
        public string Id {get;set;}
        public List<BasketItemDto> Items {get;set;} = new();
    }
}