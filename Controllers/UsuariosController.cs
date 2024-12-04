using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using adventureworks.Models;
using adventureworks.Utils;

namespace adventureworks.Controllers
{
    public class UsuariosController : Controller
    {
        private projectsEntities1 db = new projectsEntities1();

        // GET: Usuarios
        public ActionResult Index()
        {
            //return View(db.usuarios.ToList());
            return RedirectToAction("Index", "Fotos");

        }

        // GET: Usuarios/Details/5
        public ActionResult Details(int? id)
        {
            // Validar sesion
            if (Request.Cookies["UserSession"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                int cookieUserId = Convert.ToInt32(Request.Cookies["UserSession"]["usuario_id"]);
                if (id != cookieUserId)
                {
                    return RedirectToAction("Details", new { id = cookieUserId });
                }
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            usuario usuario = db.usuarios.Find(id);
            int total_publicaciones = db.fotos.Count(f => f.usuario_id == id);
            ViewBag.total_publicaciones = total_publicaciones;

            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "usuario_id,usuario_nombre,usuario_codigo,usuario_contrasena")] usuario usuario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    usuario.fecha_creacion = TimeZoneHelper.ConvertToElSalvadorTime(DateTime.UtcNow);
                    db.usuarios.Add(usuario);
                    db.SaveChanges();
                    return RedirectToAction("Login","Login");
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var validationError in ex.EntityValidationErrors)
                {
                    foreach (var error in validationError.ValidationErrors)
                    {
                        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    }
                }
                // Regresar la vista con los errores añadidos al ModelState
                return RedirectToAction("Login", "Login");
            }

            return RedirectToAction("Login", "Login");
        }

        // GET: Usuarios/Edit/5
        public ActionResult Edit(int? id)
        {
            // Validar sesion
            if (Request.Cookies["UserSession"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                int cookieUserId = Convert.ToInt32(Request.Cookies["UserSession"]["usuario_id"]);
                if (id != cookieUserId)
                {
                    return RedirectToAction("Details", new { id = cookieUserId});
                }
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            usuario usuario = db.usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "usuario_id,usuario_nombre,usuario_codigo,usuario_contrasena")] usuario usuario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            usuario usuario = db.usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            usuario usuario = db.usuarios.Find(id);
            db.usuarios.Remove(usuario);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult MisPublicaciones()
        {
            // Validar sesion
            if (Request.Cookies["UserSession"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            int usuarioId = Convert.ToInt32(Request.Cookies["UserSession"]["usuario_id"]);

            // Filtra las fotos por el usuario_id
            var fotos = db.fotos
                .Include(f => f.usuario) // Incluye los datos relacionados de usuario
                .Where(f => f.usuario_id == usuarioId) // Filtra por usuario
                .OrderByDescending(f => f.foto_fecha_creacion); // Ordenar por la fecha de creación más reciente

            return View(fotos.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
