using System.Runtime.InteropServices;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparison
	{
		[Test]
		[Description("Проверка текущего царя")]
		[Category("ToRefactor")]
		public void CheckCurrentTsar()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

            // Перепишите код на использование Fluent Assertions.
            Assert.AreEqual(actualTsar.Name, expectedTsar.Name);
            Assert.AreEqual(actualTsar.Age, expectedTsar.Age);
            Assert.AreEqual(actualTsar.Height, expectedTsar.Height);
            Assert.AreEqual(actualTsar.Weight, expectedTsar.Weight);

            Assert.AreEqual(expectedTsar.Parent.Name, actualTsar.Parent.Name);
            Assert.AreEqual(expectedTsar.Parent.Age, actualTsar.Parent.Age);
            Assert.AreEqual(expectedTsar.Parent.Height, actualTsar.Parent.Height);
            Assert.AreEqual(expectedTsar.Parent.Parent, actualTsar.Parent.Parent);
        }
        /*  Антипаттерн Freeride
         
            В случае, если тест завалится, то не будет ясно в чем проблема,
            результат теста будет выглядеть так:
            Expected: 160
            But was:  170
            
            Такой тест не поймает ошибки в конструкторе, 
            например если перепутать рост с весом.

            Не проверяется вес родителя.
            Не проверяет тип возвращаемый регистром.

          */

        private Person currentTsar;

        [SetUp]
	    public void SetUp()
        {
            currentTsar =  TsarRegistry.GetCurrentTsar();
        }

	    [Test]
	    public void GetCurrentTsar_PersoneShouldNotBeNull() =>
	        currentTsar.Should().NotBeNull();

	    [Test]
        public void GetCurrentTsar_CheckName() =>
            currentTsar.Name.Should().Be("Ivan IV The Terrible");

	    [Test]
	    public void GetCurrentTsar_CheckAge() =>
	        currentTsar.Age.Should().Be(54);

	    [Test]
	    public void GetCurrentTsar_CheckHeight() =>
	        currentTsar.Height.Should().Be(170);

	    [Test]
	    public void GetCurrentTsar_CheckWeight() =>
	        currentTsar.Weight.Should().Be(70);

	    [Test]
	    public void GetCurrentTsar_Parent_ShouldNotBeNull() =>
	        currentTsar.Parent.Should().NotBeNull();

        [Test]
	    public void GetCurrentTsar_Parent_CheckName() =>
	        currentTsar.Parent.Name.Should().Be("Vasili III of Russia");

	    [Test]
	    public void GetCurrentTsar_Parent_CheckAge() =>
	        currentTsar.Parent.Age.Should().Be(28);

	    [Test]
	    public void GetCurrentTsar_Parent_CheckHeight() =>
	        currentTsar.Parent.Height.Should().Be(170);

	    [Test]
	    public void GetCurrentTsar_Parent_CheckWeight() =>
	        currentTsar.Parent.Weight.Should().Be(60);

	    [Test]
	    public void GetCurrentTsar_Parent_ParentShouldBeNull() =>
	        currentTsar.Parent.Parent.Should().BeNull();


        [Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
			new Person("Vasili III of Russia", 28, 170, 60, null));

			// Какие недостатки у такого подхода? 
			Assert.True(AreEqual(actualTsar, expectedTsar));
		}

        private bool AreEqual(Person actual, Person expected)
		{
			if (actual == expected) return true;
			if (actual == null || expected == null) return false;
			return
			actual.Name == expected.Name
			&& actual.Age == expected.Age
			&& actual.Height == expected.Height
			&& actual.Weight == expected.Weight
			&& AreEqual(actual.Parent, expected.Parent);
		}

	    /*
            В случае, если тест завалится, то не будет ясно в чем проблема,
            результат теста будет выглядеть так:
            Expected: True
            But was:  False
            
            Такой тест не поймает ошибки в конструкторе, 
            например если перепутать рост с весом.

            В тесте используется метод AreEqual, который содержит в себе достаточно много логики, 
            из-за чего его тоже надо тестить. 
         */
    }

    public class TsarRegistry
	{
		public static Person GetCurrentTsar()
		{
		    return new Person(
		        "Ivan IV The Terrible", 54, 170, 70, 
				new Person("Vasili III of Russia", 28, 170, 60, null));
		}
	}

	public class Person
	{
		public static int IdCounter = 0;
		public int Age, Height, Weight;
		public string Name;
		public Person Parent;
		public int Id;

		public Person(string name, int age, int height, int weight, Person parent)
		{
			Id = IdCounter++;
			Name = name;
			Age = age;
			Height = height;
            Weight = weight; 
			Parent = parent;
		}
	}

}
