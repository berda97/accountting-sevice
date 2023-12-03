using AccountingService.Services;

using NUnit.Framework;


namespace AccountingService.Tests
{
    [TestFixture]
    public class NetSalaryServiceTests
    {
        private NetSalaryService netSalaryService;
        [SetUp]
        public void Setup()
        {
            netSalaryService = new NetSalaryService();
        }

        [TestCase(1000, 650)]
        public void CalculateNetSalary_ReturnsCorrectNetSalary(double grossSalary, double expectedNetSalary)
        {

            double actualNetSalary = netSalaryService.Calculate(grossSalary);
            Assert.That(actualNetSalary, Is.EqualTo(expectedNetSalary));
        }

        [TestCase(-1000)]
        public void CalculateNetSalary_ThrowsExceptionForNegativeGrossSalary(double grossSalary)
        {

            Assert.Throws<ArgumentException>(() => netSalaryService.Calculate(grossSalary));
        }
        [TestCase(0, 0)]
        public void CalculateNetSalary_ReturnsZeroForZeroGrossSalary(double grossSalary, double expectedNetSalary)
        {
            double actualNetSalary = netSalaryService.Calculate(grossSalary);
            Assert.That(actualNetSalary, Is.EqualTo(expectedNetSalary));
        }
    }
}