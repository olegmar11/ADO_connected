using System;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Threading.Tasks.Dataflow;
using System.Web;
using System.Xml.Linq;

namespace ConnectedADO
{
    class EstablishingConnection
    {
        public static SqlConnection con;
        public static SqlCommand cmd;
        public static Dictionary<string, SqlDataAdapter> dataAdapters = new Dictionary<string, SqlDataAdapter>();
        public static DataSet hSet = new DataSet("Hospital");
        public EstablishingConnection()
        {
            con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ToString());
            try
            {
                con.Open();
                Console.WriteLine("Connection Established\n");
                settingAdapters();
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            con.Close();
            Menu_main();
        }
        public static void settingAdapters()
        {
            SqlCommand cmd1 = con.CreateCommand();
            cmd1.CommandType = CommandType.Text; cmd1.CommandText = "SELECT * FROM Equipment";
            dataAdapters.Add("Equipment", new SqlDataAdapter(cmd1));
            SqlCommandBuilder bldr1 = new SqlCommandBuilder(dataAdapters["Equipment"]);
            bldr1.GetUpdateCommand();

            SqlCommand cmd2 = con.CreateCommand();
            cmd2.CommandType = CommandType.Text; cmd2.CommandText = "SELECT * FROM Medicine";
            dataAdapters.Add("Medicine", new SqlDataAdapter(cmd2));
            SqlCommandBuilder bldr2 = new SqlCommandBuilder(dataAdapters["Medicine"]);
            bldr2.GetUpdateCommand();

            SqlCommand cmd3 = con.CreateCommand();
            cmd3.CommandType = CommandType.Text; cmd3.CommandText = "SELECT * FROM Patient";
            dataAdapters.Add("Patient", new SqlDataAdapter(cmd3));
            SqlCommandBuilder bldr3 = new SqlCommandBuilder(dataAdapters["Patient"]);
            bldr3.GetUpdateCommand();

            SqlCommand cmd4 = con.CreateCommand();
            cmd4.CommandType = CommandType.Text; cmd4.CommandText = "SELECT * FROM Staff";
            dataAdapters.Add("Staff", new SqlDataAdapter(cmd4));
            SqlCommandBuilder bldr4 = new SqlCommandBuilder(dataAdapters["Staff"]);
            bldr4.GetUpdateCommand();

            SqlCommand cmd5 = con.CreateCommand();
            cmd5.CommandType = CommandType.Text; cmd5.CommandText = "SELECT * FROM Proced";
            dataAdapters.Add("Proced", new SqlDataAdapter(cmd5));
            SqlCommandBuilder bldr5 = new SqlCommandBuilder(dataAdapters["Proced"]);
            bldr5.GetUpdateCommand();
        }
        public static void Display(string tableName)
        {
            Console.WriteLine();
            con.Open();
            string command = "select * from " + tableName;
            if (tableName == "Proced")
            {
                command = "SELECT Proced.IDproc, Proced.Name, Proced.Price, Patient.First_name, Patient.Last_name, \r\n\tEquipment.Name, Staff.Name, Staff.Last_Name, Medicine.Name FROM Proced \r\n\tINNER JOIN Patient ON Proced.Patient_ID = Patient.IDpat\r\n\tINNER JOIN Equipment ON Proced.Equipment_ID = Equipment.IDeq\r\n\tINNER JOIN Staff ON Proced.Staff_ID = Staff.IDstaff\r\n\tINNER JOIN Medicine ON Proced.Medicine_ID = Medicine.IDmed;";
            }
            cmd = new SqlCommand(command, con);
            DbDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    if (dr[i] is DBNull) Console.Write("--- ");
                    else Console.Write(dr[i] + " ");
                }
                Console.WriteLine();
            }

