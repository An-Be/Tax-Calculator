using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TaxCalculator
{
    internal class Program
    {
        static void Main(string[] args)
        {


            TaxCalculator.readFile();
            TaxCalculator.TaxData.ToString();
            //TaxCalculator.ComputeTaxFor("CA", 40000);
            //TaxCalculator.Silent("NY", 96050);
            //TaxCalculator.Verbose("CA", 40000);
            EmployeeRecord record = new EmployeeRecord();
            record.FileReader();
            
            
        }
    }

    static class TaxCalculator
    {

        public static Dictionary<string, List<TaxRecord>> TaxData;
        public static string path = @"C:\Users\AndreaBerrocal\Desktop\WOZ\Exeter\TaxCalculator\TaxCalculator\taxtable.csv";

        static TaxCalculator(){}
        public static void readFile() {
            StreamReader sr = new StreamReader(path);
            Dictionary<string, List<TaxRecord>> res = new Dictionary<string, List<TaxRecord>>();

            try
            {
                //Console.WriteLine($"{sr.ReadLine()}");
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] row = line.Split(",");
                    //try-catch blocks to catch errors in csv file
                    try
                    {
                        string _stateCode = row[0];
                        if (_stateCode.Length >= 3)
                        {
                            continue;
                        }                     
                    }
                    catch
                    {
                        Console.WriteLine($"The State code on line {line} is not 2 characters long");
                        Console.WriteLine("********************************************************************************************");
                        continue;
                    }

                    try
                    {
                        long _floor = long.Parse(row[2]);
                    }
                    catch
                    {
                        Console.WriteLine($"The floor income on line {line} is not an number");
                        Console.WriteLine("********************************************************************************************"); 
                        continue;
                    }
                    try
                    {
                        long _ceiling = long.Parse(row[3]);
                    }
                    catch
                    {
                        Console.WriteLine($"The ceiling income on line {line} is not an number");
                        Console.WriteLine("********************************************************************************************");
                        continue;
                    }
                    try
                    {
                        decimal _rate = decimal.Parse(row[4]);
                    }
                    catch
                    {
                        Console.WriteLine($"The rate on line {line} is not an number");
                        Console.WriteLine("********************************************************************************************");
                        continue;
                    }
                    //use parse to convert strings into correct type
                    string stateCode = row[0];
                    string state = row[1];
                    long floor = long.Parse(row[2]);
                    long ceiling = long.Parse(row[3]);
                    decimal rate = decimal.Parse(row[4]);
                    TaxRecord record = new TaxRecord(stateCode, state, floor, ceiling, rate);

                    //add data to the dictionary/string
                    if (res.ContainsKey(stateCode))
                    {
                        res[stateCode].Add(record);
                    }
                    else {
                        res.Add(stateCode, new List<TaxRecord> { record });

                    }
                }
                TaxCalculator.TaxData = res;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"The file was not found: '{e}'");
            }
        }
        //compute the taxes for a state using an entered income and statecode
        public static decimal ComputeTaxFor(string stateCode, long income)
        {

           if (TaxCalculator.TaxData.ContainsKey(stateCode))
           {
                List<TaxRecord> records = TaxCalculator.TaxData[stateCode];
                decimal tax = 0;
                foreach (TaxRecord record in records) 
                {
                    long floor = record.lowestIncome;
                    long ceiling = record.highestIncome;
                    decimal taxRate = record.rate;

                    if (income < floor)
                    {
                        return tax;
                    }
                    else if (income > ceiling)
                    {
                        tax += (ceiling - floor) * taxRate;
                        
                    }
                    else {
                        tax += (income - floor) * taxRate;
                    }
                    //Console.WriteLine($"{stateCode} tax bracket are between {floor} and {ceiling}");
                }  
                
                return Math.Round(tax,2);
                }
            else
            {
                
               throw new Exception($"The state code you entered : '{stateCode}' does not match, please enter another one.");
            }
        }
        //method to write tax with more detail
        public static void Verbose(string stateCode, long income)
        {
            decimal tax = TaxCalculator.ComputeTaxFor(stateCode, income);
            List<TaxRecord> records = TaxCalculator.TaxData[stateCode];

            Console.WriteLine($"You entered the state code: '{stateCode}'. \nYou entered the income amount: '${income}.'");
            Console.WriteLine($"Tax Brackets for this state are: ");
            foreach (TaxRecord record in records)
            {
                long floor = record.lowestIncome;
                long ceiling = record.highestIncome;
                decimal taxRate = record.rate;
                Console.WriteLine($"between '${floor}' and '${ceiling}' and has a tax rate of '${taxRate}'");
            }
            Console.WriteLine($"Your total taxes are: '${Math.Round(tax, 2)}'");

        }
        //method to only write tax amount
        public static void Silent(string stateCode, long income)
        {
            decimal tax = TaxCalculator.ComputeTaxFor(stateCode, income);
            Console.WriteLine($"Your tax is: '${Math.Round(tax,2)}'");
        }

    }
        public class TaxRecord
        {
            public string stateCode;
            public string state;
            public long lowestIncome;
            public long highestIncome;
            public decimal rate;

            public TaxRecord(string stateCode, string state, long floor, long ceiling, decimal rate)
            {
                this.stateCode = stateCode;
                this.state = state;
                this.lowestIncome = floor;
                this.highestIncome = ceiling;
                this.rate = rate;
            }
        }

    public class EmployeeRecord
    {
        public static string emplPath = @"C:\Users\AndreaBerrocal\Desktop\WOZ\Exeter\TaxCalculator\TaxCalculator\employees.csv";
        public string[] employees = File.ReadAllLines(emplPath);
        public string ID;
        public string name;
        public string code;
        public int hours;
        public double rate;

        public void FileReader()
        {
           
            for (int i = 0; i < employees.Length; i++)
            {
                string[] rowData = employees[i].Split(',');
                //checking for error in state code
                if (TaxCalculator.TaxData.ContainsKey(rowData[2])){}
                else
                {
                    Console.WriteLine("********************************************************************************************");
                    Console.WriteLine($"{rowData[2]} is not a valid State Code");
                    Console.WriteLine("********************************************************************************************");
                    continue;
                }
                //checking for error in hours worked
                try 
                { 
                    int _hours = int.Parse(rowData[3]);
                }
                catch
                {
                    Console.WriteLine("********************************************************************************************");
                    Console.WriteLine($"'{rowData[3]}' is not number");
                    Console.WriteLine("********************************************************************************************");
                    continue;
                }
               ID = rowData[0];
               name = rowData[1];
               code = rowData[2];
               hours = int.Parse(rowData[3]);
               decimal rate = decimal.Parse(rowData[4]);
               decimal salary = (hours * rate);
               salary = Math.Round(salary, 2);

               //displaying employee info alongside taxes due 
               Console.WriteLine($"ID: {ID} \nName: {name} \nState Code: {code} \nHours Worked: {hours} \nHourly Rate: {rate}");

               //using verbose and silent method for employeerecord
               TaxCalculator.Silent(code, (long)salary);
               //TaxCalculator.Verbose(code, (long)salary);
               Console.WriteLine("********************************************************************************************");

            }
            Console.ReadKey();

        }
    }
}
