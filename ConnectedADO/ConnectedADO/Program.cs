using System;
using System.ComponentModel.Design;
using System.Configuration;
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
        public EstablishingConnection()
        {
            con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ToString());
            try
            {
                con.Open();
                Console.WriteLine("Connection Established\n");
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            con.Close();
            Menu_main();
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
        public static int Menu_main()
        {
            bool men = true;
            while(men = true)
            {
                Console.WriteLine("1)Вивести таблиці\n2)Вихід");
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
                        return 0;
                        break;
                    default: Console.WriteLine("No such choice"); Console.Clear(); break; 
                }
            }
            return -1;
        }
        public static void ReadingTables(string tableName)
        {
            string command = "Select * From" + tableName;
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