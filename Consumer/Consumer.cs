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
            var Key = Console.ReadLine();
            var factory = new ConnectionFactory() { HostName = "localhost" };
            factory.DispatchConsumersAsync = true;
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "PatientenDaten",
                                        type: "topic");
                var queueName = channel.QueueDeclare().QueueName;

                channel.QueueBind(queue: queueName,
                                  exchange: "PatientenDaten",
                                  routingKey: Key);

                Console.WriteLine("Waiting for messages. To exit press CTRL+C"); 

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += async (ch, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("Recieved: " + message);
                    channel.BasicAck(ea.DeliveryTag, false);
                    await Task.Yield();
                };
            
                channel.BasicConsume(queue: queueName,
                                     autoAck: false,
                                     consumer: consumer);
                Console.ReadLine();
            }
        }
    } 
}