using System;
using Ecommerce_Product.Data;
using Ecommerce_Product.Models;
using Org.BouncyCastle.Crypto.Utilities;
namespace Ecommerce_Product.Repository;

public interface ICartRepository
{
 public Task<Cart> getUserCart(string user_id);

 public List<CartModel> getCart();

 public Task<int> addProductToCart(CartModel model);

 public Task<int> deleteProductFromCart(int product_id);

 public Task clearCart();

 public Task<int> updateCart(int product_id,int quantity); 

 public Task saveChanges(); 

}