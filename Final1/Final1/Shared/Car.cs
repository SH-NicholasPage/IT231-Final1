namespace Final1.Shared
{
    public struct Car : IEquatable<Car>
    {
        public Guid ID { get; set; }
        public String Make { get; set; } = String.Empty;
        public String Model { get; set; } = String.Empty;
        public int Year { get; set; } = default(Int32);
        public String? Color { get; set; } = null;
        public Int32? NumberOfWheels { get; set; } = null;

        public Car()
        {
            ID = Guid.NewGuid();
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
    }
}