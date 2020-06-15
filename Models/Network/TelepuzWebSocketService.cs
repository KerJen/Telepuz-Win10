using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using MetroLog;
using WebSocketSharp;

namespace Telepuz.Models.Network
{
    // TODO: Перевести на DI Stiletto Library вместо стандартного синглтона
    internal class TelepuzWebSocketService
    {
        private const int RECONNECT_TIMEOUT = 5000;

        // Lazy-инициализация синглтона веб-сокет сервиса
        static readonly Lazy<TelepuzWebSocketService> telepuzWebSocketService
            = new Lazy<TelepuzWebSocketService>(() => new TelepuzWebSocketService());

        readonly ILogger Log = LogManagerFactory.DefaultLogManager.GetLogger<TelepuzWebSocketService>();

        readonly WebSocket _client;

        public static TelepuzWebSocketService Client => telepuzWebSocketService.Value;

        readonly Dictionary<string, Callback> _listenersPool;

        private TelepuzWebSocketService()
        {
            _client = new WebSocket(ApiOptions.Url);

            // Инициализация пула калбеков
            _listenersPool = new Dictionary<string, Callback>();
        }

        /// <summary>
        /// Подключение к серверу
        /// </summary>
        public void Connect()
        {
            ListenResponses();

            _client.OnError += (sender, e) =>
            {
                Reconnect(RECONNECT_TIMEOUT);
            };

            _client.OnClose += (sender, e) =>
            {
                Reconnect(RECONNECT_TIMEOUT);
                
            };
            
            _client.Connect();
        }

        async void Reconnect(int timeout)
        {
            await Task.Delay(timeout);

            _client.Connect();
        }

        /// <summary>
        /// /Прослушивает ошибки при WebSocket соединении
        /// </summary>
        /// <param name="callback">Калбэк ошибки</param>
        public void OnServiceResponse(Action<WebSocketResults> callback)
        {
            _client.OnOpen += (sender, e) =>
            {
                callback(WebSocketResults.CONNECTION_SUCCESS);
            };

            _client.OnClose += (sender, e) =>
            {
                callback(WebSocketResults.CONNECTION_CLOSED);
            };

            _client.OnError += (sender, e) =>
            {
                callback(WebSocketResults.CONNECTION_FAILED);
            };
        }


        /// <summary>
        /// Слушает все ответы от сервера и запускает callback из словаря callback'ов, созданный в методе On
        /// </summary>
        void ListenResponses()
        {
            _client.OnMessage += (sender, e) =>
            {
                try
                {
                    var methodName = MessageSerializer.DeserializeMethodName(e.RawData);

                    var callback = _listenersPool[methodName];

                    callback.Action(MessageSerializer.DeserializeResponse(callback.Type, e.RawData));
                }
                catch (KeyNotFoundException)
                {
                    Log.Info($"Непрослушиваемый метод: {MessageSerializer.DeserializeMethodName(e.RawData)}");
                }
                catch (MessagePackSerializationException)
                {
                    Log.Warn($"Неопознанный метод: {Encoding.UTF8.GetString(e.RawData)}");
                }
            };
        }

        /// <summary>
        /// Создает callback, который будет передавать data и result, и записывает его в словарь callback'ов
        /// </summary>
        /// <typeparam name="T">Тип объекта десериализации</typeparam>
        /// <param name="methodName">Название метода</param>
        /// <param name="callback">Callback результата прослушивания метода</param>
        public void On<T>(string methodName, Action<T> callback)
        {
            _listenersPool.Add(methodName, new Callback()
            {
                Type = typeof(T),
                Action = (response) => callback((T)response)
            });
        }

        /// <summary>
        /// Создает одноразовый callback, который будет передавать data и result, и записывает его в словарь callback'ов
        /// </summary>
        /// <typeparam name="T">Тип объекта десериализации</typeparam>
        /// <param name="methodName">Название метода</param>
        /// <param name="callback">Callback результата прослушивания метода</param>
        public void Once<T>(string methodName, Action<T> callback)
        {
            _listenersPool.Add(methodName, new Callback()
            {
                Type = typeof(T),
                Action = (response) =>
                {
                    _listenersPool.Remove(methodName);
                    callback((T)response);
                }
            });
        }

        /// <summary>
        /// Делает запрос на сервер
        /// </summary>
        /// <typeparam name="T">Тип объекта запроса</typeparam>
        /// <param name="methodName">Название метода</param>
        /// <param name="data">Объект запроса</param>
        public void Request<T>(string methodName, T data)
        {
            _client.Send(MessageSerializer.SerializeRequest<T>(methodName, data));
        }

        /// <summary>
        /// Делает запрос на сервер без параметров
        /// </summary>
        /// <param name="methodName">Название метода</param>
        public void Request(string methodName)
        {
            _client.Send(MessageSerializer.SerializeRequest(methodName));
        }
    }
}
