using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioTiposCuentas
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int id);
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
        Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados);
    }
    public class RepositorioTiposCuentas: IRepositorioTiposCuentas
    {
        private readonly string connectionString;
        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            int id = await connection.QuerySingleAsync<int>("TiposCuentas_Insertar",
                                                            new
                                                            {
                                                                usuarioId = tipoCuenta.UsuarioId,
                                                                nombre = tipoCuenta.Nombre
                                                            },
                                                            commandType: System.Data.CommandType.StoredProcedure);
            tipoCuenta.Id = id;
        }

        public async Task<bool> Existe(string nombre, int usuarioId)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            int existe = await connection.QueryFirstOrDefaultAsync<int>(
                                                    @"SELECT 1 
                                                    FROM TiposCuentas 
                                                    WHERE Nombre=@Nombre AND UsuarioId=@UsuarioId;",
                                                    new { nombre, usuarioId });
            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoCuenta>(
                                                @"SELECT Id,Nombre,Orden 
                                                FROM TiposCuentas 
                                                WHERE UsuarioId=@USuarioId
                                                ORDER BY Orden",
                                                new { usuarioId });
        }

        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"Update TiposCuentas 
                                        SET Nombre=@Nombre 
                                        WHERE Id=@Id", tipoCuenta);
        }

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT ID,Nombre,Orden
                                                                        FROM TiposCuentas
                                                                        WHERE Id=@Id AND UsuarioId=@UsuarioId",
                                                                        new { id, usuarioId });
        }

        public async Task Borrar(int id)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE TiposCuentas WHERE Id=@Id", new { id });
        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados)
        {
            string query = "UPDATE TiposCuentas SET Orden=@Orden WHERE Id=@Id";
            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(query, tipoCuentasOrdenados);
        }
    }
}
