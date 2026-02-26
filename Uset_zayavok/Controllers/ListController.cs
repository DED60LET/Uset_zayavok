using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;

public class ListController : Controller
{
  

    public DataTable GetRequests()
    {
        DataTable dt = new DataTable();
        using (NpgsqlConnection conn = new NpgsqlConnection(connString)) // Передаем строку сюда
        {
            conn.Open();
            string sql = "SELECT * FROM Requests";
            using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
            {
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    dt.Load(reader);
                }
            }
        }
        return dt;
    }

    public IActionResult Index()
    {
        var data = GetRequests();
        return View(data);
    }
}