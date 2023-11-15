/*
    * Show Me The Money Project
    *   Creates loan statistics and an amortization table for a given loan.
    *
    *   file:   ShowMeTheMoney.cs
    *   author: Bryce Tondryk
*/

namespace LoanCalculator
{
    class Loan1
    {

        private double principal; //original loan amount
        private double rate; // interest rate used for calculation
        private double apr; // interest rate as an apr
        private int term; //length of loan
        private double defaultMonthlyPayment; //default amount
        private double balance; //outstanding amount
        private double extra; 
        public Loan1(double loanAmount, double annualInterestRate, int monthsInTerm)
        {
            principal = loanAmount;
            rate = annualInterestRate / 100 / 12;
            apr = annualInterestRate;
            term = monthsInTerm;
            balance = loanAmount;
            extra = 0;
            defaultMonthlyPayment = (principal * (rate * Math.Pow(1 + rate, term)) / (Math.Pow(1 + rate, term) - 1)) ;


        }
        
        public double NumberOfMonths // all of our getters
        {
            get { return term; }
        }
        public double APR
        {
            get { return apr; }
        }
        public double Principal
        {
            get { return principal; }
        }
        public double DefaultMonthlyPayment
        {
            get { return defaultMonthlyPayment; }
        }
        public double Balance
        {
            get { return balance; }
        }
       

        public override string ToString()
        {

            string description = ($"Loan: {principal:C2} borrowed at {rate * 12 * 100}% APR over {term} months. ");
            return description;
        }

