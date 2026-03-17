using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CheckPoint.Models.Notifications
{
    public class Notification
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [DisplayName("Identificador de la notificación")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("userId")]
        [Required]
        [DisplayName("Identificador del usuario")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("type")]
        [Required]
        [StringLength(50)]
        [DisplayName("Tipo")]
        public string Type { get; set; } = string.Empty;

        [BsonElement("referenceId")]
        [Required]
        [DisplayName("Identificador de referencia")]
        public string ReferenceId { get; set; } = string.Empty;

        [BsonElement("message")]
        [Required]
        [StringLength(500)]
        [DisplayName("Mensaje")]
        public string Message { get; set; } = string.Empty;

        [BsonElement("isRead")]
        [Required]
        [DisplayName("Leída")]
        public bool IsRead { get; set; }

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [Required]
        [DisplayName("Fecha de creación")]
        public DateTime CreatedAt { get; set; }
    }
}