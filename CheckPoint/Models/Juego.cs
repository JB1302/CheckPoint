using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CheckPoint.Models
{
    public class Juego
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Identificador amigable para URL, por ejemplo "league-of-legends"</summary>
        [BsonElement("slug")]
        public string Slug { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("genre")]
        public string Genre { get; set; } = string.Empty;

        [BsonElement("platforms")]
        public List<string> Platforms { get; set; } = new();

        [BsonElement("coverImageUrl")]
        public string CoverImageUrl { get; set; } = "/img/default-game.png";

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}


