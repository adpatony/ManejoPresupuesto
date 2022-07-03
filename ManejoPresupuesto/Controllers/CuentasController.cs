using AutoMapper;
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
        private readonly IMapper mapper;

        public CuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, IServicioUsuarios servicioUsuarios, IRepositorioCuentas repositorioCuentas, IMapper mapper)
        {
            this._repositorioTiposCuentas = repositorioTiposCuentas;
            this._servicioUsuarios = servicioUsuarios;
            this._repositorioCuentas = repositorioCuentas;
            this.mapper = mapper;
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

        public async Task<IActionResult> Editar(int id)
        {
            int usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            Cuenta cuenta = await _repositorioCuentas.ObtenerPorId(id, usuarioId);

            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            CuentaCreacionViewModel modelo = mapper.Map<CuentaCreacionViewModel>(cuenta);

            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(CuentaCreacionViewModel cuentaEditar)
        {
            int usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            Cuenta cuenta = await _repositorioCuentas.ObtenerPorId(cuentaEditar.Id, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            TipoCuenta tipoCuenta = await _repositorioTiposCuentas.ObtenerPorId(cuenta.TipoCuentaId,usuarioId);

            if(tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await _repositorioCuentas.Actualizar(cuentaEditar);
            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId)
        {
            IEnumerable<TipoCuenta> tiposCuentas = await _repositorioTiposCuentas.Obtener(usuarioId);
            return tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }

        public async Task<IActionResult> Borrar(int id)
        {
            int usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            Cuenta cuenta = await _repositorioCuentas.ObtenerPorId(id, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(cuenta);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarCuenta(int id)
        {
            int usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            Cuenta cuenta = await _repositorioCuentas.ObtenerPorId(id, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await _repositorioCuentas.Borrar(id);
            return RedirectToAction("Index");
        }
    }
}
