using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infraestructura_ReservasStyle.Repositories;
using Dominio_ReservasStyle.Entities;
using BCrypt.Net;

namespace Aplicacion_ReservasStyle.Services
{
    public class UsuarioService
    {
        private readonly IGenericRepository<Usuario> _repo;

        public UsuarioService(IGenericRepository<Usuario> repo)
        {
            _repo = repo;
        }

        public async Task<List<Usuario>> GetAll() => await _repo.GetAll();

        public async Task Create(Usuario u)
        {
            u.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(u.ContrasenaHash);
            await _repo.Add(u);
        }
    }
}
