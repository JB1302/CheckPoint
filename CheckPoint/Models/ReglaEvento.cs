using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CheckPoint.Models
{
    public class ReglaEvento
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("eventId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string EventId { get; set; } = string.Empty;

        [BsonElement("minLevel")]
        public string MinLevel { get; set; } = string.Empty;

        [BsonElement("allowedRegions")]
        public List<string> AllowedRegions { get; set; } = new();

        [BsonElement("requiredDocuments")]
        public List<string> RequiredDocuments { get; set; } = new();

        [BsonElement("additionalRules")]
        public string AdditionalRules { get; set; } = string.Empty;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

