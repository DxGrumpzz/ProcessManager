namespace ProcessManager.UI
{
    using System;
    using System.IO;
    using System.Text;
    using System.Text.Json;


    /// <summary>
    /// A json impelmentation of <see cref="ISerializer"/>
    /// </summary>
    public class JsonSerializer : ISerializer
    {

        private readonly JsonSerializerOptions _serializerOptions = default;
        private readonly JsonWriterOptions _jsonWriterOptions;


        public JsonSerializer()
        {
            _serializerOptions = new JsonSerializerOptions()
            { 
                WriteIndented = true,
            };

            _jsonWriterOptions = new JsonWriterOptions()
            { 
                Indented = true,
            };

        }

        public JsonSerializer(JsonSerializerOptions serializerOptions, JsonWriterOptions jsonWriterOptions)
        {
            _serializerOptions = serializerOptions;
            _jsonWriterOptions = jsonWriterOptions;
        }


        public byte[] Serialize(object data, Type objType)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var jsonWriter = new Utf8JsonWriter(memoryStream, _jsonWriterOptions))
                {
                    System.Text.Json.JsonSerializer.Serialize(jsonWriter, data, objType, _serializerOptions);

                    return memoryStream.ToArray();
                };
            };
        }

        public byte[] Serialize<T>(T data)
        {
            return Serialize(data, typeof(T));
        }

        public string SerializeToString(object data, Type objType)
        {
            return Encoding.UTF8.GetString(Serialize(data, objType));
        }

        public string SerializeToString<T>(T data)
        {
            return SerializeToString(data, typeof(T));
        }



        public object Deserialize(byte[] data, Type objType)
        {
            var jsonReader = new Utf8JsonReader(data);
            return System.Text.Json.JsonSerializer.Deserialize(ref jsonReader, objType, _serializerOptions);
        }

        public T Deserialize<T>(byte[] data)
        {
            return (T)Deserialize(data, typeof(T));
        }

        public object DeserializeFromString(string data, Type objType)
        {
            return Deserialize(Encoding.UTF8.GetBytes(data), objType);
        }

        public T DeserializeFromString<T>(string data)
        {
            return (T)Deserialize(Encoding.UTF8.GetBytes(data), typeof(T));
        }


    };
};
