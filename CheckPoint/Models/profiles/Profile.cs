using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CheckPoint.Models.Profiles
{
    public class Profile
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [DisplayName("Identificador del perfil")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("userId")]
        [Required]
        [DisplayName("Identificador del usuario")]
        public string UserId { get; set; } = string.Empty;

        [BsonIgnore]
        [DisplayName("Nombre de usuario")]
        public string Username { get; set; } = string.Empty;

        [BsonElement("displayName")]
        [Required]
        [StringLength(100)]
        [DisplayName("Nombre para mostrar")]
        public string DisplayName { get; set; } = string.Empty;

        [BsonElement("bio")]
        [StringLength(500)]
        [DisplayName("Biografía")]
        public string? Bio { get; set; }

        [BsonElement("avatarUrl")]
        [StringLength(300)]
        [DisplayName("URL del avatar")]
        public string? AvatarUrl { get; set; }

        [BsonElement("country")]
        [StringLength(100)]
        [DisplayName("País")]
        public string? Country { get; set; }

        [BsonIgnore]
        [DisplayName("Website")]
        public string? Website { get; set; }

        [BsonIgnore]
        [DisplayName("Fecha de miembro")]
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("favoriteGameIds")]
        [DisplayName("Juegos favoritos")]
        public List<string> FavoriteGameIds { get; set; } = new List<string>();
    }
}