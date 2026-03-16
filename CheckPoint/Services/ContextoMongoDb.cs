using CheckPoint.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CheckPoint.Services
{
    /// <summary>
    /// Registrado como singleton para compartir MongoClient entre solicitudes.
    /// </summary>
    public class ContextoMongoDb
    {
        private readonly IMongoDatabase _db;

        public ContextoMongoDb(IOptions<ConfiguracionMongoDb> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _db = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<Usuario> Users =>
            _db.GetCollection<Usuario>("Users");

        public IMongoCollection<Perfil> Profiles =>
            _db.GetCollection<Perfil>("Perfiles");

        public IMongoCollection<Juego> Games =>
            _db.GetCollection<Juego>("Juegos");

        public IMongoCollection<Evento> Events =>
            _db.GetCollection<Evento>("Eventos");

        public IMongoCollection<ReglaEvento> EventRules =>
            _db.GetCollection<ReglaEvento>("EventRules");

        public IMongoCollection<Inscripcion> Registrations =>
            _db.GetCollection<Inscripcion>("Inscripciones");

        public IMongoCollection<Publicacion> Posts =>
            _db.GetCollection<Publicacion>("Publicaciones");

        public IMongoCollection<Comentario> Comments =>
            _db.GetCollection<Comentario>("Comments");

        public IMongoCollection<Reaccion> Reactions =>
            _db.GetCollection<Reaccion>("Reactions");

        public IMongoCollection<Notificacion> Notifications =>
            _db.GetCollection<Notificacion>("Notificaciones");

        public IMongoCollection<Reporte> Reports =>
            _db.GetCollection<Reporte>("Reports");

        public IMongoCollection<BitacoraAuditoria> AuditLogs =>
            _db.GetCollection<BitacoraAuditoria>("AuditLogs");
    }
}


