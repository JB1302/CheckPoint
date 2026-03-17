using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CheckPoint.Models.Events
{
    public class Events
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [DisplayName("Identificador del evento")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("title")]
        [Required]
        [StringLength(150)]
        [DisplayName("Título")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("description")]
        [Required]
        [StringLength(1000)]
        [DisplayName("Descripción")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("gameId")]
        [Required]
        [DisplayName("Identificador del juego")]
        public string GameId { get; set; } = string.Empty;

        [BsonElement("eventType")]
        [Required]
        [StringLength(50)]
        [DisplayName("Tipo de evento")]
        public string EventType { get; set; } = string.Empty;

        [BsonElement("startDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [Required]
        [DisplayName("Fecha de inicio")]
        public DateTime StartDate { get; set; }

        [BsonElement("endDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [Required]
        [DisplayName("Fecha de finalización")]
        public DateTime EndDate { get; set; }

        [BsonElement("location")]
        [StringLength(150)]
        [DisplayName("Ubicación")]
        public string? Location { get; set; }

        [BsonElement("onlineLink")]
        [StringLength(300)]
        [DisplayName("Enlace en línea")]
        public string? OnlineLink { get; set; }

        [BsonElement("maxParticipants")]
        [Required]
        [DisplayName("Máximo de participantes")]
        public int MaxParticipants { get; set; }

        [BsonElement("organizerId")]
        [Required]
        [DisplayName("Identificador del organizador")]
        public string OrganizerId { get; set; } = string.Empty;

        [BsonElement("status")]
        [Required]
        [StringLength(50)]
        [DisplayName("Estado")]
        public string Status { get; set; } = string.Empty;

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [Required]
        [DisplayName("Fecha de creación")]
        public DateTime CreatedAt { get; set; }
    }
}