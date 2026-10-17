using System;
using System.Security.Cryptography;
namespace MilkTeaShopMVC.Models {
 public static class PasswordHelper {
  public static void CreateHash(string password,out string salt,out string hash) {
   byte[] s=new byte[16]; using(var rng=RandomNumberGenerator.Create()) rng.GetBytes(s);
   using(var pbkdf=new Rfc2898DeriveBytes(password,s,120000)) {salt=Convert.ToBase64String(s);hash=Convert.ToBase64String(pbkdf.GetBytes(32));}
  }
  public static bool Verify(string password,string salt,string hash) {
   if(string.IsNullOrEmpty(password))return false;
   try {var s=Convert.FromBase64String(salt);var expected=Convert.FromBase64String(hash);byte[] actual;
    using(var pbkdf=new Rfc2898DeriveBytes(password,s,120000)) actual=pbkdf.GetBytes(expected.Length);
    int d=0;for(int i=0;i<expected.Length;i++) d|=expected[i]^actual[i];return d==0;
   }catch(FormatException){return false;}
  }
 }
}
