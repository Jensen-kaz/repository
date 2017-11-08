using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;


namespace ProkatAuto22.Classes
{
    class DataBaseClass
    {
        private bool Debug = false;
        private string DBFileName = @"DBFile.sqlite3";
        private string CreateBDCommand = @"
                                            PRAGMA foreign_keys = off;
                                            BEGIN TRANSACTION;
                                            CREATE TABLE additionalServices (ID INTEGER PRIMARY KEY AUTOINCREMENT, service VARCHAR (100));
                                            INSERT INTO additionalServices (ID, service) VALUES (2, 'Зимние шины');
                                            INSERT INTO additionalServices (ID, service) VALUES (3, 'Спортивные крепления');
                                            INSERT INTO additionalServices (ID, service) VALUES (4, 'Детские кресла');
                                            INSERT INTO additionalServices (ID, service) VALUES (5, 'GPS-навигатор');
                                            CREATE TABLE additionalServicesBinding (orderID INTEGER REFERENCES orders (ID), additionalServicesID INTEGER REFERENCES additionalServices (ID));
                                            CREATE TABLE cars (ID INTEGER PRIMARY KEY AUTOINCREMENT, model VARCHAR (100), priceForHour NUMERIC (5), typeID INTEGER REFERENCES carTypes (ID), photoFileName VARCHAR (100), carCapacity INTEGER (3), yearOfIssue INTEGER (4), gosNumber VARCHAR (9), carCapacityTrunc INTEGER (5));
                                            CREATE TABLE carTypes (ID INTEGER PRIMARY KEY AUTOINCREMENT, type VARCHAR (100));
                                            INSERT INTO carTypes (ID, type) VALUES (1, 'D');
                                            INSERT INTO carTypes (ID, type) VALUES (2, 'C');
                                            INSERT INTO carTypes (ID, type) VALUES (3, 'B');
                                            CREATE TABLE client (ID INTEGER PRIMARY KEY AUTOINCREMENT, name VARCHAR (100), phoneNumber INTEGER (19), city VARCHAR (100));
                                            CREATE TABLE driverHabits (ID INTEGER PRIMARY KEY AUTOINCREMENT, habit VARCHAR (100));
                                            INSERT INTO driverHabits (ID, habit) VALUES (1, 'Наркоман');
                                            INSERT INTO driverHabits (ID, habit) VALUES (2, 'Пьёт');
                                            INSERT INTO driverHabits (ID, habit) VALUES (3, 'Курит');
                                            CREATE TABLE driverHabitsBinding (driverID INTEGER REFERENCES drivers (ID), driverHabitsID INTEGER REFERENCES driverHabits (ID));
                                            CREATE TABLE drivers (ID INTEGER PRIMARY KEY AUTOINCREMENT, name VARCHAR (100), photoFileName VARCHAR (100), experienceFrom INTEGER (4));
                                            CREATE TABLE orders (ID INTEGER PRIMARY KEY AUTOINCREMENT, data DATETIME, carID INTEGER REFERENCES cars (ID), driverID INTEGER REFERENCES drivers (ID), duration INTEGER (2), clientID INTEGER REFERENCES client (ID), address VARCHAR (100));
                                            COMMIT TRANSACTION;
                                            PRAGMA foreign_keys = on;
                                        ";

        public DataBaseClass()
        {
            if (!System.IO.File.Exists(DBFileName)){
                // Файла БД нет - создаём его
                MyDBLogger("BD-file doesn't exist.");
                SQLiteConnection.CreateFile(DBFileName);
                using (SQLiteConnection DBConnection = new SQLiteConnection("data source=" + DBFileName))
                {
                    DBConnection.Open();
                    using (SQLiteCommand CreateCommand = new SQLiteCommand(DBConnection))
                    {
                        CreateCommand.CommandText = CreateBDCommand;
                        CreateCommand.ExecuteNonQuery();
                    }
                }
            }
            else{
                // Файла БД присутствует
                MyDBLogger("BD-file exist.");
            }
        }

        /// <summary>
        /// Локальный логгер. Вывод в стандартный вывод если (Debug == true).
        /// </summary>
        /// <param name="Message">Сообщение для вывода.</param>
        private void MyDBLogger(string Message)
        {
            if (Debug) System.Console.WriteLine(Message);
        }

