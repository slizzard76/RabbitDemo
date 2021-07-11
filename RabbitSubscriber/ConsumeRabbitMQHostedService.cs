using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Shared;
using Serilog;

namespace RabbitSubscriber
{
    public class ConsumeRabbitMQHostedService : BackgroundService
    {
        IRabbitManipulation _rabbitManipulation;
        public ConsumeRabbitMQHostedService(IRabbitManipulation rabbitManipulation)
        {
            _rabbitManipulation = rabbitManipulation;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //_rabbitManipulation.InitRabbitMQ(true);
            var model = _rabbitManipulation.GetModel();
            stoppingToken.ThrowIfCancellationRequested();
           
            var consumer = new EventingBasicConsumer(model);
            consumer.Received += (ch, ea) =>
            {
                var content = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());
                    //_logger.LogInformation("======Recieved request=============");
                Task.Run(() => DoSomething(content));
                model.BasicAck(ea.DeliveryTag, false);
            };
            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;
            model.BasicConsume(_rabbitManipulation.GetQueue(), false, consumer);
            
            return Task.CompletedTask;
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }
        public override void Dispose()
        {

            _rabbitManipulation.GetModel().Close();
            if (_rabbitManipulation.Connection != null)
                _rabbitManipulation.Connection.Close();
            base.Dispose();
        }


        private async Task DoSomething(string message)
        {
            Log.Logger.Information($"Recieved message from RabbitMQ {message}");
        }
    }
}
