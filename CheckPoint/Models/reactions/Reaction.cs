using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CheckPoint.Models.Reactions
{
    public class Reaction
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [DisplayName("Identificador de la reacción")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("targetType")]
        [Required]
        [StringLength(50)]
        [DisplayName("Tipo de objetivo")]
        public string TargetType { get; set; } = string.Empty;

        [BsonElement("targetId")]
        [Required]
        [DisplayName("Identificador del objetivo")]
        public string TargetId { get; set; } = string.Empty;

        [BsonElement("userId")]
        [Required]
        [DisplayName("Identificador del usuario")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("reactionType")]
        [Required]
        [StringLength(50)]
        [DisplayName("Tipo de reacción")]
        public string ReactionType { get; set; } = string.Empty;

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [Required]
        [DisplayName("Fecha de creación")]
        public DateTime CreatedAt { get; set; }
    }
}