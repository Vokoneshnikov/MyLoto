using System.Text.Json;
using System.Text.Json.Serialization;
using Ganss.Xss;

namespace MyLoto.WebAPI.Middlewares;

public class XssSanitizationConverter : JsonConverter<string>
{
    private static readonly HtmlSanitizer Sanitizer = new();

    static XssSanitizationConverter()
    {
        // Так как наше приложение (лотерея) не подразумевает ввод форматированного текста (строки, жирный шрифт и т.д.),
        // мы полностью очищаем списки разрешенных тегов и атрибутов.
        // Это превращает санитайзер в абсолютный "стиратель" любого HTML/JS кода.
        Sanitizer.AllowedTags.Clear();
        Sanitizer.AllowedAttributes.Clear();
    }

    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var rawValue = reader.GetString();

        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return rawValue;
        }

        // Чистим строку от любых попыток внедрения тегов
        return Sanitizer.Sanitize(rawValue);
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        // При отправке данных на фронтенд записываем строку как есть
        writer.WriteStringValue(value);
    }
}