        /// <summary>
        /// Не протестирован!!!
        /// Считывает водителя из базы по ID.
        /// </summary>
        /// <param name="DriverID"></param>
        public DriverClass ReadDriverDB(string DriverID)
        {
            DriverClass ReadedDriver = new DriverClass();
            ReadedDriver.DriverDBID = DriverID;

            using (SQLiteConnection DBConnection = new SQLiteConnection("data source=" + DBFileName))
            {
                DBConnection.Open();
                using (SQLiteCommand Command = new SQLiteCommand(DBConnection))
                {
                    // Считываем информацию о водителе
                    Command.CommandText = @"SELECT name, photoFileName, experienceFrom FROM drivers WHERE ID = '" + ReadedDriver.DriverDBID + "';";
                    MyDBLogger("Select Driver by ID: " + Command.CommandText);
                    using (SQLiteDataReader Reader = Command.ExecuteReader())
                    {
                        Reader.Read();
                        ReadedDriver.FIOdriver = Reader.GetString(0);
                        ReadedDriver.PhotoDriver = Reader.GetString(1);
                     //   ReadedDriver.ExpirienceDriver = Reader.GetString(2);                    // ОШИБКА!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    }

                    // Считываем привычки водителя
                    Command.CommandText = @"SELECT driverHabitsID FROM driverHabitsBinding WHERE driverID = '" + ReadedDriver.DriverDBID + "';";
                    MyDBLogger("Select Driver habbits by Driver ID: " + Command.CommandText);
                    using (SQLiteDataReader Reader = Command.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            if (Reader.GetInt32(0) == 1) { ReadedDriver.DriverHabitDrugs = true; }
                            if (Reader.GetInt32(0) == 2) { ReadedDriver.DriverHabitDrink = true; }
                            if (Reader.GetInt32(0) == 3) { ReadedDriver.DriverHabitSmoke = true; }
                        }
                    }
                }
            }

            return ReadedDriver;
        }

