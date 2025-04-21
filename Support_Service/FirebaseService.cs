using FirebaseAdmin.Messaging;

namespace Ecommerce_Product.Support_Serive;

public class FirebaseService
{
    public FirebaseService()
    {
    }

public async Task<string> sendFirebaseMessage(string token,string title,string message)
{
    var messages=new Message()
    {
     Token=token,
     
     Notification=new Notification()
     {
        Title=title,
        Body=message
     }
    };

    Console.WriteLine("Token here is:"+token);    
    
    return await FirebaseMessaging.DefaultInstance.SendAsync(messages);
}

}