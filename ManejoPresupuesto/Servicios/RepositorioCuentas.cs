using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{

    public interface IRepositorioCuentas
    {
        Task Actualizar(CuentaCreacionViewModel cuenta);
        Task Borrar(int id);
        Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
        Task Crear(Cuenta cuenta);
        Task<Cuenta> ObtenerPorId(int id, int usuarioId);
    }

    public class RepositorioCuentas : IRepositorioCuentas
    {
        private readonly string connectionString;
        public RepositorioCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Cuenta cuenta)
        {
            using SqlConnection connection = new(connectionString);
            int id = await connection.QuerySingleAsync<int>(@"INSERT INTO Cuentas (Nombre,TipoCuentaId,Descripcion,Balance)
                                                     VALUES (@Nombre,@TipoCuentaId,@Descripcion,@Balance);
                                                     SELECT SCOPE_IDENTITY();", cuenta);
            cuenta.Id = id;
        }

        public async Task<IEnumerable<Cuenta>> Buscar(int usuarioId)
        {
            using SqlConnection connection = new(connectionString);
            return await connection.QueryAsync<Cuenta>(@"SELECT Cuentas.Id,Cuentas.Nombre, Balance, tc.Nombre as TipoCuenta FROM Cuentas
                                                        INNER JOIN TiposCuentas tc
                                                        ON tc.Id=cuentas.TipoCuentaId
                                                        WHERE tc.UsuarioId=@UsuarioId
                                                        ORDER BY tc.Orden", new { usuarioId });
        }

        public async Task<Cuenta> ObtenerPorId(int id, int usuarioId)
        {
            using SqlConnection connection = new(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Cuenta>(@"SELECT Cuentas.Id,Cuentas.Nombre, Balance, Descripcion, TipoCuentaId FROM Cuentas
                                                                INNER JOIN TiposCuentas tc
                                                                ON tc.Id=cuentas.TipoCuentaId
                                                                WHERE tc.UsuarioId=@UsuarioId and Cuentas.Id=@Id", new { id, usuarioId });
        }

        public async Task Actualizar(CuentaCreacionViewModel cuenta)
        {
            using SqlConnection connection = new(connectionString);
            await connection.ExecuteAsync(@"UPDATE Cuentas
                                                SET Nombre=@Nombre, Balance=@Balance,Descripcion=@Descripcion, TipoCuentaId=@TipoCuentaId
                                                WHERE Id=@Id", cuenta);
        }

        public async Task Borrar(int id)
        {
            using SqlConnection connection = new(connectionString);
            await connection.ExecuteAsync("DELETE cuentas WHERE Id=@Id", new { id });
        }
    }
}
