using System;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace APBD_19._03_CW3.DAL
{
    public class CustonDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToLocalTime().ToString("dd-mm-yyyy"));
        }
    }
}