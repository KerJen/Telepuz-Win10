using System;
using MessagePack;
using Telepuz.Models.Network.Request;
using Telepuz.Models.Network.Response;

namespace Telepuz.Models.Network
{
    public static class MessageSerializer
    {
        /// <summary>
        /// Сериализация информации об объекте запроса и объекта запроса в поток байтов по MsgPack
        /// </summary>
        /// <typeparam name="T">Тип сериализуемого объекта</typeparam>
        /// <param name="requestInfo">Объект информации о запросе</param>
        /// <param name="request">Объект запроса</param>
        /// /// <exception cref="MessagePackSerializationException">Вызывается тогда, когда невозможно сериализовать
        /// информацию о запросе и/или запрос</exception>
        /// <returns>Сериализированный поток байтов</returns>
        public static byte[] SerializeRequest<T>(RequestInfo requestInfo, Request<T> request)
        {
            // Массив объекта информации о запросе
            var requestInfoBytes = MessagePackSerializer.Serialize(requestInfo);

            // Массив объекта запроса
            var requestBytes = MessagePackSerializer.Serialize(request);

            var bytes = new byte[requestInfoBytes.Length + requestBytes.Length];

            // Объединение двух массивов байтов
            Buffer.BlockCopy(requestInfoBytes, 0, bytes, 0, requestInfoBytes.Length);
            Buffer.BlockCopy(requestBytes, 0, bytes, requestInfoBytes.Length, requestBytes.Length);

            return bytes;
        }

        /// <summary>
        /// Сериализация информации об объекте запроса и пустого объекта запроса
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <returns></returns>
        public static byte[] SerializeRequest(RequestInfo requestInfo)
        {
            // Массив объекта информации о запросе
            var requestInfoBytes = MessagePackSerializer.Serialize(requestInfo);

            var bytes = new byte[requestInfoBytes.Length + 1];

            // Добавление MapHeader со значением 0
            bytes[requestInfoBytes.Length] = 128;

            return bytes;
        }

        // TODO: Вынести RequestInfo в один класс Request
        /// <summary>
        /// Десериализует поток байтов в информацию об ответе от сервера
        /// </summary>
        /// <param name="bytes">Десериализируемые байты</param>
        /// <exception cref="MessagePackSerializationException">Вызвается тогда, когда формат информации об ответе
        /// не совпадает с форматом информации об ответе сервера</exception>
        /// <returns>Информация об ответе</returns>
        public static ResponseInfo DeserializeResponseInfo(byte[] bytes)
        {
            return MessagePackSerializer.Deserialize<ResponseInfo>(bytes);
        }

        /// <summary>
        /// Особый метод десериализации без передачи дженерика
        /// </summary>
        /// <param name="type">Тип объекта десериализации, если null, то ответ от сервера только result</param>
        /// <param name="bytes">Байты ответа</param>
        /// <exception cref="MessagePackSerializationException">Вызвается тогда, когда формат ответа не совпадает с форматом ответа сервера</exception>
        /// <returns>Десериализированная информация об ответе и десериализированный ответ</returns>
        public static Response.Response DeserializeResponse(Type type, byte[] bytes)
        {
            #region Разбор MsgPack по элементам и выборка нужных

            var reader = new MessagePackReader(bytes);

            var responseInfoMapHeader = reader.ReadMapHeader();
            var methodNameKey = reader.ReadString();
            var methodName = reader.ReadString();

            var responseMapHeader = reader.ReadMapHeader();
            var resultKey = reader.ReadString();
            var result = reader.ReadInt32();

            #endregion

            #region Получение поля "data" и подготовка Response

            Response.Response response;

            if (type != null)
            {
                var dataKey = reader.ReadString();
                var dataMapHeader = reader.ReadMapHeader();

                // Массив байтов поля "data"
                var dataBytes = new byte[bytes.Length - (reader.Position.GetInteger() - 1)];

                // Копирование байтов объекта "data" из bytes в массив байтов dataBytes
                Buffer.BlockCopy(bytes, reader.Position.GetInteger() - 1, dataBytes, 0, dataBytes.Length);

                // Создание объекта ответа с результатом и объектом десериализации
                response = new Response.Response()
                {
                    Result = result,
                    Data = MessagePackSerializer.Deserialize(type, dataBytes)
                };
            }
            else
            {
                response = new Response.Response()
                {
                    Result = result,
                };
            }

            #endregion

            return response;
        }
    }
}