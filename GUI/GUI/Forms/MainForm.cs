using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProkatAuto22;
using ProkatAuto22.Classes;
using System.IO;

namespace GUI
{
    public partial class Form1 : Form
    {
        DataBaseClass DataBase;
        DriverClass DriverObjectRead;
        CustomerClass CustomerObjectRead;
        AutomobileClass CarObject;
        OrderClass RequestObject;


      // Dictionary<string, string> NameAndFilePath = new Dictionary<string, string>();
        
        string destFile;

        bool habitSmoke, habitDrink, habitDrugs;

        public Form1()
        {
            InitializeComponent();
            DriverObjectRead = new DriverClass();
            CustomerObjectRead = new CustomerClass();
            DataBase = new DataBaseClass();

            
            GetDrivers();

            CustomerObjectRead.ReadAllCustomers();
            GetCustomers();

            //   CarObject = new AutomobileClass();
            //   RequestObject = new OrderClass();

            //   GetCar(CarObject);
            //  GetRequest(RequestObject);

            //  comboBox2ClassType.DataSource = CarObject.ClassCarList;

            ButtonDriverDisabled();

            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "dd.MM.yyyy; hh:mm";
        }

        ///////////////////////////////////////// Авто

        private void button1AddAuto_Click(object sender, EventArgs e)
        {
            
        }

        private void button3RedactionAuto_Click(object sender, EventArgs e)
        {

        }

        //вывод данных на форму
        private void GetCar(AutomobileClass CarObject)
        {
            CarObject.ReadCar();

            /*
            for (int i = 0; i < CarObject.ModelCarList.Count; i++)
                listBox1.Items.Add(CarObject.ModelCarList[i]);
                */
        }

        private void button2DeleteCar_Click(object sender, EventArgs e)
        {
            
        }

        private void comboBox2ClassType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CarObject.GetCarClass(comboBox2ClassType.SelectedText);
        }

        private void comboBox1CarType_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        ////////////////////////////////// Водители

        private void button1EditPhotoDriver_Click(object sender, EventArgs e)
        {
            OpenFileDialog AddPhotoDriver = new OpenFileDialog();
            
            AddPhotoDriver.Filter = ("(*.jpg)|*.jpg|(*.png)|*.png|All files (*.*)|*.*");
             if (AddPhotoDriver.ShowDialog() == DialogResult.OK)
              {
                string fileNameDriver = AddPhotoDriver.SafeFileName;
                string sourcePath = AddPhotoDriver.FileName;
                string targetPath = @"DriverPhoto";
                
                 destFile = Path.Combine(targetPath, fileNameDriver);

               
                File.Copy(sourcePath, destFile, true);
              }
        }
 
        private void button4AddDriver_Click(object sender, EventArgs e)
        {
            DriverClass AddDriver = new DriverClass();

            AddDriver.DriverDBID = textBox2IdDriver.Text;
            AddDriver.PhotoDriver = destFile;
            AddDriver.FIOdriver = textBox2FioDriver.Text;
            AddDriver.ExpirienceDriver = textBox3ExpirienceDriver.Text;
            AddDriver.DriverHabitSmoke = habitSmoke;
            AddDriver.DriverHabitDrink = habitDrink;
            AddDriver.DriverHabitDrugs = habitDrugs;

            AddDriver.InsertDriver();


            listBox2Driver.Items.Clear();
            GetDrivers();
            
        }

        private void button6RedactionDriver_Click(object sender, EventArgs e)
        {
             DriverClass RedactionDriver= new DriverClass();
            // string idDriver;

            RedactionDriver = DataBase.ReadDriverDB(DriverObjectRead.AllDriversList[listBox2Driver.SelectedIndex].DriverDBID);
            RedactionDriver.PhotoDriver = destFile;
            RedactionDriver.FIOdriver = textBox2FioDriver.Text;
            RedactionDriver.ExpirienceDriver = textBox3ExpirienceDriver.Text;
            RedactionDriver.DriverHabitSmoke = habitSmoke;
            RedactionDriver.DriverHabitDrink = habitDrink;
            RedactionDriver.DriverHabitDrugs = habitDrugs;

            RedactionDriver.EditDriver();

            
            
        }

        

        private void GetDrivers()
        {
            DriverObjectRead.ReadAllDrivers();
            for (int i = 0; i < DriverObjectRead.AllDriversList.Count; i++)
            {
                listBox2Driver.Items.Add(DriverObjectRead.AllDriversList[i].DriverDBID + "\t " + DriverObjectRead.AllDriversList[i].FIOdriver);
            }

            
        }


