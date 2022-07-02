using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Controllers
{
    public class CuentasController:Controller
    {
        private readonly IRepositorioTiposCuentas _repositorioTiposCuentas;
        private readonly IServicioUsuarios _servicioUsuarios;
        private readonly IRepositorioCuentas _repositorioCuentas;

        public CuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, IServicioUsuarios servicioUsuarios, IRepositorioCuentas repositorioCuentas)
        {
            this._repositorioTiposCuentas = repositorioTiposCuentas;
            this._servicioUsuarios = servicioUsuarios;
            this._repositorioCuentas = repositorioCuentas;
        }

        public async Task<IActionResult> Index()
        {
            int usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            IEnumerable<Cuenta> cuentasConTipoCuenta = await _repositorioCuentas.Buscar(usuarioId);

            var modelo = cuentasConTipoCuenta.
                GroupBy(x => x.TipoCuenta)
                .Select(grupo => new IndiceCuentasViewModel
                {
                    TipoCuenta = grupo.Key,
                    Cuentas = grupo.AsEnumerable()
                }).ToList();

            return View(modelo);

        }

        public async Task<IActionResult> Crear()
        {
            int usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            IEnumerable<TipoCuenta> tiposcuentas = await _repositorioTiposCuentas.Obtener(usuarioId);

            CuentaCreacionViewModel modelo = new();
            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);


            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CuentaCreacionViewModel cuenta)
        {
            int usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            TipoCuenta tipoCuenta = await _repositorioTiposCuentas.ObtenerPorId(cuenta.TipoCuentaId, usuarioId);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            if (!ModelState.IsValid)
            {
                cuenta.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
                return View(cuenta);
            }
            await _repositorioCuentas.Crear(cuenta);
            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId)
        {
            IEnumerable<TipoCuenta> tiposCuentas = await _repositorioTiposCuentas.Obtener(usuarioId);
            return tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }
    }
}
