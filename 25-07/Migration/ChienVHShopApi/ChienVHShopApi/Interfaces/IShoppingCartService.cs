using ChienVHShopAPI.DTOs;

using PayPal.v1.Payments;

namespace ChienVHShopAPI.Interfaces
{
    public interface IShoppingCartService
    {
        Task<int> PlaceOrderAsync(CheckoutRequestDto request);
        Payment CreatePaypalPayment(List<CartItemDto> cartItems, string redirectUrl);
        Payment ExecutePaypalPayment(string paymentId, string payerId);
    }
}
