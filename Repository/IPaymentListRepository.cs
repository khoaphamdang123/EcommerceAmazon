using System;
using Ecommerce_Product.Data;
using Ecommerce_Product.Models;
using Org.BouncyCastle.Crypto.Utilities;
namespace Ecommerce_Product.Repository;

public interface IPaymentRepository
{

  public Task<IEnumerable<Payment>> getAllPayment();

public Task<Payment> findPaymentById(int id);

public Task<Payment> findPaymentByName(string name);

  public Task<PageList<Payment>> pagingPayment(int page_size,int page);

 
  public Task<int> addPaymentMethod(Payment payment);
  public Task<int> deletePaymentMethod(int id);

  public Task<int> updatePaymentMethod(int id);
 

  public Task saveChanges();


}