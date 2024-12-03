using System.Web;
using System.Web.Optimization;

namespace adventureworks
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información sobre los formularios.  De esta manera estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.           
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/css/bootstrap-icons.css",
                "~/Content/css/sweetalert2.min.css",
                "~/Content/css/fontawesome.css",
                "~/Content/css/styles.css"));

            bundles.Add(new StyleBundle("~/Signin").Include(
                "~/Content/css/signin.css"
                ));

            bundles.Add(new StyleBundle("~/SignUp").Include(
                "~/Content/assets/colorlibregform8/css/style.css"
                ));

            var scriptBundle = new ScriptBundle("~/bundles/scripts")
                .Include(
                "~/Content/js/jquery-3.7.1.min.js",
                "~/Content/js/bootstrap.bundle.min.js",
                "~/Content/js/sweetalert2.all.min.js",
                "~/Content/js/scripts.js"
                );

            var scriptBundleSignUp = new ScriptBundle("~/bundles/SignUp")
                .Include(
                "~/Content/assets/colorlibregform8/js/jquery.min.js",
                "~/Content/assets/colorlibregform8/js/main.js"
                );

            // Limpia las transformaciones predeterminadas (incluida la minimización)
            scriptBundle.Transforms.Clear();
            scriptBundleSignUp.Transforms.Clear();

            // Añade el bundle modificado
            bundles.Add(scriptBundle);
            bundles.Add(scriptBundleSignUp);
        }
    }
}
