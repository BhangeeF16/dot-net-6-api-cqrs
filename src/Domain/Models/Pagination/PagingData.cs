using Newtonsoft.Json;

namespace Domain.Models.Pagination
{
    public class PagingData
    {
        [JsonProperty("TotalCount")]
        public int TotalCount { get; set; } = 0;
    }
}
