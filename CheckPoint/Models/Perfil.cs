using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CheckPoint.Models
{
    public class Perfil
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("displayName")]
        public string DisplayName { get; set; } = string.Empty;

        [BsonElement("bio")]
        public string Bio { get; set; } = string.Empty;

        [BsonElement("gamerTag")]
        public string GamerTag { get; set; } = string.Empty;

        [BsonElement("avatarUrl")]
        public string AvatarUrl { get; set; } = "/img/default-avatar.png";

        [BsonElement("bannerUrl")]
        public string BannerUrl { get; set; } = "/img/default-banner.png";

        [BsonElement("country")]
        public string Country { get; set; } = string.Empty;

        [BsonElement("favoriteGames")]
        public List<string> FavoriteGames { get; set; } = new();

        [BsonElement("socialLinks")]
        public Dictionary<string, string> SocialLinks { get; set; } = new();

        [BsonElement("totalEventsJoined")]
        public int TotalEventsJoined { get; set; } = 0;

        [BsonElement("totalEventsOrganized")]
        public int TotalEventsOrganized { get; set; } = 0;

        [BsonElement("reputationScore")]
        public int ReputationScore { get; set; } = 0;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

