using System; 
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;

namespace Consumer
{
    class Consumer
    {
        public static void Main(string[] args)
        {   
            Console.WriteLine("Arztname: ");
            string Key = Console.ReadLine();
            var factory = new ConnectionFactory(){ HostName = "localhost" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "Patientendaten",
                                        type: "topic");
                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName,
                                  exchange: "Patientendaten",
                                  routingKey: Key);
                int messageCount = Convert.ToInt32(channel.MessageCount(queueName)); 
                Console.WriteLine("Waiting for messages. To exit press CTRL+C"); 
                var consumer = new EventingBasicConsumer(channel);    
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("Recieved: " + message);
                    channel.BasicAck(ea.DeliveryTag, false);
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);  
            
                Console.ReadLine();
            }
        }
    } 
}