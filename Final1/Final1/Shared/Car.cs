namespace Final1.Shared
{
    public struct Car
    {
        public Guid ID { get; set; }
        public String Make { get; set; }
        public String Model { get; set; }
        public int Year { get; set; }
        public String? Color { get; set; }
        public Int32? NumberOfWheels { get; set; }
    }
}