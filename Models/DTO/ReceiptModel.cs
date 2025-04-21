using System;
using System.Collections.Generic;

namespace Ecommerce_Product.Models;

public partial class ReceiptModel
{
    public Order Order {get;set;}

    public string AccountName{get;set;}

    public string AccountNumber{get;set;}
    public string BankName{get;set;}
}
