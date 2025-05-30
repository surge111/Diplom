using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.IO;

namespace Course
{
    static class Connection
    {
        static private string connStr = $"host={ConfigurationManager.AppSettings["host"]};" +
            $"user={ConfigurationManager.AppSettings["user"]};" +
            $"pwd={ConfigurationManager.AppSettings["pwd"]};";
        static private MySqlConnection conn = new MySqlConnection(connStr);
        static public bool Open()
        {
            if (conn.State == ConnectionState.Closed)
            {
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return true;
            }
            return false;
        }
        static public void ChangeDb(string db)
        {
            try
            {
                var opened = Open();
                conn.ChangeDatabase(db);
                Close(opened);
            }
            catch (Exception ex)
            {
                conn.ChangeDatabase("");
                throw ex;
            }
        }
        static public void Update(string host, string user, string pwd)
        {
            connStr = $"host={host};" +
            $"user={user};" +
            $"pwd={pwd};";
            conn = new MySqlConnection(connStr);
        }
        static public bool Test()
        {
            try
            {
                var opened = Open();
                Close(opened);
                return true;
            }
            catch
            {
                return false;
            }
        }
        static public void Close(bool opened)
        {
            if (conn.State != ConnectionState.Closed && opened)
            {
                conn.Close();
            }
        }
        static public DataTable GetUser(string login, string password)
        {
            login = login.Replace("\"", "\\\"");
            var a = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password));
            password = BitConverter.ToString(a).Replace("-", string.Empty).ToLower();
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var cmd = new MySqlCommand($"select * from `user` inner join worker inner join role on WorkerId=UserWorkerId and RoleId=UserRoleId where UserLogin=\"{login}\" and UserPassword=\"{password}\"", conn);
            var da = new MySqlDataAdapter(cmd);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Connection.Close(opened);
                throw ex;
            }
            Connection.Close(opened);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        static public string[] GetCategories()
        {
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var cmd = new MySqlCommand("select CategoryName from category", conn);
            var da = new MySqlDataAdapter(cmd);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Connection.Close(opened);
                throw ex;
            }
            var dt = new DataTable();
            da.Fill(dt);
            Connection.Close(opened);
            var categories = new string[] {};
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                categories = categories.Append(dt.Rows[i].ItemArray[0].ToString()).ToArray();
            }
            return categories;
        }
        static public string[] GetSuppliers()
        {
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var cmd = new MySqlCommand("select SupplierName from supplier", conn);
            var da = new MySqlDataAdapter(cmd);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Connection.Close(opened);
                throw ex;
            }
            var dt = new DataTable();
            da.Fill(dt);
            Connection.Close(opened);
            var suppliers = new string[] { };
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                suppliers = suppliers.Append(dt.Rows[i].ItemArray[0].ToString()).ToArray();
            }
            return suppliers;
        }
        static public KeyValuePair<string, string>[] GetWorkers()
        {
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var cmd = new MySqlCommand("select WorkerId, WorkerSurname, WorkerName, WorkerPatronymic from worker", conn);
            var da = new MySqlDataAdapter(cmd);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Connection.Close(opened);
                throw ex;
            }
            var dt = new DataTable();
            da.Fill(dt);
            Connection.Close(opened);
            var workers = new KeyValuePair<string, string>[] { };
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                workers = workers.Append(new KeyValuePair<string, string>(dt.Rows[i].ItemArray[0].ToString(), $"{dt.Rows[i].ItemArray[1].ToString()} {dt.Rows[i].ItemArray[2].ToString()[0]}.{dt.Rows[i].ItemArray[3].ToString()[0]}.")).ToArray();
            }
            return workers;
        }
        static public KeyValuePair<string, string>[] GetClients()
        {
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var cmd = new MySqlCommand("select ClientId, ClientSurname, ClientName, ClientPatronymic from client", conn);
            var da = new MySqlDataAdapter(cmd);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Connection.Close(opened);
                throw ex;
            }
            var dt = new DataTable();
            da.Fill(dt);
            Connection.Close(opened);
            var clients = new KeyValuePair<string, string>[] { };
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                clients = clients.Append(new KeyValuePair<string, string>(dt.Rows[i].ItemArray[0].ToString(), $"{dt.Rows[i].ItemArray[1].ToString()} {dt.Rows[i].ItemArray[2].ToString()[0]}.{dt.Rows[i].ItemArray[3].ToString()[0]}.")).ToArray();
            }
            return clients;
        }
        static public string[] GetRoles()
        {
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var cmd = new MySqlCommand("select RoleName from role", conn);
            var da = new MySqlDataAdapter(cmd);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Connection.Close(opened);
                throw ex;
            }
            var dt = new DataTable();
            da.Fill(dt);
            Connection.Close(opened);
            var roles = new string[] { };
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                roles = roles.Append(dt.Rows[i].ItemArray[0].ToString()).ToArray();
            }
            return roles;
        }
        static public Dictionary<string, string> GetProductById(string id)
        {
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var cmd = new MySqlCommand($"select * from product where ProductId=\"{id.Replace("\"", "\\\"")}\"", conn);
            var da = new MySqlDataAdapter(cmd);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Connection.Close(opened);
                throw ex;
            }
            var dt = new DataTable();
            da.Fill(dt);
            Connection.Close(opened);
            var product = new Dictionary<string, string>();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                product.Add(dt.Columns[i].ColumnName, dt.Rows[0].ItemArray[i].ToString());
            }
            return product;
        }
        static public string GetCategoryById(string id)
        {
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var cmd = new MySqlCommand($"select CategoryName from category where CategoryId=\"{id.Replace("\"", "\\\"")}\"", conn);
            var da = new MySqlDataAdapter(cmd);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Connection.Close(opened);
                throw ex;
            }
            var dt = new DataTable();
            da.Fill(dt);
            Connection.Close(opened);
            var category = dt.Rows[0].ItemArray[0].ToString();
            return category;
        }
        static public string GetSupplierById(string id)
        {
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var cmd = new MySqlCommand($"select SupplierName from supplier where SupplierId=\"{id.Replace("\"", "\\\"")}\"", conn);
            var da = new MySqlDataAdapter(cmd);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Connection.Close(opened);
                throw ex;
            }
            var dt = new DataTable();
            da.Fill(dt);
            Connection.Close(opened);
            var supplier = dt.Rows[0].ItemArray[0].ToString();
            return supplier;
        }
        static public string GetWorkerById(string id)
        {
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var cmd = new MySqlCommand($"select WorkerSurname, WorkerName, WorkerPatronymic from worker where WorkerId=\"{id.Replace("\"", "\\\"")}\"", conn);
            var da = new MySqlDataAdapter(cmd);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Connection.Close(opened);
                throw ex;
            }
            var dt = new DataTable();
            da.Fill(dt);
            Connection.Close(opened);
            var worker = dt.Rows[0].ItemArray[0].ToString() + " " + dt.Rows[0].ItemArray[1].ToString()[0] + "." + dt.Rows[0].ItemArray[2].ToString()[0] + ".";
            return worker;
        }
        static public string GetRoleById(string id)
        {
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var cmd = new MySqlCommand($"select RoleName from role where RoleId=\"{id.Replace("\"", "\\\"")}\"", conn);
            var da = new MySqlDataAdapter(cmd);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Connection.Close(opened);
                throw ex;
            }
            var dt = new DataTable();
            da.Fill(dt);
            Connection.Close(opened);
            var role = dt.Rows[0].ItemArray[0].ToString();
            return role;
        }
        static public string GetCategoryId(string category)
        {
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var cmd = new MySqlCommand($"select CategoryId from category where CategoryName=\"{category.Replace("\"", "\\\"")}\"", conn);
            var da = new MySqlDataAdapter(cmd);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Connection.Close(opened);
                throw ex;
            }
            var dt = new DataTable();
            da.Fill(dt);
            Connection.Close(opened);
            var id = dt.Rows[0].ItemArray[0].ToString();
            return id;
        }
        static public string GetSupplierId(string supplier)
        {
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var cmd = new MySqlCommand($"select SupplierId from supplier where SupplierName=\"{supplier.Replace("\"", "\\\"")}\"", conn);
            var da = new MySqlDataAdapter(cmd);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Connection.Close(opened);
                throw ex;
            }
            var dt = new DataTable();
            da.Fill(dt);
            Connection.Close(opened);
            var id = dt.Rows[0].ItemArray[0].ToString();
            return id;
        }
        static public string GetRoleId(string role)
        {
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var cmd = new MySqlCommand($"select RoleId from role where RoleName=\"{role.Replace("\"", "\\\"")}\"", conn);
            var da = new MySqlDataAdapter(cmd);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Connection.Close(opened);
                throw ex;
            }
            var dt = new DataTable();
            da.Fill(dt);
            Connection.Close(opened);
            var id = dt.Rows[0].ItemArray[0].ToString();
            return id;
        }
        static public DataTable SelectTable(string tableName, 
            string search = "", 
            string filter = "", 
            string sort = "", 
            string sortMode = "asc",
            int page = -1)
        {
            var cmdText = $"select * from `{tableName}`";
            //joining fk-s
            switch (tableName)
            {
                case "order":
                    {
                        cmdText += "inner join worker " +
                            "on WorkerId=OrderWorkerId";
                        break;
                    }
                case "orderitem":
                    {
                        cmdText += " inner join `order` " +
                            "inner join `product` " +
                            "inner join `worker` " +
                            "on OrderId=OrderItemOrderId " +
                            "and ProductId=OrderItemProductId " +
                            "and OrderWorkerId=WorkerId";
                        break;
                    }
                case "product":
                    {
                        cmdText += " inner join category " +
                            "inner join supplier " +
                            "on ProductCategoryId=CategoryId " +
                            "and ProductSupplierId=SupplierId";
                        break;
                    }
                case "user":
                    {
                        cmdText += " inner join worker " +
                            "inner join role " +
                            "on WorkerId=UserWorkerId " +
                            "and RoleId=UserRoleId";
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            if (search != "")
            {
                cmdText += " where";
                //get search column
                switch (tableName)
                {
                    case "user":
                        {
                            cmdText += " UserLogin";
                            break;
                        }
                    case "worker":
                        {
                            cmdText += " WorkerSurname";
                            break;
                        }
                    default:
                        {
                            cmdText += $" {tableName[0].ToString().ToUpper() + tableName.Substring(1)}Name";
                            break;
                        }
                }
                cmdText += $" like \"%{search}%\"";
            }
            if (filter != "")
            {
                cmdText += search != "" ? " and" : " where";
                cmdText += Connection.GetFilter(filter);
            }
            if (sort != "")
            {
                cmdText += " order by";
                switch (sort)
                {
                    case "Названию":
                        {
                            cmdText += " ProductName";
                            break;
                        }
                    case "Стоимости":
                        {
                            cmdText += " ProductCost";
                            break;
                        }
                }
                cmdText += " " + sortMode;
            }
            else if (tableName == "product")
            {
                cmdText += " order by ProductId";
            }
            else if (tableName == "user")
            {
                cmdText += " order by UserId";
            }
            if (page != -1)
            {
                cmdText += $" limit {(page - 1) * 20}, 20";
            }
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var cmd = new MySqlCommand(cmdText, conn);
            var da = new MySqlDataAdapter(cmd);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Connection.Close(opened);
                throw ex;
            }
            Connection.Close(opened);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        static public int SelectTableLength(string tableName,
                    string search = "",
                    string filter = "",
                    string sort = "",
                    string sortMode = "asc")
        {
            var cmdText = $"select count(*) from `{tableName}`";
            //joining fk-s
            switch (tableName)
            {
                case "order":
                    {
                        cmdText += "inner join worker " +
                            "on WorkerId=OrderWorkerId";
                        break;
                    }
                case "orderitem":
                    {
                        cmdText += " inner join `order` " +
                            "inner join `product` " +
                            "inner join `worker` " +
                            "on OrderId=OrderItemOrderId " +
                            "and ProductId=OrderItemProductId " +
                            "and OrderWorkerId=WorkerId";
                        break;
                    }
                case "product":
                    {
                        cmdText += " inner join category " +
                            "inner join supplier " +
                            "on ProductCategoryId=CategoryId " +
                            "and ProductSupplierId=SupplierId";
                        break;
                    }
                case "user":
                    {
                        cmdText += " inner join worker " +
                            "inner join role " +
                            "on WorkerId=UserWorkerId " +
                            "and RoleId=UserRoleId";
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            if (search != "")
            {
                cmdText += " where";
                //get search column
                switch (tableName)
                {
                    case "user":
                        {
                            cmdText += " UserLogin";
                            break;
                        }
                    case "worker":
                        {
                            cmdText += " WorkerSurname";
                            break;
                        }
                    default:
                        {
                            cmdText += $" {tableName[0].ToString().ToUpper() + tableName.Substring(1)}Name";
                            break;
                        }
                }
                cmdText += $" like \"%{search}%\"";
            }
            if (filter != "")
            {
                cmdText += search != "" ? " and" : " where";
                cmdText += Connection.GetFilter(filter);
            }
            if (sort != "")
            {
                cmdText += " order by";
                switch (sort)
                {
                    case "Названию":
                        {
                            cmdText += " ProductName";
                            break;
                        }
                    case "Стоимости":
                        {
                            cmdText += " ProductCost";
                            break;
                        }
                }
                cmdText += " " + sortMode;
            }
            else if (tableName == "product")
            {
                cmdText += " order by ProductId";
            }
            else if (tableName == "user")
            {
                cmdText += " order by UserId";
            }
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var cmd = new MySqlCommand(cmdText, conn);
            try
            {
                int totalPages = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                Connection.Close(opened);
                return totalPages;
            }
            catch
            {
                Connection.Close(opened);
                return 1;
            }
        }
        static private string GetFilter(string condition)
        {
            var filter = "";
            switch (condition.Substring(0, condition.IndexOf(' ') - 1))
            {
                case "Категория":
                    {
                        filter += $" CategoryName = \"{condition.Substring(condition.IndexOf(' ') + 1)}\"";
                        break;
                    }
                case "Поставщик":
                    {
                        filter += $" SupplierName = \"{condition.Substring(condition.IndexOf(' ') + 1)}\"";
                        break;
                    }
            }
            return filter;
        }
        static private string GetTableIdColumn(string tableName)
        {
            return tableName[0].ToString().ToUpper() + tableName.Substring(1) + "Id";
        }
        static public bool InsertObject(string tableName, Dictionary<string, string> obj)
        {
            foreach (string key in obj.Keys.ToArray())
            {
                if (key == GetTableIdColumn(tableName))
                {
                    continue;
                }
                else
                {
                    obj[key] = $"\"{obj[key].Replace("\"", "\\\"")}\"";
                }
            }
            var cmdStr = $"insert into `{tableName}`({GetTableIdColumn(tableName)}, {string.Join(", ", obj.Keys)}) values (null, {string.Join(", ", obj.Values)})";
            if (tableName == "orderitem")
            {
                cmdStr = $"insert into `{tableName}`({string.Join(", ", obj.Keys)}) values ({string.Join(", ", obj.Values)})";
            }
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var tr = conn.BeginTransaction();
            var cmd = new MySqlCommand(cmdStr, conn, tr);
            var res = 0;
            try
            {
                res = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                tr.Rollback();
                Connection.Close(opened);
                throw ex;
            }
            if (res != 1)
            {
                tr.Rollback();
            }
            else
            {
                tr.Commit();
            }
            Connection.Close(opened);
            return res == 1;
        }
        static public bool UpdateObject(string tableName, Dictionary<string, string> obj)
        {
            var pairs = new string[] { };
            foreach (string key in obj.Keys)
            {
                pairs = pairs.Append($"{key}=\"{obj[key].Replace("\"", "\\\"")}\"").ToArray();
            }
            var cmdStr = "";
            if (tableName == "orderitem")
            {
                cmdStr = $"update `{tableName}` set {string.Join(", ", pairs)} where OrderItemOrderId={obj["OrderItemOrderId"]} and OrderItemProductId={obj["OrderItemProductId"]}";
            }
            else
            {
                cmdStr = $"update `{tableName}` set {string.Join(", ", pairs)} where {GetTableIdColumn(tableName)}={obj[GetTableIdColumn(tableName)]}";
            }
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var tr = conn.BeginTransaction();
            var cmd = new MySqlCommand(cmdStr, conn, tr);
            var res = 0;
            try
            {
                res = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                tr.Rollback();
                Connection.Close(opened);
                throw ex;
            }
            if (res < 1)
            {
                tr.Rollback();
            }
            else
            {
                tr.Commit();
            }
            Connection.Close(opened);
            return res >= 1;
        }
        static public bool UpdateObject(string tableName, Dictionary<string, string> obj, MySqlTransaction tr)
        {
            var pairs = new string[] { };
            foreach (string key in obj.Keys)
            {
                pairs = pairs.Append($"{key}=\"{obj[key].Replace("\"", "\\\"")}\"").ToArray();
            }
            var cmdStr = $"update `{tableName}` set {string.Join(", ", pairs)} where {GetTableIdColumn(tableName)}={obj[GetTableIdColumn(tableName)]}";
            if (tableName == "orderitem")
            {
                cmdStr = $"update `{tableName}` set {string.Join(", ", pairs)} where OrderItemOrderId={obj["OrderItemOrderId"]} and OrderItemProductId={obj["OrderItemProductId"]}";
            }
            var cmd = new MySqlCommand(cmdStr, conn, tr);
            var res = 0;
            try
            {
                res = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return res >= 1;
        }
        static public bool DeleteObject(string tableName, string id)
        {
            var cmdStr = $"delete from `{tableName}` where {GetTableIdColumn(tableName)}=\"{id.Replace("\"", "\\\"")}\"";
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var tr = conn.BeginTransaction();
            var cmd = new MySqlCommand(cmdStr, conn, tr);
            int res;
            try
            {
                res = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                tr.Rollback();
                Connection.Close(opened);
                throw ex;
            }
            if (res < 1)
            {
                tr.Rollback();
            }
            else
            {
                tr.Commit();
            }
            Connection.Close(opened);
            return res == 1;
        }
        static public DataTable GetOrderItems(string orderId)
        {
            var cmdText = "select * from `orderitem`";
            cmdText += " inner join `product` on ProductId=OrderItemProductId";
            cmdText += $" where OrderItemOrderId=\"{orderId}\"";
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var cmd = new MySqlCommand(cmdText, conn);
            var da = new MySqlDataAdapter(cmd);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Connection.Close(opened);
                throw ex;
            }
            Connection.Close(opened);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        static public string GetLastOrderId()
        {
            var cmdText = "select max(OrderId) from `order`";
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var cmd = new MySqlCommand(cmdText, conn);
            var da = new MySqlDataAdapter(cmd);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Connection.Close(opened);
                throw ex;
            }
            Connection.Close(opened);
            var dt = new DataTable();
            da.Fill(dt);
            return dt.Rows[0].ItemArray[0].ToString();
        }
        static public void AddItemToOrder(string orderId, Dictionary<string, string> item)
        {
            var cmdText = $"select OrderItemQuantity from `orderitem` where OrderItemOrderId=\"{orderId}\" and OrderItemProductId=\"{item["OrderItemProductId"]}\"";
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var cmd = new MySqlCommand(cmdText, conn);
            var da = new MySqlDataAdapter(cmd);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Connection.Close(opened);
                throw ex;
            }
            Connection.Close(opened);
            var dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                item.Add("OrderItemOrderId", orderId);
                item["OrderItemQuantity"] = (Convert.ToInt32(item["OrderItemQuantity"]) + Convert.ToInt32(dt.Rows[0].ItemArray[0].ToString())).ToString();
                item.Remove("OrderItemCost");
                try
                {
                    Connection.UpdateObject("orderitem", item);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                item.Add("OrderItemOrderId", orderId);
                try
                {
                    Connection.InsertObject("orderitem", item);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        static public void DeleteOrderItem(string orderId, string productId)
        {
            var cmdStr = $"delete from `orderitem` where OrderItemOrderId=\"{orderId.Replace("\"", "\\\"")}\" and OrderItemProductId=\"{productId.Replace("\"", "\\\"")}\"";
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var tr = conn.BeginTransaction();
            var cmd = new MySqlCommand(cmdStr, conn, tr);
            int res;
            try
            {
                res = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                tr.Rollback();
                Connection.Close(opened);
                throw ex;
            }
            if (res < 1)
            {
                tr.Rollback();
            }
            else
            {
                tr.Commit();
            }
            Connection.Close(opened);
        }
        static public bool ConfirmOrder(Dictionary<string, string> order)
        {
            var orderId = order["OrderId"];
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var tr = conn.BeginTransaction();
            DataTable items;
            try
            {
                items = GetOrderItems(orderId);
            }
            catch (Exception ex)
            {
                tr.Rollback();
                Close(opened);
                throw ex;
            }
            for (int i = 0; i < items.Rows.Count; i++)
            {
                Dictionary<string, string> product;
                try
                {
                    product = Connection.GetProductById(items.Rows[i].ItemArray[items.Columns["OrderItemProductId"].Ordinal].ToString());
                }
                catch (Exception ex)
                {
                    tr.Rollback();
                    Close(opened); 
                    throw ex;
                }
                if (Convert.ToInt32(product["ProductQuantity"]) < Convert.ToInt32(items.Rows[i].ItemArray[items.Columns["OrderItemQuantity"].Ordinal].ToString()))
                {
                    tr.Rollback();
                    Close(opened);
                    return false;
                }
                product["ProductQuantity"] = (Convert.ToInt32(product["ProductQuantity"]) - Convert.ToInt32(items.Rows[i].ItemArray[items.Columns["OrderItemQuantity"].Ordinal].ToString())).ToString();
                product["ProductExpirationDate"] = DateTime.Parse(product["ProductExpirationDate"]).ToString("yyyy-MM-dd");
                try
                {
                    Connection.UpdateObject("product", product, tr);
                }
                catch (Exception ex)
                {
                    tr.Rollback();
                    Close(opened);
                    throw ex;
                }
            }
            try
            {
                Connection.UpdateObject("order", order, tr);
            }
            catch (Exception ex)
            {
                tr.Rollback();
                Close(opened);
                throw ex;
            }
            tr.Commit();
            Close(opened);
            return true;
        }
        static public string GetProductRevenue(string productId, DateTime dateFrom, DateTime dateTo)
        {
            var cmdText = $"SELECT sum(OrderItemCost * OrderItemQuantity) FROM db35.orderitem inner join `order` on OrderId = OrderItemOrderId where OrderItemProductId = \"{productId}\" and OrderDate >= \"{dateFrom.ToString("yyyy-MM-dd")}\" and OrderDate <= \"{dateTo.ToString("yyyy-MM-dd")}\"";
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var cmd = new MySqlCommand(cmdText, conn);
            var da = new MySqlDataAdapter(cmd);
            string res;
            try
            {
                res = cmd.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                Connection.Close(opened);
                throw ex;
            }
            Connection.Close(opened);
            var dt = new DataTable();
            da.Fill(dt);
            try
            {
                var sum = Convert.ToDouble(res);
                return Math.Round(sum, 2, MidpointRounding.AwayFromZero).ToString();
            }
            catch
            {
                return "0";
            }
        }
        static public DataTable GetOrderItemsForReport(string orderId)
        {
            var cmdText = $"select ProductName, OrderItemQuantity, OrderItemCost, (OrderItemQuantity * OrderItemCost) as TotalCost" +
                $" from orderitem inner join product on OrderItemProductId = ProductId " +
                $"where OrderItemOrderId = \"{orderId}\"";
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var cmd = new MySqlCommand(cmdText, conn);
            var da = new MySqlDataAdapter(cmd);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Close(opened);
                throw ex;
            }
            Close(opened);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
        static public string GetOrderTotalCost(string orderId)
        {
            var cmdText = $"select sum(OrderItemQuantity * OrderItemCost) as TotalCost from orderitem where OrderItemOrderId = \"{orderId}\"";
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var cmd = new MySqlCommand(cmdText, conn);
            var da = new MySqlDataAdapter(cmd);
            string res;
            try
            {
                res = cmd.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                Close(opened);
                throw ex;
            }
            Close(opened);
            var dt = new DataTable();
            da.Fill(dt);
            try
            {
                var sum = Convert.ToDouble(res);
                return Math.Round(sum, 2, MidpointRounding.AwayFromZero).ToString();
            }
            catch
            {
                return "0";
            }
        }
        static public void RecoverDb()
        {
            var cmdStr = Properties.Resources._struct;
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var tr = conn.BeginTransaction();
            var cmd = new MySqlCommand(cmdStr, conn, tr);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                tr.Rollback();
                Close(opened);
                throw ex;
            }
            tr.Commit();
            Close(opened);
        }
        public static string[] GetColumns(string tableName)
        {
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var cmd = new MySqlCommand($"select `COLUMN_NAME` " +
                    $"from information_schema.columns " +
                    $"where table_name=\"{tableName}\" and table_schema=\"{ConfigurationManager.AppSettings["db"]}\"", conn);
            var da = new MySqlDataAdapter(cmd);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Close(opened);
                throw ex;
            }
            var dt = new DataTable();
            da.Fill(dt);
            Connection.Close(opened);
            var columns = new string[] { };
            foreach (DataRow r in dt.Rows)
            {
                columns = columns.Append(r.ItemArray[0].ToString()).ToArray();
            }
            return columns;
        }
        public static string[] GetTables()
        {
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var cmd = new MySqlCommand($"show tables from {ConfigurationManager.AppSettings["db"]}", conn);
            var da = new MySqlDataAdapter(cmd);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Close(opened);
                throw ex;
            }
            var dt = new DataTable();
            da.Fill(dt);
            Close(opened);
            var tables = new string[] { };
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                tables = tables.Append(dt.Rows[i].ItemArray[0].ToString()).ToArray();
            }
            return tables;
        }
        public static int ImportData(string[] data, string tableName)
        {
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var tr = conn.BeginTransaction();
            var columns = new string[] { };
            try
            {
                columns = Connection.GetColumns(tableName);
            }
            catch (Exception ex)
            {
                Close(opened);
                throw ex;
            }
            for (var j = 0; j < columns.Length; j++)
            {
                columns[j] = $"`{columns[j]}`";
            }
            for (var i = 0; i < data.Length; i++)
            {
                var item = data[i];
                var values = item.Split(';');
                if (!(columns.Length == values.Length))
                {
                    tr.Rollback();
                    Close(opened);
                    return i;
                }
                for (var j = 0; j < values.Length; j++)
                {
                    values[j] = values[j] == "" ? "null" : "\"" + values[j].Replace("\"", "\\\"") + "\"";
                }
                try
                {
                    var cmd = new MySqlCommand($"insert into `{tableName}`({string.Join(", ", columns)}) values ({string.Join(", ", values)})", conn, tr);
                    var res = cmd.ExecuteNonQuery();
                    if (res != 1)
                    {
                        tr.Rollback();
                        Close(opened);
                        return i;
                    }
                }
                catch (Exception ex)
                {
                    tr.Rollback();
                    Close(opened);
                    throw ex;
                }
            }
            tr.Commit();
            conn.Close();
            return -1;
        }
        public static void ExportData(FileStream file, string tableName)
        {
            bool opened;
            try
            {
                opened = Connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var columns = new string[] { };
            try
            {
                columns = Connection.GetColumns(tableName);
            }
            catch (Exception ex)
            {
                Close(opened);
                throw ex;
            }
            var cmd = new MySqlCommand($"select {string.Join(", ", columns)} from `{tableName}`", conn);
            //var rdr = cmd.ExecuteReader();
            
            using (StreamWriter sw = new StreamWriter(file))
            {
                sw.WriteLine(string.Join(";", columns));
                using (var rdr = cmd.ExecuteReader())
                {
                    //var arr = new string[] { };
                    //foreach (var col in columns)
                    //{
                    //    arr = arr.Append(rdr[col].ToString()).ToArray();
                    //}
                    //sw.WriteLine(string.Join(";", arr));
                    while (rdr.Read())
                    {
                        var arr = new string[] { };
                        foreach (var col in columns)
                        {
                            arr = arr.Append(rdr[col].ToString()).ToArray();
                        }
                        sw.WriteLine(string.Join(";", arr));
                    }
                }
            }
            Close(opened);
        }
    }
}
