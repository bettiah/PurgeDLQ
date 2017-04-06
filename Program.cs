using Microsoft.ServiceBus.Messaging;
using System;
using System.Threading;

namespace PurgeDLQ
{
    class Program
    {
        static void Main(string[] args)
        {
            int numberOfThreads = 10;
            for (int i = 0; i < numberOfThreads; i++)
            {
                MyThread thr = new MyThread();
                Thread tid = new Thread(new ThreadStart(thr.NewThread));
                tid.Name = $"t{i}";
                tid.Start();
            }
        }

        public class MyThread
        {
            public void NewThread()
            {
                string connectionString = "";
                string topicPath = "";
                string subscriptionName = "";

                MessagingFactory factory = MessagingFactory.CreateFromConnectionString(connectionString);
                var deadLetterPath = SubscriptionClient.FormatDeadLetterPath(topicPath, subscriptionName);
                var dlqMessageReceiver = factory.CreateMessageReceiver(deadLetterPath, ReceiveMode.ReceiveAndDelete);

                var thr = Thread.CurrentThread.Name;
                int messageBatchSize = 100;

                while (dlqMessageReceiver.ReceiveBatch(messageBatchSize) != null)
                {
                    Console.Write($"{thr}#");
                }                               
            }
        }
    }
}
