using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web.Core;
using MessagePack;
using MetroLog;
using Telepuz.API.Model;
using Telepuz.API.Model.DTO.Send;
using Telepuz.API.Model.Request;
using Telepuz.API.Model.Response;
using Telepuz.Helpers;
using WebSocketSharp;

namespace Telepuz.API
{
    // TODO: Перевести на DI Stiletto Library вместо стандартного синглтона
    internal class TelepuzWebSocketService
    {
        // Lazy-инициализация синглтона веб-сокет сервиса
        static readonly Lazy<TelepuzWebSocketService> telepuzWebSocketService
            = new Lazy<TelepuzWebSocketService>(() => new TelepuzWebSocketService());

        ILogger Log = LogManagerFactory.DefaultLogManager.GetLogger<TelepuzWebSocketService>();

        readonly WebSocket _client;

        public static TelepuzWebSocketService Client => telepuzWebSocketService.Value;

        readonly Dictionary<string, Callback> _listenersPool;

        private TelepuzWebSocketService()
        {
            // Инициализация веб-сокет клиента
            _client = new WebSocket(ApiOptions.Url);

            // Инициализация пула калбеков
            _listenersPool = new Dictionary<string, Callback>();
        }

        /// <summary>
        /// Подключение к серверу
        /// </summary>
        public void Connect()
        {
            _client.OnOpen += (sender, e) =>
            {
                ListenResponses();
            };

            _client.OnError += (sender, e) =>
            {
                //TODO: Сообщение об ошибке
            };

            _client.Connect();
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
                    var responseInfo = MessageSerializer.DeserializeResponseInfo(e.RawData);

                    var callback = _listenersPool[responseInfo.MethodName];

                    callback.Action(MessageSerializer.DeserializeResponse(callback.Type, e.RawData));
                }
                catch (KeyNotFoundException)
                {
                    Log.Info($"Непрослушиваемый метод: {MessageSerializer.DeserializeResponseInfo(e.RawData)}");
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
        public void On<T>(string methodName, Action<Response> callback)
        {
            _listenersPool.Add(methodName, new Callback()
            {
                Type = typeof(T),
                Action = (response) => callback((Response)response)
            });
        }

        /// <summary>
        /// Создает callback, который будет передавать только result, и записывает его в словарь callback'ов
        /// </summary>
        /// <param name="methodName">Название метода</param>
        /// <param name="callback">Callback результата прослушивания метода</param>
        public void On(string methodName, Action<Response> callback)
        {
            _listenersPool.Add(methodName, new Callback()
            {
                Type = null,
                Action = (response) => callback((Response)response)
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
            var requestInfo = new RequestInfo()
            {
                MethodName = methodName
            };

            var request = new Request<T>()
            {
                Data = data
            };

            _client.Send(MessageSerializer.SerializeRequest<T>(requestInfo, request));
        }

        /// <summary>
        /// Делает запрос на сервер без параметров
        /// </summary>
        /// <param name="methodName">Название метода</param>
        public void Request(string methodName)
        {
            var requestInfo = new RequestInfo()
            {
                MethodName = methodName
            };

            _client.Send(MessageSerializer.SerializeRequest(requestInfo));
        }
    }
}
