namespace AccountingService.Services
{
    public class NetSalaryService
    {
        const double taxRate = 0.2;
        const double socialSecurityRate = 0.1;
        const double pensionRate = 0.05;
        public double Calculate( double grossSalary)
        {
            double tax = grossSalary * taxRate;
            double socialSecurity = grossSalary * socialSecurityRate;
            double pension = grossSalary * pensionRate;
            double netoSalary = grossSalary - tax - socialSecurity - pension;

            return netoSalary;
        }

        
    }
}

