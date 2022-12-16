namespace CloudBeat.Kit.MSTest
{
    public class OutputDataEntry
    {
        public string Name { get; set; }
        public object Data { get; set; }

        public OutputDataEntry(string name, object data)
        {
            Name = name;
            Data = data;
        }

        public override int GetHashCode() {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj) {
            if (obj == null)
                return false;

            return Name.Equals(((OutputDataEntry)obj).Name);
        }
    }
}
