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
    public class LoginController : Controller
    {
        private projectsEntities1 db = new projectsEntities1();

        public ActionResult Login(string usuario_codigo, string usuario_contrasena)
        {
            // Verifica las credenciales (este es un ejemplo básico)
            var user = db.usuarios.FirstOrDefault(u => u.usuario_codigo == usuario_codigo && u.usuario_contrasena == usuario_contrasena);

            if (user != null)
            {
                // Crea una cookie
                HttpCookie authCookie = new HttpCookie("UserSession");
                authCookie.Values["usuario_id"] = user.usuario_id.ToString();
                authCookie.Values["usuario_nombre"] = user.usuario_nombre;
                authCookie.Values["usuario_codigo"] = user.usuario_codigo;
                authCookie.Expires = TimeZoneHelper.ConvertToElSalvadorTime(DateTime.UtcNow.AddHours(1)); // Expira en 24 horas
                Response.Cookies.Add(authCookie);

                return RedirectToAction("Index", "Fotos");
            }

            ViewBag.Error = "Credenciales inválidas.";
            return View();
        }

        public ActionResult Logout()
        {
            if (Request.Cookies["UserSession"] != null)
            {
                HttpCookie authCookie = new HttpCookie("UserSession");
                authCookie.Expires = TimeZoneHelper.ConvertToElSalvadorTime(DateTime.UtcNow.AddDays(-1)); // Expira inmediatamente
                Response.Cookies.Add(authCookie);
            }

            return RedirectToAction("Login", "Login");
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
