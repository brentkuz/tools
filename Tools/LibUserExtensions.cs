/*
 * Author: Brent Kuzmanich
 * Comment: Static Class containing methods that extend PrincipalContext 
 * and its derived classes.
*/

using System;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;

namespace BSD.Library
{
    public static class LibUserExtensions
    {
        //Get employee if as int
        public static int EmployeeId(this UserPrincipal user)
        {                         
            return Convert.ToInt32(user.EmployeeId);                
        }
        //Get dequlified login
        public static string Login(this Principal user)
        {
            string login = user.SamAccountName;
            //dequalify if neccessary
            return login.Contains("\\") ? login.Split('\\')[1] : login;
        }
        //Get full name string with uppercase first letters ex. "First Last"
        public static string FullName(this UserPrincipal user)
        {
            string first = user.GivenName[0].ToString().ToLower() == "x" ? (user.GivenName.Split(' ')[1]).UppercaseFirstLetter() : user.GivenName.ToString().UppercaseFirstLetter();
            string last = user.Surname.ToString().UppercaseFirstLetter();
            return first + " " + last;
        }
        //Get property value as named by string
        public static T GetPropertyValue<T>(this UserPrincipal user, string name)
        {
            var de = (DirectoryEntry)user.GetUnderlyingObject();
            return (T)de.Properties[name][0];
        }
        //Check if user is in specified group
        public static bool InGroup(this UserPrincipal user, string name)
        {
            bool isIn = false;
            //loop through all groups and add to base.Groups
            foreach (var group in user.GetGroups())
            {
                if (name == group.ToString())
                {
                    isIn = true;
                    break;
                }
            }
            return isIn;
        }

        //Search for user by login
        public static UserPrincipal SearchUser(this PrincipalContext context, string login)
        {
            var user = new UserPrincipal(context);
            user.SamAccountName = login;
            var search = new PrincipalSearcher();
            search.QueryFilter = user;
            return (UserPrincipal)search.FindOne();
        }
        //Search for user by employee id
        public static UserPrincipal SearchUser(this PrincipalContext context, int id)
        {
            var user = new UserPrincipal(context);
            user.EmployeeId = id.ToString();
            var search = new PrincipalSearcher();
            search.QueryFilter = user;
            return (UserPrincipal)search.FindOne();
        }


        //Uppercase first letter in string
        private static string UppercaseFirstLetter(this string val)
        {
            //check for empty string.
            if (string.IsNullOrEmpty(val))
            {
                return String.Empty;
            }
            //concat substring and return 
            return char.ToUpper(val[0]) + val.Substring(1);
        }
    }
}