        public double InterestPortion()
        {
            double interestPortion = rate * balance;
            return Math.Round(interestPortion, 2);
        }
        public double PrincipalPortion(double extra=0)
        {
            double principalPortion = defaultMonthlyPayment - InterestPortion() + extra;

            if (principalPortion < balance)
            {
                return Math.Round(principalPortion, 2);
            }
            else
            {
                return balance;
            }
        }
        public double MakeMonthlyPayment(double extra=0 )
        {
            balance = balance - PrincipalPortion(extra);
            return balance;
        }
        public double[,] GetAmortizationTable(double extra = 0)
        {
            //first row is for the original loan manount
            // the next n rows are for each month's payment
            // 4 columns: monthly payment, interest portion, principal portion, balance
            double[,] table = new double[term + 1, 5];
            table[0, 0] = 0; //month
            table[0, 1] = 0; //monthlypayment
            table[0, 2] = 0; //interest
            table[0, 3] = 0; // principal
            table[0, 4] = principal;  //balance

            int i;
            for ( i=1; balance > 0 && i <= term ; i++)
            {
                if (PrincipalPortion(extra)  >= balance) //PUT EXTRA; 
                {//this loop explains what happens when our principal portion is greater than our balance.
                    // we want our balance to be 0, so we make our principal the balance to prevent negatives.
                    table[i, 0] = i;
                    table[i, 1] = PrincipalPortion(extra) + InterestPortion();
                    table[i, 2] = InterestPortion();
                    table[i, 3] = balance;
                    table[i, 4] = MakeMonthlyPayment(extra); //PUT EXTRA
                }
               
                else //this is normally how the loop works
                {
                    table[i, 0] = i;
                    table[i, 1] = defaultMonthlyPayment + extra; //PUT EXTRA(these are all for me!)
                    table[i, 2] = InterestPortion();
                    table[i, 3] = PrincipalPortion(extra); //PUT EXTRA
                    balance = balance - PrincipalPortion(extra); //PUT EXTRA
                    table[i, 4] = balance;
                }
                if (balance > 0 && i == term)
                {
                    table[i, 0] = term;
                    table[i, 1] = defaultMonthlyPayment + InterestPortion();
                    
                    table[i, 3] = balance + table[i, 3];
                    table[i, 4] = table[i-1, 4] - table[i, 3];
                }
            }
            
            double[,] at = new double[i, 5]; // second table of same length 

            at[0, 0] = 0; //month
            at[0, 1] = 0; //monthlypayment
            at[0, 2] = 0; //interest
            at[0, 3] = 0; // principal
            at[0, 4] = principal;  //balance
            // does all the extra payments
            for (int i2 = 1; i2 < i; i2++)
            {
                MakeMonthlyPayment(extra); //PUT EXTRA  
                at[i2, 0] = table[i2, 0];
                at[i2, 1] = table[i2, 1];
                at[i2, 2] = table[i2, 2];
                at[i2, 3] = table[i2, 3];
                at[i2, 4] = table[i2, 4];
            }
            return at;


        }

    }
    class Program
    {
        public static void Main(string[] args)
        {
            double amount;
            double intRate;
            int months;
            double extra = 0;
             
            if( args.Length < 3)
            {
                Console.WriteLine("Need more values for command line!");
            }         
           
            if (!double.TryParse(args[0], out  amount) ) 
            {
                Console.WriteLine("Error!");
                return;
            }

            if (!double.TryParse(args[1], out  intRate))
            {
                Console.WriteLine("Error!");
                return;
            }
            if (!int.TryParse(args[2], out  months))
            {
                Console.WriteLine("Error!");
                return;
            }
            if ((args.Length >= 4) && (!double.TryParse(args[3], out  extra)))
            {
                Console.WriteLine("Error! Invalid parameters.");
                return;
            }
           

            Loan1 mortgage = new Loan1(amount, intRate, months);
            Console.WriteLine(mortgage);
            Console.WriteLine($"The mimimum monthly payment is {mortgage.DefaultMonthlyPayment:C2}");
            Console.WriteLine($"The interest portion is {mortgage.InterestPortion():C2}");
            Console.WriteLine($"The principal portion is {mortgage.PrincipalPortion(extra):C2}");
            Console.WriteLine();

            double[,] amortTable = mortgage.GetAmortizationTable(extra);

            Console.WriteLine("Amortization Schedule");
            string [] names = {"Payment", "Amount","Interest","Principal","Balance" };
            Console.Write($"{names[0],7}");
            Console.Write($"{names[1],15}");
            Console.Write($"{names[2],15}");
            Console.Write($"{names[3],15}");
            Console.Write($"{names[4],15}");
            
            Console.WriteLine();

            for (int i = 0; i <= 5; i++)
            {
                string col0 = $"{amortTable[i, 0], 7}";
                string col1 = $"{amortTable[i, 1],15:C2}";
                string col2 = $"{amortTable[i, 2],15:C2}";
                string col3 = $"{amortTable[i, 3],15:C2}";
                string col4 = $"{amortTable[i, 4],15:C2}";
               
                Console.WriteLine($"{col0}{col1}{col2}{col3}{col4}");


            }
            Console.WriteLine();

            
            for (int x = amortTable.GetLength(0)-5; x <= amortTable.GetLength(0)-1 ; x++)

            {
                string col0 = $"{amortTable[x, 0],7}";
                string col1 = $"{amortTable[x, 1],15:C2}";
                string col2 = $"{amortTable[x, 2],15:C2}";
                string col3 = $"{amortTable[x, 3],15:C2}";
                string col4 = $"{amortTable[x, 4],15:C2}";

                Console.WriteLine($"{col0}{col1}{col2}{col3}{col4}");
                

            }

            double totalCost = 0.0;
            for (int i = 0; i < amortTable.GetLength(0); i++)
            {
                totalCost += amortTable[i, 1];
            }
            Console.WriteLine($"Your total cost is {totalCost:C2} Wowzers!");

            if (extra > 0)
            {
                double originalCost = mortgage.DefaultMonthlyPayment * months;
                double savings = originalCost - totalCost;
                Console.WriteLine($"The original repayment was {originalCost:C2}!"); 
                Console.WriteLine($"By paying extra of {extra:c2}, you were able to save {savings:C2} in interest!");
            }
           




        }
       
    }
}
