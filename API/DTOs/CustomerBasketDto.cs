

namespace API.DTOs
{
    public class CustomerBasketDto
    {
        public string Id {get;set;}
        public List<BasketItemDto> BasketItems {get;set;} = new();
    }
}