        private void listBox2Driver_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox2FioDriver.Text = DriverObjectRead.AllDriversList[listBox2Driver.SelectedIndex].FIOdriver;
            textBox3ExpirienceDriver.Text = DriverObjectRead.AllDriversList[listBox2Driver.SelectedIndex].ExpirienceDriver;


            if (DriverObjectRead.AllDriversList[listBox2Driver.SelectedIndex].DriverHabitSmoke == true)
                      { checkBox6Smoke.Checked = true; }
                else
                    checkBox6Smoke.Checked = false;

                if (DriverObjectRead.AllDriversList[listBox2Driver.SelectedIndex].DriverHabitDrink == true)
                { checkBox7Drink.Checked = true; }
                else
                    checkBox7Drink.Checked = false;

                if (DriverObjectRead.AllDriversList[listBox2Driver.SelectedIndex].DriverHabitDrugs == true)
                { checkBox5Drugs.Checked = true; }
                else
                   checkBox5Drugs.Checked = false;

            ButtonDriverDisabled();

            if (DriverObjectRead.AllDriversList[listBox2Driver.SelectedIndex].PhotoDriver == "")
            {
                pictureBox2.Image = null;
                pictureBox2.BackColor =  Color.Gray;
            }
            else
                pictureBox2.Load(DriverObjectRead.AllDriversList[listBox2Driver.SelectedIndex].PhotoDriver);



        }



        private void textBox2FioDriver_TextChanged(object sender, EventArgs e)
        {
            if (textBox2FioDriver.Text == "")
            { button4AddDriver.Enabled = false; }
            else
                button4AddDriver.Enabled = true;
        }




        private void checkBox6Smoke_Click(object sender, EventArgs e)
        {
            if (checkBox6Smoke.Checked == true)
                habitSmoke = true;
            else
                habitSmoke = false;
        }

        private void checkBox7Drink_Click(object sender, EventArgs e)
        {
            if (checkBox7Drink.Checked == true)
                habitDrink = true;
            else
                habitDrink = false;
        }

        private void checkBox5Drugs_Click(object sender, EventArgs e)
        {
            if (checkBox5Drugs.Checked == true)
                habitDrugs = true;
            else
                habitDrugs = false;
        }


        //////////////////////////// Заявки
        private void button9AddRequest_Click(object sender, EventArgs e)
        {

        }

        private void button7RedactionRequest_Click(object sender, EventArgs e)
        {

        }

        private void GetRequest (OrderClass RequestObject)
        {
            RequestObject.ReadRequest();

            /*
            textBox7RequestCar.Text = RequestObject.ModelCarList[index];
            */

        }

        private void button8DeleteRequest_Click(object sender, EventArgs e)
        {
            
        }

        ////////////////////////////////// Клиенты
        private void button12AddCustomer_Click(object sender, EventArgs e)
        {
            CustomerClass AddCustomer = new CustomerClass();

            AddCustomer.FIOcustomer = textBox12FioCustomer.Text;
            AddCustomer.PhoneCustomer = textBox11PhoneCustomer.Text;
            AddCustomer.CityCustomer = textBox10CityCustomer.Text;

            AddCustomer.InsertCustomer();

            listBox3Customers.Refresh();

        }

        private void button10RedactionCustomer_Click(object sender, EventArgs e)
        {
            CustomerClass RedactionCustomer = new CustomerClass();
            // string idDriver;
            /*
            RedactionCustomer = DataBase.ReadCustomerDB(Int32.Parse(CustomerObjectRead.AllCustomerList[listBox3Customers.SelectedIndex].IDcustomer));
            RedactionCustomer.FIOcustomer = textBox12FioCustomer.Text;
            RedactionCustomer.PhoneCustomer = textBox11PhoneCustomer.Text;
            RedactionCustomer.CityCustomer = textBox10CityCustomer.Text;

            RedactionCustomer.EditCustomer();

            listBox3Customers.Refresh();
            */
        }

       

        private void GetCustomers()
        {
         //   for (int i = 0; i < CustomerObjectRead.AllCustomerList.Count; i++)
          //      listBox3Customers.Items.Add(CustomerObjectRead.AllCustomerList[i].FIOcustomer);
        }

       

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label23_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        

        private void label4_Click(object sender, EventArgs e)
        {

        }

        

        private void label21_Click(object sender, EventArgs e)
        {

        }

        private void ButtonDriverDisabled()
        {
             button4AddDriver.Enabled = false; 
        }

    }
}
