using ApplicationCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class Element1CToGetError
    {
        [JsonPropertyName("ТипЗапросаИЗ1с")]
        public string WarningLevel { get; set; } = WarningLevels.Ошибка.ToString();
        [JsonPropertyName("ТелоЗапроса")]
        public Warning WarningInfo { get; set; } = new Warning();

        public class Warning
        {
            [JsonPropertyName("ИДлицензии")]
            public string LicenseId { get; set; } = "0";
            [JsonPropertyName("НаименованиеОрганизации")]
            public string Organization { get; set; } = "Отсутвует";
            [JsonPropertyName("Тема")]
            public string Topic { get; set; } = "Тема отсувует";
            [JsonPropertyName("Содержание")]
            public string Content { get; set; } = "Содержание отсутсвует";
            [JsonPropertyName("Исполнитель")]
            public string? Executor { get; set; }
        }
    }
}
