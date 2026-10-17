using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;

namespace MilkTeaShopMVC.Models
{
    // Explicit signed ticket roles. A customer cookie must never authorize the Admin controller.
    public static class CustomerAuth
    {
        private static FormsAuthenticationTicket GetTicket(HttpContextBase context)
        {
            if (context == null || context.User == null ||
                context.User.Identity == null || !context.User.Identity.IsAuthenticated)
                return null;
            var cookie = context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie == null || string.IsNullOrEmpty(cookie.Value)) return null;
            try
            {
                var ticket = FormsAuthentication.Decrypt(cookie.Value);
                if (ticket == null || ticket.Expired ||
                    !string.Equals(ticket.Name, context.User.Identity.Name, StringComparison.OrdinalIgnoreCase))
                    return null;
                return ticket;
            }
            catch (Exception) { return null; }
        }

        public static int CustomerId(HttpContextBase context)
        {
            var ticket = GetTicket(context);
            if (ticket == null || string.IsNullOrEmpty(ticket.UserData) || !ticket.UserData.StartsWith("customer:", StringComparison.Ordinal)) return 0;
            int id;
            return int.TryParse(ticket.UserData.Substring("customer:".Length), out id) && id > 0 ? id : 0;
        }

        public static bool IsAdmin(HttpContextBase context)
        {
            var ticket = GetTicket(context);
            return ticket != null && ticket.UserData == "admin";
        }

        public static void SignInCustomer(HttpResponseBase response, CustomerAccount customer)
        {
            IssueTicket(response, customer.Email, "customer:" + customer.Id, false);
        }
        public static void SignInAdmin(HttpResponseBase response, string email)
        {
            IssueTicket(response, email, "admin", false);
        }
        private static void IssueTicket(HttpResponseBase response, string email, string role, bool remember)
        {
            var start = DateTime.Now;
            var ticket = new FormsAuthenticationTicket(2, email, start, start.AddMinutes(60),
                remember, role, FormsAuthentication.FormsCookiePath);
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
            authCookie.HttpOnly = true;
            authCookie.Secure = FormsAuthentication.RequireSSL;
            authCookie.Path = FormsAuthentication.FormsCookiePath;
            authCookie.SameSite = SameSiteMode.Lax;
            if (remember) authCookie.Expires = ticket.Expiration;
            response.Cookies.Add(authCookie);
        }
    }

    public sealed class AdminOnlyAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase context)
        {
            return CustomerAuth.IsAdmin(context);
        }
    }

    public sealed class CustomerOnlyAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase context)
        {
            var id = CustomerAuth.CustomerId(context);
            if (id <= 0) return false;
            using (var db = new MilkTeaDb())
                return db.CustomerAccounts.Any(x => x.Id == id && x.IsActive);
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            if (!string.Equals(request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                filterContext.Result = new HttpStatusCodeResult(403, "Vui lòng đăng nhập để tiếp tục.");
                return;
            }
            var returnUrl = HttpUtility.UrlEncode(request.RawUrl);
            var root = VirtualPathUtility.ToAbsolute("~/Account/Login");
            filterContext.Result = new RedirectResult(root + "?returnUrl=" + returnUrl);
        }
    }
}
