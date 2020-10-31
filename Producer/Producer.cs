using System;
using RabbitMQ.Client;
using System.Text;
using System.Threading;

namespace Producer
{
    class Producer
    {
        public static void Main(string[] args)
        {

            var factory = new ConnectionFactory()   {  HostName = "localhost" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {   
                Console.WriteLine("behandelnder Arzt: ");
                string Key = Console.ReadLine();  
                Console.WriteLine("Patientenname: ");
                string Patientenname = Console.ReadLine();
                channel.ExchangeDeclare(exchange: "Patientendaten",
                                        type: "topic");            
                Gerät gerät = new Gerät(Patientenname);
               
                while (1 ==1){
                    var ran = new Random();
                    gerät.Simulation();
                    string message = "";
                    if (gerät.Herzfrequenz >= 220 || gerät.Herzfrequenz <= 20 || gerät.Blutdruck[0] >= 180 || gerät.Blutdruck[1] >= 110) 
                    {
                        if (gerät.Herzfrequenz >= 220 || gerät.Herzfrequenz <= 20)
                        { 
                            message += "der Puls von " + gerät.Patientname + " liegt bei " + gerät.Herzfrequenz;
                            var body = Encoding.UTF8.GetBytes(message);
                            channel.BasicPublish(exchange: "Patientendaten",
                                                 routingKey: Key,
                                                 basicProperties: null,
                                                 body: body);
                            Console.WriteLine("Sent: " + message);
                        }                         
                        if (gerät.Blutdruck[0] >= 180 || gerät.Blutdruck[1] >= 110)
                        {
                            message += "der BLutdruck von " + gerät.Patientname + " liegt bei " + gerät.Blutdruck[0] + " zu " + gerät.Blutdruck[1]; 
                            var body = Encoding.UTF8.GetBytes(message);
                            channel.BasicPublish(exchange: "Patientendaten",
                                                routingKey: Key,
                                                basicProperties: null,
                                                body: body);
                             Console.WriteLine("Sent: " + message);
                        }     
                        Thread.Sleep(1000);
                        // bei 0 war der Eingriff nicht erfolgreich bei 1 - 3 schon
                        int eingriff = ran.Next(0, 3);
                        if (eingriff == 0)
                        {
                            message = gerät.Patientname + " ist gestorben";
                            var body = Encoding.UTF8.GetBytes(message);
                            channel.BasicPublish(exchange: "Patientendaten",
                                                 routingKey: Key,
                                                 basicProperties: null,
                                                 body: body);
                            Console.WriteLine("Sent: " + message);
                            return;
                        }
                        else
                        {
                            message = gerät.Patientname + " wurde gerettet";
                            var body = Encoding.UTF8.GetBytes(message);
                            channel.BasicPublish(exchange: "Patientendaten",
                                                 routingKey: Key,
                                                 basicProperties: null,
                                                 body: body);
                            Console.WriteLine("Sent: " + message);
                            gerät = new Gerät(Patientenname);
                        }

                    }
                   // Thread.Sleep(100);
                }
            }
        }
    }
}