        /// <summary>
        /// Записывает в базу нового водителя.
        /// </summary>
        /// <param name="NewDriverAdd"></param>
        public void AddNewDriverDB(DriverClass NewDriverAdd) {
            using (SQLiteConnection DBConnection = new SQLiteConnection("data source=" + DBFileName))
            {
                DBConnection.Open();
                using (SQLiteCommand Command = new SQLiteCommand(DBConnection))
                {
                    int AddedDviverID;
                    Command.CommandText = @"INSERT INTO drivers (name, photoFileName, experienceFrom) VALUES ('" +
                         NewDriverAdd.FIOdriver.ToUpper() + "','" +
                         NewDriverAdd.PhotoDriver + "','" +
                         NewDriverAdd.ExpirienceDriver + "');";
                    MyDBLogger("Create driver whis SQL-command: " + Command.CommandText);
                    Command.ExecuteNonQuery();

                    Command.CommandText = @"SELECT ID from drivers ORDER by ID DESC LIMIT 1;";
                    using (SQLiteDataReader Reader = Command.ExecuteReader())
                    {
                        Reader.Read();
                        AddedDviverID = Reader.GetInt32(0);
                        MyDBLogger("Last record Driver ID is: " + AddedDviverID);
                    }

                    if (NewDriverAdd.DriverHabitSmoke)
                    {
                        Command.CommandText = @"INSERT INTO driverHabitsBinding (driverID, driverHabitsID) VALUES (" + AddedDviverID + "," + 3 + ");";
                        MyDBLogger("Driver habbits (smoke) SQL-command: " + Command.CommandText);
                        Command.ExecuteNonQuery();
                    }

                    if (NewDriverAdd.DriverHabitDrink)
                    {
                        Command.CommandText = @"INSERT INTO driverHabitsBinding (driverID, driverHabitsID) VALUES (" + AddedDviverID + "," + 2 + ");";
                        MyDBLogger("Driver habbits (smoke) SQL-command: " + Command.CommandText);
                        Command.ExecuteNonQuery();
                    }

                    if (NewDriverAdd.DriverHabitDrugs)
                    {
                        Command.CommandText = @"INSERT INTO driverHabitsBinding (driverID, driverHabitsID) VALUES (" + AddedDviverID + "," + 1 + ");";
                        MyDBLogger("Driver habbits (smoke) SQL-command: " + Command.CommandText);
                        Command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Не протестирован!!!
        /// Редактирует водителя.
        /// </summary>
        /// <param name="DriverEdit"></param>
        public void EditDriverDB(DriverClass DriverEdit){
            using (SQLiteConnection DBConnection = new SQLiteConnection("data source=" + DBFileName))
            {
                DBConnection.Open();
                using (SQLiteCommand Command = new SQLiteCommand(DBConnection))
                {
                    Command.CommandText = @"UPDATE drivers SET name = '" + DriverEdit.FIOdriver.ToUpper() +
                        "', photoFileName ='" + DriverEdit.PhotoDriver +
                        "', experienceFrom = '" + DriverEdit.ExpirienceDriver +
                        "' WHERE ID = '" + DriverEdit.DriverDBID +
                        ";";
                    MyDBLogger("Edit driver driver whis SQL-command: " + Command.CommandText);
                    Command.ExecuteNonQuery();                                                     // ОШИБКА!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                    // Удаляем все упоминания о вредных привычках и заново их устанавливаем
                    Command.CommandText = @"DELETE FROM drivers WHERE driverID = '" + DriverEdit.DriverDBID + ";";
                    MyDBLogger("Delete all habits binding SQL-command: " + Command.CommandText);
                    Command.ExecuteNonQuery();

                    if (DriverEdit.DriverHabitSmoke)
                    {
                        Command.CommandText = @"INSERT INTO driverHabitsBinding (driverID, driverHabitsID) VALUES (" + DriverEdit + "," + 3 + ");";
                        MyDBLogger("Driver habbits (smoke) SQL-command: " + Command.CommandText);
                        Command.ExecuteNonQuery();
                    }

                    if (DriverEdit.DriverHabitDrink)
                    {
                        Command.CommandText = @"INSERT INTO driverHabitsBinding (driverID, driverHabitsID) VALUES (" + DriverEdit + "," + 2 + ");";
                        MyDBLogger("Driver habbits (smoke) SQL-command: " + Command.CommandText);
                        Command.ExecuteNonQuery();
                    }

                    if (DriverEdit.DriverHabitDrugs)
                    {
                        Command.CommandText = @"INSERT INTO driverHabitsBinding (driverID, driverHabitsID) VALUES (" + DriverEdit + "," + 1 + ");";
                        MyDBLogger("Driver habbits (smoke) SQL-command: " + Command.CommandText);
                        Command.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Не протестирован!!!
        /// Возвращает список всех водителей.
        /// </summary>
        public List<DriverClass> ReadAllDriversDB()
        {
            List<DriverClass> ListOfDrivers = new List<DriverClass>();


            using (SQLiteConnection DBConnection = new SQLiteConnection("data source=" + DBFileName))
            {
                DBConnection.Open();
                using (SQLiteCommand Command = new SQLiteCommand(DBConnection))
                {
                    Command.CommandText = @"SELECT ID FROM drivers;";
                    MyDBLogger("Select Drivers ID: " + Command.CommandText);
                    using (SQLiteDataReader Reader = Command.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            ListOfDrivers.Add(this.ReadDriverDB(Reader.GetInt32(0).ToString()));
                        }
                    }
                }
            }

            return (ListOfDrivers);
        }




        ///Записывает в базу нового клиента. Не протестирован.
        public void AddNewCustomerDB(CustomerClass NewCostomer)
        {


            
        }


        ///Считывает из базы нового клиента. Не протестирован.
        public void ReadCustomerDB(int CustomerID)
        {

        }


        ///Редактирует в базе нового клиента. Не протестирован.
        public void EditCustomerDB(CustomerClass EditCustomer)
        {

        }

        ///Возвращает список клиентов базы. Не протестирован.
        public List<CustomerClass> ReadAllCustomersDB()
        {
            List<CustomerClass> ListOfCustomers = new List<CustomerClass>();

            return (ListOfCustomers);
        }


    }
}
