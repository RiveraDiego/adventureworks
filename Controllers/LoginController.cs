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
            // Primero buscamos al usuario por el código
            var user = db.usuarios.FirstOrDefault(u => u.usuario_codigo == usuario_codigo);

            // Verificamos si el usuario existe y si la contraseña es correcta
            if (user != null && PasswordHelper.VerifyPassword(usuario_contrasena, user.usuario_contrasena))
            {
                // Crea una cookie
                HttpCookie authCookie = new HttpCookie("UserSession");
                authCookie.Values["usuario_id"] = user.usuario_id.ToString();
                authCookie.Values["usuario_nombre"] = user.usuario_nombre;
                authCookie.Values["usuario_codigo"] = user.usuario_codigo;
                authCookie.Expires = DateTime.Now.AddHours(1); // Expira en 1 hora
                Response.Cookies.Add(authCookie);

                TempData["message"] = "Bienvenid@ "+user.usuario_codigo+"("+user.usuario_nombre+")";
                TempData["icon"] = "success";

                return RedirectToAction("Index", "Fotos");
            }
            else
            {
                // Si las credenciales no son correctas
                TempData["message"] = "Los datos ingresados no son correctos.";
                TempData["icon"] = "error";
            }

            return View();
        }

        public ActionResult Logout()
        {
            if (Request.Cookies["UserSession"] != null)
            {
                HttpCookie authCookie = new HttpCookie("UserSession");
                authCookie.Expires = DateTime.Now.AddDays(-1); // Expira inmediatamente
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
