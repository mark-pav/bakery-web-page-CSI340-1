using bakery_web_page_CSI340.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace bakery_web_page_CSI340.Controllers
{
    public class HomeController : Controller
    {
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;
        SqlConnection con = new SqlConnection();
        List<MenuItems> Menulist = new List<MenuItems>();
        List<ItemIngredients> IIList = new List<ItemIngredients>();

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            con.ConnectionString = bakery_web_page_CSI340.Properties.Resources.ConnectionString;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }
        
        public IActionResult OurMission()
        {
            return View();
        }

        public IActionResult Locations()
        {
            return View();
        }


        public IActionResult OverviewMenu()
        {
            FetchMenu();
            return View(Menulist);
        }

        public void FetchMenu()
        {
            if (Menulist.Count > 0)
            {
                Menulist.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT [MenuItemId],[Name],[Description],[Price],[NumberOfCalories],[IsVegan],[IsVegeterian] FROM [bakery-web-page-CSI340-DB].[dbo].[MenuItem]";
                //com.CommandText = "SELECT TOP (1000) [Name],[Description] FROM [bakery-web-page-CSI340-DB].[dbo].[MenuItem]";
                dr = com.ExecuteReader();

                while (dr.Read())
                {
                    string veganCheck = "Not Vegan";
                    if (dr["IsVegan"].ToString().Equals("True"))
                    {
                        veganCheck = "Vegan";
                    }
                    string vegeCheck = "Not Vegeterian";
                    if (dr["IsVegeterian"].ToString().Equals("True"))
                    {
                        vegeCheck = "Vegeterian";
                    }
                    Menulist.Add(new MenuItems()
                    {
                        MenuItemId = dr["MenuItemId"].ToString(),
                        Name = dr["Name"].ToString(),
                        Description = dr["Description"].ToString(),
                        Price = "$"+dr["Price"].ToString(),
                        NumberOfCalories = dr["NumberOfCalories"].ToString(),
                        IsVegan = veganCheck,
                        IsVegeterian = vegeCheck

                    });
                }

                con.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public IActionResult checkout()
        {
            return View();
        }

        public IActionResult Ordering()
        {
            return View();
        }

        public IActionResult SubMenu()
        {
            FetchII();
            return View(IIList);
        }

        public void FetchII()
        {
            if (IIList.Count > 0)
            {
                IIList.Clear();
            }
            try
            {
                //MenuItemId MenuItemName IngredientName IngredientType IsAllergen
                con.Open();
                com.Connection = con;
                com.CommandText =
                    "SELECT [bakery-web-page-CSI340-DB].[dbo].[MenuItem].MenuItemId as MenuItemId, [bakery-web-page-CSI340-DB].[dbo].[MenuItem].Name as Name, [bakery-web-page-CSI340-DB].[dbo].[Ingridients].IngredientName as IngredientName, [bakery-web-page-CSI340-DB].[dbo].[Ingridients].IngredientType as IngredientType, [bakery-web-page-CSI340-DB].[dbo].[Ingridients].IsAllergen as IsAllergen " +
                    "FROM [bakery-web-page-CSI340-DB].[dbo].[MenuItem] JOIN [bakery-web-page-CSI340-DB].[dbo].[ItemIngridients] ON [bakery-web-page-CSI340-DB].[dbo].[MenuItem].MenuItemId = [bakery-web-page-CSI340-DB].[dbo].[ItemIngridients].MenuItemId "+
                    "JOIN [bakery-web-page-CSI340-DB].[dbo].[Ingridients] ON [bakery-web-page-CSI340-DB].[dbo].[ItemIngridients].IngridientID = [bakery-web-page-CSI340-DB].[dbo].[Ingridients].IngridientID;";
                dr = com.ExecuteReader();

                while (dr.Read())
                {
                    string allergenCheck = " ";
                    if (dr["IsAllergen"].ToString().Equals("True"))
                    {
                        allergenCheck = "Allergen Warning";
                    }
                    
                    IIList.Add(new ItemIngredients()
                    {
                        MenuItemId = dr["MenuItemId"].ToString(),
                        MenuItemName = dr["Name"].ToString(),
                        IngredientName = dr["IngredientName"].ToString(),
                        IngredientType = dr["IngredientType"].ToString(),
                        IsAllergen = allergenCheck
                    });
                }

                con.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("ContactInfo");
            }
            
            MenuItems item = new MenuItems();
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT [MenuItemId],[Name],[Description],[Price],[NumberOfCalories],[IsVegan],[IsVegeterian] FROM [bakery-web-page-CSI340-DB].[dbo].[MenuItem] WHERE [MenuItemId]=" + id;
                dr = com.ExecuteReader();

                while (dr.Read())
                {
                    string veganCheck = "Not Vegan";
                    if (dr["IsVegan"].ToString().Equals("True"))
                    {
                        veganCheck = "Vegan";
                    }
                    string vegeCheck = "Not Vegeterian";
                    if (dr["IsVegeterian"].ToString().Equals("True"))
                    {
                        vegeCheck = "Vegeterian";
                    }
                    item.MenuItemId = dr["MenuItemId"].ToString();
                    item.Name = dr["Name"].ToString();
                    item.Description = dr["Description"].ToString();
                    item.Price = "$" + dr["Price"].ToString();
                    item.NumberOfCalories = dr["NumberOfCalories"].ToString();
                    item.IsVegan = veganCheck;
                    item.IsVegeterian = vegeCheck;
                    
                }

                con.Close();
                return View(item);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        [HttpPost]
        public async Task<IActionResult> Edit(MenuItems item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int veganCheck = 0;
                    if (item.IsVegan.Equals("Vegan"))
                    {
                        veganCheck = 1;
                    }
                    int vegeCheck = 0;
                    if (item.IsVegeterian.Equals("Vegeterian"))
                    {
                        vegeCheck = 1;
                    }
                    con.Open();
                    com.Connection = con;
                    //com.CommandText = "DELETE FROM [bakery-web-page-CSI340-DB].[dbo].[MenuItem] WHERE Id=" + item.MenuItemId;
                    com.CommandText = "UPDATE [bakery-web-page-CSI340-DB].[dbo].[MenuItem] SET [Name] = @name, [Description] = @desc, [Price] = @price, [NumberOfCalories] = @numCal, [IsVegan] = @vegan, [IsVegeterian] = @vege WHERE [MenuItemId] = @id";
                    com.Parameters.AddWithValue("@name", item.Name);
                    com.Parameters.AddWithValue("@desc", item.Description);
                    com.Parameters.AddWithValue("@price", item.Price);
                    com.Parameters.AddWithValue("@numCal", item.NumberOfCalories);
                    com.Parameters.AddWithValue("@vegan", veganCheck);
                    com.Parameters.AddWithValue("@vege", vegeCheck);
                    com.Parameters.AddWithValue("@id", item.MenuItemId);
                    com.ExecuteNonQuery();
                    Console.WriteLine("============================================================================");
                    con.Close();
                    return View(item);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            return View(item);

        }

        public IActionResult Create()
        {
            MenuItems menuItems = new MenuItems();
            return View(menuItems);
        }

        [HttpPost]
        public IActionResult Create(MenuItems item)
        {
            if (ModelState.IsValid)
            {
                /*
                con.Open();
                SqlCommand sqlCmd = new SqlCommand("CreateMenuItem", con);
                sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("MenuItemId", menuItems.MenuItemId);
                sqlCmd.Parameters.AddWithValue("Name", menuItems.Name);
                sqlCmd.Parameters.AddWithValue("Description", menuItems.Description);
                sqlCmd.Parameters.AddWithValue("Price", menuItems.Price);
                sqlCmd.Parameters.AddWithValue("NumberOfCalories", menuItems.NumberOfCalories);
                sqlCmd.Parameters.AddWithValue("IsVegan", menuItems.IsVegan);
                sqlCmd.Parameters.AddWithValue("IsVegeterian", menuItems.IsVegeterian);
                sqlCmd.ExecuteNonQuery();
                con.Close();*/

                try
                {
                    int veganCheck = 0;
                    if (item.IsVegan.Equals("Vegan"))
                    {
                        veganCheck = 1;
                    }
                    int vegeCheck = 0;
                    if (item.IsVegeterian.Equals("Vegeterian"))
                    {
                        vegeCheck = 1;
                    }

                    con.Open();
                    com.Connection = con;
                    //com.CommandText = "UPDATE [Id],[Name],[Description],[Price],[NumberOfCalories],[IsVegan],[IsVegeterian] FROM [bakery-web-page-CSI340-DB].[dbo].[MenuItem] WHERE Id=" + item.MenuItemId;
                    com.CommandText = "INSERT INTO [bakery-web-page-CSI340-DB].[dbo].[MenuItem] ([MenuItemId], [Name], [Description], [Price], [NumberOfCalories], [IsVegan], [IsVegeterian]) VALUES (@id, @name, @desc, @price, @numCal, @vegan, @vege)";
                    com.Parameters.AddWithValue("@name", item.Name);
                    com.Parameters.AddWithValue("@desc", item.Description);
                    com.Parameters.AddWithValue("@price", item.Price);
                    com.Parameters.AddWithValue("@numCal", item.NumberOfCalories);
                    com.Parameters.AddWithValue("@vegan", veganCheck);
                    com.Parameters.AddWithValue("@vege", vegeCheck);
                    com.Parameters.AddWithValue("@id", item.MenuItemId);

                    com.ExecuteNonQuery();

                    con.Close();

                    return RedirectToAction("OverviewMenu");
                }
                catch (Exception ex)
                {

                    throw ex;
                }

               

            }
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(MenuItems item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //int veganCheck = 0;
                    //if (item.IsVegan.Equals("Vegan"))
                    //{
                    //    veganCheck = 1;
                    //}
                    //int vegeCheck = 0;
                    //if (item.IsVegeterian.Equals("Vegeterian"))
                    //{
                    //    vegeCheck = 1;
                    //}
                    con.Open();
                    com.Connection = con;

                    com.CommandText = "DELETE FROM [bakery-web-page-CSI340-DB].[dbo].[MenuItem] WHERE [MenuItemId] = @id";

                    com.Parameters.AddWithValue("@id", item.MenuItemId);

                    Console.WriteLine(com.CommandText);
                    Console.WriteLine("//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////");
                    com.ExecuteNonQuery();

                    con.Close();
                    return View(item);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            return View(item);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
