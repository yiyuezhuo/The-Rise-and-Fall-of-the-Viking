using System.Collections.Generic;
using System;


namespace GameCore
{

    public interface ILoggerService
    {
        void Log(string message);
        void LogWarning(string message);
    }

    public class FallbackLogger : ILoggerService
    {
        public void Log(string message)
        {
            System.Console.WriteLine(message);
        }

        public void LogWarning(string message)
        {
            System.Console.WriteLine("[Warn]:" + message);
        }
    }

    public interface IUserMessageService
    {
        void ShowMessage(string message, string title="Message");
    }

    public class FallbackUserMessageService : IUserMessageService
    {
        public void ShowMessage(string message, string title = "Message")
        {
            System.Console.WriteLine($"title: " + message);
        }
    }

    public static class ServiceLocator
    {
        static Dictionary<Type, object> services = new()
        {
            {typeof(ILoggerService), new FallbackLogger()},
            {typeof(IUserMessageService), new FallbackUserMessageService()}
        };

        public static T Get<T>() where T : class
        {
            var type = typeof(T);
            if (services.ContainsKey(type))
            {
                return (T)services[type];
            }
            return null;
        }

        public static void Register<T>(T service) where T : class
        {
            var type = typeof(T);
            var currentValue = Get<T>();
            if (currentValue != null)
            {
                var logger = Get<ILoggerService>();
                logger.Log($"Overriding service: {currentValue} -> {service}");
            }
            services[type] = service;
        }
    }

}