            dr.Close();
            con.Close();
            Console.WriteLine();
        }
        public static void AlterDataTables(string tableName, string operation)
        {
            Console.Clear();
            switch (tableName)
            {
                case "dbo.Equipment":
                    string name = "";
                    string manuf = "";
                    string command = "";
                    int rowid = 0;
                    int quant = 0;
                    if(operation == "add")
                    {
                        Console.WriteLine("Enter Name,Quantity and manufacturer");
                        name = Console.ReadLine();
                        quant = Convert.ToInt32(Console.ReadLine());
                        manuf = Console.ReadLine();
                        command = string.Format("insert into " + tableName + " values(N'{0}',{1},N'{2}');", name, quant, manuf);
                        cmd = new SqlCommand(command, con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    if(operation == "alter")
                    {
                        con.Open();
                        Console.WriteLine("Введіть id рядка, що буде змінений");
                        rowid = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Enter Name,Quantity and manufacturer");
                        name = Console.ReadLine();
                        quant = Convert.ToInt32(Console.ReadLine());
                        manuf = Console.ReadLine();
                        command = string.Format("Update Equipment SET Name = N'{0}', Quantity = {1}, Manufacturer = N'{2}' where IDeq = {3};", name, quant, manuf, rowid);
                        cmd = new SqlCommand(command, con);
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    if(operation == "delete")
                    {
                        con.Open();
                        Console.WriteLine("Введіть id рядка, що буде видалений");
                        rowid = Convert.ToInt32(Console.ReadLine());
                        command = string.Format("DELETE FROM Equipment WHERE IDeq = {0};",rowid);
                        cmd = new SqlCommand(command, con);
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    break;
                case "dbo.Medicine":
                    string crdate = "";
                    string expdate = "";
                    if(operation == "add")
                    {
                        Console.WriteLine("Введіть Назву, Виробника і Дати виготовлення/Термін придатності у форматі рік-місяць-число ");
                        name = Console.ReadLine();
                        manuf = Console.ReadLine();
                        crdate = Console.ReadLine();
                        expdate = Console.ReadLine();
                        command = string.Format("insert into " + tableName + " values(N'{0}',N'{1}','{2}','{3}');", name, manuf, crdate, expdate);
                        cmd = new SqlCommand(command, con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    if(operation == "alter")
                    {
                        Console.WriteLine("Введіть id рядка, що буде змінений");
                        rowid = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Введіть Назву, Виробника і Дати виготовлення/Термін придатності у форматі рік-місяць-число ");
                        name = Console.ReadLine();
                        manuf = Console.ReadLine();
                        crdate = Console.ReadLine();
                        expdate = Console.ReadLine();
                        command = string.Format("update " + tableName + "SET Name = N'{0}', Producer = N'{1}', Created_Date = '{2}', Expiration_Date = '{3}' where IDmed = {4});", name, manuf, crdate, expdate,rowid);
                        cmd = new SqlCommand(command, con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    if(operation == "delete")
                    {
                        con.Open();
                        Console.WriteLine("Введіть id рядка, що буде видалений");
                        rowid = Convert.ToInt32(Console.ReadLine());
                        command = string.Format("DELETE FROM Medicine WHERE IDmed = {0};", rowid);
                        cmd = new SqlCommand(command, con);
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    break;
                case "dbo.Patient":
                    string surname = "";
                    string middlename = "";
                    int age = 0;
                    string disease = "";
                    string ardate = "";
                    string disdate = "";
                    string ward = "";
                    if (operation == "add")
                    {
                        Console.WriteLine("Введіть ПІБ, Вік, Хворобу, Дати прибуття/виписки у форматі рік-місяць-число, а також відділення ");
                        surname = Console.ReadLine();
                        name = Console.ReadLine();
                        middlename = Console.ReadLine();
                        age = Convert.ToInt32(Console.ReadLine());
                        disease = Console.ReadLine();
                        ardate = Console.ReadLine();
                        disdate = Console.ReadLine();
                        if (disdate == "")
                            disdate = "NULL";
                        ward = Console.ReadLine();
                        command = string.Format("insert into " + tableName + " values(N'{0}',N'{1}',N'{2}',{3},N'{4}','{5}',{6},N'{7}');", surname, name, middlename, age, disease, ardate, disdate, ward);
                        cmd = new SqlCommand(command, con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    if(operation == "alter")
                    {
                        Console.WriteLine("Введіть id рядка, що буде змінений");
                        rowid = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Введіть ПІБ, Вік, Хворобу, Дати прибуття/виписки у форматі рік-місяць-число, а також відділення ");
                        surname = Console.ReadLine();
                        name = Console.ReadLine();
                        middlename = Console.ReadLine();
                        age = Convert.ToInt32(Console.ReadLine());
                        disease = Console.ReadLine();
                        ardate = Console.ReadLine();
                        disdate = Console.ReadLine();
                        if (disdate == "")
                            disdate = "NULL";
                        ward = Console.ReadLine();
                        command = string.Format("update " + tableName + "SET Last_name = N'{0}', First_Name = N'{1}', Middle_Name = N'{2}', Age = {3}, Disease = '{4}'" +
                            ", Arrival_Date = '{5}, Discharge_Date = '{6}', Ward = '{7}' where IDpat = {8} ');",surname,name,middlename,age,disease,ardate,disdate,rowid);
                        cmd = new SqlCommand(command, con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    if(operation == "delete")
                    {
                        con.Open();
                        Console.WriteLine("Введіть id рядка, що буде видалений");
                        rowid = Convert.ToInt32(Console.ReadLine());
                        command = string.Format("DELETE FROM Patient WHERE IDpat = {0};", rowid);
                        cmd = new SqlCommand(command, con);
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    break;
                case "dbo.Staff":
                    if(operation == "add")
                    {
                        Console.WriteLine("Введіть ПІБ, Вік і Посаду ");
                        surname = Console.ReadLine();
                        name = Console.ReadLine();
                        middlename = Console.ReadLine();
                        age = Convert.ToInt32(Console.ReadLine());
                        string position = Console.ReadLine();
                        command = string.Format("insert into " + tableName + " values(N'{0}',N'{1}',N'{2}',{3},N'{4}');", surname, name, middlename, age, position);
                        cmd = new SqlCommand(command, con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    if(operation == "alter")
                    {
                        Console.WriteLine("Введіть id рядка, що буде змінений");
                        rowid = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Введіть ПІБ, Вік і Посаду ");
                        surname = Console.ReadLine();
                        name = Console.ReadLine();
                        middlename = Console.ReadLine();
                        age = Convert.ToInt32(Console.ReadLine());
                        string position = Console.ReadLine();
                        command = string.Format("update " + tableName + "Set Last_Name = N'{0}', First_Name = N'{1}', Middle_Name = N'{2}', Age = {3}, Position = N'{4}');", surname, name, middlename, age, position,rowid);
                        cmd = new SqlCommand(command, con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    if( operation == "delete")
                    {
                        con.Open();
                        Console.WriteLine("Введіть id рядка, що буде видалений");
                        rowid = Convert.ToInt32(Console.ReadLine());
                        command = string.Format("DELETE FROM Staff WHERE IDstaff = {0};", rowid);
                        cmd = new SqlCommand(command, con);
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    break;
                case "dbo.Proced":
                    int patid = 0;
                    int equipid = 0;
                    int staffid = 0;
                    int medid = 0;
                    int price = 0;
                    if (operation == "add")
                    {
                        Console.WriteLine("Введіть ID пацієнта, спорядження, персоналу та ліків, назву та ціну процедури: ");
                        patid = Convert.ToInt32(Console.ReadLine());
                        equipid = Convert.ToInt32(Console.ReadLine());
                        staffid = Convert.ToInt32(Console.ReadLine());
                        medid = Convert.ToInt32(Console.ReadLine());
                        name = Console.ReadLine();
                        price = Convert.ToInt32(Console.ReadLine());
                        command = string.Format("insert into " + tableName + " values({0},{1},{2},{3},N'{4}',{5});", patid, equipid, staffid, medid, name, price);
                        cmd = new SqlCommand(command, con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    if(operation == "alter")
                    {
                        Console.WriteLine("Введіть id рядка, що буде змінений");
                        rowid = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Введіть ID пацієнта, спорядження, персоналу та ліків, назву та ціну процедури: ");
                        patid = Convert.ToInt32(Console.ReadLine());
                        equipid = Convert.ToInt32(Console.ReadLine());
                        staffid = Convert.ToInt32(Console.ReadLine());
                        medid = Convert.ToInt32(Console.ReadLine());
                        name = Console.ReadLine();
                        price = Convert.ToInt32(Console.ReadLine());
                        command = string.Format("update " + tableName + "Patient_ID = {0}, Equipment_ID = {1}, Staff_ID = {2}, Medicine_ID = {3}," +
                            "Name = N'{4}', Price = {5} where IDproc = {6});", patid, equipid, staffid, medid, name, price,rowid);
                        cmd = new SqlCommand(command, con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    if(operation == "delete")
                    {
                        con.Open();
                        Console.WriteLine("Введіть id рядка, що буде видалений");
                        rowid = Convert.ToInt32(Console.ReadLine());
                        command = string.Format("DELETE FROM Proced WHERE IDproc = {0};", rowid);
                        cmd = new SqlCommand(command, con);
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    break;
                default: Console.WriteLine("Такої таблиці не існує!"); Console.Clear(); break;
            }
                
        }
        public static void Menu_operations(string tableName)
        {
            Console.Clear();
            Console.WriteLine("Виберіть наступну операцію:\n1)Додати дані до таблиці\n2)Видалити дані з таблиці\n3)Редагувати стрічку таблиці\n4)Повернутись");
            string a = Console.ReadLine();
            switch (a)
            {
                case "1":
                    AlterDataTables(tableName, "add");
                    Console.WriteLine("Дані додані успішно");
                    break;
                case "2":
                    AlterDataTables(tableName, "delete");
                    Console.WriteLine("Дані видалені успішно");
                    break;
                case "3":
                    AlterDataTables(tableName, "alter");
                    Console.WriteLine("Дані змінені успішно");
                    break;
                case "4":
                    Console.Clear();
                    Menu_pickTables();
                    break;
            }
        }
        public static void Menu_pickTables()
        {
            Console.Clear();
            Console.WriteLine("Введіть назву таблиці\n1)dbo.Equipment \n2)dbo.Medicine \n3)dbo.Staff \n4)dbo.Patient \n5)dbo.Proced\n6)Повернутись");
            string tableName = Console.ReadLine();
            switch (tableName)
            {
                case "1":
                    tableName = "dbo.Equipment";
                    Menu_operations(tableName);
                    break;
                case "2":
                    tableName = "dbo.Medicine";
                    Menu_operations(tableName);
                    break;
                case "3":
                    tableName = "dbo.Staff";
                    Menu_operations(tableName);
                    break;
                case "4":
                    tableName = "dbo.Patient";
                    Menu_operations(tableName);
                    break;
                case "5":
                    tableName = "dbo.Proced";
                    Menu_operations(tableName);
                    break;
                case "6":
                    Console.Clear();
                    Menu_main();
                    break;
                default:
                    Console.WriteLine("No such choice");
                    Console.Clear();
                    break;
            }
        }
        public static void AlterDataTablesDS(string tableName, string operation)
        {
            string tableNameAdapter = tableName.Remove(0,4);
            switch (tableName)
            {
                case "dbo.Equipment":
                    string name = "";
                    string manuf = "";
                    string command = "";
                    int rowid = 0;
                    int quant = 0;
                    if (operation == "add")
                    {
                        Console.WriteLine("Enter Name,Quantity and manufacturer");
                        name = Console.ReadLine();
                        quant = Convert.ToInt32(Console.ReadLine());
                        manuf = Console.ReadLine();

                        con.Open();
                        dataAdapters[tableNameAdapter].Fill(hSet, tableNameAdapter);
                        var equipmentTable = hSet.Tables[tableNameAdapter];

                        equipmentTable.Rows.Add(0, name, quant, manuf);

                        dataAdapters[tableNameAdapter].Update(hSet, tableNameAdapter);
                        con.Close();

                        hSet.Clear();
                    }
                    if (operation == "alter")
                    {
                        Console.WriteLine("Введіть id рядка, що буде змінений");
                        rowid = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Enter Name,Quantity and manufacturer");
                        name = Console.ReadLine();
                        quant = Convert.ToInt32(Console.ReadLine());
                        manuf = Console.ReadLine();

                        con.Open();

                        dataAdapters[tableNameAdapter].Fill(hSet, tableNameAdapter);

                        var equipmentTable = hSet.Tables[tableNameAdapter];

                        var updRow = equipmentTable.Select($"IDeq={rowid}")[0];
                        updRow["Name"] = name;
                        updRow["Quantity"] = quant;
                        updRow["Manufacturer"] = manuf;

                        dataAdapters[tableNameAdapter].Update(hSet, tableNameAdapter);

                        con.Close();

                        hSet.Clear();
                    }
                    if (operation == "delete")
                    {
                        Console.WriteLine("Введіть id рядка, що буде видалений");
                        rowid = Convert.ToInt32(Console.ReadLine());

                        con.Open();

                        dataAdapters[tableNameAdapter].Fill(hSet, tableNameAdapter);

                        var equipmentTable = hSet.Tables[tableNameAdapter];

                        var delRow = equipmentTable.Select($"IDeq={rowid}")[0];
                        delRow.Delete();

                        dataAdapters[tableNameAdapter].Update(hSet, tableNameAdapter);

                        con.Close();

                        hSet.Clear();
                    }
                    break;
                case "dbo.Medicine":
                    string crdate = "";
                    string expdate = "";
                    if (operation == "add")
                    {
                        Console.WriteLine("Введіть Назву, Виробника і Дати виготовлення/Термін придатності у форматі рік-місяць-число ");
                        name = Console.ReadLine();
                        manuf = Console.ReadLine();
                        crdate = Console.ReadLine();
                        expdate = Console.ReadLine();

                        con.Open();
                        dataAdapters[tableNameAdapter].Fill(hSet, tableNameAdapter);
                        var equipmentTable = hSet.Tables[tableNameAdapter];

                        equipmentTable.Rows.Add(0, name, manuf, crdate, expdate);

                        dataAdapters[tableNameAdapter].Update(hSet, tableNameAdapter);
                        con.Close();

                        hSet.Clear();
                    }
                    if (operation == "alter")
                    {
                        Console.WriteLine("Введіть id рядка, що буде змінений");
                        rowid = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Введіть Назву, Виробника і Дати виготовлення/Термін придатності у форматі рік-місяць-число ");
                        name = Console.ReadLine();
                        manuf = Console.ReadLine();
                        crdate = Console.ReadLine();
                        expdate = Console.ReadLine();

                        con.Open();

                        dataAdapters[tableNameAdapter].Fill(hSet, tableNameAdapter);

                        var equipmentTable = hSet.Tables[tableNameAdapter];

                        var updRow = equipmentTable.Select($"IDmed={rowid}")[0];
                        updRow["Name"] = name;
                        updRow["Producer"] = manuf;
                        updRow["Created_Date"] = crdate;
                        updRow["Expiration_Date"] = expdate;

                        dataAdapters[tableNameAdapter].Update(hSet, tableNameAdapter);

                        con.Close();

                        hSet.Clear();
                    }
                    if (operation == "delete")
                    {
                        Console.WriteLine("Введіть id рядка, що буде видалений");
                        rowid = Convert.ToInt32(Console.ReadLine());

                        con.Open();

                        dataAdapters[tableNameAdapter].Fill(hSet, tableNameAdapter);

                        var equipmentTable = hSet.Tables[tableNameAdapter];

                        var delRow = equipmentTable.Select($"IDmed={rowid}")[0];
                        delRow.Delete();

                        dataAdapters[tableNameAdapter].Update(hSet, tableNameAdapter);

                        con.Close();

                        hSet.Clear();
                    }
                    break;
                case "dbo.Patient":
                    string surname = "";
                    string middlename = "";
                    int age = 0;
                    string disease = "";
                    string ardate = "";
                    string disdate = "";
                    string ward = "";
                    if (operation == "add")
                    {
                        Console.WriteLine("Введіть ПІБ, Вік, Хворобу, Дати прибуття/виписки у форматі рік-місяць-число, а також відділення ");
                        surname = Console.ReadLine();
                        name = Console.ReadLine();
                        middlename = Console.ReadLine();
                        age = Convert.ToInt32(Console.ReadLine());
                        disease = Console.ReadLine();
                        ardate = Console.ReadLine();
                        disdate = Console.ReadLine();
                        ward = Console.ReadLine();

                        string[] temp1 = ardate.Split("-");


                        con.Open();
                        dataAdapters[tableNameAdapter].Fill(hSet, tableNameAdapter);
                        var equipmentTable = hSet.Tables[tableNameAdapter];

                        if (disdate == "")
                        {
                            equipmentTable.Rows.Add(0, surname, name, middlename, age, disease, 
                                ardate, DBNull.Value, ward);
                            dataAdapters[tableNameAdapter].Update(hSet, tableNameAdapter);
                            con.Close();
                            hSet.Clear();
                            break;
                        }

                        string[] temp2 = disdate.Split("-");
                        equipmentTable.Rows.Add(0, surname, name, middlename, age, disease, ardate, disdate, ward);

                        dataAdapters[tableNameAdapter].Update(hSet, tableNameAdapter);
                        con.Close();

                        hSet.Clear();
                    }
                    if (operation == "alter")
                    {
                        Console.WriteLine("Введіть id рядка, що буде змінений");
                        rowid = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Введіть ПІБ, Вік, Хворобу, Дати прибуття/виписки у форматі рік-місяць-число, а також відділення ");
                        surname = Console.ReadLine();
                        name = Console.ReadLine();
                        middlename = Console.ReadLine();
                        age = Convert.ToInt32(Console.ReadLine());
                        disease = Console.ReadLine();
                        ardate = Console.ReadLine();
                        disdate = Console.ReadLine();
                        ward = Console.ReadLine();
                        
                        con.Open();

                        dataAdapters[tableNameAdapter].Fill(hSet, tableNameAdapter);

                        var equipmentTable = hSet.Tables[tableNameAdapter];

                        var updRow = equipmentTable.Select($"IDpat={rowid}")[0];
                        updRow["First_name"] = name;
                        updRow["Last_name"] = surname;
                        updRow["Middle_name"] = middlename;
                        updRow["Age"] = age;
                        updRow["Disease"] = disease;
                        updRow["Arrival_date"] = ardate;

                        if (disdate == "")
                        {
                            updRow["Discharge_date"] = DBNull.Value;
                        }
                        else
                        {
                            updRow["Discharge_date"] = disdate;
                        }
                        updRow["Ward"] = ward;

                        dataAdapters[tableNameAdapter].Update(hSet, tableNameAdapter);

                        con.Close();

                        hSet.Clear();
                    }
                    if (operation == "delete")
                    {
                        Console.WriteLine("Введіть id рядка, що буде видалений");
                        rowid = Convert.ToInt32(Console.ReadLine());

                        con.Open();

                        dataAdapters[tableNameAdapter].Fill(hSet, tableNameAdapter);

                        var equipmentTable = hSet.Tables[tableNameAdapter];

                        var delRow = equipmentTable.Select($"IDpat={rowid}")[0];
                        delRow.Delete();

                        dataAdapters[tableNameAdapter].Update(hSet, tableNameAdapter);

                        con.Close();

                        hSet.Clear();
                    }
                    break;
                case "dbo.Staff":
                    if (operation == "add")
                    {
                        Console.WriteLine("Введіть ПІБ, Вік і Посаду ");
                        surname = Console.ReadLine();
                        name = Console.ReadLine();
                        middlename = Console.ReadLine();
                        age = Convert.ToInt32(Console.ReadLine());
                        string position = Console.ReadLine();

                        con.Open();
                        dataAdapters[tableNameAdapter].Fill(hSet, tableNameAdapter);
                        var equipmentTable = hSet.Tables[tableNameAdapter];

                        equipmentTable.Rows.Add(0, name, surname, middlename, age, position);

                        dataAdapters[tableNameAdapter].Update(hSet, tableNameAdapter);
                        con.Close();

                        hSet.Clear();
                    }
                    if (operation == "alter")
                    {
                        Console.WriteLine("Введіть id рядка, що буде змінений");
                        rowid = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Введіть ПІБ, Вік і Посаду ");
                        surname = Console.ReadLine();
                        name = Console.ReadLine();
                        middlename = Console.ReadLine();
                        age = Convert.ToInt32(Console.ReadLine());
                        string position = Console.ReadLine();

                        con.Open();

                        dataAdapters[tableNameAdapter].Fill(hSet, tableNameAdapter);

                        var equipmentTable = hSet.Tables[tableNameAdapter];

                        var updRow = equipmentTable.Select($"IDstaff={rowid}")[0];
                        updRow["Name"] = name;
                        updRow["Last_name"] = surname;
                        updRow["Middle_name"] = middlename;
                        updRow["Age"] = age;
                        updRow["Position"] = position;

                        dataAdapters[tableNameAdapter].Update(hSet, tableNameAdapter);

                        con.Close();

                        hSet.Clear();
                    }
                    if (operation == "delete")
                    {
                        Console.WriteLine("Введіть id рядка, що буде видалений");
                        rowid = Convert.ToInt32(Console.ReadLine());

                        con.Open();

                        dataAdapters[tableNameAdapter].Fill(hSet, tableNameAdapter);

                        var equipmentTable = hSet.Tables[tableNameAdapter];

                        var delRow = equipmentTable.Select($"IDstaff={rowid}")[0];
                        delRow.Delete();

                        dataAdapters[tableNameAdapter].Update(hSet, tableNameAdapter);

                        con.Close();

                        hSet.Clear();
                    }
                    break;
                case "dbo.Proced":
                    int patid = 0;
                    int equipid = 0;
                    int staffid = 0;
                    int medid = 0;
                    int price = 0;
                    if (operation == "add")
                    {
                        Console.WriteLine("Введіть ID пацієнта, спорядження, персоналу та ліків, назву та ціну процедури: ");
                        patid = Convert.ToInt32(Console.ReadLine());
                        equipid = Convert.ToInt32(Console.ReadLine());
                        staffid = Convert.ToInt32(Console.ReadLine());
                        medid = Convert.ToInt32(Console.ReadLine());
                        name = Console.ReadLine();
                        price = Convert.ToInt32(Console.ReadLine());

                        con.Open();
                        dataAdapters[tableNameAdapter].Fill(hSet, tableNameAdapter);
                        var equipmentTable = hSet.Tables[tableNameAdapter];

                        equipmentTable.Rows.Add(0, patid, equipid, staffid, medid, name, price);

                        dataAdapters[tableNameAdapter].Update(hSet, tableNameAdapter);
                        con.Close();

                        hSet.Clear();
                    }
                    if (operation == "alter")
                    {
                        Console.WriteLine("Введіть id рядка, що буде змінений");
                        rowid = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("Введіть ID пацієнта, спорядження, персоналу та ліків, назву та ціну процедури: ");
                        patid = Convert.ToInt32(Console.ReadLine());
                        equipid = Convert.ToInt32(Console.ReadLine());
                        staffid = Convert.ToInt32(Console.ReadLine());
                        medid = Convert.ToInt32(Console.ReadLine());
                        name = Console.ReadLine();
                        price = Convert.ToInt32(Console.ReadLine());

                        con.Open();

                        dataAdapters[tableNameAdapter].Fill(hSet, tableNameAdapter);

                        var equipmentTable = hSet.Tables[tableNameAdapter];

                        var updRow = equipmentTable.Select($"IDproc={rowid}")[0];
                        updRow["Patient_ID"] = patid;
                        updRow["Equipment_ID"] = equipid;
                        updRow["Staff_ID"] = staffid;
                        updRow["Medicine_ID"] = medid;
                        updRow["Name"] = name;
                        updRow["Price"] = price;

                        dataAdapters[tableNameAdapter].Update(hSet, tableNameAdapter);

                        con.Close();

                        hSet.Clear();
                    }
                    if (operation == "delete")
                    {
                        Console.WriteLine("Введіть id рядка, що буде видалений");
                        rowid = Convert.ToInt32(Console.ReadLine());

                        con.Open();

                        dataAdapters[tableNameAdapter].Fill(hSet, tableNameAdapter);

                        var equipmentTable = hSet.Tables[tableNameAdapter];

                        var delRow = equipmentTable.Select($"IDproc={rowid}")[0];
                        delRow.Delete();

                        dataAdapters[tableNameAdapter].Update(hSet, tableNameAdapter);

                        con.Close();

                        hSet.Clear();
                    }
                    break;
                default: Console.Clear(); Console.WriteLine("Такої таблиці не існує!"); break;
            }

        }
        public static void Menu_operationsDS(string tableName)
        {
            Console.WriteLine("Виберіть наступну операцію:\n1)Додати дані до таблиці\n2)Видалити дані з таблиці\n3)Редагувати стрічку таблиці\n4)Повернутись");
            string a = Console.ReadLine();
            switch (a)
            {
                case "1":
                    AlterDataTablesDS(tableName, "add");
                    Console.WriteLine("Дані додані успішно");
                    break;
                case "2":
                    AlterDataTablesDS(tableName, "delete");
                    Console.WriteLine("Дані видалені успішно");
                    break;
                case "3":
                    AlterDataTablesDS(tableName, "alter");
                    Console.WriteLine("Дані змінені успішно");
                    break;
                case "4":
                    Console.Clear();
                    Menu_pickTablesDS();
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("Немає такої операції!");
                    break;
            }
        }
        public static void Menu_pickTablesDS()
        {
            Console.Clear();
            Console.WriteLine("Введіть назву таблиці\n1)dbo.Equipment \n2)dbo.Medicine \n3)dbo.Staff \n4)dbo.Patient \n5)dbo.Proced\n6)Повернутись");
            string tableName = Console.ReadLine();
            Console.Clear();
            switch (tableName)
            {
                case "1":
                    tableName = "dbo.Equipment";
                    Display("Equipment");
                    Menu_operationsDS(tableName);
                    break;
                case "2":
                    tableName = "dbo.Medicine"; 
                    Display("Medicine");
                    
                    Menu_operationsDS(tableName);
                    break;
                case "3":
                    tableName = "dbo.Staff";
                    Display("Staff");
                    
                    Menu_operationsDS(tableName);
                    break;
                case "4":
                    tableName = "dbo.Patient";
                    Display("Patient");
                    Menu_operationsDS(tableName);
                    break;
                case "5":
                    tableName = "dbo.Proced";
                    Display("Proced");
                    Menu_operationsDS(tableName);
                    break;
                case "6":
                    Console.Clear();
                    Menu_main();
                    break;
                default:
                    Console.WriteLine("No such choice");
                    Console.Clear();
                    break;
            }
        }
        public static int Menu_main()
        {
            bool men = true;
            while(men = true)
            {
                Console.WriteLine("1)Вивести таблиці\n2)Вивести таблиці (DataSet)\n3)Вихід");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        Display("Equipment");
                        Display("Medicine");
                        Display("Staff");
                        Display("Patient");
                        Display("Proced");
                        Menu_pickTables();
                        break;
                    case "2":
                        Menu_pickTablesDS();
                        break;
                    case "3": return 0;
                    default: Console.WriteLine("No such choice"); Console.Clear(); break; 
                }
            }
            return -1;
        }
        
    }
    class MainProgram
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.Unicode;
            EstablishingConnection A = new EstablishingConnection();
        }
    }
}