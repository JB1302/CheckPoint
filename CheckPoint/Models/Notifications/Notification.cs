using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CheckPoint.Models.Notifications
{
    public class Notification
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [DisplayName("Identificador de la notificación")]
        public string Id { get; set; }

        [BsonElement("userId")]
        [Required]
        [DisplayName("Identificador del usuario")]
        public string UserId { get; set; }

        [BsonElement("type")]
        [Required]
        [StringLength(50)]
        [DisplayName("Tipo")]
        public string Type { get; set; }

        [BsonElement("referenceId")]
        [Required]
        [DisplayName("Identificador de referencia")]
        public string ReferenceId { get; set; }

        [BsonElement("message")]
        [Required]
        [StringLength(500)]
        [DisplayName("Mensaje")]
        public string Message { get; set; }

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