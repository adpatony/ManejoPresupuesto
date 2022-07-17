using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Controllers
{
    public class CategoriasController:Controller
    {
        private readonly IRepositorioCategorias repositorioCategorias;
        private readonly IServicioUsuarios servicioUsuarios;

        public CategoriasController(IRepositorioCategorias repositorioCategorias,IServicioUsuarios servicioUsuarios)
        {
            this.repositorioCategorias = repositorioCategorias;
            this.servicioUsuarios = servicioUsuarios;
        }

        public async Task<IActionResult> Index()
        {
            int usuarioId = servicioUsuarios.ObtenerUsuarioId();
            IEnumerable<Categoria> categorias = await repositorioCategorias.Obtener(usuarioId);
            return View(categorias);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Categoria categoria)
        {
            if (!ModelState.IsValid)
            {
                return View(categoria);
            }
            int usuarioId = servicioUsuarios.ObtenerUsuarioId();

            categoria.UsuarioId = usuarioId;
            await repositorioCategorias.Crear(categoria);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Editar(int id)
        {
            int usuarioId = servicioUsuarios.ObtenerUsuarioId();
            Categoria categoria = await repositorioCategorias.ObtenerPorId(id, usuarioId);

            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Categoria categoriaEditar)
        {
            if (!ModelState.IsValid)
                return View(categoriaEditar);

            int usuarioId = servicioUsuarios.ObtenerUsuarioId();
            Categoria categoria = await repositorioCategorias.ObtenerPorId(categoriaEditar.Id, usuarioId);

            if (categoria is null)
                return RedirectToAction("NoEncontrado", "Home");

            categoriaEditar.UsuarioId = usuarioId;

            await repositorioCategorias.Actualizar(categoriaEditar);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Borrar(int id)
        {
            int usuarioId = servicioUsuarios.ObtenerUsuarioId();
            Categoria categoria = await repositorioCategorias.ObtenerPorId(id, usuarioId);

            if(categoria is null)
                return RedirectToAction("NoEncontrado", "Home");

            return View(categoria);

        }

        [HttpPost]
        public async Task<IActionResult> BorrarCategoria(int id)
        {
            int usuarioid = servicioUsuarios.ObtenerUsuarioId();
            Categoria categoria = await repositorioCategorias.ObtenerPorId(id, usuarioid);

            if (categoria is null)
                return RedirectToAction("NoEncontrado", "Home");

            await repositorioCategorias.Borrar(id);

            return RedirectToAction("Index");
        }
    }
}
