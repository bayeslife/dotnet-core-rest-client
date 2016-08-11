using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
          Program p = new Program();
          {
              p.postRequest().Wait();
          }
          {
            Task<Product> task = p.getResult();
            Product prd = task.GetAwaiter().GetResult() as Product;
            Console.WriteLine("Name:{0},Price:{1}",prd.Name,prd.Price);    
          }   
        }
        
         public async Task<Boolean> postRequest(){
          string username = "user";
          string password = "password";
         
          Uri completeURI = new Uri("http://requestb.in/18phiyl1");
          
          var bytes = Encoding.ASCII.GetBytes(username + ":" + password);
          var token = Convert.ToBase64String(bytes);
          AuthenticationHeaderValue auth = new AuthenticationHeaderValue("Basic", token);

          HttpClient client = new HttpClient();
          client.DefaultRequestHeaders.Authorization = auth;          
          
          Product p = new Product();
          p.Name = "test";
          p.Price = 123;
              
          MemoryStream stream1 = new MemoryStream();
          DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Product));
          ser.WriteObject(stream1, p);
          stream1.Position = 0;
          StreamReader sr = new StreamReader(stream1);
          String body = sr.ReadToEnd(); 
                        
          HttpContent content = new StringContent(body);          
                
          Console.WriteLine(completeURI);
          Console.WriteLine(body);
          HttpResponseMessage response = await client.PostAsync(completeURI,content);

            if (response.IsSuccessStatusCode)
            {             
                Console.WriteLine("Posted");
                return true;                                        
            }else {
                Console.WriteLine("Error"+response.StatusCode);
                return true;
            }            
          
        }
               
        public async Task<Product> getResult(){
          string username = "user";
          string password = "password";

          Uri completeURI = new Uri("http://localhost:80/api-x/test.json?foo=bar");
                   

          var bytes = Encoding.ASCII.GetBytes(username + ":" + password);
          var token = Convert.ToBase64String(bytes);
          AuthenticationHeaderValue auth = new AuthenticationHeaderValue("Basic", token);

          HttpClient client = new HttpClient();
          client.DefaultRequestHeaders.Authorization = auth;
                
          Console.WriteLine(completeURI);
          HttpResponseMessage response = await client.GetAsync(completeURI);

         if (response.IsSuccessStatusCode)
         {              
            var stream = await response.Content.ReadAsStreamAsync();                 
            
            var serializer = new DataContractJsonSerializer(typeof(Product));
            return serializer.ReadObject(stream) as Product;
                       
         }else {
             Console.WriteLine("Error"+response.StatusCode);
         }
         return null;          
        }
    }

}
