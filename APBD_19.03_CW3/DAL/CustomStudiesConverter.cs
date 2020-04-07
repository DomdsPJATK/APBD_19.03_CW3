using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using APBD_19._03_CW3.Model;

namespace APBD_19._03_CW3.DAL
{
    public class CustomStudiesConverter : JsonConverter<Studies>
    {
        public override Studies Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            String str = reader.GetString();
            return new Studies()
            {
                name = str
            };
        }

        public override void Write(Utf8JsonWriter writer, Studies value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.name);
        }
    }
}