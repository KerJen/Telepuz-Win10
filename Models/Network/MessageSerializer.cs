using System;
using MessagePack;

namespace Telepuz.Models.Network
{
    public static class MessageSerializer
    {
        /// <summary>
        /// Сериализация запроса в поток байтов по MsgPack
        /// </summary>
        /// <typeparam name="T">Тип сериализуемого объекта</typeparam>
        /// <param name="methodName">Метод обращения запроса</param>
        /// <param name="data">Payload запроса</param>
        /// /// <exception cref="MessagePackSerializationException">Вызывается тогда, когда невозможно сериализовать запрос</exception>
        /// <returns>Сериализированный поток байтов</returns>
        public static byte[] SerializeRequest<T>(string methodName, T data)
        {
            // Массив объекта информации о запросе
            var methodNameBytes = MessagePackSerializer.Serialize(methodName);

            // Массив объекта запроса
            var dataBytes = MessagePackSerializer.Serialize(data);

            var bytes = new byte[methodNameBytes.Length + dataBytes.Length];

            // Объединение двух массивов байтов
            Buffer.BlockCopy(methodNameBytes, 0, bytes, 0, methodNameBytes.Length);
            Buffer.BlockCopy(dataBytes, 0, bytes, methodNameBytes.Length, dataBytes.Length);

            return bytes;
        }

        /// <summary>
        /// Сериализация пустого объекта запроса
        /// </summary>
        /// <param name="methodName">Метод обращения запроса</param>
        /// <returns></returns>
        public static byte[] SerializeRequest(string methodName)
        {
            // Массив объекта информации о запросе
            var methodNameBytes = MessagePackSerializer.Serialize(methodName);

            var bytes = new byte[methodNameBytes.Length + 1];

            Buffer.BlockCopy(methodNameBytes, 0, bytes, 0, methodNameBytes.Length);

            // Добавление MapHeader со значением 0
            bytes[methodNameBytes.Length] = 128;

            return bytes;
        }

        /// <summary>
        /// Десериализует поток байтов в метод обращения ответа
        /// </summary>
        /// <param name="bytes">Десериализируемые байты</param>
        /// <exception cref="">Вызвается тогда, когда формат информации об ответе
        /// не совпадает с форматом информации об ответе сервера</exception>
        /// <returns>Информация об ответе</returns>
        public static string DeserializeMethodName(byte[] bytes)
        {
            return MessagePackSerializer.Deserialize<string>(bytes);
        }

        /// <summary>
        /// Особый метод десериализации ответа сервера без передачи дженерика
        /// </summary>
        /// <param name="type">Тип объекта десериализации</param>
        /// <param name="bytes">Байты ответа</param>
        /// <exception cref="MessagePackSerializationException">Вызвается тогда, когда формат ответа не совпадает с форматом ответа сервера</exception>
        /// <returns>Десериализированная информация об ответе и десериализированный ответ</returns>
        public static object DeserializeResponse(Type type, byte[] bytes)
        {
            #region Разбор MsgPack по элементам и выборка нужных

            var reader = new MessagePackReader(bytes);

            var methodName = reader.ReadString();

            #endregion

            // Массив байтов поля "data"
            var dataBytes = new byte[bytes.Length - reader.Position.GetInteger()];

            // Копирование байтов объекта "data" из bytes в массив байтов dataBytes
            Buffer.BlockCopy(bytes, reader.Position.GetInteger(), dataBytes, 0, dataBytes.Length);

            var data = MessagePackSerializer.Deserialize(type, dataBytes);

            return data;
        }
    }
}