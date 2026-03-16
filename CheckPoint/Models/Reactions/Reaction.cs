using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CheckPoint.Models.Reactions
{
    public class Reaction
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [DisplayName("Identificador de la reacción")]
        public string Id { get; set; }

        [BsonElement("targetType")]
        [Required]
        [StringLength(50)]
        [DisplayName("Tipo de objetivo")]
        public string TargetType { get; set; }

        [BsonElement("targetId")]
        [Required]
        [DisplayName("Identificador del objetivo")]
        public string TargetId { get; set; }

        [BsonElement("userId")]
        [Required]
        [DisplayName("Identificador del usuario")]
        public string UserId { get; set; }

        [BsonElement("reactionType")]
        [Required]
        [StringLength(50)]
        [DisplayName("Tipo de reacción")]
        public string ReactionType { get; set; }

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [Required]
        [DisplayName("Fecha de creación")]
        public DateTime CreatedAt { get; set; }
    }
}