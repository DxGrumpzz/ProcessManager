using System;

namespace ProcessManager.UI
{
    /// <summary>
    /// An interface describing a serialzier used to read and write data to files
    /// </summary>
    public interface ISerializer
    {

        /// <summary>
        /// Serialize an object to a byte array
        /// </summary>
        /// <typeparam name="T"> The object type </typeparam>
        /// <param name="data"> The actuall data </param>
        /// <returns></returns>
        public byte[] Serialize<T>(T data);
        public byte[] Serialize(object data, Type objType);

        /// <summary>
        /// Deserializes a byte array to an object
        /// </summary>
        /// <typeparam name="T"> The T of object to deserialize to </typeparam>
        /// <param name="data"> The object's data as a form of bytes </param>
        /// <returns></returns>
        public T Deserialize<T>(byte[] data);
        public object Deserialize(byte[] data, Type objType);


        /// <summary>
        /// Serialize an object to a string
        /// </summary>
        /// <typeparam name="T"> The object type </typeparam>
        /// <param name="data"> The actual data </param>
        /// <returns></returns>
        public string SerializeToString<T>(T data);
        public string SerializeToString(object data, Type objType);


        /// <summary>
        /// Deserializes a string to an object
        /// </summary>
        /// <typeparam name="T"> The T of object to deserialize to </typeparam>
        /// <param name="data"> The object's data as a string (xml, json, or bson) </param>
        /// <returns></returns>
        public T DeserializeFromString<T>(string data);
        public object DeserializeFromString(string data, Type objType);

    };
};