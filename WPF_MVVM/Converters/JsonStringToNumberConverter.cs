using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WPF_MVVM.Converters
{
    internal class JsonStringToNumberConverter : JsonConverter<object>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(string).Name == typeToConvert.Name || typeof(Nullable<>).Name == typeToConvert.Name;
        }
        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                if (typeToConvert?.FullName?.Contains("Double") == true)
                {
                    return reader.GetDouble();
                }

                return reader.TryGetInt64(out long l) ? l : reader.GetDouble();
            }
            if (typeToConvert?.FullName?.ToUpper()?.Contains("DOUBLE") == true)
            {
                if (double.TryParse(reader.GetString(), out double ret))
                {
                    return ret;
                }
            }
            if (typeToConvert?.FullName?.ToUpper()?.Contains("INT") == true)
            {
                if (int.TryParse(reader.GetString(), out int ret))
                {
                    return ret;
                }
            }
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return document.RootElement.Clone().ToString();
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
