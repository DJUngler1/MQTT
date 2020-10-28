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

            var factory = new ConnectionFactory() { HostName = "localhost" };
            factory.DispatchConsumersAsync = true;
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "PatientenDaten",
                                        type: "topic");
                Console.WriteLine("Patientenname: ");
                string Patientenname = Console.ReadLine();
                Gerät gerät = new Gerät(Patientenname);
                Console.WriteLine("behandelnder Arzt: ");
                var Key = Console.ReadLine(); 
                while (1 ==1){
                    var ran = new Random();
                    gerät.Simulation();
                    string message = "";
                    if (gerät.Herzfrequenz >= 220 || gerät.Herzfrequenz <= 20 || gerät.Blutdruck[0] >= 160 || gerät.Blutdruck[1] >= 110) 
                    {
                        if (gerät.Herzfrequenz >= 220 || gerät.Herzfrequenz <= 20) 
                            message += "der Puls von " + gerät.Patientname + " liegt bei " + gerät.Herzfrequenz;
                        if (gerät.Blutdruck[0] >= 180 || gerät.Blutdruck[1] >= 110)
                            message += "der BLutdruck von " + gerät.Patientname + " liegt bei " + gerät.Blutdruck[0] + " zu " + gerät.Blutdruck[1]; 
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: "PatientenDaten",
                                             routingKey: Key,
                                             basicProperties: null,
                                             body: body);
                        Console.WriteLine("Sent: " + message);
                        Thread.Sleep(10000);
                        // bei 0 war der Eingriff nicht erfolgreich bei 1 - 3 schon
                        int eingriff = ran.Next(0, 3);
                        if (eingriff == 0)
                        {
                            message = gerät.Patientname + " ist gestorben";
                            body = Encoding.UTF8.GetBytes(message);
                            channel.BasicPublish(exchange: "PatientenDaten",
                                                 routingKey: Key,
                                                 basicProperties: null,
                                                 body: body);
                            Console.WriteLine("Sent: " + message);
                            return;
                        }
                        else
                        {
                            message = gerät.Patientname + " wurde gerettet";
                            body = Encoding.UTF8.GetBytes(message);
                            channel.BasicPublish(exchange: "PatientenDaten",
                                                 routingKey: Key,
                                                 basicProperties: null,
                                                 body: body);
                            Console.WriteLine("Sent: " + message);
                            gerät = new Gerät(Patientenname);
                        }
                    }
                    Thread.Sleep(100);
                }
            }
        }
    }
}