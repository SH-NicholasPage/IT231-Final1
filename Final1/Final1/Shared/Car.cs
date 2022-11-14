using System.Xml.Linq;

namespace Final1.Shared
{
    public struct Car : IEquatable<Car>
    {
        public Guid ID { get; init; }
        public String Make { get; init; } = String.Empty;
        public String Model { get; init; } = String.Empty;
        public int Year { get; init; } = default(Int32);
        public String? Color { get; init; } = null;
        public Int32? NumberOfWheels { get; init; } = null;

        public Car()
        {
            ID = Guid.NewGuid();
        }

        public Car(String make, String model, int year) : this(make, model, year, null, null) { }

        public Car((String make, String model, int year, String? color, Int32? numberOfWheels) cs) : this(cs.make, cs.model, cs.year, cs.color, cs.numberOfWheels) { }

        public Car(String make, String model, int year, String? color, Int32? numberOfWheels)
        {
            ID = Guid.NewGuid();
            this.Make = make;
            this.Model = model;
            this.Year = year;
            this.Color = color;
            this.NumberOfWheels = numberOfWheels;
        }

        public bool EqualsWithoutGuid(Car? other)
        {
            if (other is null) return false;

            return EqualsWithoutGuid(other.Value);
        }

        public bool EqualsWithoutGuid(Car other)
        {
            bool eq = this.Make.Equals(other.Make) && this.Model.Equals(other.Model) && this.Year == other.Year && this.NumberOfWheels == other.NumberOfWheels;

            if (String.IsNullOrWhiteSpace(this.Color) != String.IsNullOrWhiteSpace(other.Color))
            {
                eq = false;
            }
            else if (String.IsNullOrWhiteSpace(this.Color) == false && String.IsNullOrWhiteSpace(other.Color) == false)//If false, they are both null
            {
                if (this.Color.Equals(this.Color) == false)
                {
                    eq = false;
                }
            }

            return eq;
        }

        public bool Equals(Car other)
        {
            bool eq = this.ID.Equals(other.ID) && this.Make.Equals(other.Make) && this.Model.Equals(other.Model) && this.Year == other.Year && this.NumberOfWheels == other.NumberOfWheels;

            if (String.IsNullOrWhiteSpace(this.Color) != String.IsNullOrWhiteSpace(other.Color))
            {
                eq = false;
            }
            else if(String.IsNullOrWhiteSpace(this.Color) == false && String.IsNullOrWhiteSpace(other.Color) == false)//If false, they are both null
            { 
                if(this.Color.Equals(this.Color) == false)
                {
                    eq = false;
                }
            }

            return eq;
        }

        public override bool Equals(object? obj)
        {
            return obj is not null && obj is Car car && Equals(car);
        }

        public static bool operator ==(Car left, Car right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Car left, Car right)
        {
            return !(left.Equals(right));
        }

        public override int GetHashCode()
        {
            return (ID, Make, Model, Year, Color, NumberOfWheels).GetHashCode();
        }

        public override string ToString()
        {
            return "Car GUID: " + ID + "\t Make: " + Make + "\t Model: " + Model + "\t Year: " + Year + "\t Color: " + Color + "\t Number of Wheels: " + NumberOfWheels;
        }
    }
}