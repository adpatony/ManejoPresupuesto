using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioCategorias
    {
        Task Crear(Categoria categoria);
    }

    public class RepositorioCategorias:IRepositorioCategorias
    {
        private readonly string connectionString;

        public RepositorioCategorias(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Categoria categoria)
        {
            using SqlConnection connection = new(connectionString);
            int id = await connection.QuerySingleAsync<int>(@"INSERT INTO Categorias (Nombre,TipoOperacionId,UsuarioId)
                                                          VALUES (@Nombre,@TipoOperacionId,@UsuarioId);
 
                                                          SELECT SCOPE_IDENTITY();", categoria);

            categoria.Id = id;
        }
    }
}
