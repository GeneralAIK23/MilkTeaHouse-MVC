using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Routing;
using MilkTeaShopMVC.Models;
namespace MilkTeaShopMVC {
 public class MvcApplication : System.Web.HttpApplication {
  protected void Application_Start() {
   Database.SetInitializer<MilkTeaDb>(null);
   AreaRegistration.RegisterAllAreas();
   RouteConfig.RegisterRoutes(RouteTable.Routes);
   using (var db = new MilkTeaDb()) { db.Database.Initialize(false); }
  }
 }
}
