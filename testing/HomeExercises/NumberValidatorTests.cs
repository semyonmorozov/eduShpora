using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
	    [TestFixture]
	    public class NumberValidator_Should
	    {
            [TestCase(3, 2, true, "00.00", ExpectedResult = false)]
	        [TestCase(3, 2, true, "-0.00", ExpectedResult = false)]
	        [TestCase(3, 2, true, "+0.00", ExpectedResult = false)]
	        [TestCase(4, 2, true, "+1.23", ExpectedResult = true)]
	        [TestCase(3, 2, true, "+1.23", ExpectedResult = false)]
	        [TestCase(17, 2, true, "0.000", ExpectedResult = false)]
	        [TestCase(3, 2, true, "-1.23", ExpectedResult = false)]
            [TestCase(4, 2, false, "-2.23", ExpectedResult = true)]
	        [TestCase(2, 0, false, "-0", ExpectedResult = true)]
	        [TestCase(2, 0, false, "000", ExpectedResult = false)]
	        [TestCase(2, 0, false, null, ExpectedResult = false)]
	        [TestCase(2, 0, false, "", ExpectedResult = false)]
            public bool ValidateNumber(int precision, int scale, bool onlyPositive, string value)
	        {
                return new NumberValidator(precision,scale,onlyPositive).IsValidNumber(value);
	        }

	        [TestCase(3, 2, true, "a.sd", ExpectedResult = false)]
            [TestCase(10, 6, true, "0.00.0", ExpectedResult = false)]
	        [TestCase(2, 1, true, "0.", ExpectedResult = false)]
	        [TestCase(2, 1, false, ".0", ExpectedResult = false)]
            public bool ReturnFalse_WhenParsingOfNumberFailed(int precision, int scale, bool onlyPositive, string value)
	        {
	            return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);
	        }

	        [TestCase(17, 2, true, "0", ExpectedResult = true)]
	        [TestCase(17, 2, true, "0.0", ExpectedResult = true)]
	        [TestCase(2, 1, true, "0,0", ExpectedResult = true)]
	        [TestCase(5, 1, false, "-567,0", ExpectedResult = true)]
	        public bool ReturnTrue_WhenParsingOfNumberSucceed(int precision, int scale, bool onlyPositive, string value)
	        {
	            return new NumberValidator(precision, scale, onlyPositive).IsValidNumber(value);
	        }

            [TestCase(0, 0, true)]
            [TestCase(-1, 2, true)]
            public void ThrowException_OnNegativePrecision(int precision, int scale, bool onlyPositive)
	        {
	            Action act = () => new NumberValidator(precision, scale, onlyPositive);
	            act.ShouldThrow<ArgumentException>().WithMessage("precision must be a positive number");
	        }

	        [TestCase(1, 2, true, TestName = "When scale is bigger then precision")]
	        [TestCase(2, -1, false, TestName = "When scale is negative")]
	        public void ThrowException_OnIncorrectScale(int precision, int scale, bool onlyPositive)
	        {
	            Action act = () => new NumberValidator(precision, scale, onlyPositive);
	            act.ShouldThrow<ArgumentException>().WithMessage("precision must be a non-negative number less or equal than precision");
	        }

            [TestCase(1, 0, true)]
	        [TestCase(5, 3, false)]
            public void DontThrowException_OnCreatingCorrectNumberValidator(int precision, int scale, bool onlyPositive)
	        {
	            Action act = () => new NumberValidator(precision, scale, onlyPositive);
	            act.ShouldNotThrow<ArgumentException>();
	        }
        }
    }

	public class NumberValidator
	{
		private readonly Regex numberRegex;
		private readonly bool onlyPositive;
		private readonly int precision;
		private readonly int scale;

		public NumberValidator(int precision, int scale = 0, bool onlyPositive = false)
		{
			this.precision = precision;
			this.scale = scale;
			this.onlyPositive = onlyPositive;
			if (precision <= 0)
				throw new ArgumentException("precision must be a positive number");
			if (scale < 0 || scale >= precision)
				throw new ArgumentException("precision must be a non-negative number less or equal than precision");
			numberRegex = new Regex(@"^([+-]?)(\d+)([.,](\d+))?$", RegexOptions.IgnoreCase);
		}

		public bool IsValidNumber(string value)
		{
			// Проверяем соответствие входного значения формату N(m,k), в соответствии с правилом, 
			// описанным в Формате описи документов, направляемых в налоговый орган в электронном виде по телекоммуникационным каналам связи:
			// Формат числового значения указывается в виде N(m.к), где m – максимальное количество знаков в числе, включая знак (для отрицательного числа), 
			// целую и дробную часть числа без разделяющей десятичной точки, k – максимальное число знаков дробной части числа. 
			// Если число знаков дробной части числа равно 0 (т.е. число целое), то формат числового значения имеет вид N(m).

			if (string.IsNullOrEmpty(value))
				return false;

			var match = numberRegex.Match(value);
			if (!match.Success)
				return false;

			// Знак и целая часть
			var intPart = match.Groups[1].Value.Length + match.Groups[2].Value.Length;
			// Дробная часть
			var fracPart = match.Groups[4].Value.Length;

			if (intPart + fracPart > precision || fracPart > scale)
				return false;

			if (onlyPositive && match.Groups[1].Value == "-")
				return false;
			return true;
		}
	}
}