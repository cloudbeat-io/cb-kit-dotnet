namespace CloudBeat.Kit.NUnit
{
    public class ResultData
    {
        public string Name { get; set; }
        public object Data { get; set; }

        public ResultData(string name, object data)
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

            return Name.Equals(((ResultData)obj).Name);
        }
    }
}
