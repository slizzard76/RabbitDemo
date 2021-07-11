using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Serilog;

namespace Shared
{
    public interface IRabbitManipulation
    {
        public void InitRabbitMQ(bool isListener);
        public Task SendMessage(string message);
        public IConnection Connection { get; }

        public IModel GetModel();
        public string GetQueue();
    }

    public class RabbitManipulation : IRabbitManipulation
    {
        public IConnection Connection { get { return _connection; } }

        private IConnection _connection;
        private IRabbitConfig _rabbitConfiguration;
        private IModel _model;
        public RabbitManipulation(IRabbitConfig rabbitConfiguration, bool isListener)
        {
            _rabbitConfiguration = rabbitConfiguration;
            InitRabbitMQ(isListener);
        }

        public void InitRabbitMQ(bool isListener)
        {
            int port = 0;
            int.TryParse(_rabbitConfiguration.Port, out port);
            var factory = new ConnectionFactory
            {
                HostName = _rabbitConfiguration.HostName,
                Port = port,
                UserName = _rabbitConfiguration.UserName,
                Password = _rabbitConfiguration.Password
            };
            try
            {
                _connection = factory.CreateConnection();
                Log.Logger.Information("RabbitMq Connected.");
            }
            catch (Exception e)
            {
                Log.Logger.Information(e.Message);
                Log.Logger.Information("==============Restart required=============");
                return;
            }

            _model = _connection.CreateModel();
           if (isListener)
           {
                _model.ExchangeDeclare(_rabbitConfiguration.ExchangeName, ExchangeType.Direct, true);
                _model.QueueDeclare(_rabbitConfiguration.QueueName, true, false, false, null);
                _model.QueueBind(_rabbitConfiguration.QueueName, _rabbitConfiguration.ExchangeName, $"{_rabbitConfiguration.QueueName}.*", null);
                _model.BasicQos(0, 1, false);
           }
        }

        public Task SendMessage(string message)
        {
            try
            {
                var jmessage = JsonConvert.SerializeObject(message);
                var messageBuffer = Encoding.UTF8.GetBytes(jmessage);
                var property = _model.CreateBasicProperties();
                property.Persistent = false;
                _model.BasicPublish(_rabbitConfiguration.ExchangeName, $"{_rabbitConfiguration.QueueName}.*", true, property, messageBuffer);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e.Message);
                Log.Logger.Error("If NOT FOUND exception occured pls try start listener first");
            }
            return Task.CompletedTask;
        }

        public IModel GetModel()
        {
            return _model;
        }
        public string GetQueue()
        {
            return _rabbitConfiguration.QueueName;
        }
    }
}
